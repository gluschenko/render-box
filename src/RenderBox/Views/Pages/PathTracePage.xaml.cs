using System.Windows.Controls;
using RenderBox.Services.Options;
using RenderBox.Services.Renderers;

namespace RenderBox.Views.Pages
{
    public partial class PathTracePage : Page, IOptionsPage<PathTraceRenderer>
    {
        private PathTraceRenderer? _source;
        private PathTraceRenderer Source => _source ?? throw new InvalidOperationException($"{nameof(UseSource)} must be called before using this page.");

        public PathTracePage()
        {
            InitializeComponent();
        }

        public void UseSource(PathTraceRenderer source)
        {
            _source = source;

            Lighting.IsChecked = Source.Scene.LightingEnabled;
            Shadows.IsChecked = Source.Scene.ShadowsEnabled;
            SoftShadows.IsChecked = Source.Scene.SoftShadows;
            AmbientOcclusion.IsChecked = Source.Scene.AmbientOcclusion;
            GISamples.Text = Source.Scene.GISamples.ToString();
            FOV.Text = Source.MainCamera.FOV.ToString();
            CameraDistance.Text = Source.MainCamera.MaxDistance.ToString();

            BatchSize.Text = Source.BatchSize.ToString();
        }

        private void ApplyButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Source.Mode = GetRenderMode();
            Source.BatchSize = int.TryParse(BatchSize.Text, out int num) ? num : 0;

            Source.Scene.LightingEnabled = Lighting.IsChecked ?? false;
            Source.Scene.ShadowsEnabled = Shadows.IsChecked ?? false;
            Source.Scene.SoftShadows = SoftShadows.IsChecked ?? false;
            Source.Scene.AmbientOcclusion = AmbientOcclusion.IsChecked ?? false;
            Source.Scene.GISamples = int.TryParse(GISamples.Text, out num) ? num : 0;
            Source.MainCamera.FOV = float.TryParse(FOV.Text, out var f) ? f : 0;
            Source.MainCamera.MaxDistance = float.TryParse(CameraDistance.Text, out f) ? f : 0;

            Source.Render(Dispatcher);
        }

        private RenderMode GetRenderMode()
        {
            foreach (var item in RenderMode.Children)
            {
                if (item is not RadioButton radio)
                {
                    continue;
                }

                if (radio.IsChecked ?? false)
                {
                    return System.Enum.Parse<RenderMode>(radio.Tag?.ToString() ?? "");
                }
            }

            return Services.Renderers.RenderMode.Light;
        }
    }
}
