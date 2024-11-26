using UnityEngine;

public class BookUnlockCheckpoint : MonoBehaviour
{
    private bool checkpointTriggered = false; 
    public BookController bookController;

    [Tooltip("Index of the page or spread to unlock in the journal.")]
    public int pageToUnlock; 

    public bool unlockAsSpread = false; 
    public PageUnlockPopUp pageUnlockPopUp;

    private void Start()
    {
        if (bookController == null)
        {
            bookController = FindObjectOfType<BookController>();
            if (bookController == null)
            {
                Debug.LogError("BookController not found in the scene. Please assign it to the checkpoint.");
            }
        }

        if (pageUnlockPopUp == null)
        {
            Debug.LogWarning("PageUnlockPopUp reference is not assigned!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !checkpointTriggered)
        {
            checkpointTriggered = true; 

            if (unlockAsSpread)
            {
                UnlockSpread(pageToUnlock);
            }
            else
            {
                UnlockIndividualPage(pageToUnlock);
            }

            if (pageUnlockPopUp != null)
            {
                pageUnlockPopUp.ShowPopUp("New Page Unlocked!");
            }

            Debug.Log("Checkpoint triggered! Unlocking: " + (unlockAsSpread ? "Spread" : "Page") + " " + pageToUnlock);

            Destroy(gameObject);
        }
    }

    private void UnlockSpread(int spreadIndex)
    {
        if (spreadIndex < 0 || spreadIndex >= bookController.pages.Length)
        {
            Debug.LogError("Spread index is out of range: " + spreadIndex);
            return;
        }

        BookController.Page spreadPage = bookController.pages[spreadIndex];
        if (spreadPage.pageType != BookController.PageType.Spread)
        {
            Debug.LogError("Specified index does not correspond to a spread: " + spreadIndex);
            return;
        }

        bookController.UnlockSpecificPage(spreadIndex);
    }

    private void UnlockIndividualPage(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= bookController.pages.Length)
        {
            Debug.LogError("Page index is out of range: " + pageIndex);
            return;
        }

        BookController.Page individualPage = bookController.pages[pageIndex];
        if (individualPage.pageType != BookController.PageType.Individual)
        {
            Debug.LogError("Specified index does not correspond to an individual page: " + pageIndex);
            return;
        }

        bookController.UnlockSpecificPage(pageIndex);
    }
}
