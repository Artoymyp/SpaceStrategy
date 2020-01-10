using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UI
{
	/// <summary>
	///     Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class SpaceshipSideIcon : UserControl
	{
		// Using a DependencyProperty as the backing store for EndAng. This enables animation, styling, binding, etc...
		public static readonly DependencyProperty EndAngProperty =
			DependencyProperty.Register("EndAng", typeof(double), typeof(SpaceshipSideIconControl), new PropertyMetadata(Math.PI / 2));

		// Using a DependencyProperty as the backing store for Radius. This enables animation, styling, binding, etc...
		public static readonly DependencyProperty RadiusProperty =
			DependencyProperty.Register("Radius", typeof(double), typeof(SpaceshipSideIcon), new PropertyMetadata(50.0));

		// Using a DependencyProperty as the backing store for StartAng. This enables animation, styling, binding, etc...
		public static readonly DependencyProperty StartAngProperty =
			DependencyProperty.Register("StartAng", typeof(double), typeof(SpaceshipSideIconControl), new PropertyMetadata(0.0, InvalidateAreaProperty));

		public SpaceshipSideIcon()
		{
			InitializeComponent();
		}

		public double EndAng
		{
			get { return (double)GetValue(EndAngProperty); }
			set { SetValue(EndAngProperty, value); }
		}

		public PathGeometry Geom
		{
			get
			{
				UpdateLayout();
				double r = Radius / 2;
				var cP = new Point(Radius / 2, Radius / 2);
				var sP = new Point(cP.X + Math.Cos(StartAng) * r, cP.Y + Math.Sin(StartAng) * r);
				var eP = new Point(cP.X + Math.Cos(EndAng) * r, cP.Y + Math.Sin(EndAng) * r);

				var lines = new PathFigure
				{
					StartPoint = sP,
					Segments = new PathSegmentCollection
					{
						new LineSegment(cP, false),
						new LineSegment(eP, false),
						new ArcSegment(sP, new Size(r, r), EndAng - StartAng, false, SweepDirection.Counterclockwise, false)
					}
				};

				return new PathGeometry {Figures = new PathFigureCollection { lines } };
			}
		}

		public double Radius
		{
			get { return (double)GetValue(RadiusProperty); }
			set { SetValue(RadiusProperty, value); }
		}

		public double StartAng
		{
			get { return (double)GetValue(StartAngProperty); }
			set { SetValue(StartAngProperty, value); }
		}

		static void InvalidateAreaProperty(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
	}
}