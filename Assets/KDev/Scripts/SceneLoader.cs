using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadTrigger : MonoBehaviour
{
    //Nicam Liu
    //Your scene loader only worked if it was attached to a unity event

    [Header("Scene Settings")]
    [Tooltip("Name of the scene to load (leave empty to load next scene)")]
    public string sceneNameToLoad = "";

    [Tooltip("Scene build index to load (-1 to use name or load next scene)")]
    public int sceneBuildIndex = -1;

    [Header("Trigger Settings")]
    [Tooltip("Tag of the player object (default: 'Player')")]
    public string playerTag = "Player";

    [Tooltip("Delay before loading scene (in seconds)")]
    public float loadDelay = 0f;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered is the player and hasn't triggered yet
        if (other.CompareTag(playerTag) && !hasTriggered)
        {
            hasTriggered = true;

            if (loadDelay > 0)
            {
                Invoke(nameof(LoadScene), loadDelay);
            }
            else
            {
                LoadScene();
            }
        }
    }

    private void LoadScene()
    {
        // Build Index > Scene Name > Next Scene
        if (sceneBuildIndex >= 0)
        {
            SceneManager.LoadScene(sceneBuildIndex);
        }
        else if (!string.IsNullOrEmpty(sceneNameToLoad))
        {
            SceneManager.LoadScene(sceneNameToLoad);
        }
        else
        {
            // Load next scene in build order
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
    }
}