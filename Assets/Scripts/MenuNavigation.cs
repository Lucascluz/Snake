using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles navigation between game scenes
/// </summary>
public class MenuNavigation : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string gameModeMenuSceneName = "GameModeMenu";
    [SerializeField] private string gameSceneName = "GameScene";

    /// <summary>
    /// Navigate to the main menu
    /// </summary>
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    /// <summary>
    /// Navigate to the game mode selection menu
    /// </summary>
    public void GoToGameModeMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameModeMenuSceneName);
    }

    /// <summary>
    /// Start the game (or restart)
    /// </summary>
    public void GoToGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Restart the current scene
    /// </summary>
    public void RestartCurrentScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Quit the game
    /// </summary>
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}