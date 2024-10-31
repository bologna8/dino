using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public class UIStateTracker : MonoBehaviour
{
    public static UIStateTracker Instance { get; private set; }
    public enum UIScreen { Inventory, Journal } //Tracks which screen should be active/focused
    public UIScreen currentScreen = UIScreen.Inventory;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Sets the current active screen 
    public void SetActiveScreen(UIScreen screen)
    {
        currentScreen = screen;
    }

    public UIScreen GetActiveScreen()
    {
        return currentScreen;
    }
}

