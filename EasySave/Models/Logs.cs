using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using EasySave.ViewModels;

namespace EasySave.Models
{
    internal class Logs
    {
        private static bool isTypeJson;

        internal Logs()
        {

            string path = @"..\..\..\Files\Setup.xml";

            // Charger le document XML
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);

            // Trouver l'élément 'logsFormat'
            XmlNode logsFormatNode = xmlDoc.SelectSingleNode("logsFormat");


            if (logsFormatNode.InnerText == "JSON")
            {
                isTypeJson = true;
            }
            else if (logsFormatNode.InnerText == "XML")
            {
                isTypeJson = false;
            }

            //if (log.keyFormat == "JSON")
            //{
            //    isTypeJson = true;
            //}
            //else if (log.keyFormat == "XML")
            //{
            //    isTypeJson = false;
            //}

        }


        public static void addLogs(log log)
        {

            if (isTypeJson)
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EasySave\\Files", DateTime.Now.ToString("dd-MM-yyyy") + ".json");

                if (!File.Exists(path))
                {
                    File.WriteAllText(path, "[]");
                }

                //On recupère les données du Json
                string JsonLog = File.ReadAllText(path);

                //Transforme le Json en string et met les données dans une list
                var LogList = JsonConvert.DeserializeObject<List<log>>(JsonLog);

                //Ajout les données dans la list des logs
                LogList.Add(log);

                //Convertit notre liste en un json avec des indentations
                var StringToJson = JsonConvert.SerializeObject(LogList, Newtonsoft.Json.Formatting.Indented);

                //Ecrit les données dans le fichier log
                File.WriteAllText(path, StringToJson);

            }
            else if (!isTypeJson)
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EasySave\\Files", DateTime.Now.ToString("dd-MM-yyyy") + ".xml");

                if (!File.Exists(path))
                {

                    XmlTextWriter xDoc = new XmlTextWriter(path, Encoding.UTF8);
                    xDoc.Formatting = System.Xml.Formatting.Indented;

                    xDoc.WriteStartDocument();

                    xDoc.WriteStartElement("logs");

                    xDoc.WriteEndElement();
                    xDoc.Flush();
                    xDoc.Close();

                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);

                XmlNode jobs = xmlDoc.SelectSingleNode("logs");
                XmlElement jb = xmlDoc.CreateElement("log");
                jobs.AppendChild(jb);

                XmlElement name = xmlDoc.CreateElement("name");
                XmlElement source = xmlDoc.CreateElement("source");
                XmlElement target = xmlDoc.CreateElement("target");
                XmlElement size = xmlDoc.CreateElement("size");
                XmlElement transfertTime = xmlDoc.CreateElement("transfertTime");
                XmlElement cryptTime = xmlDoc.CreateElement("cryptTime");
                XmlElement time = xmlDoc.CreateElement("time");

                name.InnerText = log.name;
                source.InnerText = log.source;
                target.InnerText = log.target;
                size.InnerText = log.size;
                transfertTime.InnerText = log.transfertTime;
                cryptTime.InnerText = log.cryptTime;
                time.InnerText = log.time;


                jb.AppendChild(name);
                jb.AppendChild(source);
                jb.AppendChild(target);
                jb.AppendChild(size);
                jb.AppendChild(transfertTime);
                jb.AppendChild(cryptTime);
                jb.AppendChild(time);

                xmlDoc.Save(path);
            }
        }



        public static void updateLogsFormat(string newFormat)
        {
            string path = @"..\..\..\Files\setup.xml";

            // Charger le document XML
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);

            // Trouver l'élément 'logsFormat'
            XmlNode logsFormatNode = xmlDoc.SelectSingleNode("//logsFormat");

            // Modifier la valeur de l'élément
            logsFormatNode.InnerText = newFormat;

            // Enregistrer les modifications dans le fichier XML
            xmlDoc.Save(path);

            if (logsFormatNode.InnerText == "JSON")
            {
                isTypeJson = true;
            }
            else if (logsFormatNode.InnerText == "XML")
            {
                isTypeJson = false;
            }
        }

    }

    internal class MainWindowViwModel
    {
    }

    internal struct log
    {
        public string name { get; set; }
        public string source { get; set; }
        public string target { get; set; }
        public string size { get; set; }
        public string transfertTime { get; set; }
        public string cryptTime { get; set; }
        public string time { get; set; }


        internal log(string _name, string _source, string _target, string _size, string _transfertTime, string _cryptTime)
        {
            this.name = _name;
            this.source = _source;
            this.target = _target;
            this.size = _size;
            this.transfertTime = _transfertTime;
            this.cryptTime = _cryptTime;
            this.time = DateTime.Now.ToString();
        }
    }
}
