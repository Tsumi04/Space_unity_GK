using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using TMPro;

public static class SpaceExplorerSetup
{
    [MenuItem("Tools/Space Explorer/Setup Main Menu Scene")] 
    public static void SetupMainMenu()
    {
        // Tạo scene mới hoặc sử dụng scene hiện tại và xây UI Main Menu
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        EnsureProjectTags(new string[] { "Player", "Asteroid", "Star", "Enemy" });

        // Canvas + EventSystem
        var canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        Mark(canvasGO);

        var es = new GameObject("EventSystem");
        es.AddComponent<UnityEngine.EventSystems.EventSystem>();
        es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        Mark(es);

        // Title
        var titleGO = new GameObject("Title");
        titleGO.transform.SetParent(canvas.transform, false);
        var tRect = titleGO.AddComponent<RectTransform>();
        tRect.anchorMin = new Vector2(0.5f, 0.85f);
        tRect.anchorMax = new Vector2(0.5f, 0.85f);
        tRect.pivot = new Vector2(0.5f, 0.5f);
        var title = titleGO.AddComponent<TextMeshProUGUI>();
        title.alignment = TMPro.TextAlignmentOptions.Center;
        title.fontSize = 56;
        title.text = "Space Explorer";
        Mark(titleGO);

        // MainMenuManager host
        var managerGO = new GameObject("MainMenuManager");
        var mmm = managerGO.AddComponent<MainMenuManager>();
        Mark(managerGO);

        // Buttons
        var playBtn = CreateButton(canvas.transform, new Vector2(0.5f, 0.6f), "Play");
        if (playBtn != null)
        {
            UnityEditor.Events.UnityEventTools.AddPersistentListener(playBtn.onClick, mmm.PlayGame);
        }

        var instrBtn = CreateButton(canvas.transform, new Vector2(0.5f, 0.48f), "Instructions");
        var quitBtn = CreateButton(canvas.transform, new Vector2(0.5f, 0.36f), "Quit");
        if (quitBtn != null)
        {
            UnityEditor.Events.UnityEventTools.AddPersistentListener(quitBtn.onClick, mmm.QuitGame);
        }

        // Instructions Panel
        var panel = new GameObject("InstructionsPanel");
        panel.transform.SetParent(canvas.transform, false);
        var pRect = panel.AddComponent<RectTransform>();
        pRect.anchorMin = new Vector2(0.5f, 0.5f);
        pRect.anchorMax = new Vector2(0.5f, 0.5f);
        pRect.pivot = new Vector2(0.5f, 0.5f);
        pRect.sizeDelta = new Vector2(650, 360);
        var pImg = panel.AddComponent<UnityEngine.UI.Image>();
        pImg.color = new Color(1f, 1f, 1f, 0.08f);
        var pTextGO = new GameObject("Text");
        pTextGO.transform.SetParent(panel.transform, false);
        var ptRect = pTextGO.AddComponent<RectTransform>();
        ptRect.anchorMin = new Vector2(0, 0);
        ptRect.anchorMax = new Vector2(1, 1);
        ptRect.offsetMin = new Vector2(16, 16);
        ptRect.offsetMax = new Vector2(-16, -16);
        var pText = pTextGO.AddComponent<TextMeshProUGUI>();
        pText.text = "How to Play\n\n- Move: Arrow Keys\n- Shoot: Space\n\nGoal: Collect stars (+points), avoid asteroids and enemy fire.\nHit asteroid/enemy → game over.";
        pText.enableWordWrapping = true;
        pText.fontSize = 28;
        panel.SetActive(false);
        mmm.instructionsPanel = panel;
        Mark(panel);
        Mark(pTextGO);

        if (instrBtn != null)
        {
            UnityEditor.Events.UnityEventTools.AddPersistentListener(instrBtn.onClick, mmm.ToggleInstructionsPanel);
        }

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenu.unity");
        Debug.Log("Space Explorer: Main Menu created at Assets/Scenes/MainMenu.unity");
    }
    [MenuItem("Tools/Space Explorer/Setup Gameplay Scene")] 
    public static void SetupGameplay()
    {
        var scene = SceneManager.GetActiveScene();
        if (!scene.IsValid()) return;
        EnsureProjectTags(new string[] { "Player", "Asteroid", "Star", "Enemy" });

        // 1) GameManager
        var gm = Object.FindFirstObjectByType<GameManager>();
        if (gm == null)
        {
            var go = new GameObject("GameManager");
            gm = go.AddComponent<GameManager>();
            Mark(go);
        }

        // 2) Score UI (TMP)
        if (gm.scoreText == null)
        {
            var canvas = Object.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                var canvasGO = new GameObject("Canvas_Score");
                canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                Mark(canvasGO);
            }
            var scoreGO = new GameObject("ScoreText");
            scoreGO.transform.SetParent(canvas.transform, false);
            var rect = scoreGO.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(16, -16);
            var tmp = scoreGO.AddComponent<TextMeshProUGUI>();
            tmp.fontSize = 32;
            tmp.textWrappingMode = TMPro.TextWrappingModes.NoWrap;
            tmp.text = "Score: 0";
            gm.scoreText = tmp;
            Mark(scoreGO);
        }

        // 3) SpawnManager: ensure exists and assign prefabs if possible
        var spawner = Object.FindFirstObjectByType<SpawnManager>();
        if (spawner == null)
        {
            // Nếu đã có GameObject tên "SpawnManager" nhưng chưa có component, gắn component vào đó
            var existing = GameObject.Find("SpawnManager");
            if (existing != null)
            {
                spawner = existing.GetComponent<SpawnManager>();
                if (spawner == null)
                {
                    spawner = existing.AddComponent<SpawnManager>();
                    Mark(existing);
                }
            }
            else
            {
                var sgo = new GameObject("SpawnManager");
                spawner = sgo.AddComponent<SpawnManager>();
                Mark(sgo);
            }
        }

        // Try to auto-assign prefabs from AssetDatabase
        TryAssignPrefabArray(ref spawner.asteroidPrefabs, "Assets/Prefab", "Asteroid");
        TryAssignPrefab(ref spawner.starPrefab, "Assets/Prefab/Star.prefab");
        TryAssignPrefab(ref spawner.enemyPrefab, "Assets/Prefab/EnemyShip.prefab");

        // 4) Player wiring
        var player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            player = GameObject.Find("Player");
        }
        if (player != null)
        {
            var pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                if (pc.laserPrefab == null)
                {
                    pc.laserPrefab = LoadAssetAtPath<GameObject>("Assets/Prefab/Laser.prefab");
                    Mark(player);
                }
                if (pc.firePoint == null)
                {
                    pc.firePoint = player.transform;
                }
            }
            if (player.tag != "Player")
            {
                player.tag = "Player";
                Mark(player);
            }
        }

        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("Space Explorer: Gameplay setup complete.");
    }

    [MenuItem("Tools/Space Explorer/Create EndGame Scene")] 
    public static void CreateEndGameScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        EnsureProjectTags(new string[] { "Player", "Asteroid", "Star", "Enemy" });
        var canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        Mark(canvasGO);

        var es = new GameObject("EventSystem");
        es.AddComponent<UnityEngine.EventSystems.EventSystem>();
        es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        Mark(es);

        // Title
        var titleGO = new GameObject("Title");
        titleGO.transform.SetParent(canvas.transform, false);
        var tRect = titleGO.AddComponent<RectTransform>();
        tRect.anchorMin = new Vector2(0.5f, 0.82f);
        tRect.anchorMax = new Vector2(0.5f, 0.82f);
        tRect.pivot = new Vector2(0.5f, 0.5f);
        var title = titleGO.AddComponent<TextMeshProUGUI>();
        title.alignment = TMPro.TextAlignmentOptions.Center;
        title.fontSize = 64;
        title.text = "GAME OVER";
        Mark(titleGO);

        // Final score text
        var scoreGO = new GameObject("FinalScoreText");
        scoreGO.transform.SetParent(canvas.transform, false);
        var rect = scoreGO.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.65f);
        rect.anchorMax = new Vector2(0.5f, 0.65f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        var tmp = scoreGO.AddComponent<TextMeshProUGUI>();
        tmp.alignment = TMPro.TextAlignmentOptions.Center;
        tmp.fontSize = 48;
        tmp.text = "Final Score: 0";
        Mark(scoreGO);

        // Manager
        var mgrGO = new GameObject("EndGameManager");
        var mgr = mgrGO.AddComponent<EndGameManager>();
        mgr.finalScoreText = tmp;
        Mark(mgrGO);

        // Buttons
        var mainBtn = CreateButton(canvas.transform, new Vector2(0.5f, 0.5f), "Home");
        if (mainBtn != null)
        {
            UnityEditor.Events.UnityEventTools.AddPersistentListener(mainBtn.onClick, mgr.BackToMainMenu);
        }

        var replayBtn = CreateButton(canvas.transform, new Vector2(0.5f, 0.38f), "Play Again");
        if (replayBtn != null)
        {
            UnityEditor.Events.UnityEventTools.AddPersistentListener(replayBtn.onClick, mgr.RestartGame);
        }

        var quitBtn = CreateButton(canvas.transform, new Vector2(0.5f, 0.26f), "Quit");
        if (quitBtn != null)
        {
            UnityEditor.Events.UnityEventTools.AddPersistentListener(quitBtn.onClick, mgr.QuitGame);
        }

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/EndGame.unity");
        Debug.Log("Space Explorer: EndGame scene created at Assets/Scenes/EndGame.unity");
    }

    [MenuItem("Tools/Space Explorer/Add Scenes To Build Settings")] 
    public static void AddScenesToBuildSettings()
    {
        var list = new System.Collections.Generic.List<EditorBuildSettingsScene>();
        AddScenePath(list, "Assets/Scenes/MainMenu.unity");
        AddScenePath(list, "Assets/Scenes/Gameplay.unity");
        AddScenePath(list, "Assets/Scenes/EndGame.unity");
        EditorBuildSettings.scenes = list.ToArray();
        Debug.Log("Space Explorer: Build settings updated");
    }

    // Helpers
    private static void AddScenePath(System.Collections.Generic.List<EditorBuildSettingsScene> list, string path)
    {
        if (System.IO.File.Exists(path))
        {
            list.Add(new EditorBuildSettingsScene(path, true));
        }
    }

    private static T LoadAssetAtPath<T>(string path) where T : Object
    {
        return AssetDatabase.LoadAssetAtPath<T>(path);
    }

    private static void TryAssignPrefab(ref GameObject field, string path)
    {
        if (field == null)
        {
            field = LoadAssetAtPath<GameObject>(path);
        }
    }

    private static void TryAssignPrefabArray(ref GameObject[] arrayField, string folder, string nameContains)
    {
        if (arrayField != null && arrayField.Length > 0) return;
        var guids = AssetDatabase.FindAssets("t:Prefab", new[] { folder });
        var list = new System.Collections.Generic.List<GameObject>();
        foreach (var g in guids)
        {
            var p = AssetDatabase.GUIDToAssetPath(g);
            if (!string.IsNullOrEmpty(nameContains) && !System.IO.Path.GetFileNameWithoutExtension(p).ToLower().Contains(nameContains.ToLower()))
                continue;
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(p);
            if (go != null) list.Add(go);
        }
        if (list.Count > 0) arrayField = list.ToArray();
    }

    private static UnityEngine.UI.Button CreateButton(Transform parent, Vector2 anchor, string text)
    {
        var go = new GameObject(text + "Button");
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(220, 60);

        var img = go.AddComponent<UnityEngine.UI.Image>();
        img.color = new Color(1, 1, 1, 0.15f);
        var btn = go.AddComponent<UnityEngine.UI.Button>();

        var labelGO = new GameObject("Text");
        labelGO.transform.SetParent(go.transform, false);
        var lrect = labelGO.AddComponent<RectTransform>();
        lrect.anchorMin = new Vector2(0, 0);
        lrect.anchorMax = new Vector2(1, 1);
        lrect.offsetMin = Vector2.zero;
        lrect.offsetMax = Vector2.zero;
        var tmp = labelGO.AddComponent<TMP_Text>();
        tmp.text = text;
        tmp.alignment = TMPro.TextAlignmentOptions.Center;
        tmp.fontSize = 28;

        Selection.activeGameObject = go;
        Mark(go);
        Mark(labelGO);
        return btn;
    }

    private static void Mark(Object o)
    {
        if (o != null) EditorUtility.SetDirty(o);
    }

    // --- Ensure tags exist in ProjectSettings/TagManager ---
    private static void EnsureProjectTags(string[] tagNames)
    {
        var assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if (assets == null || assets.Length == 0) return;
        var tagManager = new SerializedObject(assets[0]);
        var tagsProp = tagManager.FindProperty("tags");
        foreach (var t in tagNames)
        {
            if (!TagExists(tagsProp, t))
            {
                AddTag(tagsProp, t);
            }
        }
        tagManager.ApplyModifiedProperties();
    }

    private static bool TagExists(SerializedProperty tagsProp, string tag)
    {
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            var sv = tagsProp.GetArrayElementAtIndex(i).stringValue;
            if (sv == tag) return true;
        }
        return false;
    }

    private static void AddTag(SerializedProperty tagsProp, string tag)
    {
        tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
        var newProp = tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1);
        newProp.stringValue = tag;
    }
}

