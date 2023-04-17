using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;

namespace EasySave.Models
{
    internal class StateFile
    {

        public static object stateFileLock = new object();

        public StateFile()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EasySave\\EasySave_states.json");


            File.WriteAllText(path, "[]");


            Jobs jobs = new Jobs();

            List<job> elements = jobs.getJobsFromXml();

            foreach (job element in elements)
            {
                state stateFile = new state(element.name, "", "", "", 0, 0, 0, 0);

                addStateFile(stateFile);
            }
        }

        public static void addStateFile(state saveState)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EasySave\\EasySave_states.json");

            if (!File.Exists(path))
            {
                File.WriteAllText(path, "[]");
            }


            //On recupère les données du Json
            string JsonState = File.ReadAllText(path);

            //Transforme le Json en string et met les données dans une list
            var stateList = JsonConvert.DeserializeObject<List<state>>(JsonState);

            //Ajout les données dans la list des logs
            stateList.Add(saveState);

            //Convertit notre liste en un json avec des indentations
            var StringToJson = JsonConvert.SerializeObject(stateList, Newtonsoft.Json.Formatting.Indented);

            //Ecrit les données dans le fichier log
            File.WriteAllText(path, StringToJson);
        }

        public static void updateStateFile(state saveState)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EasySave\\EasySave_states.json");

            if (!File.Exists(path))
            {
                File.WriteAllText(path, "[]");
            }


            //On recupère les données du Json
            string JsonState = File.ReadAllText(path);

            //Transforme le Json en string et met les données dans une list
            List<state> stateList = JsonConvert.DeserializeObject<List<state>>(JsonState);

            for (int i = 0; i < stateList.Count; i++)
            {
                if (stateList[i].Name == saveState.Name)
                {
                    stateList.Insert(i, saveState);
                    stateList.RemoveAt(i + 1);
                    break;
                }
            }


            //Convertit notre liste en un json avec des indentations
            var StringToJson = JsonConvert.SerializeObject(stateList, Newtonsoft.Json.Formatting.Indented);

            File.WriteAllText(path, StringToJson);

            /*
            while (true)
            {
                try
                {
                    //Ecrit les données dans le fichier log
                    File.WriteAllText(path, StringToJson);
                    break;
                }
                catch
                {
                    // The file is still in use - wait and try again
                    Thread.Sleep(25);
                }
                /*catch (IOException)
                {
                }*/
                /*catch (System.UnauthorizedAccessException ex)
                {
                }*/

                /*catch (System.Exception ex)
                {
                }
            }*/
        }

        public static void deleteStateFile(string fileStateName)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EasySave\\EasySave_states.json");

            if (!File.Exists(path))
            {
                File.WriteAllText(path, "[]");
            }


            //On recupère les données du Json
            string JsonState = File.ReadAllText(path);

            //Transforme le Json en string et met les données dans une list
            List<state> stateList = JsonConvert.DeserializeObject<List<state>>(JsonState);

            for (int i = 0; i < stateList.Count; i++)
            {
                if (stateList[i].Name == fileStateName)
                {
                    stateList.RemoveAt(i);
                    break;
                }
            }


            //Convertit notre liste en un json avec des indentations
            var StringToJson = JsonConvert.SerializeObject(stateList, Newtonsoft.Json.Formatting.Indented);

            //Ecrit les données dans le fichier log
            File.WriteAllText(path, StringToJson);
        }

    }

    internal struct state
    {
        public string Name { get; set; }
        public string SourceFilePath { get; set; }
        public string TargetFilePath { get; set; }
        public string State { get; set; }
        public int TotalFilesToCopy { get; set; }
        public int TotalFilesSize { get; set; }
        public int NbFilesLeftToDo { get; set; }
        public float Progression { get; set; }

        public state(string name, string sourceFilePath, string targetFilePath, string state, int totalFilesToCopy, int totalFilesSize, int nbFilesLeftToDo, float progression)
        {
            Name = name;
            SourceFilePath = sourceFilePath;
            TargetFilePath = targetFilePath;
            State = state;
            TotalFilesToCopy = totalFilesToCopy;
            TotalFilesSize = totalFilesSize;
            NbFilesLeftToDo = nbFilesLeftToDo;
            Progression = progression;
        }
    }
}
