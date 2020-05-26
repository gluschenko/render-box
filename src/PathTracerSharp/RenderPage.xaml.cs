using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Controls;

using PathTracerSharp.Core;
using PathTracerSharp.Rendering;
using PathTracerSharp.Modules;
using PathTracerSharp.Modules.PathTracer;
using PathTracerSharp.Modules.PathTracer.Shapes;

namespace PathTracerSharp
{
    public partial class RenderPage : Page, IDisposable
    {
        public bool IsActive { get; set; }
        public bool IsStarted { get; private set; }

        public Renderer Renderer { get; set; }
        public Camera MainCamera { get; set; }
        public Scene Scene { get; set; }

        readonly ObservableCollection<string> Log;
        readonly Stopwatch Timer = new Stopwatch();

        public RenderPage()
        {
            InitializeComponent();

            Loaded += OnLoaded;

            Log = new ObservableCollection<string>();
            LogList.Items.Clear();
            LogList.ItemsSource = Log;

            //

            var modules = GetSubclassesOf(typeof(Renderer));

            ModulesList.Children.Clear();
            foreach (var module in modules) 
            {
                var button = new Button
                {
                    Content = module.Name,
                    Margin = new Thickness(5)
                };

                button.Click += (s, e) => {
                    Start(module);
                    ModulesListRoot.Visibility = Visibility.Hidden;
                };
                
                ModulesList.Children.Add(button);
            }
        }

        public void Dispose()
        {
            Renderer.Dispose();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.KeyUp += OnKeyPress;
            //
            SidePanel.Visibility = Visibility.Hidden;
            //
            SizeChanged += OnSizeChanged;
        }

        private void SetupRender(Type type) 
        {
            int w = (int)(ActualWidth * Resolution.Value);
            int h = (int)(ActualHeight * Resolution.Value);

            if (Renderer == null)
            {
                Renderer = (Renderer)Activator.CreateInstance(type, new Paint(Image, w, h));
                Renderer.RenderStart += () => Timer.Restart();
                Renderer.RenderComplete += () => Log.Add($"Render frame: {Timer.ElapsedMilliseconds} ms");
            }
            else
            {
                Renderer.Stop();
                Renderer.Paint = new Paint(Image, w, h);

                /*if (Renderer.Paint.Width != w || Renderer.Paint.Height != h) 
                {
                    Renderer.Paint = new Paint(Image, w, h);
                }*/
            }
        }

        private void Render()
        {
            Renderer.Render(Dispatcher);
        }

        private void Start(Type type)
        {
            if (!IsStarted) 
            {
                SetupRender(type);
                Render();

                IsStarted = true;
            }
        }

        public void Update() 
        {
            if (IsStarted)
            {
                SetupRender(null);
                Render();
            }
        }

        public Type[] GetSubclassesOf(Type type) 
        {
            var subs = type
                .Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(type));

            return subs.ToArray();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Update();
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
            Update();
        }

        private void Resolution_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try 
            {
                if(ResolutionText != null)
                    ResolutionText.Content = Math.Round(Resolution.Value, 1).ToString();
            }
            catch(Exception)
            {
            }
        }

        private void OnKeyPress(object sender, KeyEventArgs e)
        {
            if(IsActive) Renderer?.OnKeyPress(e.Key, Update);
        }
    }
}
