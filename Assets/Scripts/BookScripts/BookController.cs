using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class BookController : MonoBehaviour
{
    public static BookController Instance;
    public GameObject journalPanel;
    public bool isJournalOpen = false;

    public GameObject[] pages;  
    private int currentPage = 0;

    private List<int> unlockedPages = new List<int>();  
    public List<int> alwaysAvailablePages;  

    public InputActionReference openJournal;

    [System.Serializable]
    public struct Tab
    {
        public Button button; //Button representing tab
        public GameObject unlockedIcon; //Icon to show page unlocked within tab 
        public List<int> associatedPages; //List of pages associated with specific tab
    }

    public Tab[] tabs;

    public Button[] tabButtons; 
    private int currentTabIndex = 0;

    public Button nextPageButton;
    public Button previousPageButton;

    public int selectedIndex = 0;

    void Awake()
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

    private void Start()
    {
        foreach (int pageIndex in alwaysAvailablePages)
        {
            unlockedPages.Add(pageIndex);
        }

        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
        } 

        UpdateTabIcons();
    }

    public void OpenJournal(InputAction.CallbackContext context)
    {
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

    public void OpenJournal()
    {
        if (journalPanel != null)
        {
            journalPanel.SetActive(true);
        }

        isJournalOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (EventSystem.current == null)
        {
            Debug.LogError("EventSystem is missing from the scene. Please add it for UI interactions.");
        }

        //Set Journal as the focus screen
        UIStateTracker.Instance.SetActiveScreen(UIStateTracker.UIScreen.Journal);
    }

    public void CloseJournal()
    {
        if (journalPanel != null)
        {
            journalPanel.SetActive(false);
        }

        isJournalOpen = false;

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
        int nextPageIndex = unlockedPages.IndexOf(currentPage) + 1;

        if (nextPageIndex < unlockedPages.Count)
        {
            pages[currentPage].SetActive(false);
            DeactivateTabIconForPage(currentPage);

            currentPage = unlockedPages[nextPageIndex];
            pages[currentPage].SetActive(true);
        }
    }

    public void FlipToPreviousPage()
    {
        int prevPageIndex = unlockedPages.IndexOf(currentPage) - 1;

        if (prevPageIndex >= 0)
        {
            pages[currentPage].SetActive(false);
            DeactivateTabIconForPage(currentPage);

            currentPage = unlockedPages[prevPageIndex];
            pages[currentPage].SetActive(true);
        }
    }

    private void DeactivateTabIconForPage(int pageIndex)
    {
        foreach (Tab tab in tabs)
        {
            if (tab.associatedPages.Contains(pageIndex))
            {
                tab.unlockedIcon.SetActive(false);
            }
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
                //pages[pageIndex].SetActive(true); this was causing the fire ank glitch
                UpdateTabIcons();
            }
            else
            {
                unlockedPages.Add(unlockedPages.Count); // Add to the end if out of range
                Debug.LogWarning("Page " + pageIndex + " is out of range. Added to the next available spot in the queue.");
            }
        }
        else
        {
            Debug.LogWarning("Page " + pageIndex + " is already unlocked.");
        }
    }

    private void UpdateTabIcons()
    {
        foreach (Tab tab in tabs)
        {
            bool anyPageUnlocked = tab.associatedPages.Any(page => unlockedPages.Contains(page));
            tab.unlockedIcon.SetActive(anyPageUnlocked);
        }
    }
}
