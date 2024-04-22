using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Save : MonoBehaviour
{
    public static Checkpoint[] allPoints;
    public Transform checkpointParent;
    public GameObject playerPrefab;
    public static Transform player;
    private Image fadeSprite;

    public GameObject deathText;
    private bool fading = false;

    private bool readyToRespawn = false;

    // Start is called before the first frame update
    void Start()
    {
        fadeSprite = GetComponentInChildren<Image>();
        if (fadeSprite) { fadeSprite.gameObject.SetActive(false); }
        if (deathText) { deathText.SetActive(false); }

        if (checkpointParent)
        {
            allPoints = checkpointParent.GetComponentsInChildren<Checkpoint>();
        }

        for(int i = 0; i <  allPoints.Length; i++) { allPoints[i].Number = i; }

        Spawn();
        StartCoroutine(Fade(Color.black, Color.clear, 2.2f));
    }

    // Update is called once per frame
    void Update()
    {
        //Death effect stuff
        if (!player)
        {
            if (!readyToRespawn) 
            { StartCoroutine(FlashSequence(0.123f, 0.42f)); }
            else if (!fading && Input.anyKey)
            { StartCoroutine(Fade(Color.clear, Color.black, 2.2f, true)); }
        }

    }

    public void Spawn()
    {
        var currentPoint = allPoints[PlayerPrefs.GetInt("CurrentCheckpoint", 0)];
        currentPoint.active = true;
        var spawnSpot = currentPoint.transform.position;
        player = Instantiate(playerPrefab, spawnSpot, Quaternion.identity).transform;
        
    }

    public static void SetCheckpoint(Checkpoint point)
    {
        PlayerPrefs.SetInt("CurrentCheckpoint", point.Number);
        foreach(var check in allPoints) { check.active = false; }
        point.active = true;
    }

    IEnumerator FlashSequence(float brightenTime, float fadeTime)
    {
        readyToRespawn = true;
        StartCoroutine(Fade(Color.clear, Color.white, brightenTime));
        yield return new WaitForSeconds(brightenTime);
        StartCoroutine(Fade(Color.white, Color.clear, fadeTime));
        if (deathText) { deathText.SetActive(true); }
    }
    IEnumerator Fade(Color startColor, Color targetColor, float fadeDuration = 1, bool reloadScene = false)
    {
        fading = true;
        float elapsedTime = 0f;
        if (fadeSprite) { fadeSprite.gameObject.SetActive(true); }
        if (deathText) { deathText.SetActive(false); }

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            if (fadeSprite) { fadeSprite.color = Color.Lerp(startColor, targetColor, elapsedTime / fadeDuration); }      
            yield return null;
        }

        fading = false;
        if (reloadScene) { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
        else if (fadeSprite) { fadeSprite.gameObject.SetActive(false); }
    }

    
}
