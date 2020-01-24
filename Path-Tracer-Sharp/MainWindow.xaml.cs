using System;
using System.Windows;
using System.Windows.Input;
using PathTracerSharp.Shapes;
using PathTracerSharp.Rendering;
using Vector = PathTracerSharp.Rendering.Vector;

/*
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Runtime.CompilerServices;
*/

namespace PathTracerSharp
{
    public partial class MainWindow : Window
    {
        public Paint Paint { get; set; }
        public Renderer Renderer { get; set; }
        public Camera MainCamera { get; set; }
        public Scene Scene { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            MouseMove += MainWindow_MouseMove;

            MainCamera = new Camera(new Vector(0, 0, 4), Vector.Zero);
            Scene = new Scene();

            Scene.Shapes.AddRange(new Shape[] {
                new Sphere(new Vector(-4, -2, 0), .5f, Color.Black),
                new Sphere(new Vector(-2, -2, 0), .6f, Color.Black),
                new Sphere(new Vector(0, -2, 0), .7f, Color.Black),
                new Sphere(new Vector(2, -2, 0), .6f, Color.Black),
                new Sphere(new Vector(4, -2, 0), .5f, Color.Black),

                new Sphere(new Vector(-4, 0, 0), .5f, Color.Red),
                new Sphere(new Vector(-2, 0, 0), .6f, Color.Yellow),
                new Sphere(new Vector(0, 0, 0), .7f, Color.Green),
                new Sphere(new Vector(2, 0, 0), .6f, Color.Blue),
                new Sphere(new Vector(4, 0, 0), .5f, Color.Red),

                new Sphere(new Vector(-4, 2, 0), .7f, Color.Black),
                new Sphere(new Vector(-2, 2, 0), .6f, Color.Black),
                new Sphere(new Vector(0, 2, 0), .5f, Color.Black),
                new Sphere(new Vector(2, 2, 0), .6f, Color.Black),
                new Sphere(new Vector(4, 2, 0), .7f, Color.Black),

                //new Sphere(new Vector(1, 1, -3), 2f, Color.Black),
                new Box(new Vector(1, 1, -3), Vector.One, Color.Black),
            });

            Scene.Lights.AddRange(new Light[] {
                new Light(new Vector(4, 2, 3), 10),
            });
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) 
            {
                var pos = e.GetPosition(Image);
                Paint.SetPixel((int)pos.X, (int)pos.Y, Color.Yellow);
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Renderer == null) 
            {
                Paint = new Paint(Image, ActualWidth, ActualHeight);
                Renderer = new Renderer(Paint);
            }

            Renderer.Render(MainCamera, Scene, Dispatcher);

            //var timer = Stopwatch.StartNew();

            /*MessageBox.Show($"{timer.ElapsedMilliseconds} ms");
            timer.Restart();*/
        }
    }
}
