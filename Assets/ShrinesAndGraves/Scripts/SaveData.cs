using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shrines
{
    public static class SaveData
    {
        [System.Serializable]
        public class SaveFile : global::SaveData
        {
            public string name;
            public Grid grid;

            public bool isValid
            {
                get
                {
                    return !string.IsNullOrEmpty(name) &&
                        grid != null;
                }
            }
        }

        public static SaveFile file;

        public static bool IsFileLoaded
        {
            get
            {
                return file != null && file.isValid;
            }
        }

        public static void Initialize(string name)
        {
            file = new SaveFile() { name = name };
        }

        public static void Save()
        {
            var success = SaveGameSystem.SaveGame(file, file.name);
            if (!success)
            {
                Debug.LogError("File " + file.name + " failed to save!");
            }
        }

        public static void Load(string name)
        {
            file = SaveGameSystem.LoadGame(name) as SaveFile;
            if (file == null)
            {
                Debug.LogError("Failed to load game "+ name+"!");
            }
        }
    }
}