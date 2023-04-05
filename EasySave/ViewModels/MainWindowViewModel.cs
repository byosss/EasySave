using EasySave.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace EasySave.ViewModels
{
    internal class MainWindowViewModel
    {
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
            Jobs.createJob(this.JobName, this.PathSource, this.PathTarget, this.Type);
        }

        public List<Jobs.job> loadJobs()
        {
            return Jobs.getJobsFromXml();
        }
    }
}
