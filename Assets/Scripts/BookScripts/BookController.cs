using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BookController : MonoBehaviour
{
    public GameObject journalPanel;
    public bool isJournalOpen = false;
    
    public GameObject[] pages; 
    private int currentPage = 0;

    private void Update()
    {
        if (isJournalOpen && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            TurnPage(-1); 
        }
        if (isJournalOpen && Input.GetKeyDown(KeyCode.RightArrow))
        {
            TurnPage(1); 
        }
    }

    public void OpenJournal(InputAction.CallbackContext context)
    {
        if (context.performed)
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

    public void TurnPage(int direction)
    {
        int newPage = Mathf.Clamp(currentPage + direction, 0, pages.Length - 1);

        if (newPage != currentPage)
        {
            pages[currentPage].SetActive(false);
            currentPage = newPage;
            pages[currentPage].SetActive(true);
        }
    }

    public void GoToTab(int tabIndex)
    {
        if (tabIndex >= 0 && tabIndex < pages.Length)
        {
            pages[currentPage].SetActive(false);
            currentPage = tabIndex;
            pages[currentPage].SetActive(true);
        }
    }
    public void OnBookmarkClick(int tabIndex)
{
    GoToTab(tabIndex);
}
}
