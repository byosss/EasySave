using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using EasySave.ViewModels;


namespace EasySave.Views
{

    public partial class MainWindow : Window
    {
        MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new MainWindowViewModel();

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

            viewModel.executeBackup(stackPanel);
        }

        void updateGrid()
        {
            List<MainWindowViewModel> list = new List<MainWindowViewModel>();

            MainWindowViewModel viewModel = new MainWindowViewModel();

            list = viewModel.loadJobs();

            dgLstFile1.ItemsSource = list;
            dgLstFile2.ItemsSource = list;
            dgLstFile3.ItemsSource = list;

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