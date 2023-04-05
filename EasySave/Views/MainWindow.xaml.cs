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
using static EasySave.Models.Jobs;

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

        void btnDeleteJob(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel viewModel = new MainWindowViewModel()
            {
                JobName = textBoxJobDelete.Text,
            };

            viewModel.deleteBackup();

            updateGrid();
        }

        void btnExecuteJob(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel viewModel = new MainWindowViewModel()
            {
                JobName = textBoxJobExecute.Text
            };

            bool error = viewModel.executeBackup();

            if (!error) 
            {
                (string jobName, int filePassed, int totalFiles) = viewModel.getThreadInfo(textBoxJobExecute.Text);

                addExecutedJob(jobName, filePassed, totalFiles);
            }
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

            dgLstFile1.ItemsSource = list;
            dgLstFile2.ItemsSource = list;

        }

        void addExecutedJob(string jobName, int filePassed, int totalFiles) 
        {
            // Créez le Border
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(1);
            border.Height = 50;
            border.VerticalAlignment = VerticalAlignment.Top;

            // Créez le Grid
            Grid grid = new Grid();

            // Créez les Labels
            Label label1 = new Label();
            label1.Content = "name : " + jobName;
            label1.HorizontalAlignment = HorizontalAlignment.Left;
            label1.VerticalAlignment = VerticalAlignment.Center;

            Label label2 = new Label();
            label2.Content = filePassed + "/" + totalFiles + " files";
            label2.HorizontalAlignment = HorizontalAlignment.Center;
            label2.VerticalAlignment = VerticalAlignment.Center;

            // Créez le Button
            Button button = new Button();
            button.Content = "Pause";
            button.HorizontalAlignment = HorizontalAlignment.Right;
            button.Margin = new Thickness(7);

            // Ajoutez les contrôles au Grid
            grid.Children.Add(label1);
            grid.Children.Add(label2);
            grid.Children.Add(button);

            // Ajoutez le Grid au Border
            border.Child = grid;

            // Ajoutez le Border au StackPanel
            stackPanel.Children.Add(border);
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