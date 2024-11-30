using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewGameConfirmation : MonoBehaviour
{
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private GameObject firstButton; 
    [SerializeField] private GameObject mainMenuFirstButton; 

    [SerializeField] private GameObject[] menuButtons; 

    private GameObject previousSelectedButton; 

    public void ShowConfirmationPanel()
    {
        previousSelectedButton = EventSystem.current.currentSelectedGameObject;

        confirmationPanel.SetActive(true);

        SetMenuButtonsInteractable(false);

        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("Gold Scene Crunch");
    }

    public void CancelNewGame()
    {
        confirmationPanel.SetActive(false);

        SetMenuButtonsInteractable(true);

        if (previousSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(previousSelectedButton);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(mainMenuFirstButton);
        }
    }

    private void SetMenuButtonsInteractable(bool state)
    {
        foreach (GameObject button in menuButtons)
        {
            if (button.TryGetComponent<Button>(out var btn))
            {
                btn.interactable = state;
            }
        }
    }
}
