using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PulsingLight : MonoBehaviour
{
    private bool pulsingUp=false;

    private bool onScreen;
    private float screenTic;

    private Light2D thisLight;
    private Color lightColor;
    private float originalAlpha;

    public float pulseTimer = 5f;
    private float pulseTic;

    private Camera cam;

    private float lightSize;
    private Transform thisTransform;
    // Start is called before the first frame update
    void Start()
    {
        thisLight = this.gameObject.GetComponent<Light2D>();
        if(thisLight == null ) Destroy(this.gameObject);

        screenTic = Time.time + 1;
        pulseTic = Time.time+pulseTimer;
        cam = Camera.main;

        lightSize = thisLight.pointLightOuterRadius;
        thisTransform = this.gameObject.GetComponent<Transform>();

        lightColor = thisLight.color;
        originalAlpha = lightColor.a;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (pulseTic < Time.time)
        {
            pulseTic = Time.time+pulseTimer;
            pulsingUp = !pulsingUp;
        }

        if (Time.time < screenTic && !onScreen) return;

        if (Time.time > screenTic && !onScreen)
        {
            screenTic = Time.time + 1;
            
            onScreen = CamCheck();

            if (!onScreen) return;
        }

        if (!pulsingUp)
        {
            lightColor.a = Mathf.Clamp(originalAlpha * (pulseTic-Time.time) / pulseTimer + (originalAlpha - .5f) * (1- ((pulseTic - Time.time) / pulseTimer)), 0, 1);
        }
        else
        {
            lightColor.a = Mathf.Clamp(originalAlpha * (1 - ((pulseTic - Time.time) / pulseTimer)) + (originalAlpha - .5f) * (pulseTic - Time.time) / pulseTimer, 0, 1);
        }
        thisLight.color = lightColor;
    }

    bool CamCheck()
    {
        if (cam.transform.position.x < thisTransform.position.x - lightSize - 19 || cam.transform.position.x > thisTransform.position.x + lightSize + 19) return false;
        if (cam.transform.position.y < thisTransform.position.y - lightSize - 10 || cam.transform.position.y > thisTransform.position.y + lightSize + 10) return false;
        return true;
    }
}
