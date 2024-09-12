using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class FootstepAudio : MonoBehaviour
{
    [SerializedDictionary("GroundType", "List")]
    public SerializedDictionary<GroundType.Type, List<GameObject>> FootstepLists;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
