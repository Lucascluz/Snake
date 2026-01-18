using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the Game Mode selection menu with difficulty options and player name input
/// </summary>
public class GameModeMenu : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private CanvasGroup mainPanel;
    [SerializeField] private GameObject difficultyPanel;
    
    [Header("Player Input")]
    [SerializeField] private InputField nameInputField;
    [SerializeField] private Text playerNameDisplay;
    
    [Header("Difficulty Selection")]
    [SerializeField] private Button easyButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Text difficultyTitle;
    [SerializeField] private Text difficultyDescription;
    
    [Header("Navigation Buttons")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button backButton;
    
    [Header("Visual Styling")]
    [SerializeField] private Image difficultyIndicator;
    [SerializeField] private Image backgroundPanel;
    
    [Header("Difficulty Colors")]
    [SerializeField] private Color easyColor = new Color(0.3f, 0.85f, 0.3f);
    [SerializeField] private Color normalColor = new Color(1f, 0.75f, 0.2f);
    [SerializeField] private Color hardColor = new Color(0.9f, 0.25f, 0.25f);
    [SerializeField] private Color buttonNormalColor = new Color(0.2f, 0.2f, 0.25f);
    [SerializeField] private Color buttonSelectedColor = new Color(0.3f, 0.3f, 0.4f);
    
    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private GameDifficulty selectedDifficulty = GameDifficulty.Normal;
    private bool isAnimating = false;

    private void Start()
    {
        SetupButtonListeners();
        LoadSavedSettings();
        SelectDifficulty(selectedDifficulty);
        
        // Fade in animation
        if (mainPanel != null)
        {
            mainPanel.alpha = 0f;
            StartCoroutine(FadeIn(mainPanel, 0.3f));
        }
    }

    private void SetupButtonListeners()
    {
        if (easyButton != null)
            easyButton.onClick.AddListener(() => SelectDifficulty(GameDifficulty.Easy));
        
        if (normalButton != null)
            normalButton.onClick.AddListener(() => SelectDifficulty(GameDifficulty.Normal));
        
        if (hardButton != null)
            hardButton.onClick.AddListener(() => SelectDifficulty(GameDifficulty.Hard));
        
        if (startGameButton != null)
            startGameButton.onClick.AddListener(StartGame);
        
        if (backButton != null)
            backButton.onClick.AddListener(GoToMainMenu);

        // Update player name display when input changes
        if (nameInputField != null)
        {
            nameInputField.onValueChanged.AddListener(OnNameChanged);
        }
    }

    private void OnNameChanged(string newName)
    {
        if (playerNameDisplay != null)
        {
            playerNameDisplay.text = string.IsNullOrEmpty(newName) ? "Player" : newName;
        }
    }

    /// <summary>
    /// Selects a difficulty and updates the UI accordingly
    /// </summary>
    public void SelectDifficulty(GameDifficulty difficulty)
    {
        if (isAnimating) return;
        
        selectedDifficulty = difficulty;
        UpdateDifficultyUI();
        UpdateButtonStyles();
        
        // Play selection feedback
        StartCoroutine(PulseDifficultyIndicator());
    }

    private void UpdateDifficultyUI()
    {
        Color currentColor = GetDifficultyColor(selectedDifficulty);
        
        if (difficultyTitle != null)
        {
            difficultyTitle.text = selectedDifficulty.ToString().ToUpper();
            difficultyTitle.color = currentColor;
        }

        if (difficultyDescription != null)
        {
            difficultyDescription.text = GetDifficultyDescription(selectedDifficulty);
        }

        if (difficultyIndicator != null)
        {
            difficultyIndicator.color = currentColor;
        }

        // Update start button color to match difficulty
        if (startGameButton != null)
        {
            ColorBlock colors = startGameButton.colors;
            colors.normalColor = currentColor;
            colors.highlightedColor = new Color(currentColor.r + 0.1f, currentColor.g + 0.1f, currentColor.b + 0.1f);
            colors.pressedColor = new Color(currentColor.r - 0.1f, currentColor.g - 0.1f, currentColor.b - 0.1f);
            startGameButton.colors = colors;
        }
    }

    private string GetDifficultyDescription(GameDifficulty difficulty)
    {
        switch (difficulty)
        {
            case GameDifficulty.Easy:
                return "◆ No obstacles\n◆ Pass through walls\n◆ Relaxed gameplay\n\n<i>Perfect for beginners!</i>";
            case GameDifficulty.Normal:
                return "◆ No obstacles\n◆ Walls are deadly\n◆ Classic experience\n\n<i>The way it was meant to be played</i>";
            case GameDifficulty.Hard:
                return "◆ Random obstacles spawn\n◆ Teleporting portals\n◆ Walls are deadly\n\n<i>Only for the brave!</i>";
            default:
                return "";
        }
    }

    private Color GetDifficultyColor(GameDifficulty difficulty)
    {
        switch (difficulty)
        {
            case GameDifficulty.Easy:
                return easyColor;
            case GameDifficulty.Normal:
                return normalColor;
            case GameDifficulty.Hard:
                return hardColor;
            default:
                return Color.white;
        }
    }

    private void UpdateButtonStyles()
    {
        UpdateSingleButtonStyle(easyButton, selectedDifficulty == GameDifficulty.Easy, easyColor);
        UpdateSingleButtonStyle(normalButton, selectedDifficulty == GameDifficulty.Normal, normalColor);
        UpdateSingleButtonStyle(hardButton, selectedDifficulty == GameDifficulty.Hard, hardColor);
    }

    private void UpdateSingleButtonStyle(Button button, bool isSelected, Color accentColor)
    {
        if (button == null) return;

        ColorBlock colors = button.colors;
        
        if (isSelected)
        {
            colors.normalColor = accentColor;
            colors.highlightedColor = new Color(accentColor.r + 0.15f, accentColor.g + 0.15f, accentColor.b + 0.15f);
            colors.pressedColor = new Color(accentColor.r - 0.1f, accentColor.g - 0.1f, accentColor.b - 0.1f);
            colors.selectedColor = accentColor;
        }
        else
        {
            colors.normalColor = buttonNormalColor;
            colors.highlightedColor = new Color(buttonNormalColor.r + 0.1f, buttonNormalColor.g + 0.1f, buttonNormalColor.b + 0.1f);
            colors.pressedColor = new Color(buttonNormalColor.r - 0.05f, buttonNormalColor.g - 0.05f, buttonNormalColor.b - 0.05f);
            colors.selectedColor = buttonNormalColor;
        }
        
        button.colors = colors;

        // Update button text color
        Text buttonText = button.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.color = isSelected ? Color.white : new Color(0.7f, 0.7f, 0.7f);
        }
    }

    private System.Collections.IEnumerator PulseDifficultyIndicator()
    {
        if (difficultyIndicator == null) yield break;

        isAnimating = true;
        Vector3 originalScale = difficultyIndicator.transform.localScale;
        Vector3 targetScale = originalScale * 1.1f;
        
        float duration = 0.1f;
        float elapsed = 0f;
        
        // Scale up
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            difficultyIndicator.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        
        elapsed = 0f;
        
        // Scale down
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            difficultyIndicator.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }
        
        difficultyIndicator.transform.localScale = originalScale;
        isAnimating = false;
    }

    private System.Collections.IEnumerator FadeIn(CanvasGroup canvasGroup, float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// Starts the game with the selected settings
    /// </summary>
    public void StartGame()
    {
        // Create or get GameSettings
        GameSettings settings = GameSettings.Instance;
        
        if (settings == null)
        {
            GameObject settingsObj = new GameObject("GameSettings");
            settings = settingsObj.AddComponent<GameSettings>();
        }
        
        // Configure player name
        string playerName = "Player";
        if (nameInputField != null && !string.IsNullOrEmpty(nameInputField.text))
        {
            playerName = nameInputField.text;
        }
        settings.SetPlayerName(playerName);

        // Configure difficulty
        settings.SetDifficulty(selectedDifficulty);

        // Save settings
        SaveSettings();

        Debug.Log($"Starting game with Player: {playerName}, Difficulty: {selectedDifficulty}");

        // Load game scene
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Returns to the main menu
    /// </summary>
    public void GoToMainMenu()
    {
        SaveSettings();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void SaveSettings()
    {
        string playerName = "Player";
        if (nameInputField != null && !string.IsNullOrEmpty(nameInputField.text))
        {
            playerName = nameInputField.text;
        }
        
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.SetInt("Difficulty", (int)selectedDifficulty);
        PlayerPrefs.Save();
    }

    private void LoadSavedSettings()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            string savedName = PlayerPrefs.GetString("PlayerName");
            if (nameInputField != null)
            {
                nameInputField.text = savedName;
            }
            if (playerNameDisplay != null)
            {
                playerNameDisplay.text = savedName;
            }
        }

        if (PlayerPrefs.HasKey("Difficulty"))
        {
            selectedDifficulty = (GameDifficulty)PlayerPrefs.GetInt("Difficulty");
        }
    }

    private void Update()
    {
        // Keyboard shortcuts
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoToMainMenu();
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            StartGame();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            SelectDifficulty(GameDifficulty.Easy);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            SelectDifficulty(GameDifficulty.Normal);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            SelectDifficulty(GameDifficulty.Hard);
        }
    }
}
