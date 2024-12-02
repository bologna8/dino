using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JournalndexCheck : MonoBehaviour
{
    [SerializeField]
    private BookController controller;

    public int pageNum;
    private Button button;
    private TMP_Text text;
    private Image image;

    Color textColor;
    Color imageColor;
    // This sets all of the required fields before doing an initial page check
    void Start()
    {

        button = this.gameObject.GetComponent<Button>();
        text = this.gameObject.GetComponentInChildren<TMP_Text>();
        image = this.gameObject.GetComponent <Image>();
        textColor = text.color;
        imageColor = image.color;

        buttonCheckTic = Time.time+1;

        pageCheck();
    }

    float buttonCheckTic;

    bool isUnlocked = false;

    // This serves as a timer for the page check so it is not called every frame to lighten the load on performance.
    void Update()
    {
        if (buttonCheckTic < Time.time)
        {
            if (!isUnlocked)
            {
                pageCheck();
            }
        }
    }
    //This checks if the page is unlocked, and if it isn't will hide it and prevent interaction
    //Otherwise, it will reveal the button and make it interactable.
    void pageCheck()
    {
        if (controller.pages[pageNum].isUnlocked == false)
        {
            textColor.a = 0;
            text.color = textColor;
            imageColor.a = 0;
            image.color = imageColor;

            button.interactable = false;
        }
        else
        {
            textColor.a = 1;
            text.color = textColor;
            imageColor.a = 1;
            image.color = imageColor;

            button.interactable = true;
            isUnlocked = true;
        }


    } 
}
