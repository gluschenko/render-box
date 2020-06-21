using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Linq;

namespace PathTracerSharp
{
    public partial class MainWindow : Window
    {
        private int _tabCounter;
        private readonly Dictionary<TabItem, RenderPage> _pages;

        public MainWindow()
        {
            InitializeComponent();

            _pages = new Dictionary<TabItem, RenderPage>();

            Tabs.SelectionChanged += OnTabsSelectionChanged;

            AddTab();

            Closing += OnClosing;
        }

        private void AddTab() 
        {
            var page = new RenderPage();

            int idx = Tabs.Items.Count;
            Tabs.SelectedIndex = idx - 1;
            var item = new TabItem()
            {
                Header = $"Render #{++_tabCounter}",
                Content = new Frame() { Content = page }
            };

            item.MouseRightButtonUp += onItemClick;

            _pages.Add(item, page);
            Tabs.Items.Insert(idx - 1, item);
            Tabs.SelectedIndex = Tabs.Items.Count - 2;

            void onItemClick(object sender, MouseButtonEventArgs e) 
            {
                if (Tabs.Items.Count > 2)
                {
                    _pages[item].Dispose();
                    _pages.Remove(item);
                    Tabs.Items.Remove(item);
                    Tabs.SelectedIndex = Tabs.Items.Count - 2;
                }
            }
        }

        private void OnPlusClick(object sender, MouseButtonEventArgs e)
        {
            AddTab();
            e.Handled = true;
        }

        private void OnTabsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var pair in _pages)
            {
                pair.Value.IsActive = pair.Key.IsSelected;
            }
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var renderers = _pages.Select(x => x.Value).Select(x => x.Renderer).ToArray();

            foreach (var renderer in renderers) 
            {
                renderer?.Dispose();
            }
        }
    }
}
