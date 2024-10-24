using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

public class FearPage : MonoBehaviour
{
    [SerializeField]
    BookController bookController;

    public int threshhold = 50;

    private float ticSeconds = 3;


    [SerializeField]
    bool swapPage = false;
    [SerializeField]
    string baseText;
    [TextArea]
    public string fearText;

    [SerializeField]
    int pageNumber = 0;

    bool unlocked = false;

    TMP_Text textbox;
    private void Start()
    {
        if (bookController == null)
        {
            bookController = FindAnyObjectByType<BookController>();
        }

        if (swapPage) return;

        nextTic = Time.time + ticSeconds;
    }

    // Checks the fear level and sets 
    void OnEnable()
    {
        //FearLevel.FearStatic.LoadFearPage(this.gameObject);
        if (swapPage && FearLevel.FearStatic.fear>threshhold && !unlocked)
        {
            textbox = this.gameObject.GetComponent<TMP_Text>();
            textbox.text = fearText;
        }
        unlocked = true;
        
    }

    float nextTic;
    bool feared = false;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!swapPage && !unlocked)
        {
            if (FearLevel.FearStatic.fear > threshhold)
            {
                bookController.UnlockSpecificPage(pageNumber);
            }
        }
        else return;
    }

    public void PageSwap()
    {

    }
}
