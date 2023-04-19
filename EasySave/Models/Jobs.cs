using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Drawing;
using System.Xml.Linq;
using System.Reflection;
using System.Collections.ObjectModel;

namespace EasySave.Models
{
    internal class Jobs
    {
        public static Dictionary<string, Thread> executedThread = new Dictionary<string, Thread>();
        public static Dictionary<string, bool> threadIsPaused = new Dictionary<string, bool>();

        public static List<string> extensionToPrioritize = new List<string>();// { "docx", "xls" };
        public static List<string> extensionToCrypt = new List<string>();// { "pdf", "txt" };
        public static List<string> _ProcessesList = new List<string>();
        
        static List<string> ExecutedJobsList = new List<string>();
        public static List<Thread> ThreadsList = new List<Thread>();

        static string XORKey = "Saucisse";

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

            state state = new state(JobName, "", "", "END", 0, 0, 0, 0);
            StateFile.addStateFile(state);
        }

        public void deleteJob(job job)
        {
            deleteJobFromXML(job);
            StateFile.deleteStateFile(job.name);
        }

        
        public void executeJob(job job, StackPanel stackPanel)
        {

            
            // MessageBox.Show(Path.GetFullPath(@"..\..\..\Files\CryptoSoft\CryptoSoft.exe"));

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
                thread.Name = job.name;
                ThreadsList.Add(thread);
            }
            else if (job.type == "Differential")
            {
                thread = new Thread(() => executeDiffJob(job, stackPanel));
                thread.Name = job.name;
                ThreadsList.Add(thread);
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
                button.Content = "Run/Pause";
                button.Name = "PauseButton";
                button.HorizontalAlignment = HorizontalAlignment.Right;
                button.Margin = new Thickness(7);
                button.Click += (sender, e) => executedThreadPause(sender, e, job.name);

                grid.Children.Add(label1);
                grid.Children.Add(label2);
                grid.Children.Add(button);

                border.Child = grid;

                stackPanel.Children.Add(border);
            });


            if (Directory.Exists(job.pathTarget))
            {
                Directory.Delete(job.pathTarget, true);
            }


            DirectoryInfo sourceDir = new DirectoryInfo(job.pathSource);
            DirectoryInfo targetDir = new DirectoryInfo(job.pathTarget);

            int count = 0;
            int totalFilesInDir = countFilesInDir(sourceDir.FullName);

            // Créer tous les dossiers et sous-dossiers dans le répertoire cible
            foreach (DirectoryInfo dir in sourceDir.GetDirectories("*", SearchOption.AllDirectories))
            {
                targetDir.CreateSubdirectory(dir.FullName.Substring(sourceDir.FullName.Length + 1));
            }

            PauseJob(job.name);


            // Copier les fichiers prioritaires
            foreach (string ext in extensionToPrioritize)
            {
                foreach (FileInfo sourceFile in sourceDir.GetFiles("*." + ext, SearchOption.AllDirectories))
                {
                    while (threadIsPaused[job.name])  // bouton pause actif
                    {
                        Thread.Sleep(250);
                    }

                    Stopwatch stopWatch = new Stopwatch();  // on démarre un chronomètre
                    stopWatch.Start();

                    string targetFilePath = Path.Combine(targetDir.FullName, sourceFile.FullName.Substring(sourceDir.FullName.Length + 1));  // on copie le file
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath));
                    File.Copy(sourceFile.FullName, targetFilePath, true);   

                    stopWatch.Stop();  // on arrête le chronomètre


                    lock (StateFile.stateFileLock)   // on update le fichier état
                    {
                        float progression = (1f - (((float)totalFilesInDir - (float)count) / (float)totalFilesInDir)) * 100f;
                        state state = new state(job.name, sourceFile.FullName, targetFilePath, "ACTIVE", totalFilesInDir, totalFilesInDir, totalFilesInDir - count, progression);
                        StateFile.updateStateFile(state);
                    }


                    Stopwatch stopWatch2 = new Stopwatch();  // on démarre un chronomètre
                    stopWatch2.Start();

                    if (extensionToCrypt.Contains(sourceFile.Extension.Substring(1)))  // on crypte le file avec CryptoSoft
                    {
                        Process pr = new Process();
                        pr.StartInfo.FileName = Path.GetFullPath(@"..\..\..\Files\CryptoSoft\CryptoSoft.exe");
                        pr.StartInfo.Arguments = targetFilePath + " " + XORKey;
                        pr.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        pr.StartInfo.CreateNoWindow = true;
                        pr.StartInfo.UseShellExecute = false;
                        pr.Start();
                    }

                    stopWatch2.Stop();


                    // on ajoute les infos dans les logs
                    log log = new log(job.name, sourceFile.FullName, targetFilePath, sourceFile.Length.ToString(), stopWatch.Elapsed.TotalSeconds.ToString(), stopWatch2.Elapsed.TotalSeconds.ToString());
                    Logs.addLogs(log);


                    Thread.Sleep(50);
                    count++;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        label2.Content = count.ToString() + "/" + totalFilesInDir.ToString() + " files";
                    });
                }
            }


            // Copier les fichiers non prioritaires
            foreach (FileInfo sourceFile in sourceDir.GetFiles("*", SearchOption.AllDirectories))
            {
                if (!extensionToPrioritize.Contains(sourceFile.Extension.TrimStart('.')))
                {
                    while (threadIsPaused[job.name])  // bouton pause actif
                    {
                        Thread.Sleep(250);
                    }

                    Stopwatch stopWatch = new Stopwatch();  // on démarre un chronomètre
                    stopWatch.Start();


                    string targetFilePath = Path.Combine(targetDir.FullName, sourceFile.FullName.Substring(sourceDir.FullName.Length + 1));  // on copie le file
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath));
                    File.Copy(sourceFile.FullName, targetFilePath, true);


                    stopWatch.Stop();  // on arrête le chronomètre


                    lock (StateFile.stateFileLock)   // on update le fichier état
                    {
                        float progression = (1f - (((float)totalFilesInDir - (float)count) / (float)totalFilesInDir)) * 100f;
                        state state = new state(job.name, sourceFile.FullName, targetFilePath, "ACTIVE", totalFilesInDir, totalFilesInDir, totalFilesInDir - count, progression);
                        StateFile.updateStateFile(state);
                    }


                    Stopwatch stopWatch2 = new Stopwatch();  // on démarre un chronomètre
                    stopWatch2.Start();

                    if (extensionToCrypt.Contains(sourceFile.Extension.Substring(1)))  // on crypte le file avec CryptoSoft
                    {
                        Process pr = new Process();
                        pr.StartInfo.FileName = Path.GetFullPath(@"..\..\..\Files\CryptoSoft\CryptoSoft.exe");
                        pr.StartInfo.Arguments = targetFilePath + " " + XORKey;
                        pr.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        pr.StartInfo.CreateNoWindow = true;
                        pr.StartInfo.UseShellExecute = false;
                        pr.Start();
                    }

                    stopWatch2.Stop();


                    // on ajoute les infos dans les logs
                    log log = new log(job.name, sourceFile.FullName, targetFilePath, sourceFile.Length.ToString(), stopWatch.Elapsed.TotalSeconds.ToString(), stopWatch2.Elapsed.TotalSeconds.ToString());
                    Logs.addLogs(log);


                    Thread.Sleep(50);
                    count++;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        label2.Content = count.ToString() + "/" + totalFilesInDir.ToString() + " files";
                    });
                }
            }

            lock (StateFile.stateFileLock)
            {
                StateFile.updateStateFile(new state(job.name, "", "", "END", 0, 0, 0, 0));
            }
            

            Application.Current.Dispatcher.Invoke(() =>
            {
                stackPanel.Children.Remove(border);
            });

            executedThread.Remove(job.name);
            threadIsPaused.Remove(job.name);
            ThreadsList.RemoveAll(x => x.Name == job.name);

        }

        static void executeDiffJob(job job, StackPanel stackPanel)
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
                button.Content = "Run/Pause";
                button.HorizontalAlignment = HorizontalAlignment.Right;
                button.Margin = new Thickness(7);
                button.Click += (sender, e) => executedThreadPause(sender, e, job.name);

                grid.Children.Add(label1);
                grid.Children.Add(label2);
                grid.Children.Add(button);

                border.Child = grid;

                stackPanel.Children.Add(border);
            });


            if (Directory.Exists(job.pathTarget))
            {
                Directory.Delete(job.pathTarget, true);
            }


            DirectoryInfo sourceDir = new DirectoryInfo(job.pathSource);
            DirectoryInfo targetDir = new DirectoryInfo(job.pathTarget);

            int count = 0;
            int totalFilesInDir = countFilesInDir(sourceDir.FullName);

            // Créer tous les dossiers et sous-dossiers dans le répertoire cible
            foreach (DirectoryInfo dir in sourceDir.GetDirectories("*", SearchOption.AllDirectories))
            {
                targetDir.CreateSubdirectory(dir.FullName.Substring(sourceDir.FullName.Length + 1));
            }

            PauseJob(job.name);

            // Copier les fichiers prioritaires et les autres fichiers
            foreach (string ext in extensionToPrioritize)
            {
                foreach (FileInfo sourceFile in sourceDir.GetFiles("*." + ext, SearchOption.AllDirectories))
                {
                    while (threadIsPaused[job.name])
                    {
                        Thread.Sleep(250);
                    }

                    FileInfo targetFile = new FileInfo(Path.Combine(targetDir.FullName, sourceFile.FullName.Substring(sourceDir.FullName.Length + 1)));

                    if (!targetFile.Exists || sourceFile.LastWriteTimeUtc > targetFile.LastWriteTimeUtc)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(targetFile.FullName));
                        File.Copy(sourceFile.FullName, targetFile.FullName, true);
                    }

                    Thread.Sleep(50);
                    count++;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        label2.Content = count.ToString() + "/" + totalFilesInDir.ToString() + " files";
                    });
                }
            }

            foreach (FileInfo sourceFile in sourceDir.GetFiles("*", SearchOption.AllDirectories))
            {
                if (!extensionToPrioritize.Contains(sourceFile.Extension.TrimStart('.')))
                {
                    while (threadIsPaused[job.name])
                    {
                        Thread.Sleep(250);
                    }

                    FileInfo targetFile = new FileInfo(Path.Combine(targetDir.FullName, sourceFile.FullName.Substring(sourceDir.FullName.Length + 1)));

                    if (!targetFile.Exists || sourceFile.LastWriteTimeUtc > targetFile.LastWriteTimeUtc)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(targetFile.FullName));
                        File.Copy(sourceFile.FullName, targetFile.FullName, true);
                    }

                    Thread.Sleep(50);
                    count++;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        label2.Content = count.ToString() + "/" + countFilesInDir(sourceDir.FullName).ToString() + " files";
                    });
                }
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                stackPanel.Children.Remove(border);
            });

            executedThread.Remove(job.name);
            threadIsPaused.Remove(job.name);
            ThreadsList.RemoveAll(x => x.Name == job.name);
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
            PauseJob(jobName);
        }

        static void PauseJob(string jobName)
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
        

    }
    public struct job
    {
        internal string name { get; set; }
        internal string pathSource { get; set; }
        internal string pathTarget { get; set; }
        internal string type { get; set; }

    }
}
