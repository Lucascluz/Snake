using UnityEngine;

/// <summary>
/// Auto-initializes the Game Scene UI
/// Add this to the GameScene to automatically build the HUD, game over screen, etc.
/// </summary>
public class GameSceneUISetup : MonoBehaviour
{
    private void Awake()
    {
        // Check if GameUIBuilder already exists
        if (FindFirstObjectByType<GameUIBuilder>() != null)
        {
            return;
        }

        // Create the UI builder
        GameObject uiManager = new GameObject("GameUIManager");
        uiManager.AddComponent<GameUIBuilder>();

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

        // Self-destruct after setup
        Destroy(gameObject);
    }
}
