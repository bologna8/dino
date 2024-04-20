using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Budding : MonoBehaviour
{
    //[HideInInspector] public bool playerTouching = false;

    private Animator myAnim;

    private List<AI> myAIList = new List<AI>();
    [HideInInspector]public bool flower;
    public Budding linked;

    private float toggleTime;
    //private PlayerControls player;


    // Start is called before the first frame update
    void Start()
    {
        myAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        myAnim.SetBool("open", flower);

        if (toggleTime > 0f) { toggleTime -= Time.deltaTime; }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        var player = other.gameObject.GetComponent<PlayerControls>();
        if (player)
        { 
            if (player.interacting && toggleTime <= 0f) { ToggleBud(); }
        }

        var ai = other.gameObject.GetComponent<AI>();
        if (ai)
        {
            ai.touchingBud = this;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var ai = other.gameObject.GetComponent<AI>();
        if (ai)
        {
            ai.touchingBud = null;
        }
    }

    void ToggleBud()
    {
        flower = !flower;
        toggleTime = 0.5f;
        if (linked) { linked.flower = !flower; }
    }



}
