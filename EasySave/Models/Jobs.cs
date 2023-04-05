using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows;
using EasySave.ViewModels;
using System.Xml.Linq;

namespace EasySave.Models
{
    internal class Jobs
    {

        static public void createJob(string JobName, string PathSource, string PathTarget, string Type)
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

        static public void deleteJobFromXML(job jbIn)
        {
            string path = @"..\..\..\file\jobs.xml";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            var jobs = xmlDoc.SelectSingleNode("jobs");
            var job = jobs.SelectSingleNode("job[name='" + jbIn.name + "']");
            jobs.RemoveChild(job);
            xmlDoc.Save(path);
        }

        static void addJobToXml(job jb)
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
            XmlElement job = xmlDoc.CreateElement("job");
            jobs.AppendChild(job);

            XmlElement name = xmlDoc.CreateElement("name");
            XmlElement source = xmlDoc.CreateElement("source");
            XmlElement target = xmlDoc.CreateElement("target");
            XmlElement type = xmlDoc.CreateElement("type");

            name.InnerText = jb.name;
            source.InnerText = jb.pathSource;
            target.InnerText = jb.pathTarget;
            type.InnerText = jb.type;


            job.AppendChild(name);
            job.AppendChild(source);
            job.AppendChild(target);
            job.AppendChild(type);

            xmlDoc.Save(path);

        }

        public static List<job> getJobsFromXml()
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

        public struct job                                             // Structure d'un travail de sauvegarde | Structure of a backup job
        {
            internal string name { get; set; }
            internal string pathSource { get; set; }
            internal string pathTarget { get; set; }
            internal string type { get; set; }
        }
    }
}
