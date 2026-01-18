using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the leaderboard UI display with improved styling
/// </summary>
public class LeaderboardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private Transform entriesContainer;
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private Button closeButton;
    [SerializeField] private Text titleText;
    
    [Header("Game Over Integration")]
    [SerializeField] private GameObject newRecordIndicator;
    [SerializeField] private Text rankText;
    
    [Header("Styling")]
    [SerializeField] private Color goldColor = new Color(1f, 0.84f, 0f, 0.4f);
    [SerializeField] private Color silverColor = new Color(0.75f, 0.75f, 0.78f, 0.35f);
    [SerializeField] private Color bronzeColor = new Color(0.8f, 0.5f, 0.2f, 0.3f);
    [SerializeField] private Color panelColor = new Color(0.1f, 0.1f, 0.15f, 0.98f);

    private CanvasGroup canvasGroup;
    private bool isAnimating = false;
    
    private void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }
        
        canvasGroup = leaderboardPanel?.GetComponent<CanvasGroup>();
        if (canvasGroup == null && leaderboardPanel != null)
        {
            canvasGroup = leaderboardPanel.AddComponent<CanvasGroup>();
        }
        
        Hide();
    }

    private void Update()
    {
        // Close on ESC or clicking outside
        if (leaderboardPanel != null && leaderboardPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.L))
            {
                Hide();
            }
        }
    }

    /// <summary>
    /// Shows the leaderboard with animation
    /// </summary>
    public void Show()
    {
        if (leaderboardPanel != null && !isAnimating)
        {
            leaderboardPanel.SetActive(true);
            UpdateLeaderboardDisplay();
            StartCoroutine(AnimateShow());
        }
    }

    /// <summary>
    /// Hides the leaderboard with animation
    /// </summary>
    public void Hide()
    {
        if (leaderboardPanel != null && !isAnimating)
        {
            StartCoroutine(AnimateHide());
        }
    }

    private System.Collections.IEnumerator AnimateShow()
    {
        isAnimating = true;
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            float elapsed = 0f;
            float duration = 0.2f;
            
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
                yield return null;
            }
            
            canvasGroup.alpha = 1f;
        }
        
        isAnimating = false;
    }

    private System.Collections.IEnumerator AnimateHide()
    {
        isAnimating = true;
        
        if (canvasGroup != null)
        {
            float elapsed = 0f;
            float duration = 0.15f;
            
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                yield return null;
            }
            
            canvasGroup.alpha = 0f;
        }
        
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
        }
        
        isAnimating = false;
    }

    /// <summary>
    /// Updates the leaderboard display with current data
    /// </summary>
    public void UpdateLeaderboardDisplay()
    {
        if (LeaderboardManager.Instance == null || entriesContainer == null)
        {
            return;
        }

        // Clear old entries
        foreach (Transform child in entriesContainer)
        {
            Destroy(child.gameObject);
        }

        // Create new entries
        List<LeaderboardEntry> entries = LeaderboardManager.Instance.GetLeaderboard();
        
        for (int i = 0; i < entries.Count; i++)
        {
            CreateLeaderboardEntry(i + 1, entries[i]);
        }
        
        // Show empty state if no entries
        if (entries.Count == 0)
        {
            CreateEmptyStateEntry();
        }
    }

    private void CreateEmptyStateEntry()
    {
        if (entryPrefab == null) return;
        
        GameObject entryObj = Instantiate(entryPrefab, entriesContainer);
        Text[] texts = entryObj.GetComponentsInChildren<Text>();
        
        if (texts.Length >= 1)
        {
            texts[0].text = "";
            if (texts.Length >= 2) texts[1].text = "No scores yet!";
            if (texts.Length >= 3) texts[2].text = "Be the first!";
        }
        
        Image bg = entryObj.GetComponent<Image>();
        if (bg != null)
        {
            bg.color = new Color(0.2f, 0.2f, 0.25f, 0.3f);
        }
    }

    /// <summary>
    /// Creates a visual leaderboard entry
    /// </summary>
    private void CreateLeaderboardEntry(int rank, LeaderboardEntry entry)
    {
        if (entryPrefab == null)
        {
            return;
        }

        GameObject entryObj = Instantiate(entryPrefab, entriesContainer);
        
        // Configure texts
        Text[] texts = entryObj.GetComponentsInChildren<Text>();
        
        if (texts.Length >= 3)
        {
            // Rank with medal emoji for top 3
            string rankDisplay = rank.ToString();
            if (rank == 1) rankDisplay = "🥇";
            else if (rank == 2) rankDisplay = "🥈";
            else if (rank == 3) rankDisplay = "🥉";
            
            texts[0].text = rankDisplay;
            texts[1].text = entry.playerName;
            texts[2].text = entry.score.ToString("N0");
            
            // Style top 3 text
            if (rank <= 3)
            {
                texts[1].fontStyle = FontStyle.Bold;
                texts[2].fontStyle = FontStyle.Bold;
            }
        }

        // Highlight top 3 with colored backgrounds
        Image bg = entryObj.GetComponent<Image>();
        if (bg != null)
        {
            switch (rank)
            {
                case 1:
                    bg.color = goldColor;
                    break;
                case 2:
                    bg.color = silverColor;
                    break;
                case 3:
                    bg.color = bronzeColor;
                    break;
                default:
                    bg.color = new Color(0.15f, 0.15f, 0.2f, 0.2f);
                    break;
            }
        }
        
        // Add hover effect
        AddEntryHoverEffect(entryObj, rank);
    }

    private void AddEntryHoverEffect(GameObject entryObj, int rank)
    {
        // Could add EventTrigger for hover effects if needed
        // For now, keeping it simple
    }

    /// <summary>
    /// Shows new record indicator on Game Over
    /// </summary>
    public void ShowNewRecordIndicator(int rank)
    {
        if (newRecordIndicator != null)
        {
            newRecordIndicator.SetActive(true);
            StartCoroutine(AnimateNewRecord());
        }

        if (rankText != null)
        {
            if (rank == 1)
            {
                rankText.text = "🏆 NEW HIGH SCORE! 🏆";
                rankText.color = new Color(1f, 0.84f, 0f);
            }
            else if (rank <= 3)
            {
                rankText.text = $"🎉 #{rank} on Leaderboard! 🎉";
                rankText.color = new Color(0.3f, 0.85f, 0.4f);
            }
            else
            {
                rankText.text = $"#{rank} on Leaderboard!";
                rankText.color = new Color(0.7f, 0.7f, 0.75f);
            }
        }
    }

    private System.Collections.IEnumerator AnimateNewRecord()
    {
        if (rankText == null) yield break;
        
        Vector3 originalScale = rankText.transform.localScale;
        float time = 0f;
        
        while (newRecordIndicator != null && newRecordIndicator.activeSelf && time < 2f)
        {
            time += Time.unscaledDeltaTime;
            float scale = 1f + Mathf.Sin(time * 8f) * 0.05f;
            rankText.transform.localScale = originalScale * scale;
            yield return null;
        }
        
        rankText.transform.localScale = originalScale;
    }

    /// <summary>
    /// Hides new record indicator
    /// </summary>
    public void HideNewRecordIndicator()
    {
        if (newRecordIndicator != null)
        {
            newRecordIndicator.SetActive(false);
        }
    }
}