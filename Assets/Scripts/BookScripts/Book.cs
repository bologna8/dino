using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Book : MonoBehaviour
{
    [SerializeField] float pageSpeed = 0.5f;
    [SerializeField] List<Transform> pages;
    int index = -1; // Start with no pages flipped
    bool rotate = false;

    //[SerializeField] GameObject backButton;
    //[SerializeField] GameObject nextButton;

    // Callback for Next Page Action 
    public void OnNextPage(InputAction.CallbackContext context)
    {
        RotationNext();
    }

    // Callback for Previous Page Action 
    public void OnPreviousPage(InputAction.CallbackContext context)
    {
        RotateBack();
    }

    public void RotationNext()
    {
        if (rotate || index >= pages.Count - 1) // check index doesn't exceed the page count
            return;

        index++;
        float angle = 180;
        // NextButtonActions();
        pages[index].SetAsLastSibling();
        // StartCoroutine(Rotate(angle, true)); 
    }

    public void RotateBack()
    {
        if (rotate || index < 0) // check if you're at the first page
            return;

        pages[index].SetAsLastSibling();
        index--;  // Decrement the index to move to the previous page
        float angle = 0;
        // StartCoroutine(Rotate(angle, false)); 
    }

    // public void NextButtonActions()
    //{
    //    if(backButton.activeInHierarchy == false)
    //    {
    //        backButton.SetActive(true);
    //    }
    //    if(index == pages.Count - 1)
    //    {
    //        nextButton.SetActive(false);
    //    }
    //}

    // public void BackButtonActions()
    //{
    //    if(nextButton.activeInHierarchy == false)
    //    {
    //        nextButton.SetActive(true);
    //    }
    //    if(index - 1 == -1)
    //    {
    //        backButton.SetActive(false);
    //    }
    //}

    public void GoToPage(int pageIndex)
    {
        if (pageIndex <= 0 || pageIndex >= pages.Count || rotate) return;

        // Update index
        index = pageIndex;

        // Transition to the new page without rotation
        pages[index].SetAsLastSibling();
        float angle = 180;
        // StartCoroutine(Rotate(angle, true)); // Commented out rotation call
        // NextButtonActions();
        // BackButtonActions();
    }

    // IEnumerator Rotate(float angle, bool forward)
    //{
    //    float value = 0f;
    //    rotate = true;
    //    Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
    //    while (true)
    //    {
    //        value += Time.unscaledDeltaTime * pageSpeed;
    //        pages[index].rotation = Quaternion.Slerp(pages[index].rotation, targetRotation, value);
    //        if (Quaternion.Angle(pages[index].rotation, targetRotation) < 0.1f)
    //        {
    //            if (!forward)
    //            {
    //                index--; // Decrement index if moving backwards
    //            }
    //            rotate = false;
    //            yield break;
    //        }
    //        yield return null;
    //    }
    //}
}
