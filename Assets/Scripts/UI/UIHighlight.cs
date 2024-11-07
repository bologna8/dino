using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public Color highlightColor = Color.yellow;  // Color to highlight the item
    private Color originalColor;
    private Image itemImage;

    private void Awake()
    {
        itemImage = GetComponent<Image>();
        if (itemImage != null)
        {
            originalColor = itemImage.color;
        }
    }

    // Mouse hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        HighlightItem();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RemoveHighlight();
    }

    // Controller or keyboard selection
    public void OnSelect(BaseEventData eventData)
    {
        HighlightItem();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        RemoveHighlight();
    }

    private void HighlightItem()
    {
        if (itemImage != null)
        {
            itemImage.color = highlightColor;
        }
    }

    private void RemoveHighlight()
    {
        if (itemImage != null)
        {
            itemImage.color = originalColor;
        }
    }
}
