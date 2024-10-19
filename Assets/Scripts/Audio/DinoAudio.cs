using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinoAudio : MonoBehaviour
{
    [Header ("References")]
    public AI dinoAI;

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

    private AI.State previousAIState;

    void Start(){
        ChaseSpawnTime = chaseMaximumSpawnTime;
        NeutralSpawnTime = neutralMaximumSpawnTime;
        previousAIState = AI.State.idle;
    }

    void Update()
    {
        if(dinoAI != null){
            if(dinoAI.currentState == AI.State.chase){

                ChaseTime += Time.deltaTime;

                if(ChaseTime >= ChaseSpawnTime || previousAIState != AI.State.chase){
                    if(chaseSounds != null && chaseSounds.Count > 0){
                        int x = Random.Range(0, chaseSounds.Count);
                        GameObject EffectToSpawn = chaseSounds[x]; 
                        if (PoolManager.Instance != null) { PoolManager.Instance.Spawn(EffectToSpawn, transform.position); }
                        
                    }
                    ChaseTime = 0;
                    ChaseSpawnTime = Random.Range((float)chaseMinimumSpawnTime, (float)chaseMaximumSpawnTime);
                }

            }else{

                NeutralTime += Time.deltaTime;

                if(NeutralTime >= NeutralSpawnTime){
                    
                    if(neutralSounds != null && neutralSounds.Count > 0){
                        int x = Random.Range(0, neutralSounds.Count);
                        GameObject EffectToSpawn = neutralSounds[x]; 
                        if (PoolManager.Instance != null) { PoolManager.Instance.Spawn(EffectToSpawn, transform.position); }
                    }
                    NeutralTime = 0;
                    NeutralSpawnTime = Random.Range((float)neutralMinimumSpawnTime, (float)neutralMaximumSpawnTime);
    
                }

            }
            
            
            previousAIState = dinoAI.currentState;
        }
    }
}
