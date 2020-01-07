using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SpaceStrategy;
using UI.Properties;
using Image = System.Drawing.Image;

namespace SpaceshipStrategy.ViewModels
{
	public static class ImageExtensions
	{
		/// <summary>
		///     Returns a BitmapImage from the passed bitmap.
		/// </summary>
		/// Bitmap to convert.
		public static BitmapImage ToBitmapImage(this Image image)
		{
			var ms = new MemoryStream();
			image.Save(ms, image.RawFormat);
			ms.Seek(0, SeekOrigin.Begin);
			var bi = new BitmapImage();
			bi.BeginInit();
			bi.StreamSource = ms;
			bi.EndInit();
			return bi;
		}
	}


	public static class SpecialOrderButtonBehaviour
	{
		public static readonly DependencyProperty PreviewSpecialOrderAttachedProperty = DependencyProperty.RegisterAttached("PreviewSpecialOrder", typeof(bool), typeof(SpecialOrderButtonBehaviour), new UIPropertyMetadata(false, OnPreviewSpecialOrderAttachedPropertyChanged));
		public static Game Game;

		public static bool GetPreviewSpecialOrder(Button target)
		{
			return (bool)target.GetValue(PreviewSpecialOrderAttachedProperty);
		}

		public static void SetPreviewSpecialOrder(Button target, bool value)
		{
			target.SetValue(PreviewSpecialOrderAttachedProperty, value);
		}

		static void OnPreviewSpecialOrderAttachedPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (Game != null) {
				var b = o as Button;
				if (b != null) {
					var vm = b.DataContext as SpecialOrderCommand;
					if (vm != null) {
						if ((bool)e.NewValue) {
							Game.SelectedSpaceship.PreviewSpecialOrder(vm.Order);
						}
						else {
							Game.SelectedSpaceship.PreviewSpecialOrder(GothicOrder.None);
						}
					}
				}
			}
		}
	}


	public class BaseViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void NotifyPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) {
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}


	public class CruiserCrashPlayerPointsViewModel : BaseViewModel
	{
		public CruiserCrashPlayerPointsViewModel(Player player)
		{
			Player = player;
		}

		public int CrippledPoints
		{
			get
			{
				int result = 0;
				foreach (Player player in Game.Players) {
					if (player == Player) {
						continue;
					}

					foreach (GothicSpaceship ss in player.Spaceships)
						if (ss.HitPoints < ss.Class.HitPoints / 2.0 && ss.HitPoints > 0) {
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
				foreach (Player player in Game.Players) {
					if (player == Player) {
						continue;
					}

					foreach (GothicSpaceship ss in player.Spaceships)
						if (ss.HitPoints == 0) {
							result += 1;
						}
				}

				foreach (GothicSpaceship ss in Game.DestroyedSpaceships)
					if (ss.Player != Player) {
						result += 1;
					}

				return result;
			}
		}

		public int HitPoints
		{
			get
			{
				int result = 0;
				foreach (Player player in Game.Players) {
					if (player == Player) {
						continue;
					}

					foreach (GothicSpaceship ss in player.Spaceships) result += ss.Class.HitPoints - ss.HitPoints;
				}

				foreach (GothicSpaceship ss in Game.DestroyedSpaceships)
					if (ss.Player != Player) {
						result += ss.Class.HitPoints;
					}

				return result;
			}
		}

		public Player Player { get; }

		public int TotalPoints
		{
			get { return HitPoints + CrippledPoints + DestroyedPoints * 3; }
		}

		Game Game
		{
			get { return Player.Game; }
		}
	}


	public class DamageTypeToViewModelConverter : IValueConverter
	{
		static readonly Dictionary<Type, SpaceshipStatusViewModel> _StatusesDictionary;

		static DamageTypeToViewModelConverter()
		{
			_StatusesDictionary = new Dictionary<Type, SpaceshipStatusViewModel>();
			_StatusesDictionary.Add(typeof(FireDamage), new SpaceshipDamageStatusViewModel("Пожар!", Resources.DamagedFire, "Пожар. В конце каждого хода пожар попытаются потушить (1к6). В случае провала корабль получить 1 очко урона."));
			_StatusesDictionary.Add(typeof(ThrustersDamaged), new SpaceshipDamageStatusViewModel("Основные двигатели повреждены!", Resources.DamagedSpeed, "Скорость снижена на 10 пунктов до восстановления повреждения."));
			_StatusesDictionary.Add(typeof(EngineRoomDamaged), new SpaceshipDamageStatusViewModel("Маневровые двигатели повреждены!", Resources.DamagedTurn, "Корабль не может поворачивать до восстановления повреждения."));
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var result = new List<SpaceshipStatusViewModel>();
			var damageCollection = value as GothicSpaceship.CriticalDamageCollection;
			if (damageCollection != null) {
				foreach (CriticalDamageBase damage in damageCollection)
					if (_StatusesDictionary.TryGetValue(damage.GetType(), out SpaceshipStatusViewModel damageViewModel)) {
						result.Add(damageViewModel);
					}
			}

			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}


	public class GameCommand : ICommand
	{
		readonly CommandOnCanExecute _canExecute;

		readonly CommandOnExecute _execute;

		public GameCommand(CommandOnExecute onExecuteMethod, CommandOnCanExecute onCanExecuteMethod)
		{
			_execute = onExecuteMethod;
			_canExecute = onCanExecuteMethod;
		}

		public GameCommand(CommandOnExecute onExecuteMethod, CommandOnCanExecute onCanExecuteMethod, Image image)
		{
			_execute = onExecuteMethod;
			_canExecute = onCanExecuteMethod;
			var c = new ImageSourceConverter();
			Image = image.ToBitmapImage();
		}

		public delegate bool CommandOnCanExecute(object parameter);


		public delegate void CommandOnExecute(object parameter);

		public ImageSource Image { get; }

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

		#endregion ICommand Members
	}


	public class GameViewModel : BaseViewModel
	{
		Game _game;

		public GameViewModel(Game game)
		{
			_game = game;
		}
	}


	public class PlayerToPointsViewModelConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var p = value as IEnumerable<Player>;
			if (p != null) {
				IEnumerable<CruiserCrashPlayerPointsViewModel> t = p.Select(a => new CruiserCrashPlayerPointsViewModel(a));
				return p.Select(a => new CruiserCrashPlayerPointsViewModel(a));
			}

			return new List<Player>();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}


	public class SpaceshipDamageStatusViewModel : SpaceshipStatusViewModel
	{
		public SpaceshipDamageStatusViewModel(string name, Bitmap icon, string description)
			: base(name, icon)
		{
			Description = description;
		}
	}


	public class SpaceshipSideToImageSourceConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var side = (Side)value;
			switch (side) {
				case Side.Left:
					return Resources.SpaceshipSideLeft.ToBitmapImage();
					break;

				case Side.Front:
					return Resources.SpaceshipSideFront.ToBitmapImage();
					break;

				case Side.Right:
					return Resources.SpaceshipSideRight.ToBitmapImage();
					break;

				case Side.Back:
					return null;
					break;

				case Side.LeftFrontRight:
					return Resources.SpaceshipSideDorsal.ToBitmapImage();
					break;

				case Side.All:
					return Resources.SpaceshipSideAll.ToBitmapImage();
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


	public abstract class SpaceshipStatusViewModel
	{
		string _name;

		public SpaceshipStatusViewModel(string name, Bitmap image)
		{
			Image = image.ToBitmapImage();
			_name = name;
		}

		public string Description { get; protected set; }

		public ImageSource Image { get; }

		public string Name { get; set; }
	}


	public class SpecialOrderCommand : GameCommand
	{
		public SpecialOrderCommand() : base(delegate { }, a => true)
		{
		}

		public SpecialOrderCommand(CommandOnExecute onExecuteMethod, CommandOnCanExecute onCanExecuteMethod, Image image, GothicOrder order, string name)
			: base(onExecuteMethod, onCanExecuteMethod, image)
		{
			Order = order;
			Name = name;
		}

		public string Description { get; set; }

		public string Name { get; set; }

		public GothicOrder Order { get; }
	}


	public class SpecialOrdersPanelViewModel : BaseViewModel
	{
		GothicSpaceship _gothicSpaceship;

		public SpecialOrdersPanelViewModel()
		{
			Commands = new List<SpecialOrderCommand>();
			AddCommand(Resources.AllAheadFull, GothicOrder.AllAheadFull, "Полный вперед!");
			AddCommand(Resources.BurnRetros, GothicOrder.BurnRetros, "Полный назад!");
			AddCommand(Resources.ChangeDirection, GothicOrder.ComeToNewDirection, "Резкий поворот!");

			AddCommand(Resources.LaunchTorpedoSalvo, GothicOrder.LaunchOrdnance, "Запустить торпеды!");
			AddCommand(Resources.ReloadOrdnance, GothicOrder.ReloadOrdnance, "Перезарядить торпедные аппараты!");
			AddCommand(Resources.TargetLock, GothicOrder.LockOn, "Держать орудия на цели!");
			AddCommand(Resources.BraceForImpact, GothicOrder.BraceForImpact, "Приготовиться к повреждениям!");

			//NotifyPropertyChanged("Commands");
		}

		public List<SpecialOrderCommand> Commands { get; }

		public GothicSpaceship Spaceship
		{
			get { return _gothicSpaceship; }
			set
			{
				if (_gothicSpaceship != value) {
					if (_gothicSpaceship != null) {
						_gothicSpaceship.PropertyChanged -= OnSpaceshipPropertyChanged;
					}

					_gothicSpaceship = value;
					if (_gothicSpaceship != null) {
						_gothicSpaceship.PropertyChanged += OnSpaceshipPropertyChanged;
					}

					CommandManager.InvalidateRequerySuggested();
					NotifyPropertyChanged("Statuses");
				}
			}
		}

		internal void OnSpaceshipPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "AvailableOrders") {
				CommandManager.InvalidateRequerySuggested();
			}
			else if (e.PropertyName == "CriticalDamage") {
				NotifyPropertyChanged("Statuses");
			}
		}

		void AddCommand(Bitmap icon, GothicOrder order, string name)
		{
			Commands.Add(new SpecialOrderCommand(GiveSpecialOrder, CanGiveSpecialOrder, icon, order, name));
		}

		bool CanGiveSpecialOrder(object param)
		{
			if (param == null) {
				return false;
			}

			if (_gothicSpaceship == null) {
				return false;
			}

			var order = (GothicOrder)param;
			return _gothicSpaceship.AvailableOrders.Contains(order);
		}

		void GiveSpecialOrder(object param)
		{
			var order = (GothicOrder)param;
			_gothicSpaceship.SetSpecialOrder(order);
		}
	}
}