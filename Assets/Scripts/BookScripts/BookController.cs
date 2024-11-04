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
    public int selectedJournalIndex = 0;

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

    private void OpenJournal()
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

    public void HighlightJournalItem(int index)
    {
        //Unhighlight all journal slots
        foreach (ItemSlot slot in journalSlotList)
        {
            slot.SetHighlight(false);
        }

        //Highlight the selected journal slot
        if (index >= 0 && index < journalSlotList.Count)
        {
            journalSlotList[index].SetHighlight(true);
        }
    }

    public void NavigateJournal(InputAction.CallbackContext context)
    {
       // if (context.performed && UIStateTracker.Instance.GetActiveScreen() == UIStateTracker.UIScreen.Journal)
       if(context.performed)
        {
            Vector2 navigationInput = context.ReadValue<Vector2>();

            //Only move if the journal is open
            if (!isJournalOpen) return;

            if (navigationInput.y > 0)  //Up
            {
                selectedJournalIndex = Mathf.Max(0, selectedJournalIndex - 1);
            }
            else if (navigationInput.y < 0)  //Down
            {
                selectedJournalIndex = Mathf.Min(journalSlotList.Count - 1, selectedJournalIndex + 1);
            }
            if (navigationInput.x > 0)  //Right
            {
                selectedJournalIndex = Mathf.Min(journalSlotList.Count - 1, selectedJournalIndex + 1);
            }
            else if (navigationInput.x < 0)  //Left
            {
                selectedJournalIndex = Mathf.Max(0, selectedJournalIndex - 1);
            }

            HighlightJournalItem(selectedJournalIndex);
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
