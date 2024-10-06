using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompyAudio : MonoBehaviour
{
    [Header ("References")]
    public AI compyAI;

    [Header ("Neutral")]
    public List<GameObject> neutralSounds;

    public double neutralMinimumSpawnTime;
    public double neutralMaximumSpawnTime;

    [Header ("Chase")]
    public List<GameObject> chaseSounds;

    public double chaseMinimumSpawnTime;
    public double chaseMaximumSpawnTime;
    

    private double ChaseTime;
    private double NeutralTime;

    private double ChaseSpawnTime;
    private double NeutralSpawnTime;

    void Start(){
        ChaseSpawnTime = chaseMaximumSpawnTime;
        NeutralSpawnTime = neutralMaximumSpawnTime;
    }

    void Update()
    {
        if(compyAI != null){
            if(compyAI.currentState == AI.State.chase){

                ChaseTime += Time.deltaTime;

                if(ChaseTime >= ChaseSpawnTime){
                    if(chaseSounds != null && chaseSounds.Count > 0){
                        int x = Random.Range(0, chaseSounds.Count);
                        Instantiate(chaseSounds[x], this.transform.position, Quaternion.identity);
                    }
                    ChaseTime = 0;
                    ChaseSpawnTime = Random.Range((float)chaseMinimumSpawnTime, (float)chaseMaximumSpawnTime);
                }

            }else{

                NeutralTime += Time.deltaTime;

                if(NeutralTime >= NeutralSpawnTime){
                    if(neutralSounds != null && neutralSounds.Count > 0){
                        int x = Random.Range(0, neutralSounds.Count);
                        Instantiate(neutralSounds[x], this.transform.position, Quaternion.identity);
                    }
                    NeutralTime = 0;
                    NeutralSpawnTime = Random.Range((float)neutralMinimumSpawnTime, (float)neutralMaximumSpawnTime);
                }

            }
        }
    }
}
