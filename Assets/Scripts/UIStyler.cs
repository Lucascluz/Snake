using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Utility class for consistent UI styling across menus
/// Attach to any UI element to apply consistent styling
/// </summary>
public class UIStyler : MonoBehaviour
{
    [Header("Color Scheme")]
    public static readonly Color PrimaryGreen = new Color(0.3f, 0.85f, 0.4f);
    public static readonly Color SecondaryGreen = new Color(0.2f, 0.65f, 0.3f);
    public static readonly Color DarkBackground = new Color(0.08f, 0.08f, 0.12f);
    public static readonly Color PanelBackground = new Color(0.12f, 0.12f, 0.18f, 0.95f);
    public static readonly Color ButtonNormal = new Color(0.15f, 0.15f, 0.22f);
    public static readonly Color ButtonHover = new Color(0.22f, 0.22f, 0.32f);
    public static readonly Color TextPrimary = new Color(0.95f, 0.95f, 0.95f);
    public static readonly Color TextSecondary = new Color(0.7f, 0.7f, 0.75f);
    
    // Difficulty colors
    public static readonly Color EasyGreen = new Color(0.3f, 0.85f, 0.35f);
    public static readonly Color NormalYellow = new Color(1f, 0.75f, 0.2f);
    public static readonly Color HardRed = new Color(0.9f, 0.25f, 0.25f);

    [Header("Apply Styling")]
    [SerializeField] private bool applyOnStart = true;
    [SerializeField] private StyleType styleType = StyleType.None;
    
    public enum StyleType
    {
        None,
        PrimaryButton,
        SecondaryButton,
        DangerButton,
        Panel,
        Title,
        Subtitle,
        BodyText
    }

    private void Start()
    {
        if (applyOnStart)
        {
            ApplyStyle();
        }
    }

    public void ApplyStyle()
    {
        switch (styleType)
        {
            case StyleType.PrimaryButton:
                ApplyPrimaryButtonStyle();
                break;
            case StyleType.SecondaryButton:
                ApplySecondaryButtonStyle();
                break;
            case StyleType.DangerButton:
                ApplyDangerButtonStyle();
                break;
            case StyleType.Panel:
                ApplyPanelStyle();
                break;
            case StyleType.Title:
                ApplyTitleStyle();
                break;
            case StyleType.Subtitle:
                ApplySubtitleStyle();
                break;
            case StyleType.BodyText:
                ApplyBodyTextStyle();
                break;
        }
    }

    private void ApplyPrimaryButtonStyle()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = PrimaryGreen;
            colors.highlightedColor = new Color(PrimaryGreen.r + 0.15f, PrimaryGreen.g + 0.1f, PrimaryGreen.b + 0.15f);
            colors.pressedColor = new Color(PrimaryGreen.r - 0.1f, PrimaryGreen.g - 0.1f, PrimaryGreen.b - 0.1f);
            colors.selectedColor = PrimaryGreen;
            colors.fadeDuration = 0.1f;
            button.colors = colors;
        }
        
        Text text = GetComponentInChildren<Text>();
        if (text != null)
        {
            text.color = Color.white;
            text.fontStyle = FontStyle.Bold;
        }
    }

    private void ApplySecondaryButtonStyle()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = ButtonNormal;
            colors.highlightedColor = ButtonHover;
            colors.pressedColor = new Color(ButtonNormal.r - 0.05f, ButtonNormal.g - 0.05f, ButtonNormal.b - 0.05f);
            colors.selectedColor = ButtonNormal;
            colors.fadeDuration = 0.1f;
            button.colors = colors;
        }
        
        Text text = GetComponentInChildren<Text>();
        if (text != null)
        {
            text.color = TextSecondary;
        }
    }

    private void ApplyDangerButtonStyle()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = new Color(0.6f, 0.2f, 0.2f);
            colors.highlightedColor = HardRed;
            colors.pressedColor = new Color(0.5f, 0.15f, 0.15f);
            colors.selectedColor = new Color(0.6f, 0.2f, 0.2f);
            colors.fadeDuration = 0.1f;
            button.colors = colors;
        }
        
        Text text = GetComponentInChildren<Text>();
        if (text != null)
        {
            text.color = Color.white;
        }
    }

    private void ApplyPanelStyle()
    {
        Image image = GetComponent<Image>();
        if (image != null)
        {
            image.color = PanelBackground;
        }
    }

    private void ApplyTitleStyle()
    {
        Text text = GetComponent<Text>();
        if (text != null)
        {
            text.color = PrimaryGreen;
            text.fontStyle = FontStyle.Bold;
        }
    }

    private void ApplySubtitleStyle()
    {
        Text text = GetComponent<Text>();
        if (text != null)
        {
            text.color = TextSecondary;
            text.fontStyle = FontStyle.Italic;
        }
    }

    private void ApplyBodyTextStyle()
    {
        Text text = GetComponent<Text>();
        if (text != null)
        {
            text.color = TextPrimary;
        }
    }

    /// <summary>
    /// Static method to style a button with a specific color
    /// </summary>
    public static void StyleButton(Button button, Color baseColor)
    {
        if (button == null) return;
        
        ColorBlock colors = button.colors;
        colors.normalColor = baseColor;
        colors.highlightedColor = new Color(baseColor.r + 0.15f, baseColor.g + 0.15f, baseColor.b + 0.15f);
        colors.pressedColor = new Color(baseColor.r - 0.1f, baseColor.g - 0.1f, baseColor.b - 0.1f);
        colors.selectedColor = baseColor;
        colors.fadeDuration = 0.1f;
        button.colors = colors;
    }

    /// <summary>
    /// Get the color associated with a difficulty level
    /// </summary>
    public static Color GetDifficultyColor(GameDifficulty difficulty)
    {
        switch (difficulty)
        {
            case GameDifficulty.Easy:
                return EasyGreen;
            case GameDifficulty.Normal:
                return NormalYellow;
            case GameDifficulty.Hard:
                return HardRed;
            default:
                return TextPrimary;
        }
    }
}
