using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;

using PathTracerSharp.Core;
using PathTracerSharp.Rendering;
using PathTracerSharp.Modules.PathTracer;
using PathTracerSharp.Modules.PathTracer.Shapes;

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

        private ObservableCollection<string> Log;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            MouseMove += MainWindow_MouseMove;

            Log = new ObservableCollection<string>();
            LogList.Items.Clear();
            LogList.ItemsSource = Log;

            MainCamera = new Camera(new Vector3(0, 0, 4), Vector3.Zero);
            Scene = new Scene();

            Scene.Shapes.AddRange(new Shape[] {
                new Sphere(new Vector3(-4, -2, 0), .5f, Color.Black),
                new Sphere(new Vector3(-2, -2, 0), .6f, Color.Black),
                new Sphere(new Vector3(0, -2, 0), .7f, Color.Black),
                new Sphere(new Vector3(2, -2, 0), .6f, Color.Black),
                new Sphere(new Vector3(4, -2, 0), .5f, Color.Black),

                new Sphere(new Vector3(-4, 0, 0), .5f, Color.Red),
                new Sphere(new Vector3(-2, 0, 0), .6f, Color.Yellow),
                new Sphere(new Vector3(0, 0, 0), .7f, Color.Green),
                new Sphere(new Vector3(2, 0, 0), .6f, Color.Blue),
                new Sphere(new Vector3(4, 0, 0), .5f, Color.Red),

                new Sphere(new Vector3(-4, 2, 0), .7f, Color.Black),
                new Sphere(new Vector3(-2, 2, 0), .6f, Color.Black),
                new Sphere(new Vector3(0, 2, 0), .5f, Color.Black),
                new Sphere(new Vector3(2, 2, 0), .6f, Color.Black),
                new Sphere(new Vector3(4, 2, 0), .7f, Color.Black),

                //new Sphere(new Vector(1, 1, -3), 2f, Color.Black),
                new Box(new Vector3(1, 1, -3), Vector3.One, Color.Black),
            });

            Scene.Lights.AddRange(new Light[] {
                new Light(new Vector3(4, 2, 3), 10),
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
            Stopwatch timer = new Stopwatch();

            if (Renderer == null) 
            {
                Paint = new Paint(Image, ActualWidth, ActualHeight);
                Renderer = new Renderer(Paint)
                {
                    RenderStart = () => timer.Restart(),
                    RenderComplete = () => Log.Add($"Render frame: {timer.ElapsedMilliseconds} ms")
                };
            }

            Renderer.Render(MainCamera, Scene, Dispatcher);

            //var timer = Stopwatch.StartNew();

            /*MessageBox.Show($"{timer.ElapsedMilliseconds} ms");
            timer.Restart();*/
        }

        private void ShowHideButton_Click(object sender, RoutedEventArgs e)
        {
            SidePanel.Visibility = 
                SidePanel.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }
    }
}
