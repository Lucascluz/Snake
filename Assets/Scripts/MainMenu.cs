using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Main Menu controller - handles the primary menu screen with Play, Leaderboard, and Quit options
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private CanvasGroup mainPanel;
    [SerializeField] private GameObject titleContainer;
    
    [Header("Title Animation")]
    [SerializeField] private Text titleText;
    [SerializeField] private Text subtitleText;
    
    [Header("Menu Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Button quitButton;
    
    [Header("Leaderboard")]
    [SerializeField] private LeaderboardUI leaderboardUI;
    
    [Header("Visual Styling")]
    [SerializeField] private Color primaryColor = new Color(0.3f, 0.85f, 0.4f);
    [SerializeField] private Color secondaryColor = new Color(0.2f, 0.6f, 0.3f);
    [SerializeField] private Color buttonColor = new Color(0.15f, 0.15f, 0.2f);
    [SerializeField] private Color buttonHoverColor = new Color(0.25f, 0.25f, 0.35f);
    
    [Header("Scene Settings")]
    [SerializeField] private string gameModeSceneName = "GameModeMenu";

    private float titleAnimationTime = 0f;
    private Vector3 titleOriginalScale = Vector3.one;

    private void Start()
    {
        SetupButtonListeners();
        SetupButtonStyles();
        
        // Store original title scale for animation
        if (titleText != null)
        {
            titleOriginalScale = titleText.transform.localScale;
        }
        
        // Fade in animation
        if (mainPanel != null)
        {
            mainPanel.alpha = 0f;
            StartCoroutine(FadeIn(mainPanel, 0.4f));
        }
        
        // Animate title entrance
        if (titleContainer != null || titleText != null)
        {
            StartCoroutine(AnimateTitleEntrance());
        }
    }

    private void Update()
    {
        // Subtle title animation
        AnimateTitle();
        
        // Keyboard shortcuts
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
        {
            PlayGame();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            ShowLeaderboard();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    private void AnimateTitle()
    {
        if (titleText == null) return;
        
        titleAnimationTime += Time.deltaTime;
        
        // Gentle floating animation
        float yOffset = Mathf.Sin(titleAnimationTime * 1.5f) * 3f;
        float scaleOffset = 1f + Mathf.Sin(titleAnimationTime * 2f) * 0.02f;
        
        titleText.transform.localPosition = new Vector3(
            titleText.transform.localPosition.x,
            yOffset,
            titleText.transform.localPosition.z
        );
        
        titleText.transform.localScale = titleOriginalScale * scaleOffset;
    }

    private System.Collections.IEnumerator AnimateTitleEntrance()
    {
        if (titleText != null)
        {
            titleText.transform.localScale = Vector3.zero;
        }
        if (subtitleText != null)
        {
            subtitleText.color = new Color(subtitleText.color.r, subtitleText.color.g, subtitleText.color.b, 0f);
        }
        
        yield return new WaitForSeconds(0.2f);
        
        // Animate title scale
        if (titleText != null)
        {
            float elapsed = 0f;
            float duration = 0.5f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                // Ease out back
                float overshoot = 1.5f;
                t = 1f - Mathf.Pow(1f - t, 2f);
                t = t * (1f + overshoot * (1f - t));
                
                titleText.transform.localScale = titleOriginalScale * Mathf.Min(t, 1.1f);
                yield return null;
            }
            
            titleText.transform.localScale = titleOriginalScale;
        }
        
        // Fade in subtitle
        if (subtitleText != null)
        {
            float elapsed = 0f;
            float duration = 0.3f;
            Color startColor = subtitleText.color;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                subtitleText.color = new Color(startColor.r, startColor.g, startColor.b, t);
                yield return null;
            }
        }
    }

    private void SetupButtonListeners()
    {
        if (playButton != null)
            playButton.onClick.AddListener(PlayGame);
        
        if (leaderboardButton != null)
            leaderboardButton.onClick.AddListener(ShowLeaderboard);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    private void SetupButtonStyles()
    {
        StyleButton(playButton, primaryColor);
        StyleButton(leaderboardButton, buttonColor, secondaryColor);
        StyleButton(quitButton, buttonColor, new Color(0.8f, 0.3f, 0.3f));
    }

    private void StyleButton(Button button, Color normalColor, Color? hoverColor = null)
    {
        if (button == null) return;
        
        Color hover = hoverColor ?? new Color(normalColor.r + 0.15f, normalColor.g + 0.15f, normalColor.b + 0.15f);
        
        ColorBlock colors = button.colors;
        colors.normalColor = normalColor;
        colors.highlightedColor = hover;
        colors.pressedColor = new Color(normalColor.r - 0.1f, normalColor.g - 0.1f, normalColor.b - 0.1f);
        colors.selectedColor = normalColor;
        colors.fadeDuration = 0.1f;
        button.colors = colors;
    }

    /// <summary>
    /// Navigate to game mode selection
    /// </summary>
    public void PlayGame()
    {
        StartCoroutine(TransitionToScene(gameModeSceneName));
    }

    /// <summary>
    /// Show the leaderboard panel
    /// </summary>
    public void ShowLeaderboard()
    {
        if (leaderboardUI != null)
        {
            leaderboardUI.Show();
        }
    }

    /// <summary>
    /// Exit the game
    /// </summary>
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private System.Collections.IEnumerator TransitionToScene(string sceneName)
    {
        // Fade out
        if (mainPanel != null)
        {
            float elapsed = 0f;
            float duration = 0.25f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                mainPanel.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                yield return null;
            }
        }
        
        SceneManager.LoadScene(sceneName);
    }

    private System.Collections.IEnumerator FadeIn(CanvasGroup canvasGroup, float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
    }
}