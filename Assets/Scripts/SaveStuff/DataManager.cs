using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Save system following this tutorial: https://www.youtube.com/watch?v=aUi9aijvpgs

public class DataManager : MonoBehaviour
{
    public string nameOfFile;
    public bool encryptData;
    private FileHandler dataHandler;
    private GameData gameData;

    private List<DataPersistence> dataPersistentObjects;
    public static DataManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        this.dataHandler = new FileHandler(Application.persistentDataPath, nameOfFile, encryptData);
        this.dataPersistentObjects = FindAllDataPersistences();
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void  LoadGame()
    {
        this.gameData = dataHandler.Load();

        if(this.gameData == null)
        {
            NewGame();
        }
        else
        {

        }

        foreach(DataPersistence dataObj in dataPersistentObjects)
        {
            dataObj.LoadData(gameData);
        }

    }

    public void SaveGame()
    {
        foreach(DataPersistence dataObj in dataPersistentObjects)
        {
            dataObj.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<DataPersistence> FindAllDataPersistences()
    {
        IEnumerable<DataPersistence> dataPersistentObjects = FindObjectsOfType<MonoBehaviour>().OfType<DataPersistence>();

        return new List<DataPersistence>(dataPersistentObjects);
    }

}
