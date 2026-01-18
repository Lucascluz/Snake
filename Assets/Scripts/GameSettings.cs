using UnityEngine;

public enum GameDifficulty
{
    Easy,    // Sem obstáculos, passa pelas paredes
    Normal,  // Sem obstáculos, paredes matam, sem portais
    Hard     // Obstáculos, portais, paredes matam
}

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    [Header("Player Settings")]
    public string playerName = "Player";

    [Header("Difficulty Settings")]
    public GameDifficulty difficulty = GameDifficulty.Normal;

    private void Awake()
    {
        // Singleton que persiste entre cenas
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Define o nome do jogador
    /// </summary>
    public void SetPlayerName(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            playerName = name.Trim();
            if (playerName.Length > 15)
            {
                playerName = playerName.Substring(0, 15);
            }
        }
        else
        {
            playerName = "Player";
        }
    }

    /// <summary>
    /// Define a dificuldade do jogo
    /// </summary>
    public void SetDifficulty(GameDifficulty newDifficulty)
    {
        difficulty = newDifficulty;
    }

    /// <summary>
    /// Configurações para modo Fácil
    /// </summary>
    public bool IsEasyMode()
    {
        return difficulty == GameDifficulty.Easy;
    }

    /// <summary>
    /// Configurações para modo Normal
    /// </summary>
    public bool IsNormalMode()
    {
        return difficulty == GameDifficulty.Normal;
    }

    /// <summary>
    /// Configurações para modo Difícil
    /// </summary>
    public bool IsHardMode()
    {
        return difficulty == GameDifficulty.Hard;
    }

    /// <summary>
    /// Verifica se obstáculos devem aparecer
    /// </summary>
    public bool ShouldSpawnObstacles()
    {
        return difficulty == GameDifficulty.Hard;
    }

    /// <summary>
    /// Verifica se portais devem aparecer
    /// </summary>
    public bool ShouldSpawnPortals()
    {
        return difficulty == GameDifficulty.Hard;
    }

    /// <summary>
    /// Verifica se pode atravessar paredes
    /// </summary>
    public bool CanMoveThroughWalls()
    {
        return difficulty == GameDifficulty.Easy;
    }

    /// <summary>
    /// Retorna a descrição da dificuldade
    /// </summary>
    public string GetDifficultyDescription()
    {
        switch (difficulty)
        {
            case GameDifficulty.Easy:
                return "No obstacles\nCan pass through walls";
            case GameDifficulty.Normal:
                return "No obstacles\nWalls are deadly";
            case GameDifficulty.Hard:
                return "Obstacles + Portals\nWalls are deadly";
            default:
                return "";
        }
    }
}