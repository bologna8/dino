using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiSpriteHandler : MonoBehaviour
{

    public bool imagesInUI;
    private SpriteRenderer[] subSprites;
    private Image[] subImages;

    [HideInInspector] public Sprite[] startSprites;
    private Color[] startColors;

    private Vector3 startScale;

    private bool initialized;

    void Awake()
    {
        if (imagesInUI)
        {
            subImages = GetComponentsInChildren<Image>(); 
            startSprites = new Sprite[subImages.Length];
            startColors = new Color[subImages.Length];
            for (int i = 0; i < subImages.Length; i++)
            { 
                startSprites[i] = subImages[i].sprite;
                startColors[i] = subImages[i].color; 
            }
        }
        else
        { 
            subSprites = GetComponentsInChildren<SpriteRenderer>();
            startSprites = new Sprite[subSprites.Length];
            startColors = new Color[subSprites.Length];
            for (int i = 0; i < subImages.Length; i++)
            { 
                startSprites[i] = subSprites[i].sprite;
                startColors[i] = subSprites[i].color;
            }
        }

        startScale = transform.localScale;
        
        initialized = true;
           
    }

    public void changeAlpha(float newAlpha, int index = 0)
    {
        if (!initialized) { return; }

        if (imagesInUI)
        {
            if (index > subImages.Length) { return; }
            subImages[index].color = new Color (startColors[index].r, startColors[index].g, startColors[index].b, newAlpha);
        }
        else
        {
            if (index > subSprites.Length) { return; }
            subSprites[index].color = new Color (startColors[index].r, startColors[index].g, startColors[index].b, newAlpha);
        }
    }

    public void changeSprite(Sprite newSprite = null, int index = 0)
    {
        if (!initialized) { return; }

        if (newSprite == null) { newSprite = startSprites[index]; }

        if (imagesInUI)
        {
            if (index > subImages.Length) { return; }
            subImages[index].sprite = newSprite;
        }
        else
        {
            if (index > subSprites.Length) { return; }
            subSprites[index].sprite = newSprite;
        }
    }

    public void changeSize(float percent = 1)
    {
        if (!initialized) { return; }

        transform.localScale = startScale * percent;
    }
}
