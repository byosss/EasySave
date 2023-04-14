using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EasySave.Models;


namespace EasySave.ViewModels
{
    internal class MainWindowViewModel
    {
        Jobs _jobs;
        stateFile _stateFile;

        public MainWindowViewModel()
        {
            _jobs = new Jobs();
            _stateFile = new stateFile();

            _stateFile.stateFileInnit();
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
