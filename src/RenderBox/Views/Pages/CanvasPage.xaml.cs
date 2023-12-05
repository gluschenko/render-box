using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RenderBox.Services.Options;
using RenderBox.Services.Rendering;
using RenderBox.Shared.Modules.PathTracer;

namespace RenderBox.Views.Pages
{
    public partial class CanvasPage : Page, IDisposable
    {
        public bool IsActive { get; set; }
        public bool IsStarted { get; private set; }

        public Renderer Renderer { get; set; }
        public Camera MainCamera { get; set; }
        public Scene Scene { get; set; }

        private readonly ObservableCollection<string> _log;
        private readonly Stopwatch _timer = new();

        public CanvasPage()
        {
            InitializeComponent();

            Loaded += OnLoaded;

            _log = new ObservableCollection<string>();
            LogList.Items.Clear();
            LogList.ItemsSource = _log;

            //

            var modules = typeof(Renderer).GetSubclasses();

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

                _ = ModulesList.Children.Add(button);
            }
        }

        public void Dispose()
        {
            Renderer?.Dispose();
            GC.SuppressFinalize(this);
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
            var scale = Resolution.Value;
            var w = (int)(ActualWidth * scale);
            var h = (int)(ActualHeight * scale);

            if (Renderer == null)
            {
                Renderer = (Renderer)Activator.CreateInstance(type, new Paint(Image, w, h, scale));
                Renderer.OnRenderStarted += () => _timer.Restart();
                Renderer.OnRenderComplete += () => _log.Add($"Render frame: {_timer.ElapsedMilliseconds} ms");

                var pageType = Renderer.GetOptionPageType();

                if (pageType is not null)
                {
                    var page = Activator.CreateInstance(pageType);
                    var useSource = pageType.GetMethod(nameof(IOptionsPage<Renderer>.UseSource));
                    _ = useSource.Invoke(page, new[] { Renderer });

                    _ = OptionsFrame.Navigate(page);
                }
            }
            else
            {
                Renderer.Reset(new Paint(Image, w, h, scale));
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

        /*public Type[] GetSubclassesOf(Type type) 
        {
            var subs = type
                .Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(type));

            return subs.ToArray();
        }*/

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
                if (ResolutionText != null)
                {
                    ResolutionText.Content = Math.Round(Resolution.Value, 1).ToString(CultureInfo.InvariantCulture);
                }
            }
            catch (Exception)
            {
            }
        }

        private void OnKeyPress(object sender, KeyEventArgs e)
        {
            if (IsActive)
            {
                Renderer?.OnKeyPress(e.Key, Update);
            }
        }
    }

    public static class Extensions
    {
        public static Type[] GetSubclasses(this Type type)
        {
            var subs = type
                .Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(type));

            return subs.ToArray();
        }

        public static Type[] GetImplementations(this Type type)
        {
            var subs = type
                .Assembly.GetTypes()
                .Where(t => type.IsAssignableFrom(t));

            return subs.ToArray();
        }
    }
}
