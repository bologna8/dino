using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableCanvas : MonoBehaviour
{
    // Time in seconds before the canvas is disabled
    [SerializeField]
    private float disableDelay = 10f;

    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();

       
        if (canvas == null)
        {
            Debug.LogError("No Canvas component found on this GameObject.");
        }
    }

    private void OnEnable()
    {
        // Start the coroutine to disable the canvas
        if (canvas != null)
        {
            StartCoroutine(DisableCanvasAfterDelay());
        }
    }

    private System.Collections.IEnumerator DisableCanvasAfterDelay()
    {
       
        yield return new WaitForSeconds(disableDelay);
        canvas.enabled = false;
    }
}
