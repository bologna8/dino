using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    public static ItemInfo instance;

    public Text itemName;
    public Text itemDescription;
    public GameObject descriptionPanel; 

    private void Awake()
    {
        instance = this;
    }

    public void SetUp(string name, string description)
    {
        itemName.text = name;
        itemDescription.text = description;
    }

    public void DeactivatePanel()
    {
        if (descriptionPanel != null)
        {
           
             Destroy(descriptionPanel);
        }
    }
}
