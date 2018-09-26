using System.Windows;
using PostSermonUploader.Views;

namespace PostSermonUploader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var sermonDetailsView = new SermonDetailsView { DataContext = new SermonDetailsViewModel() };
            sermonDetailsView.ShowDialog();
        }
    }
}
