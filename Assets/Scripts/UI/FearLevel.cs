using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FearLevel : MonoBehaviour
{
   // enum FearState { fearless, cowardly };

    [HideInInspector]
    public static FearLevel FearStatic;

    public float fear;
    [SerializeField]
    private float maxFear = 100;

    [Tooltip("how long it takes for Arizona to get 1 point more afraid while in the dark")]
    public float darknessDuration = 3;
    private float darknessTic;
    private float damageTic;

    private bool hasLantern = false;
    [Tooltip("This is the player prefab.")]
    [SerializeField]
    private GameObject player;

    private LanternController lanternControl;
    private Health health;

    [Tooltip("The hallucinogenic noises that Arizona might hear on a high fear.")]
    public List<GameObject> roars;
    [Tooltip("Arizona's Journal UI.")]
    public BookController bookControl;
    //private List<FearPage> FearPages;
    void Awake()
    {
        if (player == null)
        {
            Debug.LogError("FearLevel requires a player prefab to be defined.");
            Destroy(this);
        }

        if (FearStatic == null)
        {
            FearStatic = this;
        }
        else
        {
            Debug.LogWarning("There are multiple fearlevel systems in the scene.");
        }

        fear = 0;
        

        if (lanternControl == null)
        {
            lanternControl = player.GetComponentInChildren<LanternController>();
            if (lanternControl == null) Debug.LogWarning("No script called lanternController found on player or its children. Please add one.");
        }

        if (health == null)
        {
            health = player.GetComponentInChildren<Health>();
            if (lanternControl == null) Debug.LogWarning("No script called lanternController found on player or its children. Please add one.");
        }


        darknessTic = Time.time + darknessDuration;
        damageTic = Time.time + 1;

        roarChance = maxFear*1000;
        lastCheckedHealth = health.currentHP;
    }

    bool unlockedPages = false;
    float roarChance;
    float lastCheckedHealth;
    void FixedUpdate()
    {

        //Plays a sound efect somewhere around the player when the player has high fear.
        if (fear > 50 && fear>Random.Range(0,roarChance))
        {
            float randX = this.gameObject.transform.position.x + Random.Range(-50, 50);
            if (randX < 0) Mathf.Clamp(randX, -50, -30); else Mathf.Clamp(randX, 30, 50);

            float randY = this.gameObject.transform.position.y + Random.Range(-20, 20);
            if (randY < 0) Mathf.Clamp(randY, -20, -10); else Mathf.Clamp(randY, 10, 20);

            Vector3 randomLocation = new Vector3(randX, randY, this.gameObject.transform.position.z);
            Instantiate(roars[Random.Range(0,roars.Count-1)],randomLocation, this.gameObject.transform.rotation);
        }
        //Checks for darkness or when the player lacks a lantern and increases fear if so.
        if (!hasLantern || lanternControl.currentEnergy<=10f)
        {
            if (!hasLantern)
            {
                if (lanternControl.isLanternActive)
                {
                    hasLantern = true;
                }
                else return;
            }

            int darkMod = hasLantern ? 0 : 1;
            if (Time.time > darknessTic)
            {
                darknessTic = Time.time + darknessDuration;
                FearIncrease(1+darkMod);
            }
        }
        //Checks if the player got beat up.
        if (Time.time > damageTic)
        {
            if (lastCheckedHealth == health.currentHP) return;
            float healthDifference = lastCheckedHealth - health.currentHP;
            FearIncrease(healthDifference*5);
        }
    }

    public void FearIncrease(float fearVal)
    {
        fear += fearVal;
    }


    // public void LoadFearPage(Int pageNo)
    //  {
    //      bookController.UnlockSpecificPage(pageNo);
    // }
    //int test;
    //  void PageThreshholdCheck()
    //  {
    //     for (int i = 0; i < FearPages.Count; i++)
    //     {
    //        test = FearPages[i].threshhold;
    //    }
    // }
}
