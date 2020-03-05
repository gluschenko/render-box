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
    public partial class RenderPage : Page
    {
        public Renderer Renderer { get; set; }
        public Camera MainCamera { get; set; }
        public Scene Scene { get; set; }

        private bool IsStarted = false;
        readonly ObservableCollection<string> Log;
        readonly Stopwatch timer = new Stopwatch();

        public RenderPage()
        {
            InitializeComponent();

            SidePanel.Visibility = Visibility.Hidden;

            SizeChanged += PageSizeChanged;

            Log = new ObservableCollection<string>();
            LogList.Items.Clear();
            LogList.ItemsSource = Log;

            //

            var modules = new Type[] { typeof(PathRenderer), typeof(MandelbrotRenderer), typeof(PerlinRenderer) };
                //GetSubclassesOf(typeof(Renderer<RenderContext>));

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

        private void Render()
        {
            Renderer.Render(Dispatcher);
        }

        private void Start(Type type)
        {
            try 
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
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void Update() 
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

        public Type[] GetSubclassesOf(Type type) 
        {
            var subs = type
                .Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(type));

            return subs.ToArray();
        }

        private void PageSizeChanged(object sender, SizeChangedEventArgs e)
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
            Render();
        }
    }
}
