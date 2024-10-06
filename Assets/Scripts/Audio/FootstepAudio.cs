using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class FootstepAudio : MonoBehaviour
{
    [Header("References")]
    [SerializedDictionary("GroundType", "List")]
    public SerializedDictionary<GroundType.Type, List<GameObject>> FootstepLists;

    public Movement playerMovement;

    public DetectGround detectGround;

    [Header("Properties")]
    public double SpawnRate = 0.5;

    private double timeAlive = 0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerMovement != null){
            if(playerMovement.onGround && (playerMovement.moveInput >= 0.5 || playerMovement.moveInput <= -0.5)){
                timeAlive += Time.deltaTime;
            }
        }else{
            Debug.Log("Error: no player movement script attached to this script");
        }

        if(timeAlive >= SpawnRate){
            if(detectGround != null){
                GameObject g = GetFootstep(detectGround.standingOn);
                PoolManager.Instance.Spawn(g, transform.position, Quaternion.identity);
            }else{
                Debug.Log("Error: no detect ground script attached to this script");
            }
            timeAlive = 0;
        }

    }

    GameObject GetFootstep(GroundType.Type gt){
        if(FootstepLists != null){
            List<GameObject> lg = FootstepLists[gt];
            if(lg.Count > 0){
                int rand = Random.Range(0, lg.Count);
                return lg[rand];
            }
        }else{
            Debug.Log("Error: Footstep dictionary is empty");
        }
        
        return null;            
        
    }
}
