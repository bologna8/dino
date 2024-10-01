using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FearLevel : MonoBehaviour
{
    enum FearState{fearless, cowardly};

    public float fear;

    [Tooltip("how long it takes for Arizona to get 1 point more afraid while in the dark")]
    public float darknessDuration = 3;
    private float darknessTic;

    private bool hasLantern = false;
    private GameObject lantern;


    public AudioSource roar;
    public BookController bookController;
    public int ScaredPage;
    void Start()
    {
        fear = 0;
        lantern = GameObject.Find("Lantern");
        if (lantern == null)
        {
            Debug.LogWarning("No game obect called 'Lantern' found in the scene");
        }
        darknessTic = Time.time + darknessDuration;
    }

    bool unlockedPages = false;
    void FixedUpdate()
    {
        if (fear > 30 && unlockedPages == false)
        {
            roar.Play();
            bookController.UnlockSpecificPage(ScaredPage);
            unlockedPages = true;
        }
        if (hasLantern == false)
        {
            if (lantern.activeInHierarchy == true)
            {
                hasLantern = true;
                return;
            }
            if (Time.time > darknessTic)
            {
                darknessTic = Time.time + darknessDuration;
                fear += 1;
            }
        }
    }
}
