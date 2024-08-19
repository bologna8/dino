using UnityEngine;
using UnityEngine.EventSystems; // Import for EventSystem

public class BookController : MonoBehaviour
{
    public GameObject journalPanel; // Assign your journal UI panel here
    public bool isJournalOpen = false; // Track the state of the journal

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleJournal();
        }
    }

    private void ToggleJournal()
    {
        isJournalOpen = !isJournalOpen;

        if (isJournalOpen)
        {
            OpenJournal();
        }
        else
        {
            CloseJournal();
        }
    }

    private void OpenJournal()
    {
        if (journalPanel != null)
        {
            journalPanel.SetActive(true);
        }
        Time.timeScale = 0f; // Freeze time

        // Ensure cursor is enabled and visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Check if EventSystem is present
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
        Time.timeScale = 1f; // Unfreeze time

        // Hide and lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
