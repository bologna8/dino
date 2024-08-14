using UnityEngine;
using UnityEngine.UI;

public class FlipPage : MonoBehaviour
{
    private enum ButtonType
    {
        NextButton,
        PrevButton,
    }

    [SerializeField] Button prevButton;
    [SerializeField] Button nextButton;
    [SerializeField] Button closeButton;

    private Vector3 rotationVector;
    private Quaternion startRotation;
    private bool isFlipping;
    private float rotationDuration = 1.0f;
    private float rotationTime;

    void Start()
    {
        startRotation = transform.rotation;

        if (nextButton != null)
        {
            nextButton.onClick.AddListener(() => TurnOnePage(ButtonType.NextButton));
        }
        else
        {
            Debug.LogError("Next Button is not assigned.");
        }

        if (prevButton != null)
        {
            prevButton.onClick.AddListener(() => TurnOnePage(ButtonType.PrevButton));
        }
        else
        {
            Debug.LogError("Prev Button is not assigned.");
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseBook);
        }
        else
        {
            Debug.LogError("Close Button is not assigned.");
        }
    }

    void Update()
    {
        if (isFlipping)
        {
            float t = (Time.time - rotationTime) / rotationDuration;
            if (t >= 1)
            {
                isFlipping = false;
                transform.rotation = startRotation * Quaternion.Euler(rotationVector);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(startRotation, startRotation * Quaternion.Euler(rotationVector), t);
            }
        }
    }

    private void TurnOnePage(ButtonType type)
    {
        Debug.Log($"Button clicked: {type}");
        isFlipping = true;
        rotationTime = Time.time;

        rotationVector = type == ButtonType.NextButton ? new Vector3(0, 180, 0) : new Vector3(0, -180, 0);
    }

    private void CloseBook()
    {
        BookEvent.CloseBookFunction();
    }
}
