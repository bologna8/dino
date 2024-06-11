using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [HideInInspector] public Health tracking;

    public Vector3 Offset = new Vector3(0, 10, 0);
    private Slider mySlider;
    private bool displayed = true;


    // Start is called before the first frame update
    void Start()
    {
        mySlider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (tracking)
        { 
            transform.position = Camera.main.WorldToScreenPoint(tracking.transform.position + Offset);

            var percent = tracking.currentHP / tracking.maxHP;
            mySlider.value = percent;
            if (percent == 1 && displayed) 
            {
                displayed = false;
                foreach (Transform child in transform) 
                { child.gameObject.SetActive(false); }
            }
            else if (percent < 1 && !displayed)
            { 
                displayed = true;
                foreach (Transform child in transform) 
                { child.gameObject.SetActive(true); }
            }
            
        }
        else { Destroy(gameObject); }
    }
}
