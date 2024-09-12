using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DEMO_FakeJournal : MonoBehaviour
{
    public Canvas myCanvas;
    public Canvas textPopUp;
    // Start is called before the first frame update
    void Start()
    {
        if (myCanvas == null)
        {
            myCanvas = this.gameObject.GetComponentInChildren<Canvas>();
        }
        myCanvas.enabled = false;
        textPopUp.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (inTrigger == true && Input.GetKeyDown(KeyCode.E))
        {
            myCanvas.enabled = true;
        }   
        if (myCanvas.enabled == true && Input.GetKeyDown(KeyCode.R))
        {
            myCanvas.enabled = false;
        }
    }
    bool inTrigger;
    private void OnTriggerEnter2D(Collider2D player)
    {
        if (player.gameObject.CompareTag("Player") == false)
        {
            return;
        }
        textPopUp.enabled = true;
        inTrigger = true;
    }

    private void OnTriggerExit2D(Collider2D player)
    {
        if (player.gameObject.CompareTag("Player") == false)
        {
            return;
        }
        textPopUp.enabled = false;
        inTrigger = false;
    }
}
