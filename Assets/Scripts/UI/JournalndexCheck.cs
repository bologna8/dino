using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalndexCheck : MonoBehaviour
{
    BookController controller;

    public int pageNum;
    private Button button;
    private Image image;

    Color color;
    // Start is called before the first frame update
    void Start()
    {
        button = this.gameObject.GetComponent<Button>();
        image = this.gameObject.GetComponent<Image>();
        color = image.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnEnable()
    {
        if (controller.pages[pageNum].isUnlocked == false)
        {
            color.a = 0;
            image.color = color;

            button.interactable = false;
        }
        else
        {
            color.a = 1;
            image.color = color;

            button.interactable = true;
        }


    }
}
