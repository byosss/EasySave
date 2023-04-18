using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using EasySave.ViewModels;
using System.Collections.ObjectModel;


namespace EasySave.Views
{

    public partial class MainWindow : Window
    {
        MainWindowViewModel viewModel;

        static List<string> _ProcessesList = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new MainWindowViewModel();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += viewModel.Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1); // Vérifier toutes les 1 seconde
            timer.Start();

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

        
        private void InsertStopProcessButton_Click(object sender, RoutedEventArgs e)
        {
            string text = StopProcessTextBox.Text;
            if (text != "")
            {
                //string filepath_settings = fileHelper.FormatFilePath(fileHelper.filepath_settings);

                //Settings settings = dataHelper.ReadSettingsFromJson(filepath_settings);

                //settings.StopProcesses.Add(text);
                //dataHelper.WriteSettingsToJson(filepath_settings, settings);
                //mainWindow.StopProcessTextBox.Text = "";
                //mainController.UpdateView(mainWindow); // Updating all window

                //viewModel._ProcessesList.Add(text);
                viewModel.VMtoModelJobsAdd(text);
                StopProcessListBox.Items.Add(text);
                StopProcessTextBox.Clear();
            }
        }

        private void DeleteStopProcessButton_Click(object sender, RoutedEventArgs e)
        {
            string text = StopProcessListBox.SelectedItem.ToString();
            if (text != "")
            {
                viewModel.VMtoModelJobsRemove(text);
                int selectedIndex = StopProcessListBox.SelectedIndex;
                if (selectedIndex != -1)
                {
                    StopProcessListBox.Items.RemoveAt(selectedIndex);
                }
            }
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