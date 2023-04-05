using EasySave.Models;
using EasySave.ViewModels;
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

namespace EasySave
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            updateGrid();
        }

        void btnCreateJob(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel viewModel = new MainWindowViewModel()
            {
                JobName = textBoxJobName.Text,
                PathSource = textBoxJobSource.Text,
                PathTarget = textBoxJobTarget.Text,
                Type = textBoxJobType.Text
            };

            viewModel.createBackup();

            updateGrid();
        }

        void updateGrid()
        {
            List<MainWindowViewModel> list = new List<MainWindowViewModel>();

            MainWindowViewModel viewModel = new MainWindowViewModel();

            List<Jobs.job> jobs = viewModel.loadJobs();


            for (int i = 0; i < jobs.Count; i++)
            {
                list.Add(new MainWindowViewModel()
                {
                    JobName = jobs[i].name,
                    PathSource = jobs[i].pathSource,
                    PathTarget = jobs[i].pathTarget,
                    Type = jobs[i].type
                });
            }

            dgLstFile.ItemsSource = list;

        }

        private void BrowseSource_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = "Sélectionner un dossier";
            dialog.Filter = "Dossier|*.";
            dialog.CheckFileExists = false;
            dialog.CheckPathExists = true;
            dialog.FileName = "Sélectionner un dossier";

            if (dialog.ShowDialog() == true)
            {
                textBoxJobSource.Text = System.IO.Path.GetDirectoryName(dialog.FileName);
            }
        }
        private void BrowseTarget_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = "Sélectionner un dossier";
            dialog.Filter = "Dossier|*.";
            dialog.CheckFileExists = false;
            dialog.CheckPathExists = true;
            dialog.FileName = "Sélectionner un dossier";

            if (dialog.ShowDialog() == true)
            {
                textBoxJobTarget.Text = System.IO.Path.GetDirectoryName(dialog.FileName);
            }
        }
    }
}
