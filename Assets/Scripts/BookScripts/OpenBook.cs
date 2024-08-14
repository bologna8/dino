using UnityEngine;
using UnityEngine.UI;
using System;

public class OpenBook : MonoBehaviour
{
    [SerializeField] GameObject openedBook;
    [SerializeField] GameObject insideBackCover;

    [SerializeField] Button openButton; 
    [SerializeField] Button closeButton;

    private Vector3 rotationVector;
    private bool isOpenClicked;
    private float rotationDuration = 1.0f; 
    private float rotationTime;

    void Start()
    {
        if (openButton != null)
        {
            openButton.onClick.AddListener(OpenBook_Click);
        }
        else
        {
            Debug.LogError("Open Button is not assigned.");
        }

        BookEvent.CloseBook += CloseBook_Click;
    }

    void Update()
    {
        if (isOpenClicked)
        {
            float t = (Time.time - rotationTime) / rotationDuration;
            if (t >= 1)
            {
                isOpenClicked = false;
                gameObject.SetActive(false);
                insideBackCover.SetActive(false);
                openedBook.SetActive(true);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rotationVector), t);
            }
        }
    }

    private void OpenBook_Click()
    {
        Debug.Log("Open button clicked");
        isOpenClicked = true;
        rotationTime = Time.time;
        rotationVector = new Vector3(0, 180, 0);
    }

    private void CloseBook_Click(object sender, EventArgs e)
    {
        Debug.Log("Close book clicked in OpenBook");
        openedBook.SetActive(false);
        insideBackCover.SetActive(true);
        gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        BookEvent.CloseBook -= CloseBook_Click;
    }
}
