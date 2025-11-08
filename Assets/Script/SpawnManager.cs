using UnityEngine;
using System.Collections; // Cần thiết cho Coroutines

/// <summary>
/// Quản lý việc sinh ra (spawn) Thiên thạch, Ngôi sao, và Tàu Địch.
/// </summary>
public class SpawnManager : MonoBehaviour
{
    // --- ĐÂY LÀ DÒNG ĐÃ SỬA LỖI ---
    // Chúng ta cần một MẢNG (array) các GameObjects, ký hiệu là
    public GameObject[] asteroidPrefabs; // <--- SỬA LỖI: dùng mảng prefab

    public GameObject starPrefab;
    public GameObject enemyPrefab; // Prefab Tàu Địch mới

    public float asteroidSpawnDelay = 1.5f;
    public float starSpawnDelay = 3.0f;
    public float enemySpawnDelay = 5.0f;

    private Camera mainCamera;
    private float xMin, xMax, ySpawnPos; // Ranh giới để sinh ra

    /// <summary>
    /// Được gọi khi trò chơi bắt đầu.
    /// </summary>
    void Start()
    {
        mainCamera = Camera.main;

        // Fallback: nếu chưa gán trong Inspector, thử nạp từ thư mục Resources/ hoặc đường dẫn mặc định "Prefab/"
        EnsurePrefabsAssigned();

        ySpawnPos = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 1.1f, 0)).y;

        xMin = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        xMax = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;

        StartCoroutine(SpawnAsteroidRoutine());
        StartCoroutine(SpawnStarRoutine());
        StartCoroutine(SpawnEnemyRoutine());
    }

    private void EnsurePrefabsAssigned()
    {
        // Đảm bảo asteroidPrefabs có ít nhất 1 phần tử
        bool needAsteroid = asteroidPrefabs == null || asteroidPrefabs.Length == 0 || asteroidPrefabs[0] == null;
        if (needAsteroid)
        {
            // Nếu không có Resources, hỗ trợ gán trong Editor thông qua AssetDatabase
#if UNITY_EDITOR
            var guids = UnityEditor.AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefab" });
            var list = new System.Collections.Generic.List<GameObject>();
            foreach (var g in guids)
            {
                var p = UnityEditor.AssetDatabase.GUIDToAssetPath(g);
                var name = System.IO.Path.GetFileNameWithoutExtension(p).ToLower();
                if (name.Contains("asteroid"))
                {
                    var go = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(p);
                    if (go != null) list.Add(go);
                }
            }
            if (list.Count > 0) asteroidPrefabs = list.ToArray();
#endif
        }

        if (starPrefab == null)
        {
            #if UNITY_EDITOR
            starPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/Star.prefab");
            #endif
        }

        if (enemyPrefab == null)
        {
            #if UNITY_EDITOR
            enemyPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/EnemyShip.prefab");
            #endif
        }
    }

    /// <summary>
    /// Coroutine: Một vòng lặp vô hạn để sinh ra thiên thạch.
    /// </summary>
    private IEnumerator SpawnAsteroidRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(asteroidSpawnDelay);

            float randomX = Random.Range(xMin, xMax);
            Vector3 spawnPosition = new Vector3(randomX, ySpawnPos, 0);

            // --- CÁC DÒNG SỬA LỖI KHÁC ---
            // (Đảm bảo mảng asteroidPrefabs không bị rỗng)
            if (asteroidPrefabs.Length > 0) // Dòng này BÂY GIỜ đã đúng
            {
                // Chọn một prefab ngẫu nhiên từ mảng
                int randomIndex = Random.Range(0, asteroidPrefabs.Length);
                GameObject prefabToSpawn = asteroidPrefabs[randomIndex]; // <--- SỬA LỖI: Lấy 1 phần tử từ mảng

                // 4. Spawn (Instantiate) thiên thạch
                var go = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                if (go != null)
                {
                    try { go.tag = "Asteroid"; } catch {}
                }
            }
            // -------------------------
        }
    }

    /// <summary>
    /// Coroutine: Một vòng lặp vô hạn để sinh ra ngôi sao.
    /// </summary>
    private IEnumerator SpawnStarRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(starSpawnDelay);

            float randomX = Random.Range(xMin, xMax);
            Vector3 spawnPosition = new Vector3(randomX, ySpawnPos, 0);

            var go = Instantiate(starPrefab, spawnPosition, Quaternion.identity);
            if (go != null)
            {
                try { go.tag = "Star"; } catch {}
            }
        }
    }

    /// <summary>
    /// Coroutine: Một vòng lặp vô hạn để sinh ra tàu địch.
    /// </summary>
    private IEnumerator SpawnEnemyRoutine()
    {
        // Chờ một chút trước khi kẻ thù đầu tiên xuất hiện
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            yield return new WaitForSeconds(enemySpawnDelay);

            float randomX = Random.Range(xMin, xMax);
            Vector3 spawnPosition = new Vector3(randomX, ySpawnPos, 0);

            if (enemyPrefab != null)
            {
                var enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                if (enemy != null)
                {
                    try { enemy.tag = "Enemy"; } catch {}
                }
            }
        }
    }
}