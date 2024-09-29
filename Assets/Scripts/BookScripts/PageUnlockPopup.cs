using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PageUnlockPopUp : MonoBehaviour
{
    public GameObject popUpPanel; 
    public Text messageText; 

    private void Start()
    {
        popUpPanel.SetActive(false); 
    }

    public void ShowPopUp(string message)
    {
        messageText.text = message;
        popUpPanel.SetActive(true); 
        StartCoroutine(HidePopUpAfterDelay(3f)); 
    }

    private IEnumerator HidePopUpAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        popUpPanel.SetActive(false); 
    }
}
