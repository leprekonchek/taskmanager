using System.ComponentModel;
using Lab_05_Lopukhina.Managers;
using Lab_05_Lopukhina.ViewModels;

namespace Lab_05_Lopukhina
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ProcessViewModel();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            StationManager.CloseApp();
        }
    }
}
