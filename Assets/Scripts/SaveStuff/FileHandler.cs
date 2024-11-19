using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public bool useEncryption = false;

    private readonly string encryptionCodeWord = "test";

    public FileHandler(string filePath, string fileName, bool encrypted)
    {
        this.dataDirPath = filePath;
        this.dataFileName = fileName;
        this.useEncryption = encrypted;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;

        Debug.Log(fullPath);

        if (File.Exists(fullPath))
        {
            try
            {
                string rawData = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        rawData = reader.ReadToEnd();
                    }
                }

                if (useEncryption) { rawData = EncryptDecrypt(rawData); }

                loadedData = JsonUtility.FromJson<GameData>(rawData);
            }
            catch (Exception e)
            { Debug.Log("Error occured while trying to load file: " + fullPath + "\n" + e);}
        }
        return loadedData;
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try 
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string storeData = JsonUtility.ToJson(data, true);

            if (useEncryption) { storeData = EncryptDecrypt(storeData); }

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(storeData);
                }
            }
        }
        catch (Exception e)
        { Debug.Log("Error occured while trying to save file: " + fullPath + "\n" + e); }
    }

    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }
        return modifiedData;
    }
}
