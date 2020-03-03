using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Controls;

using PathTracerSharp.Core;
using PathTracerSharp.Rendering;
using PathTracerSharp.Modules.PathTracer;
using PathTracerSharp.Modules.PathTracer.Shapes;

namespace PathTracerSharp
{
    public partial class RenderPage : Page
    {
        public Renderer Renderer { get; set; }
        public Camera MainCamera { get; set; }
        public Scene Scene { get; set; }

        readonly ObservableCollection<string> Log;
        readonly Stopwatch timer = new Stopwatch();
        private bool IsStarted = false;

        public RenderPage()
        {
            InitializeComponent();

            SidePanel.Visibility = Visibility.Hidden;

            SizeChanged += PageSizeChanged;

            Log = new ObservableCollection<string>();
            LogList.Items.Clear();
            LogList.ItemsSource = Log;

            MainCamera = new Camera(new Vector3(0, 0, 4), Vector3.Zero);
            Scene = new Scene();

            Scene.Shapes.AddRange(new Shape[]
            {
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

            Scene.Lights.AddRange(new Light[]
            {
                new Light(new Vector3(4, 2, 3), 10),
            });
        }

        private void Render()
        {
            Renderer.Render(MainCamera, Scene, Dispatcher);
        }

        private void Start(Type type)
        {
            if (type.IsSubclassOf(typeof(Renderer)))
            {
                if (Renderer == null)
                {
                    Renderer = (Renderer)Activator.CreateInstance(type, new Paint(Image, ActualWidth, ActualHeight));
                    Renderer.RenderStart += () => timer.Restart();
                    Renderer.RenderComplete += () => Log.Add($"Render frame: {timer.ElapsedMilliseconds} ms");
                }

                Render();

                IsStarted = true;
            }
            else 
            {
                throw new ArgumentException("Type does not inherit abstract Renderer", nameof(type));
            }
        }

        private void PageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsStarted) 
            {
                if (Renderer != null)
                {
                    Renderer.Stop();
                    Renderer.Paint = new Paint(Image, ActualWidth, ActualHeight);
                }

                Render();
            }
        }

        private void ShowHideButton_Click(object sender, RoutedEventArgs e)
        {
            SidePanel.Visibility =
                SidePanel.Visibility == Visibility.Visible 
                ? Visibility.Hidden 
                : Visibility.Visible;
        }

        private void RenderButton_Click(object sender, RoutedEventArgs e)
        {
            Render();
        }
    }
}
