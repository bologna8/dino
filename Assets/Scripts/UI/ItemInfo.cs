using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    public static ItemInfo instance;

    public Text itemName;
    public Text itemDescription;
    public GameObject descriptionPanel; // Reference to the description panel GameObject

    private void Awake()
    {
        instance = this;
    }

    public void SetUp(string name, string description)
    {
        itemName.text = name;
        itemDescription.text = description;
    }

    // Method to deactivate or delete the item description panel
    public void DeactivatePanel()
    {
        // You can choose to deactivate or delete the panel based on your requirement
        if (descriptionPanel != null)
        {
           
             Destroy(descriptionPanel);
        }
    }
}
