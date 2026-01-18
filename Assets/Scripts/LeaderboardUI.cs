using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private Transform entriesContainer;
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private Button closeButton;
    
    [Header("Game Over Integration")]
    [SerializeField] private GameObject newRecordIndicator;
    [SerializeField] private Text rankText;
    
    private void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }
        
        Hide();
    }

    /// <summary>
    /// Mostra o leaderboard completo
    /// </summary>
    public void Show()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(true);
        }
        
        UpdateLeaderboardDisplay();
    }

    /// <summary>
    /// Esconde o leaderboard
    /// </summary>
    public void Hide()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Atualiza a exibição do leaderboard
    /// </summary>
    public void UpdateLeaderboardDisplay()
    {
        if (LeaderboardManager.Instance == null || entriesContainer == null)
        {
            return;
        }

        // Limpa entradas antigas
        foreach (Transform child in entriesContainer)
        {
            Destroy(child.gameObject);
        }

        // Cria novas entradas
        List<LeaderboardEntry> entries = LeaderboardManager.Instance.GetLeaderboard();
        
        for (int i = 0; i < entries.Count; i++)
        {
            CreateLeaderboardEntry(i + 1, entries[i]);
        }
    }

    /// <summary>
    /// Cria uma entrada visual do leaderboard
    /// </summary>
    private void CreateLeaderboardEntry(int rank, LeaderboardEntry entry)
    {
        if (entryPrefab == null)
        {
            return;
        }

        GameObject entryObj = Instantiate(entryPrefab, entriesContainer);
        
        // Configura os textos (ajuste conforme sua hierarquia de UI)
        Text[] texts = entryObj.GetComponentsInChildren<Text>();
        
        if (texts.Length >= 3)
        {
            texts[0].text = rank.ToString(); // Posição
            texts[1].text = entry.playerName; // Nome
            texts[2].text = entry.score.ToString(); // Score
        }

        // Opcional: destacar top 3
        if (rank <= 3)
        {
            Image bg = entryObj.GetComponent<Image>();
            if (bg != null)
            {
                switch (rank)
                {
                    case 1:
                        bg.color = new Color(1f, 0.84f, 0f, 0.3f); // Dourado
                        break;
                    case 2:
                        bg.color = new Color(0.75f, 0.75f, 0.75f, 0.3f); // Prata
                        break;
                    case 3:
                        bg.color = new Color(0.8f, 0.5f, 0.2f, 0.3f); // Bronze
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Mostra indicador de novo recorde no Game Over
    /// </summary>
    public void ShowNewRecordIndicator(int rank)
    {
        if (newRecordIndicator != null)
        {
            newRecordIndicator.SetActive(true);
        }

        if (rankText != null)
        {
            if (rank == 1)
            {
                rankText.text = "NEW HIGH SCORE!";
                rankText.color = Color.yellow;
            }
            else
            {
                rankText.text = $"#{rank} on Leaderboard!";
                rankText.color = Color.green;
            }
        }
    }

    /// <summary>
    /// Esconde indicador de novo recorde
    /// </summary>
    public void HideNewRecordIndicator()
    {
        if (newRecordIndicator != null)
        {
            newRecordIndicator.SetActive(false);
        }
    }
}