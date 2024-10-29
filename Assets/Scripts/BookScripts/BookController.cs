using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;

public class BookController : MonoBehaviour
{
    public GameObject journalPanel;
    public bool isJournalOpen = false;

    public GameObject[] pages;  
    private int currentPage = 0;

    private List<int> unlockedPages = new List<int>();  
    public List<int> alwaysAvailablePages;  

   // public InputActionReference nextPageAction;
    //public InputActionReference previousPageAction;
    public InputActionReference openJournal;

    //For tab navigation
    public Button[] tabButtons; 
    private int currentTabIndex = 0;

    //Page buttons 
    public Button nextPageButton;
    public Button previousPageButton;

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

        //The first tab is highlighted at the start
        HighlightTab(currentTabIndex);

        // Highlight the next/previous page buttons
        HighlightPageButton();
    }

   /* private void OnEnable()
    {
        nextPageAction.action.performed += FlipToNextPage;
        previousPageAction.action.performed += FlipToPreviousPage;
    }

    private void OnDisable()
    {
        nextPageAction.action.performed -= FlipToNextPage;
        previousPageAction.action.performed -= FlipToPreviousPage;
    }*/

    public void OpenJournal(InputAction.CallbackContext context)
    {
        if (context.performed && UIStateTracker.Instance.GetActiveScreen() == UIStateTracker.UIScreen.Journal)
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


        //Highlight the current tab when the journal opens
        HighlightTab(currentTabIndex);
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

    //Switch to the next tab
    public void SwitchToNextTab(InputAction.CallbackContext context)
    {
        if (context.performed && UIStateTracker.Instance.GetActiveScreen() == UIStateTracker.UIScreen.Journal)
        {
            currentTabIndex = (currentTabIndex + 1) % tabButtons.Length; //Wrap around to the first tab
            HighlightTab(currentTabIndex);
        }
    }


    //Switch to the previous tab
    public void SwitchToPreviousTab(InputAction.CallbackContext context)
    {
        if (context.performed && UIStateTracker.Instance.GetActiveScreen() == UIStateTracker.UIScreen.Journal)
        {
            currentTabIndex = (currentTabIndex - 1 + tabButtons.Length) % tabButtons.Length; //Wrap around to the last tab
            HighlightTab(currentTabIndex);
        }
    }

    //Highlight the currently selected tab
    private void HighlightTab(int tabIndex)
    {
        EventSystem.current.SetSelectedGameObject(tabButtons[tabIndex].gameObject);
    }


    private void CloseJournal()
    {
        if (journalPanel != null)
        {
            journalPanel.SetActive(false);
        }
        Time.timeScale = 1f;

        //Cursor.lockState = CursorLockMode.Locked;
        #if !UNITY_EDITOR
        Cursor.visible = false;
        #endif
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

    public void FlipToNextPage()
    {
      //  if (UIStateTracker.Instance.GetActiveScreen() == UIStateTracker.UIScreen.Journal)
        //{
            int nextPageIndex = unlockedPages.IndexOf(currentPage) + 1;

            if (nextPageIndex < unlockedPages.Count)
            {
                pages[currentPage].SetActive(false);
                currentPage = unlockedPages[nextPageIndex];
                pages[currentPage].SetActive(true);
            }
       // }
    }

    public void FlipToPreviousPage()
    {
     //   if (UIStateTracker.Instance.GetActiveScreen() == UIStateTracker.UIScreen.Journal)
        //{
            int prevPageIndex = unlockedPages.IndexOf(currentPage) - 1;

            if (prevPageIndex >= 0)
            {
                pages[currentPage].SetActive(false);
                currentPage = unlockedPages[prevPageIndex];
                pages[currentPage].SetActive(true);
          }
        //}
    }
   
    private void HighlightPageButton()
    {
        if (currentPage == 0)
        {
            EventSystem.current.SetSelectedGameObject(nextPageButton.gameObject);
        }
        else if (currentPage == pages.Length - 1)
        {
            EventSystem.current.SetSelectedGameObject(previousPageButton.gameObject);
        }
        else
        {
            //Default to next page button if on any middle page
            EventSystem.current.SetSelectedGameObject(nextPageButton.gameObject);
        }
    }

    public void UnlockSpecificPage(int pageIndex)
    {
        if (!unlockedPages.Contains(pageIndex))
        {
            if (pageIndex >= 0 && pageIndex < pages.Length)
            {
                int insertIndex = alwaysAvailablePages.Count;  
                for (int i = insertIndex; i < unlockedPages.Count; i++)
                {
                    if (unlockedPages[i] > pageIndex)
                    {
                        insertIndex = i;
                        break;
                    }
                }
                unlockedPages.Insert(insertIndex, pageIndex);
                pages[pageIndex].SetActive(true); 
                Debug.Log("Page " + pageIndex + " has been unlocked and added at the correct position.");
            }
            else
            {
                unlockedPages.Add(unlockedPages.Count);
                Debug.LogWarning("Page " + pageIndex + " is out of range. Added to the next available spot in the queue.");
            }
        }
        else
        {
            Debug.LogWarning("Page " + pageIndex + " is already unlocked.");
        }
    }
}
