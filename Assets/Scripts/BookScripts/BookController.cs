using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class BookController : MonoBehaviour
{
    public GameObject journalPanel;
    public bool isJournalOpen = false;

    public GameObject[] pages;  
    private int currentPage = 0;

    private List<int> unlockedPages = new List<int>();  

    public List<int> alwaysAvailablePages;  

    public InputActionReference nextPageAction;
    public InputActionReference previousPageAction;
    public InputActionReference openJournal;

    private void Start()
    {
        foreach (int pageIndex in alwaysAvailablePages)
        {
            unlockedPages.Add(pageIndex);
            pages[pageIndex].SetActive(true); 
        }

        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == currentPage || alwaysAvailablePages.Contains(i));
        }
    }

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
        if (unlockedPages.Contains(tabIndex))
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
            int nextPageIndex = unlockedPages.IndexOf(currentPage) + 1;

            if (nextPageIndex < unlockedPages.Count)
            {
                pages[currentPage].SetActive(false);
                currentPage = unlockedPages[nextPageIndex];
                pages[currentPage].SetActive(true);
            }
        }
    }

    public void FlipToPreviousPage(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            int prevPageIndex = unlockedPages.IndexOf(currentPage) - 1;

            if (prevPageIndex >= 0)
            {
                pages[currentPage].SetActive(false);
                currentPage = unlockedPages[prevPageIndex];
                pages[currentPage].SetActive(true);
            }
        }
    }

    public void UnlockSpecificPage(int pageIndex)
    {
        if (!unlockedPages.Contains(pageIndex) && pageIndex >= 0 && pageIndex < pages.Length)
        {
            unlockedPages.Insert(pageIndex, pageIndex);
            Debug.Log("Page " + pageIndex + " has been unlocked.");
        }
        else
        {
            Debug.LogWarning("Page " + pageIndex + " is either already unlocked or out of range.");
        }
    }
}
