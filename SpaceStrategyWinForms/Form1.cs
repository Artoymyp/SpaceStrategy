using SpaceshipStrategy.ViewModels;
using SpaceStrategy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceStrategyWinForms
{
	public partial class Form1 : Form
	{

		public Form1()
		{
			InitializeComponent();
			spaceshipLibraryGridView.AutoGenerateColumns = false;

			_gameEngine = new Game();
			_gameEngine.PropertyChanged += gameEngine_PropertyChanged;
			
			bool fullscreen = true;
			if (fullscreen)
			{
				this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
				this.ClientSize = new Size(_gameEngine.Size.Width + splitContainer1.Panel1.Width, _gameEngine.Size.Height);
			}
			else
			{
				this.ClientSize = new Size((_gameEngine.Size.Width + splitContainer1.Panel1.Width)/2, _gameEngine.Size.Height);			
			}
			
			starfieldControl1.GameEngine = _gameEngine;

			_gameEngine.StartPositioningShips += gameEngine_StartPositioningShips;
			_gameEngine.BattleStarted += gameEngine_BattleStarted;
			_gameEngine.StartPositioningPhase += gameEngine_StartPositioningPhase;
			_gameEngine.NextTurn += gameEngine_NextTurn;
			_gameEngine.NextBattlePhase += gameEngine_NextBattlePhase;
			Application.Idle += HandleApplicationIdle;


			timer1.Interval = _gameEngine.TimerStep.Milliseconds;
			timer1.Start();
		}
		void gameEngine_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "SelectedSpaceship")
			{

				SpecialOrdersPanelViewModel curCommandPanelViewModel = commandPanel1.DataContext as SpecialOrdersPanelViewModel;
				if (curCommandPanelViewModel == null) {
					SpecialOrderButtonBehaviour.Game = _gameEngine;
					curCommandPanelViewModel = new SpecialOrdersPanelViewModel();
					commandPanel1.DataContext = curCommandPanelViewModel;
				}
				curCommandPanelViewModel.Spaceship = _gameEngine.SelectedSpaceship;
				spaceshipInfo1.DataContext = curCommandPanelViewModel.Spaceship;
			}
			else if (e.PropertyName == "GameState") {
				if (_gameEngine.GameState == GameState.End) {
					var endDialog = new UI.EndDialog();
					endDialog.DataContext = _gameEngine.Players;
					endDialog.ShowDialog();
				}
			}
		}

		void gameEngine_NextBattlePhase(object sender, NextBattlePhaseEventArgs e)
		{
			turnEndButton.Enabled = true;
		}

		void gameEngine_NextTurn(object sender, NextPlayerEventArgs e)
		{
			label1.Visible = true;
			label2.Visible = true;
			
			label2.Text = e.Player.Name;
			label2.ForeColor = e.Player.Color;
		}

		void gameEngine_StartPositioningPhase(object sender, EventArgs e)
		{
			spaceshipLibraryGridView.Visible = true;
		}

		void gameEngine_BattleStarted(object sender, EventArgs e)
		{
			spaceshipLibraryGridView.Visible = false;
			turnEndButton.Visible = true;
			//toolStrip1.Visible = true;
		}

		void gameEngine_StartPositioningShips(object sender, NextPlayerEventArgs e)
		{
			int raceIndex = -1;
			foreach (DataRowView raceItem in comboBox1.Items) {
				var raceRow = raceItem.Row as GameDataSet.RaceRow;
				if (raceRow.Name == _gameEngine.CurrentPlayer.Race)
					raceIndex = comboBox1.Items.IndexOf(raceItem);
			}
			if (raceIndex != -1)
				comboBox1.SelectedIndex = raceIndex;
		}

		void HandleApplicationIdle(object sender, EventArgs e)
		{
			while (IsApplicationIdle()) {
				starfieldControl1.Refresh();
			}
		}
		bool IsApplicationIdle()
		{
			return PeekMessage(out var result, IntPtr.Zero, (uint)0, (uint)0, (uint)0) == 0;
		}
		Game _gameEngine;
		private void Form1_Load(object sender, EventArgs e)
		{
			comboBox1.ValueMember = "Name";
			comboBox1.DisplayMember = "Name";
			comboBox1.DataSource = _gameEngine.GameData.Races;
			comboBox1.SelectedValueChanged += comboBox1_SelectedValueChanged;
			comboBox1.SelectedIndex = 1;


		}

		private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
		{
			spaceshipLibraryGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;// ExceptHeader;
			string raceId = (string)comboBox1.SelectedValue;
			//dataGridView1.DataSource = gameEngine.GameData.GetSpaceshipClassesByRaceId(raceId, gameEngine.Scenario.SingleShipMaxPoints);
			spaceshipLibraryGridView.DataSource = _gameEngine.Scenario.GetAvailableSpaceshipClasses(raceId);

		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			_gameEngine.OnTimer();
			//specialOrderPanel.Enabled = true;

			//List<Button> orderButtons = new List<Button>(){
			//	allAheadButton,
			//	comeToNewDirectionButton,
			//	burnRetrosButton,
			//	lockOnButton,
			//	braceForImpactButton,
			//	reloadOrdnanceButton,
			//	launchTorpedoButton
			//};

			//var availableOrders = gameEngine.AvailableOrders;
			//foreach (var orderButton in orderButtons) {
			//	if ((orderButton == allAheadButton && availableOrders.Contains(GothicTrajectorySpecialOrder.AllAheadFull)) ||
			//		(orderButton == comeToNewDirectionButton && availableOrders.Contains(GothicTrajectorySpecialOrder.ComeToNewDirection)) ||
			//		(orderButton == burnRetrosButton && availableOrders.Contains(GothicTrajectorySpecialOrder.BurnRetros)) ||
			//		(orderButton == lockOnButton && availableOrders.Contains(GothicTrajectorySpecialOrder.LockOn)) ||
			//		(orderButton == braceForImpactButton && availableOrders.Contains(GothicTrajectorySpecialOrder.BraceForImpact)) ||
			//		(orderButton == reloadOrdnanceButton && availableOrders.Contains(GothicTrajectorySpecialOrder.ReloadOrdnance)) ||
			//		(orderButton == launchTorpedoButton && availableOrders.Contains(GothicTrajectorySpecialOrder.LaunchOrdnance))) {
			//		orderButton.Enabled = true;
			//	}
			//	else
			//		orderButton.Enabled = false;
			//}
			
		}
		
		private void dataGridView1_SelectionChanged(object sender, EventArgs e)
		{
			if (spaceshipLibraryGridView.CurrentRow !=null) {
				var selectedSpaceshipClass = spaceshipLibraryGridView.CurrentRow.DataBoundItem as SpaceshipClass;
				if (selectedSpaceshipClass != null) {
					_gameEngine.CurrentPlayer.PositioningZone.BeginSpaceshipCreation(selectedSpaceshipClass);
				}
			}
		}

		private void starfieldControl1_MouseClick(object sender, MouseEventArgs e)
		{
			_gameEngine.OnMouseClick(new Point2d(e.X,e.Y),e.Button);
			//if (gameEngine.SelectedSpaceship != null)
			//	weaponryTable.DataSource = gameEngine.SelectedSpaceship.Weapons;
			//else
			//	weaponryTable.DataSource = null;
		}
		private void starfieldControl1_MouseMove(object sender, MouseEventArgs e)
		{
			_gameEngine.MouseMove(new Point2d(e.X, e.Y), e.Button);
		}
		private void starfieldControl1_MouseDown(object sender, MouseEventArgs e)
		{
			_gameEngine.OnMouseDown(new Point2d(e.X, e.Y), e.Button);
		}

		private void starfieldControl1_Paint(object sender, PaintEventArgs e)
		{
		}
		[DllImport("user32.dll")]
		public static extern int PeekMessage(out NativeMessage message, IntPtr window, uint filterMin, uint filterMax, uint remove);

		private void startBattleButton_Click(object sender, EventArgs e)
		{
			_gameEngine.StartBattle();
			comboBox1.Enabled = false;
			//dataGridView1.Enabled = false;
		}

		//private void allAheadButton_MouseEnter(object sender, EventArgs e)
		//{
		//	gameEngine.PreviewSpecialOrder(GothicOrder.AllAheadFull);
		//}

		//private void allAheadButton_MouseLeave(object sender, EventArgs e)
		//{
		//	gameEngine.PreviewSpecialOrder(GothicOrder.NormalMove);
		//}

		//private void burnRetrosButton_MouseEnter(object sender, EventArgs e)
		//{
		//	gameEngine.PreviewSpecialOrder(GothicOrder.BurnRetros);
		//}

		//private void burnRetrosButton_MouseLeave(object sender, EventArgs e)
		//{
		//	gameEngine.PreviewSpecialOrder(GothicOrder.NormalMove);
		//}

		//private void changeCourse_MouseEnter(object sender, EventArgs e)
		//{
		//	gameEngine.PreviewSpecialOrder(GothicOrder.ComeToNewDirection);
		//}

		//private void changeCourse_MouseLeave(object sender, EventArgs e)
		//{
		//	gameEngine.PreviewSpecialOrder(GothicOrder.NormalMove);
		//}

		//private void allAheadButton_Click(object sender, EventArgs e)
		//{
		//	gameEngine.GiveSpecialOrder(GothicTrajectorySpecialOrder.AllAheadFull);
		//}

		//private void burnRetrosButton_Click(object sender, EventArgs e)
		//{
		//	gameEngine.GiveSpecialOrder(GothicTrajectorySpecialOrder.BurnRetros);
		//}

		//private void changeCourse_Click(object sender, EventArgs e)
		//{
		//	gameEngine.GiveSpecialOrder(GothicTrajectorySpecialOrder.ComeToNewDirection);
		//}
		//private void launchTorpedoButton_Click(object sender, EventArgs e)
		//{
		//	gameEngine.GiveSpecialOrder(GothicTrajectorySpecialOrder.LaunchOrdnance);
		//}
		//private void reloadOrdnanceButton_Click(object sender, EventArgs e)
		//{
		//	gameEngine.GiveSpecialOrder(GothicTrajectorySpecialOrder.ReloadOrdnance);
		//}

		private void startGameButton_Click(object sender, EventArgs e)
		{
			startGameButton.Visible = false;
			_gameEngine.StartGame();
		}

		private void turnEndButton_Click(object sender, EventArgs e)
		{
			turnEndButton.Enabled = false;
			_gameEngine.EndBattlePhase();
		}

		private void starfieldControl1_KeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = true;
			_gameEngine.OnKeyDown(e);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct NativeMessage
	{
		public IntPtr Handle;
		public uint Message;
		public IntPtr WParameter;
		public IntPtr LParameter;
		public uint Time;
		public Point Location;
	}
}
