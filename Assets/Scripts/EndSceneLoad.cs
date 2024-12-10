using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader2D : MonoBehaviour
{
    [SerializeField]
    private string sceneToLoad;

    [SerializeField]
    private string triggeringTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object has the specified tag
        if (collision.CompareTag(triggeringTag))
        {
            LoadScene();
        }
    }

    private void LoadScene()
    {
        // Ensure the scene name is valid
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("Scene to load is not specified or is empty.");
        }
    }
}
