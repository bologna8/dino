using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region singleton

    public static UIManager instance; 

    public float moveX = 0f;
    public float moveY = 0f; 

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    #endregion

    public Transform canvas;
    public GameObject itemInfoPrefab;
    private GameObject currentItemInfo = null;

    public void DisplayItemInfo(string itemName, string itemDescription, Vector2 buttonPos)
    {
        if(currentItemInfo != null)
        {
            Destroy(currentItemInfo.gameObject);
        }

        buttonPos.x += -moveX;
        buttonPos.y += moveY;

        currentItemInfo = Instantiate(itemInfoPrefab, buttonPos, Quaternion.identity, canvas);
        currentItemInfo.GetComponent<ItemInfo>().SetUp(itemName, itemDescription);
    }

    public void DestroyItemInfo()
    {
        if(currentItemInfo != null)
        {
            Destroy(currentItemInfo.gameObject);
        }
    }
}