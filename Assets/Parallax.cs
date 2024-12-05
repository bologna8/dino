using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{

    private float _startingPosition;
    private float _lengthOfSprite;
    public float amountOfParallax;
    //public Camera mainCamera;

    public Transform mainCamera;


    // Start is called before the first frame update
    void Start()
    {
        //starting x position of the sprint
        _startingPosition = transform.position.x;

        //x length of the sprite
        _lengthOfSprite = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        //get camera position
        Vector3 cameraPosition = mainCamera.transform.position;
        float tempPosition = cameraPosition.x * (1 - amountOfParallax);
        float distance = cameraPosition.x * amountOfParallax;

        Vector3 newPosition = new Vector3(_startingPosition + distance, transform.position.y, transform.position.z);
        transform.position = newPosition;
    }
}
