using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private InputField nameInputField;
    [SerializeField] private Text difficultyText;
    [SerializeField] private Text descriptionText;
    
    [Header("Buttons")]
    [SerializeField] private Button easyButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Button quitButton;

    [Header("Leaderboard")]
    [SerializeField] private LeaderboardUI leaderboardUI;

    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = "GameScene";

    private GameDifficulty selectedDifficulty = GameDifficulty.Normal;

    private void Start()
    {
        // Configurar listeners dos botões
        if (easyButton != null)
            easyButton.onClick.AddListener(() => SelectDifficulty(GameDifficulty.Easy));
        
        if (normalButton != null)
            normalButton.onClick.AddListener(() => SelectDifficulty(GameDifficulty.Normal));
        
        if (hardButton != null)
            hardButton.onClick.AddListener(() => SelectDifficulty(GameDifficulty.Hard));
        
        if (playButton != null)
            playButton.onClick.AddListener(StartGame);
        
        if (leaderboardButton != null)
            leaderboardButton.onClick.AddListener(ShowLeaderboard);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        // Carregar configurações salvas (se existirem)
        LoadSavedSettings();

        // Selecionar dificuldade inicial
        SelectDifficulty(selectedDifficulty);
    }

    /// <summary>
    /// Seleciona a dificuldade do jogo
    /// </summary>
    private void SelectDifficulty(GameDifficulty difficulty)
    {
        selectedDifficulty = difficulty;

        // Atualizar UI
        UpdateDifficultyUI();
        UpdateButtonHighlight();
    }

    /// <summary>
    /// Atualiza texto de dificuldade e descrição
    /// </summary>
    private void UpdateDifficultyUI()
    {
        if (difficultyText != null)
        {
            difficultyText.text = selectedDifficulty.ToString().ToUpper();
        }

        if (descriptionText != null)
        {
            switch (selectedDifficulty)
            {
                case GameDifficulty.Easy:
                    descriptionText.text = "• No obstacles\n• Can pass through walls\n• Perfect for beginners!";
                    break;
                case GameDifficulty.Normal:
                    descriptionText.text = "• No obstacles\n• Walls are deadly\n• Classic Snake experience";
                    break;
                case GameDifficulty.Hard:
                    descriptionText.text = "• Random obstacles spawn\n• Portals teleport you\n• Walls are deadly\n• Ultimate challenge!";
                    break;
            }
        }
    }

    /// <summary>
    /// Destaca o botão da dificuldade selecionada
    /// </summary>
    private void UpdateButtonHighlight()
    {
        // Reset todos os botões
        ResetButtonColor(easyButton);
        ResetButtonColor(normalButton);
        ResetButtonColor(hardButton);

        // Destacar botão selecionado
        Button selectedButton = null;
        Color highlightColor = Color.white;

        switch (selectedDifficulty)
        {
            case GameDifficulty.Easy:
                selectedButton = easyButton;
                highlightColor = new Color(0.5f, 1f, 0.5f); // Verde claro
                break;
            case GameDifficulty.Normal:
                selectedButton = normalButton;
                highlightColor = new Color(1f, 0.8f, 0.3f); // Amarelo
                break;
            case GameDifficulty.Hard:
                selectedButton = hardButton;
                highlightColor = new Color(1f, 0.3f, 0.3f); // Vermelho
                break;
        }

        if (selectedButton != null)
        {
            ColorBlock colors = selectedButton.colors;
            colors.normalColor = highlightColor;
            colors.selectedColor = highlightColor;
            selectedButton.colors = colors;
        }
    }

    /// <summary>
    /// Reseta a cor do botão
    /// </summary>
    private void ResetButtonColor(Button button)
    {
        if (button == null) return;

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.selectedColor = Color.white;
        button.colors = colors;
    }

    /// <summary>
    /// Inicia o jogo
    /// </summary>
    private void StartGame()
    {
        // Validar nome da cena
        if (string.IsNullOrEmpty(gameSceneName))
        {
            Debug.LogError("Game Scene Name não está configurado no MainMenu!");
            gameSceneName = "GameScene"; // Valor padrão
        }

        // Criar ou obter GameSettings
        GameSettings settings = GameSettings.Instance;
        
        if (settings == null)
        {
            GameObject settingsObj = new GameObject("GameSettings");
            settings = settingsObj.AddComponent<GameSettings>();
        }
        
        // Configurar nome do jogador
        string playerName = "Player";
        if (nameInputField != null && !string.IsNullOrEmpty(nameInputField.text))
        {
            playerName = nameInputField.text;
        }
        settings.SetPlayerName(playerName);

        // Configurar dificuldade
        settings.SetDifficulty(selectedDifficulty);

        // Salvar configurações
        SaveSettings();

        Debug.Log($"Starting game with Player: {playerName}, Difficulty: {selectedDifficulty}");

        // Carregar cena do jogo
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Mostra o leaderboard
    /// </summary>
    private void ShowLeaderboard()
    {
        if (leaderboardUI != null)
        {
            leaderboardUI.Show();
        }
    }

    /// <summary>
    /// Sai do jogo
    /// </summary>
    private void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    /// <summary>
    /// Salva configurações usando PlayerPrefs
    /// </summary>
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
        
        Debug.Log($"Settings saved: {playerName}, Difficulty: {selectedDifficulty}");
    }

    /// <summary>
    /// Carrega configurações salvas
    /// </summary>
    private void LoadSavedSettings()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            string savedName = PlayerPrefs.GetString("PlayerName");
            if (nameInputField != null)
            {
                nameInputField.text = savedName;
            }
        }

        if (PlayerPrefs.HasKey("Difficulty"))
        {
            selectedDifficulty = (GameDifficulty)PlayerPrefs.GetInt("Difficulty");
        }
    }
}