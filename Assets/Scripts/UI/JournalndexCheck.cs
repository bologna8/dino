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

    Color color;
    // Start is called before the first frame update
    void Start()
    {

        button = this.gameObject.GetComponent<Button>();
        text = this.gameObject.GetComponentInChildren<TMP_Text>();
        color = text.color;

        buttonCheckTic = Time.time+1;

        pageCheck();
    }

    float buttonCheckTic;

    bool isUnlocked = false;

    // Update is called once per frame
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
    void pageCheck()
    {
        if (controller.pages[pageNum].isUnlocked == false)
        {
            color.a = 0;
            text.color = color;

            button.interactable = false;
        }
        else
        {
            color.a = 1;
            text.color = color;

            button.interactable = true;
            isUnlocked = true;
        }


    } 
}
