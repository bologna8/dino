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

    public void PlayJumpSound(GameObject player){
        if(jumpSound != null){
            Instantiate(jumpSound, player.transform.position, Quaternion.identity);
        }
    }

}
