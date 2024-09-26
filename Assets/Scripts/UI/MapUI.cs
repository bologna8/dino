using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUI : MonoBehaviour
{
    //list of all the masks on the journal map
    public List<SpriteMask> mapSpriteMasks;
    //list of all the colliders that activate the masks
    public List<Collider2D> mapColliders;

    //list of campfire colliders
    public List<Collider2D> campfireColliders;

    //list of campfire icons
    public List<GameObject> campfireIcons;

    //dictionary for corresponding sprite masks and colliders
    private Dictionary<SpriteMask, Collider2D> mapSections;

    //index of the collider found
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

        if (campfireColliders.Contains(collision))
        {
            colliderIndex = campfireColliders.IndexOf(collision);
            campfireIcons[colliderIndex].gameObject.SetActive(true);
        }
    }
}
