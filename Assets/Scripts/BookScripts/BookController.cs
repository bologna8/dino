using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BookController : MonoBehaviour
{
    public GameObject journalPanel; 
    public bool isJournalOpen = false;

    // private void Update()
    //   {
    //      if (Input.GetKeyDown(KeyCode.J))
    //     {
    //         ToggleJournal();
    //     }
    // }

    public void OpenJournal(InputAction.CallbackContext context)
    {
        isJournalOpen = !isJournalOpen;
        if (context.performed)
           {
               if (isJournalOpen)
               {
                   OpenJournal();
               }
               else
               {
                   CloseJournal();
               }
           }

    }

    private void OpenJournal()
    {
        if (journalPanel != null)
        {
            journalPanel.SetActive(true);
        }
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (EventSystem.current == null)
        {
            Debug.LogError("EventSystem is missing from the scene. Please add it for UI interactions.");
        }
    }

    private void CloseJournal()
    {
        if (journalPanel != null)
        {
            journalPanel.SetActive(false);
        }
        Time.timeScale = 1f; 

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
