using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempStalagtite : MonoBehaviour
{
    public GameObject player;
    public Rigidbody2D rb;
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {

      if (Mathf.Abs( player.transform.position.x - transform.position.x) < 2)
        {
            rb.WakeUp();
        }
    }
}
