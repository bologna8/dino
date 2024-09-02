using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DEMO_PopUpText : MonoBehaviour
{
    public TMP_Text text;
    public float duration;

    //Destroys this object if there is no text
    void Awake()
    {
        if (text == null)
        {
            Destroy(this.gameObject);
        }
        startTime = Time.time;
        endTime = Time.time + duration;
        textColor = text.color;
        alpha = text.color.a;
    }

    public float startTime;
    public float endTime;
    public float currentTime;
    public Vector4 textColor;
    public float alpha;
    // Update is called once per frame
    void FixedUpdate()
    {
        alpha = duration/2 + (duration - (Time.time - startTime))/2;
        textColor.w = alpha;
        text.color = textColor;
        currentTime = Time.time;
        if (alpha <= 0)
        {
            Debug.Log("a");
        }
    }

    public void TextChange(string GOName)
    {
        text.text = "Discovered " + GOName;
    }
}
