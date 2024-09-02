using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour
{
    [SerializeField] float pageSpeed = 0.5f;
    [SerializeField] List<Transform> pages;
    int index = -1; 
    bool rotate = false; 
    [SerializeField] GameObject backButton;
    [SerializeField] GameObject nextButton;

    private void Start()
    {
        backButton.SetActive(false);
    }
    
    public void RotationNext()
    {
        if(rotate == true) {return;}
        index++;
        float angle = 180; 
        NextButtonActions();
        pages[index].SetAsLastSibling();
        StartCoroutine(Rotate(angle, true));
    }

    public void NextButtonActions()
    {
        if(backButton.activeInHierarchy == false)
        {
            backButton.SetActive(true);
        }
        if(index==pages.Count - 1)
        {
            nextButton.SetActive(false);
        }
    }

    public void RotateBack()
    {
        if(rotate == true) {return;}
        float angle = 0; 
        pages[index].SetAsLastSibling();
        BackButtonActions();
        StartCoroutine(Rotate(angle, false));
    }

    public void BackButtonActions()
    {
        if(nextButton.activeInHierarchy==false)
        {
            nextButton.SetActive(true);
        }
        if(index -1 == -1)
        {
            backButton.SetActive(false);
        }
    }

  IEnumerator Rotate(float angle, bool forward)
{
    float value = 0f;
    rotate = true;
    Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
    while (true)
    {
        value += Time.unscaledDeltaTime * pageSpeed; 
        pages[index].rotation = Quaternion.Slerp(pages[index].rotation, targetRotation, value);
        if (Quaternion.Angle(pages[index].rotation, targetRotation) < 0.1f)
        {
            if (!forward)
            {
                index--;
            }
            rotate = false;
            yield break;
        }
        yield return null;
    }
}
}