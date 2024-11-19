using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]

public class GameData
{
    //public string savedName;

    public Vector3 savedPosition;

    public GameData()
    {
        this.savedPosition = new Vector3(-20, 0, 0);
    }

}
