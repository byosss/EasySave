using EasySave.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using static EasySave.Models.Jobs;

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

        public void deleteBackup()
        {
            List<Jobs.job> jobs = this.loadJobs();

            for (int i = 0; i < jobs.Count; i++)
            {
                if (jobs[i].name == this.JobName)
                {
                    Jobs.deleteJob(jobs[i]);
                    return;
                }
            }
            MessageBox.Show("job not found");
        }

        public bool executeBackup()
        {
            List<Jobs.job> jobs = this.loadJobs();

            for (int i = 0; i < jobs.Count; i++)
            {
                if (jobs[i].name == this.JobName)
                {
                    // ici fait le check dans le dictionnaire sur chatgpt

                    Jobs.executeJob(jobs[i]);
                    return false;
                }
            }

            MessageBox.Show("job not found");
            return true;
        }

        public (string, int, int) getThreadInfo(String jobName)
        {
            List<Jobs.job> jobs = this.loadJobs();

            for (int i = 0; i < jobs.Count; i++)
            {
                if (jobs[i].name == this.JobName)
                {

                    return (jobs[i].name, 0, Jobs.countFilesInDir(jobs[i].pathSource));
                }
            }

            return ("error", 0, 0);
        }

        public List<Jobs.job> loadJobs()
        {
            return Jobs.getJobsFromXml();
        }
    }
}
