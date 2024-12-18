using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [HideInInspector] public Health tracking;
    private Image currentHealth;
    public List<Sprite> healthSprites;
    public bool trackPosition = true;
    public Vector3 Offset = new Vector3(0, 10, 0);
    private Slider mySlider;
    private bool displayed = true;

    private RectTransform myRect;

    private Spawned mySpawn;


    void Awake()
    {
        GameObject GO = GameObject.FindGameObjectWithTag("Player");
        tracking = GO.GetComponentInChildren<Health>();
        
        if (!mySlider) { mySlider = GetComponent<Slider>(); }
        if (!myRect) { myRect = GetComponent<RectTransform>(); }

        if (!mySpawn) { mySpawn = GetComponent<Spawned>(); }

        if (!currentHealth) { currentHealth = GetComponent<Image>(); }
        if (currentHealth) { currentHealth.sprite = healthSprites[healthSprites.Count-1]; }
    }

    public void UpdateHealthUI()
    {
        if (tracking)
        {
            var percent = (tracking.currentHP / tracking.maxHP) * healthSprites.Count;
            if (percent > healthSprites.Count - 1)
            {
                currentHealth.sprite = healthSprites[healthSprites.Count - 1];
            }
            else
            {
                currentHealth.sprite = healthSprites[(int)percent];
            }
        }
}

    // Update is called once per frame
    void Update()
    {

        if (tracking)
        {
            if (currentHealth) { UpdateHealthUI(); return; } //do it with images


            gameObject.SetActive(tracking.gameObject.activeSelf);

            var percent = (tracking.currentHP / tracking.maxHP) * healthSprites.Count;
            if (percent > healthSprites.Count - 1)
            {
                currentHealth.sprite = healthSprites[healthSprites.Count - 1];
            }
            else
            {
                currentHealth.sprite = healthSprites[(int)percent];
            }

            if (trackPosition)
            {
                //transform.position = Camera.main.WorldToScreenPoint(tracking.transform.position + Offset);

                if (percent == 1 && displayed) 
                {
                    displayed = false;
                    foreach (Transform child in transform) 
                    { child.gameObject.SetActive(false); }
                }
                else if (percent < 1 && !displayed)
                { 
                    displayed = true;
                    foreach (Transform child in transform) 
                    { child.gameObject.SetActive(true); }
                }
            }
            else if (myRect) { myRect.anchoredPosition = Offset; }
            
        }
        else if (trackPosition) { gameObject.SetActive(false); }

    }
}
