using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUI : MonoBehaviour
{
    //list of all the masks on the journal map
    public List<SpriteMask> mapSpriteMasks;
    //list of all the colliders that activate the masks
    public List<Collider2D> mapColliders;

    //dictionary for corresponding sprite masks and colliders
    public Dictionary<SpriteMask, Collider2D> mapSections;

    private int colliderIndex;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("map ui: START");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("map ui: COLLISION");
        if (mapColliders.Contains(collision))
        {
            colliderIndex = mapColliders.IndexOf(collision);
            mapSpriteMasks[colliderIndex].gameObject.SetActive(true);
        }
    }
}
