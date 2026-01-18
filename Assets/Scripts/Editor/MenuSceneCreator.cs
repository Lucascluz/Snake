#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

/// <summary>
/// Editor utility to automatically create and setup menu scenes
/// Access via menu: Tools > Snake > Setup Menu Scenes
/// </summary>
public class MenuSceneCreator : EditorWindow
{
    [MenuItem("Tools/Snake/Setup Menu Scenes")]
    public static void ShowWindow()
    {
        GetWindow<MenuSceneCreator>("Snake Scene Setup");
    }

    [MenuItem("Tools/Snake/Create All Scenes Now")]
    public static void CreateAllScenes()
    {
        CreateMainMenuScene();
        CreateGameModeMenuScene();
        SetupGameScene();
        AddScenesToBuildSettings();
        
        Debug.Log("✅ All Snake menu scenes created successfully!");
        EditorUtility.DisplayDialog("Success", "All menu scenes have been created!\n\n• MainMenu\n• GameModeMenu\n• GameScene (updated)\n\nScenes have been added to Build Settings.", "OK");
    }

    private void OnGUI()
    {
        GUILayout.Label("Snake Menu Scene Creator", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        GUILayout.Label("This tool will create all necessary menu scenes\nwith pre-built UI (no manual setup required).", EditorStyles.wordWrappedLabel);
        GUILayout.Space(20);

        if (GUILayout.Button("Create All Scenes", GUILayout.Height(40)))
        {
            CreateAllScenes();
        }

        GUILayout.Space(10);
        GUILayout.Label("Or create individual scenes:", EditorStyles.miniLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Main Menu"))
        {
            CreateMainMenuScene();
            Debug.Log("✅ MainMenu scene created");
        }
        if (GUILayout.Button("Game Mode Menu"))
        {
            CreateGameModeMenuScene();
            Debug.Log("✅ GameModeMenu scene created");
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Setup GameScene UI"))
        {
            SetupGameScene();
            Debug.Log("✅ GameScene UI setup added");
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Add Scenes to Build Settings"))
        {
            AddScenesToBuildSettings();
            Debug.Log("✅ Scenes added to Build Settings");
        }
    }

    private static void CreateMainMenuScene()
    {
        // Create new scene
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // Setup camera
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.backgroundColor = new Color(0.05f, 0.05f, 0.08f);
            mainCam.clearFlags = CameraClearFlags.SolidColor;
            mainCam.orthographic = true;
        }

        // Create setup object
        GameObject setupObj = new GameObject("_SceneSetup");
        setupObj.AddComponent<MainMenuSceneSetup>();

        // Ensure LeaderboardManager exists
        GameObject lbManager = new GameObject("LeaderboardManager");
        lbManager.AddComponent<LeaderboardManager>();

        // Save scene
        string scenePath = "Assets/Scenes/MainMenu.unity";
        EnsureDirectoryExists(scenePath);
        EditorSceneManager.SaveScene(newScene, scenePath);
    }

    private static void CreateGameModeMenuScene()
    {
        // Create new scene
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // Setup camera
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.backgroundColor = new Color(0.05f, 0.05f, 0.08f);
            mainCam.clearFlags = CameraClearFlags.SolidColor;
            mainCam.orthographic = true;
        }

        // Create setup object
        GameObject setupObj = new GameObject("_SceneSetup");
        setupObj.AddComponent<GameModeMenuSceneSetup>();

        // Ensure LeaderboardManager exists
        GameObject lbManager = new GameObject("LeaderboardManager");
        lbManager.AddComponent<LeaderboardManager>();

        // Save scene
        string scenePath = "Assets/Scenes/GameModeMenu.unity";
        EnsureDirectoryExists(scenePath);
        EditorSceneManager.SaveScene(newScene, scenePath);
    }

    private static void SetupGameScene()
    {
        // Try to open existing GameScene
        string gameScenePath = "Assets/Scenes/GameScene.unity";
        
        if (File.Exists(gameScenePath))
        {
            EditorSceneManager.OpenScene(gameScenePath);
            
            // Check if UI setup already exists
            if (Object.FindFirstObjectByType<GameSceneUISetup>() == null && 
                Object.FindFirstObjectByType<GameUIBuilder>() == null)
            {
                // Add the UI setup
                GameObject setupObj = new GameObject("_UISetup");
                setupObj.AddComponent<GameSceneUISetup>();
                
                EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            }
        }
        else
        {
            Debug.LogWarning("GameScene.unity not found at " + gameScenePath);
        }
    }

    private static void AddScenesToBuildSettings()
    {
        var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

        string[] scenePaths = new string[]
        {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/GameModeMenu.unity",
            "Assets/Scenes/GameScene.unity"
        };

        foreach (string path in scenePaths)
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning($"Scene not found: {path}");
                continue;
            }

            bool alreadyExists = false;
            foreach (var scene in scenes)
            {
                if (scene.path == path)
                {
                    alreadyExists = true;
                    break;
                }
            }

            if (!alreadyExists)
            {
                scenes.Add(new EditorBuildSettingsScene(path, true));
            }
        }

        EditorBuildSettings.scenes = scenes.ToArray();
    }

    private static void EnsureDirectoryExists(string filePath)
    {
        string directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}
#endif
