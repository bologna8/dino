using UnityEngine;

public class BookUnlockCheckpoint : MonoBehaviour
{
    private bool checkpointTriggered = false; 
    public BookController bookController;

    [Tooltip("Index of the spread to unlock in the journal.")]
    public int spreadToUnlock; //Element that you want to unlock from pages array 

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

            UnlockSpread(spreadToUnlock);

            if (pageUnlockPopUp != null)
            {
                pageUnlockPopUp.ShowPopUp("New Pages Unlocked!");
            }

            Debug.Log("Checkpoint triggered! Unlocking spread: " + spreadToUnlock);

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
}
