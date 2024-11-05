using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;

public class BookController : MonoBehaviour
{

    public static BookController Instance;
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

    //Item slots in journal to index through 
    public List<ItemSlot> journalSlotList = new List<ItemSlot>();
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

    public void AddRecipeToJournal(List<ItemSlot> recipeAndIngredients)
    {
        foreach (ItemSlot item in recipeAndIngredients)
        {
            journalSlotList.Add(item);
        }
    }
    //Note: Something I noticed is that the pages don't really need to be set to active at the start since they are set to active when opening that page anyway,
    //them all being active caused some weird issues with the pages overlapping each other.
        private void Start()
    {
        foreach (int pageIndex in alwaysAvailablePages)
        {
            unlockedPages.Add(pageIndex);
            //pages[pageIndex].SetActive(true); 
        }

        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
           // pages[i].SetActive(i == currentPage || alwaysAvailablePages.Contains(i));
        } 

        //The first tab is highlighted at the start
        //HighlightTab(currentTabIndex);

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


        //Highlight the current tab when the journal opens
       // HighlightTab(currentTabIndex);
    }

    public void OpenJournal()
    {
        if (journalPanel != null)
        {
            journalPanel.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (EventSystem.current == null)
        {
            Debug.LogError("EventSystem is missing from the scene. Please add it for UI interactions.");
        }
        //Set Journal as the focus screen
        UIStateTracker.Instance.SetActiveScreen(UIStateTracker.UIScreen.Journal);

    }

  /*  //Switch to the next tab
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
    }*/



    private void CloseJournal()
    {
        if (journalPanel != null)
        {
            journalPanel.SetActive(false);
        }

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
    public void HighlightItem(int index)
    {
        foreach (ItemSlot slot in journalSlotList)
        {
            slot.SetHighlight(false);
        }

        if (index >= 0 && index < journalSlotList.Count)
        {
            journalSlotList[index].SetHighlight(true);
        }
    }

public void NavigateCraftingTab(InputAction.CallbackContext context)
{
    if (context.performed && isJournalOpen)
    {
        Vector2 navigationInput = context.ReadValue<Vector2>();

        if (navigationInput.y > 0) // Up
        {
            selectedIndex = Mathf.Max(0, selectedIndex - 1);
        }
        else if (navigationInput.y < 0) // Down
        {
            selectedIndex = Mathf.Min(InventoryUI.Instance.journalSlotList.Count - 1, selectedIndex + 1);
        }
        else if (navigationInput.x > 0) // Right
        {
            selectedIndex = Mathf.Min(InventoryUI.Instance.journalSlotList.Count - 1, selectedIndex + 1);
        }
        else if (navigationInput.x < 0) // Left
        {
            selectedIndex = Mathf.Max(0, selectedIndex - 1);
        }

        HighlightItem(selectedIndex);
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
