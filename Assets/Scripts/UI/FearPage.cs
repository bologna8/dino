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


    [SerializeField]
    string baseText;
    [TextArea]
    public string fearText;

    bool unlocked = false;

    float startTime;

    TMP_Text textbox;
    private void Awake()
    {
        if (bookController == null)
        {
            bookController = FindAnyObjectByType<BookController>();
        }
    }


    // Checks the fear level and sets 
    void OnEnable()
    {
        //FearLevel.FearStatic.LoadFearPage(this.gameObject);
        if (Time.time > 1 && FearLevel.FearStatic.fear > threshhold && !unlocked)
        {
            textbox = this.gameObject.GetComponent<TMP_Text>();
            textbox.text = fearText;
            Debug.Log("text swapped");
        }
        Debug.Log("Fear swap page enabled");
        if (Time.time >1) unlocked = true;        
    }
}
