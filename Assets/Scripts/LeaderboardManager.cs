using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;
    public string date;

    public LeaderboardEntry(string name, int score)
    {
        this.playerName = name;
        this.score = score;
        this.date = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
    }
}

[System.Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    [Header("Leaderboard Settings")]
    [SerializeField] private int maxEntries = 50;
    [SerializeField] private string defaultPlayerName = "Player";
    
    private LeaderboardData leaderboardData;
    private string saveFilePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        saveFilePath = Path.Combine(Application.persistentDataPath, "leaderboard.json");
        LoadLeaderboard();
    }

    /// <summary>
    /// Adiciona uma nova pontuação ao leaderboard
    /// </summary>
    public bool AddScore(int score, string playerName = null)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            playerName = defaultPlayerName;
        }

        LeaderboardEntry newEntry = new LeaderboardEntry(playerName, score);
        leaderboardData.entries.Add(newEntry);

        // Ordena por score (maior para menor)
        leaderboardData.entries = leaderboardData.entries
            .OrderByDescending(e => e.score)
            .Take(maxEntries)
            .ToList();

        SaveLeaderboard();

        // Retorna true se está no top 10
        return leaderboardData.entries.Contains(newEntry);
    }

    /// <summary>
    /// Verifica se um score entra no leaderboard
    /// </summary>
    public bool IsHighScore(int score)
    {
        if (leaderboardData.entries.Count < maxEntries)
        {
            return true;
        }

        int lowestScore = leaderboardData.entries[leaderboardData.entries.Count - 1].score;
        return score > lowestScore;
    }

    /// <summary>
    /// Retorna a posição do score no ranking (1 = primeiro lugar)
    /// </summary>
    public int GetScoreRank(int score)
    {
        int rank = 1;
        foreach (var entry in leaderboardData.entries)
        {
            if (score > entry.score)
            {
                return rank;
            }
            rank++;
        }
        return rank;
    }

    /// <summary>
    /// Obtém todas as entradas do leaderboard
    /// </summary>
    public List<LeaderboardEntry> GetLeaderboard()
    {
        return new List<LeaderboardEntry>(leaderboardData.entries);
    }

    /// <summary>
    /// Obtém o top score
    /// </summary>
    public int GetTopScore()
    {
        if (leaderboardData.entries.Count > 0)
        {
            return leaderboardData.entries[0].score;
        }
        return 0;
    }

    /// <summary>
    /// Limpa todo o leaderboard
    /// </summary>
    public void ClearLeaderboard()
    {
        leaderboardData.entries.Clear();
        SaveLeaderboard();
    }

    private void SaveLeaderboard()
    {
        try
        {
            string json = JsonUtility.ToJson(leaderboardData, true);
            File.WriteAllText(saveFilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Erro ao salvar leaderboard: {e.Message}");
        }
    }

    private void LoadLeaderboard()
    {
        try
        {
            if (File.Exists(saveFilePath))
            {
                string json = File.ReadAllText(saveFilePath);
                leaderboardData = JsonUtility.FromJson<LeaderboardData>(json);
            }
            else
            {
                leaderboardData = new LeaderboardData();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Erro ao carregar leaderboard: {e.Message}");
            leaderboardData = new LeaderboardData();
        }
    }
}