using System.Collections;
using TMPro;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public TextMeshProUGUI popupText;  
    public float displayDuration = 3.0f;  

    private Coroutine currentPopupCoroutine;

    private void Start()
    {
        popupText.gameObject.SetActive(false);
    }

    public void ShowPopup(string collectedItem)
    {
        if (currentPopupCoroutine != null)
        {
            StopCoroutine(currentPopupCoroutine); 
        }

        currentPopupCoroutine = StartCoroutine(DisplayPopup(collectedItem));
    }

    private IEnumerator DisplayPopup(string collectedItem)
    {
        popupText.text = "You collected " + collectedItem + "!";
        popupText.gameObject.SetActive(true);

        yield return new WaitForSeconds(displayDuration);

        popupText.gameObject.SetActive(false);
    }
}
