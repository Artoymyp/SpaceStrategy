using SpaceStrategy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;
namespace SpaceshipStrategy.ViewModels
{
	public class BaseViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void NotifyPropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	public class GameViewModel:BaseViewModel
	{
		private Game game;
		public GameViewModel(Game game)
		{
			this.game = game;
		}
	}
		public class GameCommand : ICommand
	{
		public delegate void ICommandOnExecute(object parameter);
		public delegate bool ICommandOnCanExecute(object parameter);

		private ICommandOnExecute _execute;
		private ICommandOnCanExecute _canExecute;

		public GameCommand(ICommandOnExecute onExecuteMethod, ICommandOnCanExecute onCanExecuteMethod)
		{
			_execute = onExecuteMethod;
			_canExecute = onCanExecuteMethod;
		}
		public GameCommand(ICommandOnExecute onExecuteMethod, ICommandOnCanExecute onCanExecuteMethod, System.Drawing.Image image)
		{
			_execute = onExecuteMethod;
			_canExecute = onCanExecuteMethod;
			ImageSourceConverter c = new ImageSourceConverter();
			Image = image.ToBitmapImage();
		}
		public ImageSource Image { get; private set; }
		#region ICommand Members

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute.Invoke(parameter);
		}

		public void Execute(object parameter)
		{
			_execute.Invoke(parameter);
		}

		#endregion
	}
		public static class ImageExtensions
		{
			/// <summary> 
			/// Returns a BitmapImage from the passed bitmap. 
			/// </summary> 
			/// Bitmap to convert. 
			/// 
			public static BitmapImage ToBitmapImage(this System.Drawing.Image image)
			{
				MemoryStream ms = new MemoryStream();
				image.Save(ms, image.RawFormat);
				ms.Seek(0, SeekOrigin.Begin);
				BitmapImage bi = new BitmapImage();
				bi.BeginInit();
				bi.StreamSource = ms;
				bi.EndInit();
				return bi;
			}
		} 
	public class SpecialOrderCommand : GameCommand
	{
		GothicOrder order;
		public SpecialOrderCommand() : base(delegate { }, a => true) { }
		public SpecialOrderCommand(ICommandOnExecute onExecuteMethod, ICommandOnCanExecute onCanExecuteMethod, System.Drawing.Image image, GothicOrder order, string name)
			: base(onExecuteMethod, onCanExecuteMethod, image)
		{
			this.order = order;
			this.Name = name;
		}
		public GothicOrder Order { get { return order; } }
		string name;
		public string Name { get { return name; } set { name = value; } }
		public string Description { get; set; }

	}
	public static class SpecialOrderButtonBehaviour
	{
		public static Game Game;
		public static bool GetPreviewSpecialOrder(Button target)
		{
			return (bool)target.GetValue(PreviewSpecialOrderAttachedProperty);
		}

		public static void SetPreviewSpecialOrder(Button target, bool value)
		{
			target.SetValue(PreviewSpecialOrderAttachedProperty, value);
		}

		public static readonly DependencyProperty PreviewSpecialOrderAttachedProperty = DependencyProperty.RegisterAttached("PreviewSpecialOrder", typeof(bool), typeof(SpecialOrderButtonBehaviour), new UIPropertyMetadata(false, OnPreviewSpecialOrderAttachedPropertyChanged));

		static void OnPreviewSpecialOrderAttachedPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (Game != null) {
				Button b = o as Button;
				if (b != null) {
					SpecialOrderCommand vm = b.DataContext as SpecialOrderCommand;
					if (vm != null) {
						if ((bool)e.NewValue == true)
							Game.SelectedSpaceship.PreviewSpecialOrder(vm.Order);
						else
							Game.SelectedSpaceship.PreviewSpecialOrder(GothicOrder.None);
					}
				}
			}
		}
	}
	public abstract class SpaceshipStatusViewModel
	{
		string name;
		public SpaceshipStatusViewModel(string name, Bitmap image)
		{
			Image = image.ToBitmapImage();
			this.name = name;
		}
		public string Name { get; set; }
		public ImageSource Image { get; private set; }
		public string Description { get; protected set; }
	}
	public class SpaceshipDamageStatusViewModel : SpaceshipStatusViewModel
	{
		public SpaceshipDamageStatusViewModel(string name, Bitmap icon, string description)
			: base(name, icon)
		{
			Description = description;
		}
	}

	public class SpecialOrdersPanelViewModel:BaseViewModel
	{
		List<SpecialOrderCommand> commands;
		GothicSpaceship gothicSpaceship;
		public GothicSpaceship Spaceship
		{
			get { return gothicSpaceship; }
			set
			{
				if (gothicSpaceship != value) {
					if (gothicSpaceship != null) {
						gothicSpaceship.PropertyChanged -= OnSpaceshipPropertyChanged;
					}
					gothicSpaceship = value;
					if (gothicSpaceship != null) {
						gothicSpaceship.PropertyChanged += OnSpaceshipPropertyChanged;
					}
					CommandManager.InvalidateRequerySuggested();
					NotifyPropertyChanged("Statuses");
				}
			}
		}
		public SpecialOrdersPanelViewModel()
		{
			commands = new List<SpecialOrderCommand>();
			AddCommand(UI.Properties.Resources.AllAheadFull, GothicOrder.AllAheadFull,"Полный вперед!");
			AddCommand(UI.Properties.Resources.BurnRetros, GothicOrder.BurnRetros,"Полный назад!");
			AddCommand(UI.Properties.Resources.ChangeDirection, GothicOrder.ComeToNewDirection, "Резкий поворот!");

			AddCommand(UI.Properties.Resources.LaunchTorpedoSalvo, GothicOrder.LaunchOrdnance, "Запустить торпеды!");
			AddCommand(UI.Properties.Resources.ReloadOrdnance, GothicOrder.ReloadOrdnance, "Перезарядить торпедные аппараты!");
			AddCommand(UI.Properties.Resources.TargetLock, GothicOrder.LockOn, "Держать орудия на цели!");
			AddCommand(UI.Properties.Resources.BraceForImpact, GothicOrder.BraceForImpact, "Приготовиться к повреждениям!");

			//NotifyPropertyChanged("Commands");
		}
		public List<SpecialOrderCommand> Commands { get { return commands; } }
		
		internal void OnSpaceshipPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "AvailableOrders") {
				CommandManager.InvalidateRequerySuggested();
			}
			else if (e.PropertyName == "CriticalDamage") {
				NotifyPropertyChanged("Statuses");
			}
		}
		void GiveSpecialOrder(object param)
		{
			GothicOrder order = (GothicOrder)param;
			gothicSpaceship.SetSpecialOrder(order);
		}
		bool CanGiveSpecialOrder(object param)
		{
			if (param == null)
				return false;
			if (gothicSpaceship == null)
				return false;
			GothicOrder order = (GothicOrder)param;
			return gothicSpaceship.AvailableOrders.Contains(order);
		}
		private void AddCommand(Bitmap icon, GothicOrder order, string name)
		{
			commands.Add(new SpecialOrderCommand(GiveSpecialOrder, CanGiveSpecialOrder, icon, order, name));
		}
	}
	public class DamageTypeToViewModelConverter : IValueConverter
	{
		static Dictionary<Type, SpaceshipStatusViewModel> statusesDictionary;
		static DamageTypeToViewModelConverter(){
			statusesDictionary = new Dictionary<Type, SpaceshipStatusViewModel>();
			statusesDictionary.Add(typeof(FireDamage), new SpaceshipDamageStatusViewModel("Пожар!", UI.Properties.Resources.DamagedFire, "Пожар. В конце каждого хода пожар попытаются потушить (1к6). В случае провала корабль получить 1 очко урона."));
			statusesDictionary.Add(typeof(ThrustersDamaged), new SpaceshipDamageStatusViewModel("Основные двигатели повреждены!", UI.Properties.Resources.DamagedSpeed, "Скорость снижена на 10 пунктов до восстановления повреждения."));
			statusesDictionary.Add(typeof(EngineRoomDamaged), new SpaceshipDamageStatusViewModel("Маневровые двигатели повреждены!", UI.Properties.Resources.DamagedTurn, "Корабль не может поворачивать до восстановления повреждения."));
		}
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			List<SpaceshipStatusViewModel> result = new List<SpaceshipStatusViewModel>();
			SpaceStrategy.GothicSpaceship.CriticalDamageCollection damageCollection = value as SpaceStrategy.GothicSpaceship.CriticalDamageCollection;
			if (damageCollection != null) {
				foreach (var damage in damageCollection) {
					if (statusesDictionary.TryGetValue(damage.GetType(), out var damageViewModel)) {
						result.Add(damageViewModel);
					}
				}
			}
			return result;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	
	public class SpaceshipSideToImageSourceConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Side side = (Side)value;
			switch (side)
			{
				case Side.Left:
					return UI.Properties.Resources.SpaceshipSideLeft.ToBitmapImage();
					break;
				case Side.Front:
					return UI.Properties.Resources.SpaceshipSideFront.ToBitmapImage();
					break;
				case Side.Right:
					return UI.Properties.Resources.SpaceshipSideRight.ToBitmapImage();
					break;
				case Side.Back:
					return null;
					break;
				case Side.LFR:
					return UI.Properties.Resources.SpaceshipSideDorsal.ToBitmapImage();
					break;
				case Side.All:
					return UI.Properties.Resources.SpaceshipSideAll.ToBitmapImage();
					break;
				default:
					return null;
					break;
			}
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

	}
	public class PlayerToPointsViewModelConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			IEnumerable<Player> p = value as IEnumerable<Player>;
			if (p != null) {
				var t = p.Select(a => new CruiserCrashPlayerPointsViewModel(a));
				return p.Select(a => new CruiserCrashPlayerPointsViewModel(a));
			}
			return new List<Player>();
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

	}

	public class CruiserCrashPlayerPointsViewModel : BaseViewModel
	{
		private Game game { get { return Player.Game; } }
		public CruiserCrashPlayerPointsViewModel(Player player)
		{
			this.Player = player;
		}

		public Player Player { get; private set; }
		public int HitPoints
		{
			get
			{
				int result = 0;
				foreach (var player in game.Players) {
					if (player == Player)
						continue;
					foreach (var ss in player.Spaceships) {
						result += ss.Class.HP - ss.HitPoints;
					}
				}
				foreach (var ss in game.DestroyedSpaceships) {
					if (ss.Player != Player) {
						result += ss.Class.HP;
					}
				}
				return result;
			}
		}
		public int CrippledPoints
		{
			get
			{
				int result = 0;
				foreach (var player in game.Players) {
					if (player == Player)
						continue;
					foreach (var ss in player.Spaceships) {
						if (ss.HitPoints < (float)ss.Class.HP / 2.0 && ss.HitPoints>0)
							result += 1;
					}
				}
				return result;
			}
		}
		public int DestroyedPoints
		{
			get
			{
				int result = 0;
				foreach (var player in game.Players) {
					if (player == Player)
						continue;
					foreach (var ss in player.Spaceships) {
						if (ss.HitPoints == 0)
							result += 1;
					}
				}
				foreach (var ss in game.DestroyedSpaceships) {
					if (ss.Player != Player) {
						result += 1;
					}
				}
				return result;
			}
		}
		public int TotalPoints { get { return HitPoints + CrippledPoints + DestroyedPoints*3; } }
	}
}