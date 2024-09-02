using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    #region singleton
    public static AudioManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    #endregion

    // Ethan made this

    [Header("Jump Sound")]
    [Tooltip("Jump sound effect")] public GameObject jumpSound;

    [Header("Dig Sounds")]
    [Tooltip("List of digging sound effects")] public List<GameObject> diggingSounds;

    public void PlayJumpSound(GameObject player){
        if(jumpSound != null){
            Debug.Log("Jump sound played!");
            Instantiate(jumpSound, player.transform.position, Quaternion.identity);
        }
    }

    public void PlayDigSound(GameObject player){
        if(diggingSounds != null){
            Debug.Log("Dig sound played!");
            int x = Random.Range(0, diggingSounds.Count - 1);
            Instantiate(diggingSounds[x], player.transform.position, Quaternion.identity);
        }
    }

}
