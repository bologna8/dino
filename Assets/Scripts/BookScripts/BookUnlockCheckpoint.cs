using UnityEngine;

public class BookUnlockCheckpoint : MonoBehaviour
{
    private bool checkpointReached = false; 
    public BookController bookController;    

    [Tooltip("Index of the page to unlock in the journal.")]
    public int pageToUnlock; 

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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !checkpointReached)
        {
            checkpointReached = true; 

            bookController.UnlockSpecificPage(pageToUnlock);

            Debug.Log("Checkpoint reached! Unlocking page: " + pageToUnlock);
        }
    }
}
