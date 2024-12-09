using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceOpenBookPage : MonoBehaviour
{
    [Header("The BookController")]
    [SerializeField]
    BookController book;

    [Header("The page to open up to")]
    public int pageNumber;

    [Header("please set at least one of the following to true")]
    public bool activateOnStart = false;

    public bool activateOnTrigger = false;

    public bool deactivateAfterTrigger = true;

    private Collider2D trigger;

    private float delay;
    private bool isDelayed = false;

    // If the script is set to activate on start, then the page will be opened on start. If it is set to activate on trigger,
    // the script will set a collider2D component to its trigger.
    void Start()
    {
        if (book == null)
        {
            book = FindObjectsOfType<BookController>(true)[0];
        }

        PickUp thisPickup = this.gameObject.GetComponent<PickUp>();
        if (thisPickup != null)
        {
            delay = thisPickup.delayPickupTime;
            isDelayed = true;
        }

        if (!activateOnStart && !activateOnTrigger) return;

        if (activateOnStart)
        {
            PageOpenTo();
        }
        if (activateOnTrigger)
        {
            trigger = GetComponent<Collider2D>();
            if (trigger == null)
            {
                Debug.LogWarning(this.gameObject.ToString() + " either doesn't have a collider2D and is set to require one.");
            }

        }

        
    }
    bool inTrigger;
    //if the player enters the trigger, the page will be opened after some amount of delay.
    private void OnTriggerEnter2D(Collider2D player)
    {
        if (player.gameObject.CompareTag("Player"))
        {
            if (!isDelayed) Invoke("PageOpenTo", 0.1f);
            if (isDelayed) Invoke("PageOpenTo", 0.1f + delay);
        }
    }
    
    //Uses BookController to open up to a specific page. 
    void PageOpenTo()
    {
        if (book.pages[pageNumber].isUnlocked) return;
        book.UnlockSpecificPage(pageNumber);
        book.OpenJournal();
        book.GoToTab(pageNumber);
    }
}
