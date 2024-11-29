using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameConfirmation : MonoBehaviour
{
    [SerializeField] private GameObject confirmationPanel;

    public void ShowConfirmationPanel()
    {
        confirmationPanel.SetActive(true);
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("Gold Scene Crunch");
    }

    public void CancelNewGame()
    {
        confirmationPanel.SetActive(false);
    }
}
