using UnityEngine;

/// <summary>
/// Điều khiển hành vi của tàu địch: di chuyển xuống và bắn súng.
/// </summary>
public class EnemyController : MonoBehaviour
{
    [Header("Movement")]

    private float minSpeed = 2.0f;

    private float maxSpeed = 4.0f;


    // Kéo EnemyLaser Prefab vào đây
    public GameObject laserPrefab;

    // --- CHÚNG TA THÊM DÒNG NÀY ---
    // Kéo FirePoint (là con của EnemyShip) vào đây
    public Transform firePoint;
    // -------------------------


    private float fireRate = 2.0f; // Bắn mỗi 2 giây
    private float nextFireTime;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Cho một hướng đi xuống ngẫu nhiên
        Vector2 randomDirection = new Vector2(Random.Range(-0.2f, 0.2f), -1.0f);
        float speed = Random.Range(minSpeed, maxSpeed);
        rb.linearVelocity = randomDirection.normalized * speed;

        // Đặt thời gian bắn ngẫu nhiên
        nextFireTime = Time.time + Random.Range(1.0f, fireRate);

        // --- CHÚNG TA THÊM DÒNG NÀY ---
        // An toàn: Nếu chúng ta quên gán FirePoint, nó sẽ bắn từ tâm
        if (firePoint == null)
        {
            firePoint = transform;
        }
        // -------------------------
    }

    void Update()
    {
        // Logic bắn súng
        if (Time.time > nextFireTime)
        {
            // Reset thời gian bắn
            nextFireTime = Time.time + fireRate;

            if (laserPrefab != null)
            {
                // --- CHÚNG TA SỬA DÒNG NÀY ---
                // Bắn từ vị trí của firePoint, không phải transform.position [1]
                Instantiate(laserPrefab, firePoint.position, Quaternion.identity);
            }
        }
    }
}