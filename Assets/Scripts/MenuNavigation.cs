using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigation : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string gameSceneName = "GameScene";

    /// <summary>
    /// Volta para o menu principal
    /// </summary>
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Garante que o tempo está normal
        SceneManager.LoadScene(mainMenuSceneName);
    }

    /// <summary>
    /// Inicia o jogo (ou reinicia)
    /// </summary>
    public void GoToGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Reinicia a cena atual
    /// </summary>
    public void RestartCurrentScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Sai do jogo
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