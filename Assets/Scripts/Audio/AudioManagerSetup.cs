using UnityEngine;

/// <summary>
/// Automatically initializes the AudioManager if it doesn't exist.
/// Add this to any scene or it will be created automatically.
/// </summary>
public class AudioManagerSetup : MonoBehaviour
{
    private void Awake()
    {
        // Check if AudioManager already exists
        if (AudioManager.Instance == null)
        {
            GameObject audioManager = new GameObject("AudioManager");
            audioManager.AddComponent<AudioManager>();
        }

        // Self-destruct after setup
        Destroy(gameObject);
    }

    /// <summary>
    /// Static method to ensure AudioManager exists (call from anywhere)
    /// </summary>
    public static void EnsureAudioManager()
    {
        if (AudioManager.Instance == null)
        {
            GameObject audioManager = new GameObject("AudioManager");
            audioManager.AddComponent<AudioManager>();
        }
    }
}
