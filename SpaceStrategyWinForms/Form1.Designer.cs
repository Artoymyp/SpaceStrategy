namespace SpaceStrategyWinForms
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.spaceshipLibraryGridView = new System.Windows.Forms.DataGridView();
			this.spaceshipClassDataTableBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.spaceshipClassDataTableBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.startGameButton = new System.Windows.Forms.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.panel1 = new System.Windows.Forms.Panel();
			this.specialOrderPanel = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.turnEndButton = new System.Windows.Forms.Button();
			this.spaceshipInfoHost = new System.Windows.Forms.Integration.ElementHost();
			this.spaceshipInfo1 = new UI.SpaceshipInfo();
			this.CommandPanel = new System.Windows.Forms.Integration.ElementHost();
			this.commandPanel1 = new UI.CommandPanelControl();
			this.starfieldControl1 = new UI.StarfieldControl();
			this.gameDataAdapterBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.SpaceshipClassColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SpaceshipTypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SpaceshipHPColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SpaceshipSpeedColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SpaceshipMinRunBeforeTurnColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SpaceshipMaxTurnAngleColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SpaceshipShield = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.FrontArmorColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SideArmorColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SpaceshipTurretCountColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SpaceshipPointsColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.spaceshipLibraryGridView)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spaceshipClassDataTableBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spaceshipClassDataTableBindingSource1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gameDataAdapterBindingSource)).BeginInit();
			this.SuspendLayout();
			// 
			// spaceshipLibraryGridView
			// 
			this.spaceshipLibraryGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.spaceshipLibraryGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SpaceshipClassColumn,
            this.SpaceshipTypeColumn,
            this.SpaceshipHPColumn,
            this.SpaceshipSpeedColumn,
            this.SpaceshipMinRunBeforeTurnColumn,
            this.SpaceshipMaxTurnAngleColumn,
            this.SpaceshipShield,
            this.FrontArmorColumn,
            this.SideArmorColumn,
            this.SpaceshipTurretCountColumn,
            this.SpaceshipPointsColumn});
			this.spaceshipLibraryGridView.Location = new System.Drawing.Point(2, 262);
			this.spaceshipLibraryGridView.Name = "spaceshipLibraryGridView";
			this.spaceshipLibraryGridView.Size = new System.Drawing.Size(508, 260);
			this.spaceshipLibraryGridView.TabIndex = 0;
			this.spaceshipLibraryGridView.Visible = false;
			this.spaceshipLibraryGridView.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
			// 
			// spaceshipClassDataTableBindingSource
			// 
			this.spaceshipClassDataTableBindingSource.DataSource = typeof(SpaceStrategy.GameDataSet.SpaceshipClassDataTable);
			// 
			// spaceshipClassDataTableBindingSource1
			// 
			this.spaceshipClassDataTableBindingSource1.DataSource = typeof(SpaceStrategy.GameDataSet.SpaceshipClassDataTable);
			// 
			// comboBox1
			// 
			this.comboBox1.Enabled = false;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(168, 204);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(232, 21);
			this.comboBox1.TabIndex = 1;
			this.comboBox1.Visible = false;
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// startGameButton
			// 
			this.startGameButton.Location = new System.Drawing.Point(313, 231);
			this.startGameButton.Name = "startGameButton";
			this.startGameButton.Size = new System.Drawing.Size(101, 66);
			this.startGameButton.TabIndex = 6;
			this.startGameButton.Text = "Начать игру";
			this.startGameButton.UseVisualStyleBackColor = true;
			this.startGameButton.Click += new System.EventHandler(this.startGameButton_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.panel1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.CommandPanel);
			this.splitContainer1.Panel2.Controls.Add(this.startGameButton);
			this.splitContainer1.Panel2.Controls.Add(this.comboBox1);
			this.splitContainer1.Panel2.Controls.Add(this.spaceshipLibraryGridView);
			this.splitContainer1.Panel2.Controls.Add(this.starfieldControl1);
			this.splitContainer1.Size = new System.Drawing.Size(790, 692);
			this.splitContainer1.SplitterDistance = 165;
			this.splitContainer1.TabIndex = 7;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.spaceshipInfoHost);
			this.panel1.Controls.Add(this.specialOrderPanel);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.turnEndButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(165, 692);
			this.panel1.TabIndex = 15;
			// 
			// specialOrderPanel
			// 
			this.specialOrderPanel.AutoSize = true;
			this.specialOrderPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.specialOrderPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.specialOrderPanel.Location = new System.Drawing.Point(0, 692);
			this.specialOrderPanel.Name = "specialOrderPanel";
			this.specialOrderPanel.Size = new System.Drawing.Size(165, 0);
			this.specialOrderPanel.TabIndex = 15;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.Location = new System.Drawing.Point(0, 64);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(84, 13);
			this.label1.TabIndex = 7;
			this.label1.Text = "Текущий игрок";
			this.label1.Visible = false;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Dock = System.Windows.Forms.DockStyle.Top;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label2.Location = new System.Drawing.Point(0, 38);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(161, 26);
			this.label2.TabIndex = 8;
			this.label2.Text = "Текущий игрок";
			this.label2.Visible = false;
			// 
			// turnEndButton
			// 
			this.turnEndButton.AutoSize = true;
			this.turnEndButton.Dock = System.Windows.Forms.DockStyle.Top;
			this.turnEndButton.Location = new System.Drawing.Point(0, 0);
			this.turnEndButton.Name = "turnEndButton";
			this.turnEndButton.Size = new System.Drawing.Size(165, 38);
			this.turnEndButton.TabIndex = 6;
			this.turnEndButton.Text = "Завершить ход";
			this.turnEndButton.UseVisualStyleBackColor = true;
			this.turnEndButton.Visible = false;
			this.turnEndButton.Click += new System.EventHandler(this.turnEndButton_Click);
			// 
			// spaceshipInfoHost
			// 
			this.spaceshipInfoHost.AutoSize = true;
			this.spaceshipInfoHost.Dock = System.Windows.Forms.DockStyle.Top;
			this.spaceshipInfoHost.Location = new System.Drawing.Point(0, 77);
			this.spaceshipInfoHost.Name = "spaceshipInfoHost";
			this.spaceshipInfoHost.Size = new System.Drawing.Size(165, 78);
			this.spaceshipInfoHost.TabIndex = 16;
			this.spaceshipInfoHost.Text = "elementHost1";
			this.spaceshipInfoHost.Child = this.spaceshipInfo1;
			// 
			// CommandPanel
			// 
			this.CommandPanel.Location = new System.Drawing.Point(0, 0);
			this.CommandPanel.Name = "CommandPanel";
			this.CommandPanel.Size = new System.Drawing.Size(315, 45);
			this.CommandPanel.TabIndex = 7;
			this.CommandPanel.Text = "elementHost1";
			this.CommandPanel.Child = this.commandPanel1;
			// 
			// starfieldControl1
			// 
			this.starfieldControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.starfieldControl1.GameEngine = null;
			this.starfieldControl1.Location = new System.Drawing.Point(0, 0);
			this.starfieldControl1.Name = "starfieldControl1";
			this.starfieldControl1.Size = new System.Drawing.Size(621, 692);
			this.starfieldControl1.TabIndex = 2;
			this.starfieldControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.starfieldControl1_KeyDown);
			this.starfieldControl1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.starfieldControl1_MouseClick);
			this.starfieldControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.starfieldControl1_MouseDown);
			this.starfieldControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.starfieldControl1_MouseMove);
			// 
			// gameDataAdapterBindingSource
			// 
			this.gameDataAdapterBindingSource.DataSource = typeof(SpaceStrategy.GameDataAdapter);
			// 
			// SpaceshipClassColumn
			// 
			this.SpaceshipClassColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.SpaceshipClassColumn.DataPropertyName = "ClassName";
			this.SpaceshipClassColumn.HeaderText = "Класс";
			this.SpaceshipClassColumn.Name = "SpaceshipClassColumn";
			this.SpaceshipClassColumn.Width = 63;
			// 
			// SpaceshipTypeColumn
			// 
			this.SpaceshipTypeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.SpaceshipTypeColumn.DataPropertyName = "Type";
			this.SpaceshipTypeColumn.HeaderText = "Тип";
			this.SpaceshipTypeColumn.Name = "SpaceshipTypeColumn";
			this.SpaceshipTypeColumn.Width = 51;
			// 
			// SpaceshipHPColumn
			// 
			this.SpaceshipHPColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
			this.SpaceshipHPColumn.DataPropertyName = "HP";
			this.SpaceshipHPColumn.HeaderText = "HP";
			this.SpaceshipHPColumn.MinimumWidth = 25;
			this.SpaceshipHPColumn.Name = "SpaceshipHPColumn";
			this.SpaceshipHPColumn.Width = 25;
			// 
			// SpaceshipSpeedColumn
			// 
			this.SpaceshipSpeedColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
			this.SpaceshipSpeedColumn.DataPropertyName = "Speed";
			this.SpaceshipSpeedColumn.HeaderText = "Скорость";
			this.SpaceshipSpeedColumn.MinimumWidth = 35;
			this.SpaceshipSpeedColumn.Name = "SpaceshipSpeedColumn";
			this.SpaceshipSpeedColumn.Width = 35;
			// 
			// SpaceshipMinRunBeforeTurnColumn
			// 
			this.SpaceshipMinRunBeforeTurnColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
			this.SpaceshipMinRunBeforeTurnColumn.DataPropertyName = "MinRunBeforeTurn";
			this.SpaceshipMinRunBeforeTurnColumn.HeaderText = "Расстояние до поворота";
			this.SpaceshipMinRunBeforeTurnColumn.MinimumWidth = 40;
			this.SpaceshipMinRunBeforeTurnColumn.Name = "SpaceshipMinRunBeforeTurnColumn";
			this.SpaceshipMinRunBeforeTurnColumn.Width = 40;
			// 
			// SpaceshipMaxTurnAngleColumn
			// 
			this.SpaceshipMaxTurnAngleColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
			this.SpaceshipMaxTurnAngleColumn.DataPropertyName = "MaxTurnAngle";
			this.SpaceshipMaxTurnAngleColumn.HeaderText = "Поворот";
			this.SpaceshipMaxTurnAngleColumn.MinimumWidth = 40;
			this.SpaceshipMaxTurnAngleColumn.Name = "SpaceshipMaxTurnAngleColumn";
			this.SpaceshipMaxTurnAngleColumn.Width = 40;
			// 
			// SpaceshipShield
			// 
			this.SpaceshipShield.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
			this.SpaceshipShield.DataPropertyName = "Shield";
			this.SpaceshipShield.HeaderText = "Щит";
			this.SpaceshipShield.MinimumWidth = 35;
			this.SpaceshipShield.Name = "SpaceshipShield";
			this.SpaceshipShield.Width = 35;
			// 
			// FrontArmorColumn
			// 
			this.FrontArmorColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
			this.FrontArmorColumn.DataPropertyName = "FrontArmor";
			this.FrontArmorColumn.HeaderText = "Передняя броня";
			this.FrontArmorColumn.MinimumWidth = 40;
			this.FrontArmorColumn.Name = "FrontArmorColumn";
			this.FrontArmorColumn.Width = 40;
			// 
			// SideArmorColumn
			// 
			this.SideArmorColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
			this.SideArmorColumn.DataPropertyName = "LeftArmor";
			this.SideArmorColumn.HeaderText = "Боковая броня";
			this.SideArmorColumn.MinimumWidth = 40;
			this.SideArmorColumn.Name = "SideArmorColumn";
			this.SideArmorColumn.Width = 40;
			// 
			// SpaceshipTurretCountColumn
			// 
			this.SpaceshipTurretCountColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
			this.SpaceshipTurretCountColumn.DataPropertyName = "TurretsPower";
			this.SpaceshipTurretCountColumn.HeaderText = "Турели";
			this.SpaceshipTurretCountColumn.MinimumWidth = 45;
			this.SpaceshipTurretCountColumn.Name = "SpaceshipTurretCountColumn";
			this.SpaceshipTurretCountColumn.Width = 45;
			// 
			// SpaceshipPointsColumn
			// 
			this.SpaceshipPointsColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
			this.SpaceshipPointsColumn.DataPropertyName = "Points";
			this.SpaceshipPointsColumn.HeaderText = "Очки";
			this.SpaceshipPointsColumn.MinimumWidth = 35;
			this.SpaceshipPointsColumn.Name = "SpaceshipPointsColumn";
			this.SpaceshipPointsColumn.Width = 35;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(790, 692);
			this.Controls.Add(this.splitContainer1);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.spaceshipLibraryGridView)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spaceshipClassDataTableBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spaceshipClassDataTableBindingSource1)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.gameDataAdapterBindingSource)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView spaceshipLibraryGridView;
		private System.Windows.Forms.DataGridViewTextBoxColumn idDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn classDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn typeDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn hPDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn shieldDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn frontArmorDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn leftArmorDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn rightArmorDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn speedDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn minTurnRadiusDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn raceDataGridViewTextBoxColumn;
		private System.Windows.Forms.BindingSource spaceshipClassDataTableBindingSource;
		private System.Windows.Forms.BindingSource spaceshipClassDataTableBindingSource1;
		private System.Windows.Forms.BindingSource gameDataAdapterBindingSource;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Timer timer1;
		private UI.StarfieldControl starfieldControl1;
		private System.Windows.Forms.Button startGameButton;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Button turnEndButton;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel specialOrderPanel;
		private System.Windows.Forms.Integration.ElementHost CommandPanel;
		private UI.CommandPanelControl commandPanel1;
		private System.Windows.Forms.Integration.ElementHost spaceshipInfoHost;
		private UI.SpaceshipInfo spaceshipInfo1;
		private System.Windows.Forms.DataGridViewTextBoxColumn SpaceshipClassColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn SpaceshipTypeColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn SpaceshipHPColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn SpaceshipSpeedColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn SpaceshipMinRunBeforeTurnColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn SpaceshipMaxTurnAngleColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn SpaceshipShield;
		private System.Windows.Forms.DataGridViewTextBoxColumn FrontArmorColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn SideArmorColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn SpaceshipTurretCountColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn SpaceshipPointsColumn;
	}
}

