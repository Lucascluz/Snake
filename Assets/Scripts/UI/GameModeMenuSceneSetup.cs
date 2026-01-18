using UnityEngine;

/// <summary>
/// Auto-initializes the Game Mode Menu scene
/// Just attach this script to any GameObject in a new scene
/// </summary>
public class GameModeMenuSceneSetup : MonoBehaviour
{
    private void Awake()
    {
        // Check if GameModeMenuBuilder already exists
        if (FindFirstObjectByType<GameModeMenuBuilder>() != null)
        {
            return;
        }

        // Create the menu builder
        GameObject menuManager = new GameObject("GameModeMenuManager");
        menuManager.AddComponent<GameModeMenuBuilder>();

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
