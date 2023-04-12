using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;


namespace EasySave.Models
{
    internal class Jobs
    {
        public static Dictionary<string, Thread> executedThread = new Dictionary<string, Thread>();
        public static Dictionary<string, bool> threadIsPaused = new Dictionary<string, bool>();

        public void createJob(string JobName, string PathSource, string PathTarget, string Type)
        {


            if (JobName == "")
            {
                MessageBox.Show("Name vide");
                return;
            }

            List<job> jobs = getJobsFromXml();

            foreach (job job in jobs)
            {
                if (JobName == job.name)
                {
                    MessageBox.Show("Name déjà pris");
                    return;
                }
            }

            if (!System.IO.Directory.Exists(PathSource))
            {
                MessageBox.Show("Source existe pas");
                return;
            }

            if (!System.IO.Directory.Exists(PathTarget))
            {
                MessageBox.Show("Target existe pas");
                return;
            }

            if (PathSource == PathTarget)
            {
                MessageBox.Show("Source égal Target");
                return;
            }



            job jb = new job();
            jb.name = JobName;
            jb.pathSource = PathSource;
            jb.pathTarget = PathTarget;
            jb.type = Type;


            addJobToXml(jb);
        }

        public void deleteJob(job job)
        {
            deleteJobFromXML(job);
        }

        public void executeJob(job job, StackPanel stackPanel)
        {

            if (!Directory.Exists(job.pathSource))
            {
                MessageBox.Show("Le chemin d'accès source n'existe pas.");
                return;
            }
            if (!Directory.Exists(job.pathTarget))
            {
                Directory.CreateDirectory(job.pathTarget);
            }
            Directory.CreateDirectory(job.pathTarget);


            Thread thread = null;
            if (job.type == "Full")
            {
                thread = new Thread(() => executeFullJob(job, stackPanel));
            }
            else if (job.type == "Diff")
            {
                thread = new Thread(() => executeDiffJob(job, stackPanel));
            }


            executedThread.Add(job.name, thread);
            threadIsPaused.Add(job.name, false);
            executedThread[job.name].Start();
        }



        static void executeFullJob(job job, StackPanel stackPanel)
        {

            Border border = null;
            Label label2 = null;

            Application.Current.Dispatcher.Invoke(() =>
            {
                border = new Border();
                border.BorderBrush = Brushes.Black;
                border.BorderThickness = new Thickness(1);
                border.Height = 50;
                border.VerticalAlignment = VerticalAlignment.Top;

                Grid grid = new Grid();

                Label label1 = new Label();
                label1.Content = "name : " + job.name;
                label1.HorizontalAlignment = HorizontalAlignment.Left;
                label1.VerticalAlignment = VerticalAlignment.Center;

                label2 = new Label();
                label2.Content = "0" + "/" + countFilesInDir(job.pathSource).ToString() + " files";
                label2.HorizontalAlignment = HorizontalAlignment.Center;
                label2.VerticalAlignment = VerticalAlignment.Center;

                Button button = new Button();
                button.Content = "Pause";
                button.HorizontalAlignment = HorizontalAlignment.Right;
                button.Margin = new Thickness(7);
                button.Click += (sender, e) => executedThreadPause(sender, e, job.name);

                grid.Children.Add(label1);
                grid.Children.Add(label2);
                grid.Children.Add(button);

                border.Child = grid;

                stackPanel.Children.Add(border);
            });


            //Directory.Delete(job.pathTarget);

            int count = 0;
            CopyAll(new DirectoryInfo(job.pathSource), new DirectoryInfo(job.pathTarget), job.name,label2, ref count, countFilesInDir(job.pathSource));
            //BackupDirectory(new DirectoryInfo(job.pathSource), new DirectoryInfo(job.pathTarget));

            Thread.Sleep(1000);


            Application.Current.Dispatcher.Invoke(() =>
            {
                stackPanel.Children.Remove(border);
            });

            executedThread.Remove(job.name);
            threadIsPaused.Remove(job.name);

        }


        static void executeDiffJob(job job, StackPanel stackPanel)
        {
            MessageBox.Show("ptit diff job oklm");
        }


        private static void CopyAll(DirectoryInfo source, DirectoryInfo target, string jobName, Label label, ref int count, int totalFiles)
        {
            // Copie de tous les fichiers du répertoire source vers le répertoire cible
            foreach (FileInfo file in source.GetFiles())
            {
                while (threadIsPaused[jobName])
                {
                    Thread.Sleep(250);
                }
                Thread.Sleep(50);
                file.CopyTo(Path.Combine(target.FullName, file.Name), true);
                count++;
                int temp = count;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    label.Content = temp.ToString() + "/" + totalFiles.ToString() + " files";
                });
            }

            // Copie de tous les sous-dossiers du répertoire source vers le répertoire cible
            foreach (DirectoryInfo subDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(subDir.Name);
                CopyAll(subDir, nextTargetSubDir, jobName, label, ref count, totalFiles);
            }
        }

        static void BackupDirectory(DirectoryInfo source, DirectoryInfo target)
        {
            if (!source.Exists)
            {
                throw new DirectoryNotFoundException($"Le répertoire source {source.FullName} n'existe pas.");
            }

            if (!target.Exists)
            {
                target.Create();
            }

            foreach (FileInfo file in source.GetFiles())
            {
                string targetFilePath = Path.Combine(target.FullName, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            foreach (DirectoryInfo subDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(subDir.Name);
                BackupDirectory(subDir, nextTargetSubDir);
            }
        }


        static void deleteJobFromXML(job job)
        {
            string path = @"..\..\..\Files\Jobs.xml";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            var jobs = xmlDoc.SelectSingleNode("jobs");
            var jb = jobs.SelectSingleNode("job[name='" + job.name + "']");
            jobs.RemoveChild(jb);
            xmlDoc.Save(path);
        }

        static void addJobToXml(job job)
        {

            string path = @"..\..\..\Files\Jobs.xml";

            if (!File.Exists(path))
            {
                XmlTextWriter doc = new XmlTextWriter(path, System.Text.Encoding.UTF8);
                doc.Formatting = Formatting.Indented;

                doc.WriteStartDocument();

                doc.WriteStartElement("logs");

                doc.WriteEndElement();
                doc.Flush();
                doc.Close();
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);

            XmlNode jobs = xmlDoc.SelectSingleNode("jobs");
            XmlElement jb = xmlDoc.CreateElement("job");
            jobs.AppendChild(jb);

            XmlElement name = xmlDoc.CreateElement("name");
            XmlElement source = xmlDoc.CreateElement("source");
            XmlElement target = xmlDoc.CreateElement("target");
            XmlElement type = xmlDoc.CreateElement("type");

            name.InnerText = job.name;
            source.InnerText = job.pathSource;
            target.InnerText = job.pathTarget;
            type.InnerText = job.type;


            jb.AppendChild(name);
            jb.AppendChild(source);
            jb.AppendChild(target);
            jb.AppendChild(type);

            xmlDoc.Save(path);

        }

        public List<job> getJobsFromXml()
        {
            List<job> jobsInXml = new List<job>();

            XmlDocument xmlDoc = new XmlDocument();
            string path = @"..\..\..\Files\Jobs.xml";
            xmlDoc.Load(path);

            XmlNodeList joblist = xmlDoc.GetElementsByTagName("job");

            for (int i = 0; i < joblist.Count; i++)
            {
                XmlNodeList child = joblist[i].ChildNodes;

                job job = new job();
                job.name = child[0].InnerText;
                job.pathSource = child[1].InnerText;
                job.pathTarget = child[2].InnerText;
                job.type = child[3].InnerText;

                jobsInXml.Add(job);

            }

            return jobsInXml;
        }

        public static int countFilesInDir(string path)
        {
            int count = 0;

            try
            {
                // Récupération des fichiers du répertoire
                string[] files = Directory.GetFiles(path);

                // Incrémentation du compteur pour chaque fichier trouvé
                count += files.Length;

                // Récupération des sous-répertoires
                string[] directories = Directory.GetDirectories(path);

                // Pour chaque sous-répertoire, on appelle récursivement la méthode CountFiles
                foreach (string directory in directories)
                {
                    count += countFilesInDir(directory);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Gestion des erreurs d'autorisation d'accès
            }

            return count;
        }

        public bool isThreadExecuted(string jobName)
        {
            if (executedThread.ContainsKey(jobName))
            {
                return true;
            }
            return false;
        }

        static void executedThreadPause(object sender, RoutedEventArgs e, string jobName)
        {
            if (threadIsPaused[jobName])
            {
                threadIsPaused[jobName] = false;
            }
            else
            {
                threadIsPaused[jobName] = true;
            }
        }


        public struct job
        {
            internal string name { get; set; }
            internal string pathSource { get; set; }
            internal string pathTarget { get; set; }
            internal string type { get; set; }
        }

    }
}
