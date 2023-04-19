using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EasySave.Models;


namespace EasySave.ViewModels
{
    internal class MainWindowViewModel
    {
        public static string language = "en";
        public static ResourceDictionary dictionary = new ResourceDictionary();
        Jobs _jobs;
        Logs _logs;
        StateFile _stateFile;

        public MainWindowViewModel()
        {
            _jobs = new Jobs();
            _logs = new Logs();
            _stateFile = new StateFile();
            FormatOptions = new List<string> { "JSON", "XML" };
        }
        public List<string> FormatOptions { get; set; }

        private static string _keyFormat = "JSON";

        public string keyFormat
        {
            get
            {
                return _keyFormat;
            }
            set
            {
                _keyFormat = value;
                //OnPropertyChanged(_logFormat);

            }
        }

        private string _jobName = string.Empty;
        public string JobName
        {
            get
            {
                return _jobName;
            }
            set
            {
                _jobName = value;
            }
        }

        private string _pathSource = string.Empty;
        public string PathSource
        {
            get
            {
                return _pathSource;
            }
            set
            {
                _pathSource = value;
            }
        }

        private string _pathTarget = string.Empty;
        public string PathTarget
        {
            get
            {
                return _pathTarget;
            }
            set
            {
                _pathTarget = value;
            }
        }

        private string _type = string.Empty;
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        public List<string> _ProcessesList = new List<string>();

        public void VMtoModelJobsAdd(string text)
        {
            Jobs._ProcessesList.Add(text);
        }

        public void VMtoModelJobsRemove(string text)
        {
            Jobs._ProcessesList.Remove(text);
        }

        public void VMtoModelFilesToEncryptAdd(string text)
        {
            Jobs.extensionToCrypt.Add(text);
        }

        public void VMtoModelFilesToEncryptRemove(string text)
        {
            Jobs.extensionToCrypt.Remove(text);
        }

        public void VMtoModelPriorityFilesAdd(string text)
        {
            Jobs.extensionToPrioritize.Add(text);
        }

        public void VMtoModelPriorityFilesRemove(string text)
        {
            Jobs.extensionToPrioritize.Remove(text);
        }

        public void Timer_Tick(object sender, EventArgs e)
        {
            // Vérifier le contenu de la liste ici
            // Par exemple, afficher un message si la liste contient plus de 10 éléments
                    
            //if ( maliste.Count > 10)
            //{
            //    MessageBox.Show("La liste contient plus de 10 éléments !");
            //}
            foreach (string lProcess in Jobs._ProcessesList)
            {
                
                    if (Process.GetProcessesByName(lProcess).Length > 0)
                    {
                        foreach (Thread lThread in Jobs.ThreadsList)
                        {
                            // Arrêter la tâche en cours si le processus est détecté
                            Jobs.threadIsPaused[lThread.Name] = true;
                            MessageBox.Show("Un logiciel métier empêche la sauvegarde !");
                        }
                    }
                else if (Process.GetProcessesByName(lProcess).Length == 0)// && Jobs.threadIsPaused[lThread.Name] == false)
                {

                }

            }
        }
        public void createBackup()
        {
            _jobs.createJob(this.JobName, this.PathSource, this.PathTarget, this.Type);
        }

        public void deleteBackup()
        {
            List<job> jobs = _jobs.getJobsFromXml();

            for (int i = 0; i < jobs.Count; i++)
            {
                if (jobs[i].name == this.JobName)
                {
                    _jobs.deleteJob(jobs[i]);
                    return;
                }
            }
            MessageBox.Show("job not found");
        }

        public void executeBackup(StackPanel stackPanel)
        {
            List<job> jobs = _jobs.getJobsFromXml();

            for (int i = 0; i < jobs.Count; i++)
            {
                if (jobs[i].name == this.JobName)
                {
                    // ici fait le check dans le dictionnaire
                    if (_jobs.isThreadExecuted(this.JobName))
                    {
                        MessageBox.Show("job already executed");
                        return;
                    }

                    _jobs.executeJob(jobs[i], stackPanel);

                    return;
                }
            }

            MessageBox.Show("job not found");
            return;
        }


        public List<MainWindowViewModel> loadJobs()
        {
            List<MainWindowViewModel> list = new List<MainWindowViewModel>();

            MainWindowViewModel viewModel = new MainWindowViewModel();

            List<job> jobs = _jobs.getJobsFromXml();


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


            return list;
        }


    }

}
