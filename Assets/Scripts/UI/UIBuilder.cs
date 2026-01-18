using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// Utility class for building UI elements programmatically
/// </summary>
public static class UIBuilder
{
    // Color scheme
    public static readonly Color PrimaryGreen = new Color(0.3f, 0.85f, 0.4f);
    public static readonly Color DarkBackground = new Color(0.08f, 0.08f, 0.12f);
    public static readonly Color PanelBackground = new Color(0.12f, 0.12f, 0.18f, 0.95f);
    public static readonly Color ButtonNormal = new Color(0.18f, 0.18f, 0.25f);
    public static readonly Color ButtonHover = new Color(0.25f, 0.25f, 0.35f);
    public static readonly Color EasyGreen = new Color(0.3f, 0.85f, 0.35f);
    public static readonly Color NormalYellow = new Color(1f, 0.75f, 0.2f);
    public static readonly Color HardRed = new Color(0.9f, 0.25f, 0.25f);

    /// <summary>
    /// Creates a full-screen canvas with EventSystem (required for UI interaction)
    /// </summary>
    public static Canvas CreateCanvas(string name = "Canvas")
    {
        // IMPORTANT: Create EventSystem if it doesn't exist (required for button clicks!)
        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
        }

        GameObject canvasObj = new GameObject(name);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        return canvas;
    }

    /// <summary>
    /// Creates a panel with background color
    /// </summary>
    public static RectTransform CreatePanel(Transform parent, string name, Color color)
    {
        GameObject panelObj = new GameObject(name);
        panelObj.transform.SetParent(parent, false);

        RectTransform rect = panelObj.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = panelObj.AddComponent<Image>();
        image.color = color;

        return rect;
    }

    /// <summary>
    /// Creates a centered panel with fixed size
    /// </summary>
    public static RectTransform CreateCenteredPanel(Transform parent, string name, float width, float height, Color color)
    {
        GameObject panelObj = new GameObject(name);
        panelObj.transform.SetParent(parent, false);

        RectTransform rect = panelObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = Vector2.zero;

        Image image = panelObj.AddComponent<Image>();
        image.color = color;

        return rect;
    }

    /// <summary>
    /// Creates a text element
    /// </summary>
    public static Text CreateText(Transform parent, string name, string content, int fontSize, Color color, TextAnchor alignment = TextAnchor.MiddleCenter)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);

        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(600, fontSize + 20);

        Text text = textObj.AddComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        return text;
    }

    /// <summary>
    /// Creates a button with text
    /// </summary>
    public static Button CreateButton(Transform parent, string name, string label, float width, float height, Color normalColor, UnityAction onClick = null)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);

        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);

        Image image = buttonObj.AddComponent<Image>();
        image.color = normalColor;

        Button button = buttonObj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = normalColor;
        colors.highlightedColor = new Color(normalColor.r + 0.12f, normalColor.g + 0.12f, normalColor.b + 0.12f);
        colors.pressedColor = new Color(normalColor.r - 0.08f, normalColor.g - 0.08f, normalColor.b - 0.08f);
        colors.selectedColor = normalColor;
        colors.fadeDuration = 0.1f;
        button.colors = colors;

        if (onClick != null)
        {
            button.onClick.AddListener(onClick);
        }

        // Create button text
        Text buttonText = CreateText(buttonObj.transform, "Text", label, 28, Color.white);
        RectTransform textRect = buttonText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return button;
    }

    /// <summary>
    /// Creates an input field
    /// </summary>
    public static InputField CreateInputField(Transform parent, string name, string placeholder, float width, float height)
    {
        GameObject inputObj = new GameObject(name);
        inputObj.transform.SetParent(parent, false);

        RectTransform rect = inputObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);

        Image image = inputObj.AddComponent<Image>();
        image.color = new Color(0.1f, 0.1f, 0.15f);

        InputField inputField = inputObj.AddComponent<InputField>();
        inputField.characterLimit = 15;

        // Placeholder
        Text placeholderText = CreateText(inputObj.transform, "Placeholder", placeholder, 24, new Color(0.5f, 0.5f, 0.55f));
        RectTransform phRect = placeholderText.GetComponent<RectTransform>();
        phRect.anchorMin = Vector2.zero;
        phRect.anchorMax = Vector2.one;
        phRect.offsetMin = new Vector2(15, 0);
        phRect.offsetMax = new Vector2(-15, 0);
        placeholderText.alignment = TextAnchor.MiddleLeft;

        // Input text
        Text inputText = CreateText(inputObj.transform, "Text", "", 24, Color.white);
        RectTransform inputTextRect = inputText.GetComponent<RectTransform>();
        inputTextRect.anchorMin = Vector2.zero;
        inputTextRect.anchorMax = Vector2.one;
        inputTextRect.offsetMin = new Vector2(15, 0);
        inputTextRect.offsetMax = new Vector2(-15, 0);
        inputText.alignment = TextAnchor.MiddleLeft;
        inputText.supportRichText = false;

        inputField.textComponent = inputText;
        inputField.placeholder = placeholderText;

        return inputField;
    }

    /// <summary>
    /// Creates a vertical layout group
    /// </summary>
    public static VerticalLayoutGroup CreateVerticalLayout(Transform parent, string name, float spacing = 10f, TextAnchor childAlignment = TextAnchor.MiddleCenter)
    {
        GameObject layoutObj = new GameObject(name);
        layoutObj.transform.SetParent(parent, false);

        RectTransform rect = layoutObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(400, 500);

        VerticalLayoutGroup layout = layoutObj.AddComponent<VerticalLayoutGroup>();
        layout.spacing = spacing;
        layout.childAlignment = childAlignment;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        ContentSizeFitter fitter = layoutObj.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        return layout;
    }

    /// <summary>
    /// Creates a horizontal layout group
    /// </summary>
    public static HorizontalLayoutGroup CreateHorizontalLayout(Transform parent, string name, float spacing = 10f)
    {
        GameObject layoutObj = new GameObject(name);
        layoutObj.transform.SetParent(parent, false);

        RectTransform rect = layoutObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(500, 60);

        HorizontalLayoutGroup layout = layoutObj.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = spacing;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        ContentSizeFitter fitter = layoutObj.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        return layout;
    }

    /// <summary>
    /// Sets the anchored position of a RectTransform
    /// </summary>
    public static void SetPosition(RectTransform rect, float x, float y)
    {
        rect.anchoredPosition = new Vector2(x, y);
    }

    /// <summary>
    /// Makes a RectTransform stretch to fill parent
    /// </summary>
    public static void StretchToFill(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    /// <summary>
    /// Adds a CanvasGroup for fade effects
    /// </summary>
    public static CanvasGroup AddCanvasGroup(GameObject obj)
    {
        CanvasGroup group = obj.GetComponent<CanvasGroup>();
        if (group == null)
        {
            group = obj.AddComponent<CanvasGroup>();
        }
        return group;
    }

    /// <summary>
    /// Creates a scroll view with vertical scrolling
    /// </summary>
    public static ScrollRect CreateScrollView(Transform parent, string name, float width, float height, out RectTransform content)
    {
        // Scroll View container
        GameObject scrollObj = new GameObject(name);
        scrollObj.transform.SetParent(parent, false);

        RectTransform scrollRect = scrollObj.AddComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0.5f, 0.5f);
        scrollRect.anchorMax = new Vector2(0.5f, 0.5f);
        scrollRect.sizeDelta = new Vector2(width, height);

        ScrollRect scroll = scrollObj.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Elastic;
        scroll.elasticity = 0.1f;
        scroll.inertia = true;
        scroll.decelerationRate = 0.135f;
        scroll.scrollSensitivity = 25f;

        // Add mask
        Image scrollBg = scrollObj.AddComponent<Image>();
        scrollBg.color = new Color(0, 0, 0, 0.1f);
        Mask mask = scrollObj.AddComponent<Mask>();
        mask.showMaskGraphic = true;

        // Viewport (same as scroll view in this simple case)
        GameObject viewportObj = new GameObject("Viewport");
        viewportObj.transform.SetParent(scrollObj.transform, false);

        RectTransform viewportRect = viewportObj.AddComponent<RectTransform>();
        StretchToFill(viewportRect);
        viewportObj.AddComponent<Image>().color = Color.clear;
        viewportObj.AddComponent<Mask>().showMaskGraphic = false;

        // Content container
        GameObject contentObj = new GameObject("Content");
        contentObj.transform.SetParent(viewportObj.transform, false);

        content = contentObj.AddComponent<RectTransform>();
        content.anchorMin = new Vector2(0.5f, 1f);
        content.anchorMax = new Vector2(0.5f, 1f);
        content.pivot = new Vector2(0.5f, 1f);
        content.sizeDelta = new Vector2(width - 20, 0); // Will grow with content

        // Add vertical layout and content size fitter
        VerticalLayoutGroup layout = contentObj.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 8f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
        layout.padding = new RectOffset(5, 5, 10, 10);

        ContentSizeFitter fitter = contentObj.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Link scroll rect
        scroll.viewport = viewportRect;
        scroll.content = content;

        // Create scrollbar
        GameObject scrollbarObj = new GameObject("Scrollbar");
        scrollbarObj.transform.SetParent(scrollObj.transform, false);

        RectTransform scrollbarRect = scrollbarObj.AddComponent<RectTransform>();
        scrollbarRect.anchorMin = new Vector2(1, 0);
        scrollbarRect.anchorMax = new Vector2(1, 1);
        scrollbarRect.pivot = new Vector2(1, 0.5f);
        scrollbarRect.sizeDelta = new Vector2(8, 0);
        scrollbarRect.anchoredPosition = new Vector2(0, 0);

        Image scrollbarBg = scrollbarObj.AddComponent<Image>();
        scrollbarBg.color = new Color(0.15f, 0.15f, 0.2f, 0.5f);

        Scrollbar scrollbar = scrollbarObj.AddComponent<Scrollbar>();
        scrollbar.direction = Scrollbar.Direction.BottomToTop;

        // Scrollbar handle
        GameObject handleArea = new GameObject("HandleArea");
        handleArea.transform.SetParent(scrollbarObj.transform, false);
        RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
        StretchToFill(handleAreaRect);
        handleAreaRect.offsetMin = new Vector2(2, 2);
        handleAreaRect.offsetMax = new Vector2(-2, -2);

        GameObject handleObj = new GameObject("Handle");
        handleObj.transform.SetParent(handleArea.transform, false);

        RectTransform handleRect = handleObj.AddComponent<RectTransform>();
        handleRect.anchorMin = Vector2.zero;
        handleRect.anchorMax = Vector2.one;
        handleRect.sizeDelta = Vector2.zero;

        Image handleImg = handleObj.AddComponent<Image>();
        handleImg.color = PrimaryGreen;
        handleImg.type = Image.Type.Sliced;

        scrollbar.targetGraphic = handleImg;
        scrollbar.handleRect = handleRect;

        scroll.verticalScrollbar = scrollbar;
        scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
        scroll.verticalScrollbarSpacing = 3f;

        return scroll;
    }
}
