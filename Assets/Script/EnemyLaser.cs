using UnityEngine;

/// <summary>
/// Điều khiển đạn của địch. Di chuyển xuống và tự hủy.
/// </summary>
public class EnemyLaser : MonoBehaviour
{

    private float speed = 4.5f; // Tốc độ di chuyển của đạn
    private Camera mainCamera;
    private float yMin; // Ranh giới dưới của màn hình

    void Start()
    {
        mainCamera = Camera.main;
        // Lấy ranh giới dưới của màn hình để biết khi nào cần tự hủy
        yMin = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
    }

    void Update()
    {
        // --- 1. LOGIC "TRÔI XUỐNG" ---
        // Di chuyển viên đạn ĐI XUỐNG (Vector3.down) mỗi khung hình
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // --- 2. LOGIC "BIẾN MẤT" ---
        // Nếu vị trí Y của viên đạn thấp hơn mép dưới màn hình (trừ đi 1 khoảng đệm)
        if (transform.position.y < yMin - 1.0f)
        {
            Destroy(this.gameObject); // Tự hủy viên đạn
        }
    }

    // Hàm này được gọi khi nó đi vào một trigger khác (ví dụ: Player)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem có va chạm với "Player" không
        if (other.CompareTag("Player"))
        {
            // Phá hủy viên đạn ngay lập tức
            Destroy(this.gameObject);

            // (Script PlayerController sẽ xử lý việc GameOver khi bị trúng đạn)
        }
    }
}