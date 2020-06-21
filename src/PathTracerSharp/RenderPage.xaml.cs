using System;
using System.CodeDom;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PathTracerSharp.Options;
using PathTracerSharp.Pages;
using PathTracerSharp.Rendering;
using PathTracerSharp.Shared.Modules.PathTracer;

namespace PathTracerSharp
{
    public partial class RenderPage : Page, IDisposable
    {
        public bool IsActive { get; set; }
        public bool IsStarted { get; private set; }

        public Renderer Renderer { get; set; }
        public Camera MainCamera { get; set; }
        public Scene Scene { get; set; }

        private readonly ObservableCollection<string> _log;
        private readonly Stopwatch _timer = new Stopwatch();

        public RenderPage()
        {
            InitializeComponent();

            Loaded += OnLoaded;

            _log = new ObservableCollection<string>();
            LogList.Items.Clear();
            LogList.ItemsSource = _log;

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

                button.Click += (s, e) => 
                {
                    Start(module);
                    ModulesListRoot.Visibility = Visibility.Hidden;
                };
                
                ModulesList.Children.Add(button);
            }
        }

        public void Dispose()
        {
            Renderer?.Dispose();
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
                Renderer.RenderStart += () => _timer.Restart();
                Renderer.RenderComplete += () => _log.Add($"Render frame: {_timer.ElapsedMilliseconds} ms");

                var attributes = Renderer.GetType().GetCustomAttributes();
                foreach (var attribute in attributes) 
                {
                    if (attribute is OptionsPageAttribute optionsPageAttribute) 
                    {
                        var page = Activator.CreateInstance(optionsPageAttribute.OptionsPageType);
                        if (page is IOptionsPage optionsPage) 
                        {
                            optionsPage.UseSource(Renderer);
                        }
                        OptionsFrame.Navigate(page);
                    }
                }
            }
            else
            {
                Renderer.Stop();
                Renderer.Paint = new Paint(Image, w, h);
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
