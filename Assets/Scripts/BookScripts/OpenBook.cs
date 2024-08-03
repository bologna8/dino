using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class OpenBook : MonoBehaviour
{
    [SerializeField] Button openButton; 
    private Vector3 rotationVector;
    private bool isOpenClicked;

    public DateTime startTime; 
    public DateTime endTime;

    void Start()
    {
        if (openButton != null)
        {
            openButton.onClick.AddListener(OpenBook_Click);
        }
    }

    void Update()
    {
        if (isOpenClicked)
        {
            transform.Rotate(rotationVector * Time.deltaTime);
            endTime = DateTime.Now;

            if((endTime - startTime).TotalSeconds >= 1)
            {
                isOpenClicked = false;
            }
        }
    }

    private void OpenBook_Click()
    {
        isOpenClicked = true;
        startTime = DateTime.Now;
        rotationVector = new Vector3(0, 180, 0);
    }
}
