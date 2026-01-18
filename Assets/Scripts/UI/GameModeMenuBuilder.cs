using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Builds and manages the Game Mode Menu UI programmatically
/// Attach this to an empty GameObject in the GameModeMenu scene
/// </summary>
public class GameModeMenuBuilder : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    // UI References
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private InputField nameInput;
    private Text difficultyTitle;
    private Text difficultyDescription;
    private Button easyButton;
    private Button normalButton;
    private Button hardButton;
    private Button startButton;
    private Image difficultyIndicator;

    private GameDifficulty selectedDifficulty = GameDifficulty.Normal;

    private void Start()
    {
        BuildUI();
        LoadSavedSettings();
        SelectDifficulty(selectedDifficulty);
        StartCoroutine(AnimateEntrance());
    }

    private void Update()
    {
        HandleInput();
    }

    private void BuildUI()
    {
        // Create canvas
        canvas = UIBuilder.CreateCanvas("GameModeCanvas");
        canvasGroup = UIBuilder.AddCanvasGroup(canvas.gameObject);
        canvasGroup.alpha = 0f;

        // Background
        UIBuilder.CreatePanel(canvas.transform, "Background", UIBuilder.DarkBackground);

        // Main content panel
        RectTransform contentPanel = UIBuilder.CreateCenteredPanel(canvas.transform, "Content", 900, 700, Color.clear);

        // Back button (top left)
        Button backButton = UIBuilder.CreateButton(canvas.transform, "BackButton", "← BACK", 150, 50, UIBuilder.ButtonNormal, OnBackClicked);
        RectTransform backRect = backButton.GetComponent<RectTransform>();
        backRect.anchorMin = new Vector2(0, 1);
        backRect.anchorMax = new Vector2(0, 1);
        backRect.anchoredPosition = new Vector2(100, -50);

        // Title
        Text titleText = UIBuilder.CreateText(contentPanel, "Title", "SELECT GAME MODE", 48, UIBuilder.PrimaryGreen);
        titleText.fontStyle = FontStyle.Bold;
        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 280);

        // Player Name Section
        Text nameLabel = UIBuilder.CreateText(contentPanel, "NameLabel", "PLAYER NAME", 22, new Color(0.7f, 0.7f, 0.75f));
        RectTransform nameLabelRect = nameLabel.GetComponent<RectTransform>();
        nameLabelRect.anchoredPosition = new Vector2(0, 200);

        nameInput = UIBuilder.CreateInputField(contentPanel, "NameInput", "Enter your name...", 350, 55);
        RectTransform nameInputRect = nameInput.GetComponent<RectTransform>();
        nameInputRect.anchoredPosition = new Vector2(0, 150);

        // Difficulty Section
        Text diffLabel = UIBuilder.CreateText(contentPanel, "DifficultyLabel", "DIFFICULTY", 22, new Color(0.7f, 0.7f, 0.75f));
        RectTransform diffLabelRect = diffLabel.GetComponent<RectTransform>();
        diffLabelRect.anchoredPosition = new Vector2(0, 80);

        // Difficulty buttons (horizontal)
        HorizontalLayoutGroup diffButtons = UIBuilder.CreateHorizontalLayout(contentPanel, "DifficultyButtons", 20f);
        RectTransform diffButtonsRect = diffButtons.GetComponent<RectTransform>();
        diffButtonsRect.anchoredPosition = new Vector2(0, 20);

        easyButton = UIBuilder.CreateButton(diffButtons.transform, "EasyButton", "EASY", 150, 60, UIBuilder.ButtonNormal, () => SelectDifficulty(GameDifficulty.Easy));
        normalButton = UIBuilder.CreateButton(diffButtons.transform, "NormalButton", "NORMAL", 150, 60, UIBuilder.ButtonNormal, () => SelectDifficulty(GameDifficulty.Normal));
        hardButton = UIBuilder.CreateButton(diffButtons.transform, "HardButton", "HARD", 150, 60, UIBuilder.ButtonNormal, () => SelectDifficulty(GameDifficulty.Hard));

        // Difficulty indicator bar
        GameObject indicatorObj = new GameObject("DifficultyIndicator");
        indicatorObj.transform.SetParent(contentPanel, false);
        RectTransform indicatorRect = indicatorObj.AddComponent<RectTransform>();
        indicatorRect.anchoredPosition = new Vector2(0, -25);
        indicatorRect.sizeDelta = new Vector2(480, 6);
        difficultyIndicator = indicatorObj.AddComponent<Image>();
        difficultyIndicator.color = UIBuilder.NormalYellow;

        // Selected difficulty display
        difficultyTitle = UIBuilder.CreateText(contentPanel, "DifficultyTitle", "NORMAL", 42, UIBuilder.NormalYellow);
        difficultyTitle.fontStyle = FontStyle.Bold;
        RectTransform diffTitleRect = difficultyTitle.GetComponent<RectTransform>();
        diffTitleRect.anchoredPosition = new Vector2(0, -70);

        // Difficulty description box
        RectTransform descBox = UIBuilder.CreateCenteredPanel(contentPanel, "DescriptionBox", 500, 130, new Color(0.1f, 0.1f, 0.14f, 0.8f));
        descBox.anchoredPosition = new Vector2(0, -155);

        difficultyDescription = UIBuilder.CreateText(descBox, "Description", "", 20, new Color(0.8f, 0.8f, 0.85f), TextAnchor.MiddleCenter);
        RectTransform descRect = difficultyDescription.GetComponent<RectTransform>();
        UIBuilder.StretchToFill(descRect);
        descRect.offsetMin = new Vector2(20, 10);
        descRect.offsetMax = new Vector2(-20, -10);

        // Start button
        startButton = UIBuilder.CreateButton(contentPanel, "StartButton", "▶  START GAME", 350, 75, UIBuilder.PrimaryGreen, OnStartClicked);
        RectTransform startRect = startButton.GetComponent<RectTransform>();
        startRect.anchoredPosition = new Vector2(0, -280);

        // Footer hints
        Text footerText = UIBuilder.CreateText(contentPanel, "Footer", "Press 1/2/3 to select difficulty  •  ENTER to start  •  ESC to go back", 16, new Color(0.4f, 0.4f, 0.45f));
        RectTransform footerRect = footerText.GetComponent<RectTransform>();
        footerRect.anchoredPosition = new Vector2(0, -340);
        footerRect.sizeDelta = new Vector2(800, 25);
    }

    private void SelectDifficulty(GameDifficulty difficulty)
    {
        selectedDifficulty = difficulty;
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuSelectSound();
            
        UpdateDifficultyUI();
        StartCoroutine(AnimateDifficultyChange());
    }

    private void UpdateDifficultyUI()
    {
        Color diffColor = GetDifficultyColor(selectedDifficulty);

        // Update title
        if (difficultyTitle != null)
        {
            difficultyTitle.text = selectedDifficulty.ToString().ToUpper();
            difficultyTitle.color = diffColor;
        }

        // Update indicator
        if (difficultyIndicator != null)
        {
            difficultyIndicator.color = diffColor;
        }

        // Update description
        if (difficultyDescription != null)
        {
            difficultyDescription.text = GetDifficultyDescription(selectedDifficulty);
        }

        // Update button colors
        UpdateButtonColor(easyButton, selectedDifficulty == GameDifficulty.Easy, UIBuilder.EasyGreen);
        UpdateButtonColor(normalButton, selectedDifficulty == GameDifficulty.Normal, UIBuilder.NormalYellow);
        UpdateButtonColor(hardButton, selectedDifficulty == GameDifficulty.Hard, UIBuilder.HardRed);

        // Update start button color
        if (startButton != null)
        {
            ColorBlock colors = startButton.colors;
            colors.normalColor = diffColor;
            colors.highlightedColor = new Color(diffColor.r + 0.1f, diffColor.g + 0.1f, diffColor.b + 0.1f);
            colors.pressedColor = new Color(diffColor.r - 0.1f, diffColor.g - 0.1f, diffColor.b - 0.1f);
            startButton.colors = colors;
        }
    }

    private void UpdateButtonColor(Button button, bool isSelected, Color accentColor)
    {
        if (button == null) return;

        ColorBlock colors = button.colors;
        if (isSelected)
        {
            colors.normalColor = accentColor;
            colors.highlightedColor = new Color(accentColor.r + 0.12f, accentColor.g + 0.12f, accentColor.b + 0.12f);
        }
        else
        {
            colors.normalColor = UIBuilder.ButtonNormal;
            colors.highlightedColor = UIBuilder.ButtonHover;
        }
        button.colors = colors;

        // Update text color
        Text btnText = button.GetComponentInChildren<Text>();
        if (btnText != null)
        {
            btnText.color = isSelected ? Color.white : new Color(0.7f, 0.7f, 0.7f);
        }
    }

    private Color GetDifficultyColor(GameDifficulty diff)
    {
        switch (diff)
        {
            case GameDifficulty.Easy: return UIBuilder.EasyGreen;
            case GameDifficulty.Normal: return UIBuilder.NormalYellow;
            case GameDifficulty.Hard: return UIBuilder.HardRed;
            default: return Color.white;
        }
    }

    private string GetDifficultyDescription(GameDifficulty diff)
    {
        switch (diff)
        {
            case GameDifficulty.Easy:
                return "◆ No obstacles\n◆ Pass through walls\n◆ Perfect for beginners!";
            case GameDifficulty.Normal:
                return "◆ No obstacles\n◆ Walls are deadly\n◆ The classic Snake experience";
            case GameDifficulty.Hard:
                return "◆ Random obstacles spawn\n◆ Teleporting portals\n◆ Only for the brave!";
            default:
                return "";
        }
    }

    private IEnumerator AnimateDifficultyChange()
    {
        if (difficultyIndicator == null) yield break;

        Vector3 originalScale = difficultyIndicator.transform.localScale;
        float elapsed = 0f;
        float duration = 0.15f;

        // Scale up
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.15f;
            difficultyIndicator.transform.localScale = new Vector3(originalScale.x * scale, originalScale.y * 1.5f, 1f);
            yield return null;
        }

        difficultyIndicator.transform.localScale = originalScale;
    }

    private IEnumerator AnimateEntrance()
    {
        yield return new WaitForSeconds(0.05f);

        float elapsed = 0f;
        float duration = 0.4f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private void HandleInput()
    {
        // Don't handle shortcuts if typing in input field
        if (nameInput != null && nameInput.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                // Deselect input and start game
                nameInput.DeactivateInputField();
                OnStartClicked();
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
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
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnStartClicked();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnBackClicked();
        }
    }

    private void OnStartClicked()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuClickSound();
            
        // Setup GameSettings
        GameSettings settings = GameSettings.Instance;
        if (settings == null)
        {
            GameObject settingsObj = new GameObject("GameSettings");
            settings = settingsObj.AddComponent<GameSettings>();
        }

        string playerName = "Player";
        if (nameInput != null && !string.IsNullOrEmpty(nameInput.text))
        {
            playerName = nameInput.text;
        }

        settings.SetPlayerName(playerName);
        settings.SetDifficulty(selectedDifficulty);

        SaveSettings();

        Debug.Log($"Starting game - Player: {playerName}, Difficulty: {selectedDifficulty}");

        StartCoroutine(TransitionToScene(gameSceneName));
    }

    private void OnBackClicked()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuClickSound();
            
        SaveSettings();
        StartCoroutine(TransitionToScene(mainMenuSceneName));
    }

    private void SaveSettings()
    {
        string playerName = nameInput != null ? nameInput.text : "Player";
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.SetInt("Difficulty", (int)selectedDifficulty);
        PlayerPrefs.Save();
    }

    private void LoadSavedSettings()
    {
        if (PlayerPrefs.HasKey("PlayerName") && nameInput != null)
        {
            nameInput.text = PlayerPrefs.GetString("PlayerName");
        }

        if (PlayerPrefs.HasKey("Difficulty"))
        {
            selectedDifficulty = (GameDifficulty)PlayerPrefs.GetInt("Difficulty");
        }
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        float elapsed = 0f;
        float duration = 0.25f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}
