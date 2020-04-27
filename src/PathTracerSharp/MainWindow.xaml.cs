using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace PathTracerSharp
{
    public partial class MainWindow : Window
    {
        private int TabCounter;
        private Dictionary<TabItem, RenderPage> Pages;

        public MainWindow()
        {
            InitializeComponent();

            Pages = new Dictionary<TabItem, RenderPage>();

            Tabs.SelectionChanged += OnTabsSelectionChanged;

            AddTab();
        }

        private void AddTab() 
        {
            var page = new RenderPage();

            int idx = Tabs.Items.Count;
            Tabs.SelectedIndex = idx - 1;
            var item = new TabItem()
            {
                Header = $"Render #{++TabCounter}",
                Content = new Frame() { Content = page }
            };

            item.MouseRightButtonUp += onItemClick;

            Pages.Add(item, page);
            Tabs.Items.Insert(idx - 1, item);
            Tabs.SelectedIndex = Tabs.Items.Count - 2;

            void onItemClick(object sender, MouseButtonEventArgs e) 
            {
                if (Tabs.Items.Count > 2)
                {
                    Pages[item].Dispose();
                    Pages.Remove(item);
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
            foreach (var pair in Pages)
            {
                pair.Value.IsActive = pair.Key.IsSelected;
            }
        }
    }
}
