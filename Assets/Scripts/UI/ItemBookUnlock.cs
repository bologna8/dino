using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBookUnlock : BookUnlockCheckpoint
{
    // As a variant of the Book Unlock Checkpoint class, this should inherit any methods we don't want to overrride
    public override void Start()
    {
        bookController = FindObjectsOfType<BookController>(true)[0];
    }


    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            if (unlockAsSpread)
            {
                UnlockSpread(pageToUnlock);
            }
            else
            {
                UnlockIndividualPage(pageToUnlock);
            }

            Debug.Log("Checkpoint triggered! Unlocking: " + (unlockAsSpread ? "Spread" : "Page") + " " + pageToUnlock);
        }
    }


}
