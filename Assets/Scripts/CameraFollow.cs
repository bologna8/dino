using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Follow;
    private Vector3 lastPos;
    private float zOffset;
    public float timeToTrack = 0.5f;
    private float currentTrackingTime;

    // Start is called before the first frame update
    void Start()
    {
        zOffset = transform.position.z;        
    }

    void FixedUpdate()
    {
        if (Follow)
        {
            if (Follow.position != lastPos) 
            { lastPos = Follow.position; currentTrackingTime = 0f; }

            if (currentTrackingTime < timeToTrack)
            { currentTrackingTime += Time.deltaTime; }            

            var nextPos = Vector3.Lerp(transform.position, Follow.position, currentTrackingTime / timeToTrack);
            nextPos.z = zOffset;

            transform.position = nextPos;
        }
    }
}
