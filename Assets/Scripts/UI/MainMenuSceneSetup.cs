using UnityEngine;

/// <summary>
/// Auto-initializes the Main Menu scene
/// Just attach this script to any GameObject in a new scene, or create an empty scene
/// and add a GameObject with this script - everything will be built automatically
/// </summary>
public class MainMenuSceneSetup : MonoBehaviour
{
    private void Awake()
    {
        // Check if MainMenuBuilder already exists
        if (FindFirstObjectByType<MainMenuBuilder>() != null)
        {
            return;
        }

        // Create the menu builder
        GameObject menuManager = new GameObject("MainMenuManager");
        menuManager.AddComponent<MainMenuBuilder>();

        // Ensure LeaderboardManager exists (singleton)
        if (LeaderboardManager.Instance == null)
        {
            GameObject lbManager = new GameObject("LeaderboardManager");
            lbManager.AddComponent<LeaderboardManager>();
        }
        
        // Ensure AudioManager exists
        if (AudioManager.Instance == null)
        {
            GameObject audioManager = new GameObject("AudioManager");
            audioManager.AddComponent<AudioManager>();
        }

        // Set camera background color
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.backgroundColor = new Color(0.05f, 0.05f, 0.08f);
            mainCam.clearFlags = CameraClearFlags.SolidColor;
        }

        // Self-destruct after setup
        Destroy(gameObject);
    }
}
