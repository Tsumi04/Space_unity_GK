using UnityEngine;
using System.Collections; // Cần thiết cho Coroutines

/// <summary>
/// Quản lý việc sinh ra (spawn) Thiên thạch, Ngôi sao, và Tàu Địch.
/// </summary>
public class SpawnManager : MonoBehaviour
{
    // --- ĐÂY LÀ DÒNG ĐÃ SỬA LỖI ---
    // Chúng ta cần một MẢNG (array) các GameObjects, ký hiệu là
    public GameObject asteroidPrefabs; // <--- SỬA LỖI: Thêm

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

        ySpawnPos = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 1.1f, 0)).y;

        xMin = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        xMax = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;

        StartCoroutine(SpawnAsteroidRoutine());
        StartCoroutine(SpawnStarRoutine());
        StartCoroutine(SpawnEnemyRoutine());
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
                Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
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

            Instantiate(starPrefab, spawnPosition, Quaternion.identity);
        }
    }

    /// <summary>
    /// Coroutine: Một vòng lặp vô hạn để sinh ra tàu địch.
    /// </summary>
    private IEnumerator SpawnEnemyRoutine()
    {
        // Chờ một chút trước khi kẻ thù đầu tiên xuất hiện
        yield return new WaitForSeconds(3.0f);

        while (true)
        {
            yield return new WaitForSeconds(enemySpawnDelay);

            float randomX = Random.Range(xMin, xMax);
            Vector3 spawnPosition = new Vector3(randomX, ySpawnPos, 0);

            if (enemyPrefab != null)
            {
                Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }
}