using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.Rendering.Universal;

public class AmbientLightColor : MonoBehaviour
{
    [SerializeField]
    Light2D ambientLight;

    public UnityEngine.Color ambientColor = UnityEngine.Color.grey;

    public float colorChangeDuration = 1;

    private UnityEngine.Color currentColor;
    private UnityEngine.Color previousColor;

    private float startDuration;
    private float endDuration;

    private void OnTriggerStay2D(Collider2D player)
    {
        if (player.gameObject.CompareTag("Player"))
        {
            if(Time.time < endDuration)
            {
                currentColor = previousColor*(endDuration-Time.time)/colorChangeDuration + ambientColor*(colorChangeDuration-(endDuration-Time.time))/colorChangeDuration;
                ambientLight.color = currentColor;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D player)
    {
        if (player.gameObject.CompareTag("Player"))
        {
            startDuration = Time.time;
            endDuration = Time.time + colorChangeDuration;
            previousColor = ambientLight.color;
        }
    }
}
