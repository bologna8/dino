using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FearLevel : MonoBehaviour
{
   // enum FearState { fearless, cowardly };

    [HideInInspector]
    public static FearLevel FearStatic;

    public float fear = 0;
    [SerializeField]
    private float maxFear = 100;

    [Tooltip("how long it takes for Arizona to get more afraid while in the dark")]
    public float darknessDuration = 3;
    [Tooltip("how long it takes for Arizona to get more afraid while hidden")]
    public float hiddenDuration = 3;
    [Tooltip("how much more fear is gained per 1 damage")]
    public float damageFearMultiplier = 5;

    private float darknessTic;
    private float hiddenTic;
    private float damageTic;
    private float pageTic;

    private bool hasLantern = false;
    [Tooltip("This is the player prefab.")]
    [SerializeField]
    private GameObject player;

    private LanternController lanternControl;
    private Health health;
    private Core core;

    [Tooltip("The hallucinogenic noises that Arizona might hear on a high fear.")]
    public List<GameObject> roars;
    [Tooltip("Arizona's Journal UI.")]
    public BookController bookControl;
    [Tooltip("The popup to see if a page is unlocked")]
    public PageUnlockPopUp popUp;
    [Tooltip("X = Fear Threshold to unlock, Y = Page number")]
    [SerializeField]
    private List<Vector2> fearUnlockPages;

    private bool hasPlayer = true;
    void Awake()
    {
        //If player isn't assigned, this script destroys itself to prevent errors.
        if (player == null)
        {
            Debug.LogError("FearLevel requires a player prefab to be defined.");
            hasPlayer = false;
        }

        //Sets this script to be a static object.
        if (FearStatic == null)
        {
            FearStatic = this;
        }
        else
        {
            Debug.LogWarning("There are multiple fearlevel systems in the scene.");
        }

        //Assigns necessary scripts from the Player GameObject's Children
        if (lanternControl == null)
        {
            lanternControl = player.GetComponentInChildren<LanternController>();
            if (lanternControl == null) Debug.LogWarning("No script called lanternController found on player or its children. Please add one.");
        }

        if (health == null)
        {
            health = player.GetComponentInChildren<Health>();
            if (lanternControl == null) Debug.LogWarning("No script called Health found on player or its children. Please add one.");
        }
        if (core == null)
        {
            core = player.GetComponent<Core>();
            if (lanternControl == null) Debug.LogWarning("No script called Core found on player or its children. Please add one.");
        }

        //Sets initial tic timers
        darknessTic = Time.time + darknessDuration;
        hiddenTic = Time.time + hiddenDuration;
        damageTic = Time.time + 1;
        pageTic = Time.time + 1;

        //Sets important fields scaled to other fields.
        roarChance = maxFear*1000;
    }

    bool unlockedPages = false;
    float roarChance;
    float lastCheckedHealth = -1;
    bool hidden = false;
    void FixedUpdate()
    {
        if (!hasPlayer) return;

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
            }

            int darkMod = hasLantern ? 0 : 1;
            if (Time.time > darknessTic)
            {
                darknessTic = Time.time + darknessDuration;
                FearChange(1+darkMod);
            }
        }
        //Checks if the player got beat up.
        if (Time.time > damageTic)
        {
            if (lastCheckedHealth == -1)
            {
                lastCheckedHealth = health.currentHP;
            }
            if (lastCheckedHealth != health.currentHP)
            {
                float healthDifference = lastCheckedHealth - health.currentHP;
                FearChange(healthDifference * damageFearMultiplier);
                lastCheckedHealth = health.currentHP;
            }
            damageTic = Time.time;
        }
        //Checks if player is hiding.
        if (Time.time > hiddenTic)
        {
            if (core.hidden)
            {
                if (!hidden)
                {
                    hidden = true;
                }
                else FearChange(1);
            }
            else
            {
                if (!hidden) hidden = false;
            }
        }

        if (Time.time > pageTic)
        {
            for (int i = fearUnlockPages.Count - 1; i >= 0; i--)
            {
                if (fear >= fearUnlockPages[i].x)
                {
                    bookControl.UnlockSpecificPage(((int)fearUnlockPages[i].y));
                    if (popUp != null)
                    {
                        popUp.ShowPopUp("New Page Unlocked!");
                    }
                    fearUnlockPages.RemoveAt(i);
                }
            }
        }
        
        fear = Mathf.Clamp(fear, 0, maxFear);
        
    }

    //Increases (or decreases  with a negative) Fear by the float fed into the method.
    public void FearChange(float fearVal)
    {
        fear += fearVal;
    }
}
