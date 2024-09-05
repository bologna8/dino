using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawned : MonoBehaviour
{
    //[HideInInspector] public Quaternion startRotation; //Rotation to start at
    [HideInInspector] public Transform source; //Keep track of original object that spawned this object
    [HideInInspector] public Vector3 origin; //Keep track of the starting position of this object
    [HideInInspector] public int team; //Keep track of what team is assigned to this object
    [HideInInspector] public ObjectPool myPool; //Return to pool
    //[HideInInspector] public int index; //What number object in the item pool list this object is

    [Tooltip("Maximum number of this object in a scene before it starts to recycle itself")] public int maxCount = 99;

    [Tooltip("Time before each child in this transform is enabled, sequentially")] public float delayBetweenChildren;
    [Tooltip("Maintain the same position and angle as the source of this spawn")] public bool tracking = false;
    [HideInInspector] public Aim myAim;
    [HideInInspector] public Core myCore;

    void Awake()
    {
        foreach(Transform child in transform)
        { child.gameObject.SetActive(false); }
    }

    void OnEnable()
    {
        if (!source) { source = transform; }
        if (team == 0) { team = gameObject.layer; }

        if(source) { myCore = source.GetComponent<Core>(); }

        if(tracking && myCore && !myAim) { myAim = myCore.myAim; }


        origin = transform.position;
        //Debug.Log(gameObject.name + " - team #" + team);

        StartCoroutine(EnableChildren());
        //transform.rotation = startRotation;
    }


    void Update()
    {
        /*
        if (tracking && myAim)
        {
            transform.position = myAim.transform.position;
            transform.rotation = myAim.transform.rotation;
        }
        */
    }


    void OnDisable()
    {
        if (myPool != null)
        {
            myPool.inactiveQ.Add(gameObject);
            myPool.activeQ.Remove(gameObject);
        }

        foreach(Transform child in transform)
        { child.gameObject.SetActive(false); }

    }

    IEnumerator EnableChildren()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(true);
            if(delayBetweenChildren > 0)
            { yield return new WaitForSeconds(delayBetweenChildren); }            
        }
    }

}
