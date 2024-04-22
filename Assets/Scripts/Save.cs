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

    public static float deadTime;
    private Image fadeSprite;
    private bool fading = false;

    // Start is called before the first frame update
    void Start()
    {
        fadeSprite = GetComponentInChildren<Image>();

        if (checkpointParent)
        {
            allPoints = checkpointParent.GetComponentsInChildren<Checkpoint>();
        }

        for(int i = 0; i <  allPoints.Length; i++) { allPoints[i].Number = i; }

        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        //Death effect stuff
        if (player) { deadTime = 0f; }
        else { deadTime += Time.deltaTime; }

        var flashTime = 0.1f;
        var flashFade = 0.4f;

        if (fadeSprite)
        {
            if (deadTime > 0) { fadeSprite.gameObject.SetActive(true); }
            else { fadeSprite.gameObject.SetActive(false); }

            if (deadTime < flashTime) { fadeSprite.color = new Color(1,1,1, deadTime / (flashTime)); }
            else { fadeSprite.color = new Color(1,1,1, (flashTime + flashFade - deadTime) / (flashTime + flashFade)); }
        }   

        bool respawnReady = false;
        if (deadTime > (flashTime + flashFade)) { respawnReady = true; }

        if (Input.anyKey && respawnReady && !fading) { StartCoroutine(FadeToBlack()); }

    }

    public void Spawn()
    {
        deadTime = 0f;
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

    IEnumerator FadeToBlack()
    {
        fading = true;
        
        var initialColor = new Color (0,0,0,0);
        var targetColor = Color.black;
        float elapsedTime = 0f;
        float fadeDuration = 1f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            if (fadeSprite) { fadeSprite.color = Color.Lerp(initialColor, targetColor, elapsedTime / fadeDuration); }      
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    
}
