using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MarkdownCustom
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            CefSettings settings = new CefSettings();
            settings.RegisterScheme(new CefCustomScheme()
            {
                SchemeName = LocalSchemeHandlerFactory.SchemeName,
                SchemeHandlerFactory = new LocalSchemeHandlerFactory()
            });
            Cef.Initialize(settings);

            InitializeComponent();
            var viewModel = DataContext as MarkdownViewModel;

            browser.RegisterJsObject("app", viewModel.Helper);
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Html")
                {
                    browser.ExecuteScriptAsync("reload();");
                }
            };
        }

        private void ListBox_LostFocus(object sender, RoutedEventArgs e)
        {
            expander.IsExpanded = false;
        }

    }
}
