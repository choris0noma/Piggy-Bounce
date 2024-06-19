using CubeHopper.SavingData;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CubeHopper
{
    public class JsonDataService : IDataService
    {
        public bool SaveData<T>(string relativePath, T data, bool isEncrypted)
        {
            string path = Application.persistentDataPath + relativePath;
            try 
            {
                if (File.Exists(path)) File.Delete(path);
                using FileStream stream = File.Create(path);
                stream.Close();
                File.WriteAllText(path, JsonConvert.SerializeObject(data));
                return true;
            }
            catch(Exception e)
            {
                Debug.LogError($"Unable to save data due to: {e.Message} {e.StackTrace}");
                return false;
            }
        }
        
        public T LoadData<T>(string relativePath, bool isEncrypted)
        {
            string path = Application.persistentDataPath + relativePath;
            if (!File.Exists(path)) 
            {
                Debug.LogError("File doesn't exist");
                throw new FileNotFoundException($"");
            }
            try
            {
                T data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
                return data;
            }
            catch (Exception e) 
            {
                Debug.LogError($"Error while loading data: {e.Message} {e.StackTrace}");
                throw e;
            }
        }

    }
}
