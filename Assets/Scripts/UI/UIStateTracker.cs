using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIStateTracker : MonoBehaviour
{
    public static UIStateTracker Instance { get; private set; }
    public enum UIScreen { Inventory, Journal }
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

    public void SwitchToInventory()
    {
        currentScreen = UIScreen.Inventory;
        Debug.Log("Switched to Inventory");
    }

    public void SwitchToJournal()
    {
        currentScreen = UIScreen.Journal;
        Debug.Log("Switched to Journal");
    }

    public UIScreen GetActiveScreen()
    {
        return currentScreen;
    }

    public void SwitchFocus(InputAction.CallbackContext context)
    {
        if (context.performed && context.control.device is Gamepad)
        {
            if (UIStateTracker.Instance.GetActiveScreen() == UIStateTracker.UIScreen.Inventory)
            {
                UIStateTracker.Instance.SwitchToJournal();
            }
            else if (UIStateTracker.Instance.GetActiveScreen() == UIStateTracker.UIScreen.Journal)
            {
                UIStateTracker.Instance.SwitchToInventory();
            }
        }
    }
}
