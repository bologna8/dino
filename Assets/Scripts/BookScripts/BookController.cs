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

    //Individual is one page, Spread is two pages 
    [System.Serializable]
    public enum PageType
    {
        Individual,
        Spread
    }

    [System.Serializable]
    public class Page
    {
        public GameObject[] pageObjects;  //Prefabs 
        public PageType pageType;         // Page type (individual or book spread)
        public bool isUnlocked;           //bool for if page is unlocked or not
    }

    public Page[] pages;  
    private int currentPageIndex = 0;  

    private List<int> unlockedPages = new List<int>();  
    public List<int> alwaysAvailablePages;
    private List<int> viewedPages = new List<int>();

    public InputActionReference openJournal;

    [System.Serializable]
    public struct Tab
    {
        public Button button; // Button representing tab
        public GameObject unlockedIcon; // Icon to show page unlocked within tab 
        public List<int> associatedPages; // List of pages associated with specific tab
    }

    public Tab[] tabs;

    public Button[] tabButtons; 
    private int currentTabIndex = 0;

    public Button nextPageButton;
    public Button previousPageButton;

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
        //Pages that are available from start will be automatically unlocked and available 
        foreach (int pageIndex in alwaysAvailablePages)
        {
            unlockedPages.Add(pageIndex);
            pages[pageIndex].isUnlocked = true;
        }

        foreach (var page in pages)
        {
            foreach (var pageObject in page.pageObjects)
            {
                pageObject.SetActive(false);
            }
        }

        if (pages.Length > 0)
        {
            DisplayPage(currentPageIndex);
        }

        UpdateTabIcons();

        if (journalPanel != null)
        {
            journalPanel.SetActive(true);
        }

        isJournalOpen = true;
        Cursor.visible = true;
    }

    //why is there two open journals 
    public void OpenJournal(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isJournalOpen)
            {
                CloseJournal();
            }
            else
            {
                if (InventoryUI.Instance != null && InventoryUI.Instance.inventoryOpen)
                {
                    InventoryUI.Instance.CloseInventory();
                }

                OpenJournal();
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

    //This brings the user to a specific page index on a button click.
    public void GoToTab(int tabIndex)
    {
        if (unlockedPages.Contains(tabIndex) || alwaysAvailablePages.Contains(tabIndex))
        {
            DisplayPage(currentPageIndex);

            currentPageIndex = tabIndex;

            Page currentPage = pages[currentPageIndex];

            if (currentPage.pageType == PageType.Spread)
            {
                DisplaySpread(currentPageIndex); 
            }
            else
            {
                DisplayPage(currentPageIndex);
            }
        }
    }

    private void DisplayPage(int pageIndex)
    {
        Page currentPage = pages[pageIndex];

        foreach (var page in pages)
        {
            foreach (var pageObject in page.pageObjects)
            {
                pageObject.SetActive(false);
            }
        }

        if (currentPage.isUnlocked)
        {
            foreach (var pageObject in currentPage.pageObjects)
            {
                pageObject.SetActive(true);
            }

            if(!viewedPages.Contains(pageIndex))
            {
                viewedPages.Add(pageIndex);
                UpdateTabIcons();
            }
        }
        else
        {
            Debug.LogWarning("Page " + pageIndex + " is locked and cannot be displayed.");
        }
    }

    public void DisplaySpread(int spreadIndex)
    {
        foreach (Page page in pages)
        {
            foreach (GameObject pageObject in page.pageObjects)
            {
                pageObject.SetActive(false);
            }
        }

        Page spreadPage = pages[spreadIndex];

        if (!spreadPage.isUnlocked)
        {
            Debug.LogWarning("Spread " + spreadIndex + " is locked and cannot be displayed.");
            return;
        }

        if (spreadPage.pageObjects.Length == 2)
        {
            GameObject leftPage = spreadPage.pageObjects[0]; 
            GameObject rightPage = spreadPage.pageObjects[1]; 

            leftPage.SetActive(true);
            rightPage.SetActive(true);

            RectTransform leftRect = leftPage.GetComponent<RectTransform>();
            RectTransform rightRect = rightPage.GetComponent<RectTransform>();

            leftRect.anchoredPosition = new Vector2(-leftRect.rect.width / 2, leftRect.anchoredPosition.y); //Shift left

            rightRect.anchoredPosition = new Vector2(leftRect.rect.width / 2, rightRect.anchoredPosition.y);  // Shift right

            float gap = 10f; // adjust gap as needed 
            rightRect.anchoredPosition += new Vector2(gap, 0f); 
        }

        if(!viewedPages.Contains(spreadIndex))
        {
            viewedPages.Add(spreadIndex);
            UpdateTabIcons();
        }
    }

    private int GetNextUnlockedPageIndex(int currentIndex)
    {
        int nextIndex = currentIndex + 1;
        while (nextIndex < pages.Length && (!pages[nextIndex].isUnlocked && !alwaysAvailablePages.Contains(nextIndex)))
        {
            nextIndex++;
        }
        return nextIndex < pages.Length ? nextIndex : -1;
    }

    public void FlipToNextPage()
    {
        int nextPageIndex = GetNextUnlockedPageIndex(currentPageIndex);

        if (nextPageIndex != -1)
        {
            DisplayPage(currentPageIndex);

            Page nextPage = pages[nextPageIndex];
            if (nextPage.pageType == PageType.Spread)
            {
                DisplaySpread(nextPageIndex);
            }
            else
            {
                DisplayPage(nextPageIndex);
            }

            currentPageIndex = nextPageIndex;
        }
        else
        {
            Debug.LogWarning("No more available pages to flip to.");
        }
    }

    public void FlipToPreviousPage()
    {
        int prevPageIndex = currentPageIndex - 1;

        while (prevPageIndex >= 0 && (!pages[prevPageIndex].isUnlocked && !alwaysAvailablePages.Contains(prevPageIndex)))
        {
            prevPageIndex--;
        }

        if (prevPageIndex >= 0)
        {
            DisplayPage(currentPageIndex);

            Page currentPage = pages[prevPageIndex];
            if (currentPage.pageType == PageType.Spread)
            {
                DisplaySpread(prevPageIndex); 
            }
            else
            {
                DisplayPage(prevPageIndex);
            }

            currentPageIndex = prevPageIndex;
        }
        else
        {
            Debug.LogWarning("No more available pages to flip to.");
        }
    }

    public void UnlockSpecificPage(int pageIndex)
    {
        if (pages[pageIndex].pageType == PageType.Spread)
        {
            Page spreadPage = pages[pageIndex];
            if (!spreadPage.isUnlocked)
            {
                spreadPage.isUnlocked = true;
                unlockedPages.Add(pageIndex);
                UpdateTabIcons();
            }
        }
        else
        {
            if (!pages[pageIndex].isUnlocked)
            {
                pages[pageIndex].isUnlocked = true;
                unlockedPages.Add(pageIndex);
                UpdateTabIcons();
            }
        }
    }

private void UpdateTabIcons()
{
    foreach (Tab tab in tabs)
    {
        bool anyPageUnlockedAndUnviewed = tab.associatedPages.Any(pageIndex =>
            unlockedPages.Contains(pageIndex) && !viewedPages.Contains(pageIndex));
        
        tab.unlockedIcon.SetActive(anyPageUnlockedAndUnviewed);
         }
    }
}
