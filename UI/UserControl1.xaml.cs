using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UI
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class SpaceshipSideIcon : UserControl
	{
		public SpaceshipSideIcon()
		{
			InitializeComponent();
		}


		public PathGeometry Geom
		{
			get
			{
				UpdateLayout();
				double r = Radius / 2;
				Point cP = new Point(Radius / 2, Radius / 2);
				Point sP = new Point(cP.X + Math.Cos(StartAng) * r, cP.Y + Math.Sin(StartAng) * r);
				Point eP = new Point(cP.X + Math.Cos(EndAng) * r, cP.Y + Math.Sin(EndAng) * r);
				
				PathFigure lines = new PathFigure();
				lines.StartPoint = sP;

				PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();
				myPathSegmentCollection.Add(new LineSegment(cP,false));
				myPathSegmentCollection.Add(new LineSegment(eP, false));
				myPathSegmentCollection.Add(new ArcSegment(sP,new Size(r,r),EndAng-StartAng,false,SweepDirection.Counterclockwise,false));
				lines.Segments = myPathSegmentCollection;

				PathFigureCollection myPathFigureCollection = new PathFigureCollection();
				myPathFigureCollection.Add(lines);

				PathGeometry pathGeometry = new PathGeometry();
				pathGeometry.Figures = myPathFigureCollection;
				return pathGeometry;
			}
		}


		public double Radius
		{
			get { return (double)GetValue(RadiusProperty); }
			set { SetValue(RadiusProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Radius.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty RadiusProperty =
			DependencyProperty.Register("Radius", typeof(double), typeof(SpaceshipSideIcon), new PropertyMetadata(50.0));


		public double StartAng
		{
			get { return (double)GetValue(StartAngProperty); }
			set { SetValue(StartAngProperty, value); }
		}

		// Using a DependencyProperty as the backing store for StartAng.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty StartAngProperty =
			DependencyProperty.Register("StartAng", typeof(double), typeof(SpaceshipSideIconControl), new PropertyMetadata(0.0, new PropertyChangedCallback(InvalidateAreaProperty)));

		static void InvalidateAreaProperty(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			
		}

		public double EndAng
		{
			get { return (double)GetValue(EndAngProperty); }
			set { SetValue(EndAngProperty, value); }
		}

		// Using a DependencyProperty as the backing store for EndAng.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty EndAngProperty =
			DependencyProperty.Register("EndAng", typeof(double), typeof(SpaceshipSideIconControl), new PropertyMetadata(Math.PI / 2));


	}
}
