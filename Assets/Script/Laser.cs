using UnityEngine;

/// <summary>
/// Điều khiển hành vi của một viên đạn laser: di chuyển về phía trước và
/// tự hủy khi ra khỏi màn hình hoặc va chạm.
/// </summary>
public class Laser : MonoBehaviour
{

    private float speed = 15.0f; // Tốc độ di chuyển của laser

    private Camera mainCamera;
    private float yMax; // Ranh giới trên của màn hình

    // Tham chiếu đến Trình quản lý Game
    private GameManager gameManager;

    void Start()
    {
        mainCamera = Camera.main;
        // Lấy ranh giới trên của màn hình
        yMax = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0)).y;

        // Tìm GameManager
        gameManager = FindObjectOfType<GameManager>();
    }

    /// <summary>
    /// Di chuyển laser lên trên mỗi khung hình.
    /// </summary>
    void Update()
    {
        // transform.Translate di chuyển đối tượng một cách tương đối
        // so với vị trí hiện tại của nó. Nó đơn giản và không cần vật lý.
        // Vector3.up là cách viết tắt của (0, 1, 0).
        // Time.deltaTime đảm bảo di chuyển mượt mà, độc lập với tốc độ khung hình.
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        // Tự hủy nếu ra khỏi màn hình.[46, 48]
        // Điều này cực kỳ quan trọng để quản lý bộ nhớ.
        if (transform.position.y > yMax + 1.0f) // +1.0f để tạo vùng đệm
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Xử lý va chạm trigger (ví dụ: bắn trúng thiên thạch).
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Bắn trúng thiên thạch → cộng điểm và phá hủy
        if (other.gameObject.CompareTag("Asteroid"))
        {
            if (gameManager != null)
            {
                gameManager.AddScore(gameManager.pointsPerAsteroidKill);
            }
            Destroy(other.gameObject);
            Destroy(this.gameObject);
            return;
        }

        // Bắn trúng tàu địch → cộng điểm và phá hủy
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (gameManager != null)
            {
                gameManager.AddScore(gameManager.pointsPerEnemyKill);
            }
            Destroy(other.gameObject);
            Destroy(this.gameObject);
            return;
        }
    }
}