using System.Windows.Controls;
using RenderBox.Options;
using RenderBox.Renderers;

namespace RenderBox.Pages
{
    public partial class PathTracePage : Page, IOptionsPage<PathTraceRenderer>
    {
        private PathTraceRenderer _source;

        public PathTracePage()
        {
            InitializeComponent();
        }

        public void UseSource(PathTraceRenderer source)
        {
            _source = source;

            Lighting.IsChecked = _source.Scene.LightingEnabled;
            Shadows.IsChecked = _source.Scene.ShadowsEnabled;
            SoftShadows.IsChecked = _source.Scene.SoftShadows;
            AmbientOcclusion.IsChecked = _source.Scene.AmbientOcclusion;
            GISamples.Text = _source.Scene.GISamples.ToString();
            FOV.Text = _source.MainCamera.FOV.ToString();
            CameraDistance.Text = _source.MainCamera.MaxDistance.ToString();

            BatchSize.Text = _source.BatchSize.ToString();
        }

        private void ApplyButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _source.Mode = GetRenderMode();
            _source.BatchSize = int.TryParse(BatchSize.Text, out int num) ? num : 0;

            _source.Scene.LightingEnabled = Lighting.IsChecked ?? false;
            _source.Scene.ShadowsEnabled = Shadows.IsChecked ?? false;
            _source.Scene.SoftShadows = SoftShadows.IsChecked ?? false;
            _source.Scene.AmbientOcclusion = AmbientOcclusion.IsChecked ?? false;
            _source.Scene.GISamples = int.TryParse(GISamples.Text, out num) ? num : 0;
            _source.MainCamera.FOV = float.TryParse(FOV.Text, out var f) ? f : 0;
            _source.MainCamera.MaxDistance = float.TryParse(CameraDistance.Text, out f) ? f : 0;

            _source.Render(Dispatcher);
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

            return Renderers.RenderMode.Light;
        }
    }
}
