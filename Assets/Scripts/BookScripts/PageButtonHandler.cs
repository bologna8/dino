using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageButtonHandler : MonoBehaviour
{
    [SerializeField] Book book;
    [SerializeField] int pageIndex;

    public void OnButtonClick()
    {
        if(book != null)
        {
            book.GoToPage(pageIndex);
        }
    }
}