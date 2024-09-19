using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BookController : MonoBehaviour
{
    public GameObject journalPanel;
    public bool isJournalOpen = false;

    public GameObject[] pages; 
    private int currentPage = 0;

    public InputActionReference nextPageAction;
    public InputActionReference previousPageAction;

    private void OnEnable()
    {
        nextPageAction.action.performed += FlipToNextPage;
        previousPageAction.action.performed += FlipToPreviousPage;
    }

    private void OnDisable()
    {
        nextPageAction.action.performed -= FlipToNextPage;
        previousPageAction.action.performed -= FlipToPreviousPage;
    }

    private void Update()
    {
        if (isJournalOpen)
        {
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

    public void GoToTab(int tabIndex)
    {
        if (tabIndex >= 0 && tabIndex < pages.Length)
        {
            pages[currentPage].SetActive(false);
            
            currentPage = tabIndex;
            
            pages[currentPage].SetActive(true);
        }
    }

    public void FlipToNextPage(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentPage < pages.Length - 1)
            {
                pages[currentPage].SetActive(false);
                currentPage++;
                pages[currentPage].SetActive(true);
            }
        }
    }

    public void FlipToPreviousPage(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentPage > 0)
            {
                pages[currentPage].SetActive(false);
                currentPage--;
                pages[currentPage].SetActive(true);
            }
        }
    }
}
