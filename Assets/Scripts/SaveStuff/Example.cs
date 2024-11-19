using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Example : MonoBehaviour, DataPersistence
{
    public string currentName;
    public TMP_InputField inputName;

    // Start is called before the first frame update
    void Start()
    {
        if (inputName) { inputName.text = currentName; }
    }

    // Update is called once per frame
    void Update()
    {
        if (inputName) 
        { 
            if (currentName != inputName.text)
            {
                currentName = inputName.text; 
            }
            
        }
    }

    public void LoadData(GameData data)
    {
        //this.currentName = data.savedName;
    }

    public void SaveData(ref GameData data)
    {
        //data.savedName = this.currentName;
    }

}
