using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayDestroy : MonoBehaviour
{
    public float lifetime = 1f;
    private float currentTime;

    // Start is called before the first frame update
    void OnEnable()
    {
        currentTime = lifetime;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0) 
        { 
            if (PoolManager.Instance != null) { gameObject.SetActive(false); }
            else { Destroy(gameObject); } 
        }
    }
}
