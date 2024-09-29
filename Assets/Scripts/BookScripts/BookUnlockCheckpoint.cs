using UnityEngine;

public class BookUnlockCheckpoint : MonoBehaviour
{
    private bool checkpointReached = false; 
    public BookController bookController;    

    [Tooltip("Index of the page to unlock in the journal.")]
    public int pageToUnlock; 

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
        if (collision.CompareTag("Player") && !checkpointReached)
        {
            checkpointReached = true; 

            bookController.UnlockSpecificPage(pageToUnlock);

            if (pageUnlockPopUp != null)
            {
                pageUnlockPopUp.ShowPopUp("New Page Unlocked!"); 
            }

            Debug.Log("Checkpoint reached! Unlocking page: " + pageToUnlock);

            Destroy(gameObject);
        }
    }
}
