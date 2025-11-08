using UnityEngine;

/// <summary>
/// Xử lý tất cả các đầu vào của người chơi, chuyển động dựa trên vật lý,
/// và logic bắn vũ khí.
/// </summary>
public class PlayerController : MonoBehaviour
{

    private float moveSpeed = 5.0f; // Tốc độ di chuyển


    private float xMin, xMax, yMin, yMax; // Biến lưu trữ ranh giới camera
    private float playerWidth, playerHeight; // Biến lưu trữ một nửa kích thước của sprite

    // Tham chiếu đến các thành phần
    private Rigidbody2D rb;
    private Camera mainCamera;

    // Biến lưu trữ đầu vào
    private Vector2 moveInput;

    // --- Biến cho việc bắn súng ---

    // Các biến này là PUBLIC để chúng ta có thể gán chúng trong Inspector
    public GameObject laserPrefab; // Tham chiếu đến Prefab Laser
    public Transform firePoint; // Vị trí mà laser sẽ được bắn ra


    private float fireRate = 0.25f; // Thời gian giữa các phát bắn

    private float nextFireTime = 0.0f; // Bộ đếm thời gian

    // --- Tham chiếu đến Trình quản lý Game ---
    private GameManager gameManager; // Tham chiếu đến GameManager

    /// <summary>
    /// Được gọi một lần khi đối tượng được bật.
    /// </summary>
    void Start()
    {
        // Lấy các thành phần và lưu trữ chúng
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        // Nếu không có điểm bắn (fire point) nào được gán, mặc định là tâm của tàu
        if (firePoint == null)
        {
            firePoint = this.transform;
        }

        // Tìm đối tượng GameManager trong scene.
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("Không tìm thấy GameManager trong scene!");
        }

        // Tính toán ranh giới màn hình
        CalculateScreenBounds();
    }

    /// <summary>
    /// Được gọi mỗi khung hình. Lý tưởng để kiểm tra Đầu vào (Input).
    /// </summary>
    void Update()
    {
        // 1. Lấy Đầu vào Di chuyển
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        // 2. Lấy Đầu vào Bắn súng
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.time > nextFireTime)
            {
                // Cập nhật thời gian bắn tiếp theo
                nextFireTime = Time.time + fireRate;

                // Tạo một bản sao (instance) của laserPrefab tại vị trí firePoint
                Instantiate(laserPrefab, firePoint.position, Quaternion.identity);
            }
        }
    }

    /// <summary>
    /// Được gọi mỗi bước vật lý cố định. Lý tưởng cho các cập nhật Vật lý (Physics).
    /// </summary>
    void FixedUpdate()
    {
        // Áp dụng di chuyển vào Rigidbody
        Vector2 newVelocity = moveInput.normalized * moveSpeed;
        rb.linearVelocity = newVelocity;

        // Áp dụng giới hạn màn hình sau khi vật lý di chuyển
        ClampPlayerPosition();
    }

    /// <summary>
    /// Tính toán ranh giới có thể chơi được dựa trên kích thước camera và sprite.
    /// </summary>
    private void CalculateScreenBounds()
    {
        // Chuyển đổi từ Tọa độ Viewport sang Tọa độ Thế giới
        Vector3 lowerLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 upperRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));

        xMin = lowerLeft.x;
        xMax = upperRight.x;
        yMin = lowerLeft.y;
        yMax = upperRight.y;

        // Bù đắp cho kích thước của sprite
        playerWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
        playerHeight = GetComponent<SpriteRenderer>().bounds.extents.y;
    }

    /// <summary>
    /// Giữ người chơi trong tầm nhìn của camera.
    /// </summary>
    private void ClampPlayerPosition()
    {
        // Lấy vị trí hiện tại
        Vector3 currentPosition = transform.position;

        // Kẹp (Clamp) vị trí
        float clampedX = Mathf.Clamp(currentPosition.x, xMin + playerWidth, xMax - playerWidth);
        float clampedY = Mathf.Clamp(currentPosition.y, yMin + playerHeight, yMax - playerHeight);

        // Cập nhật vị trí của transform
        transform.position = new Vector3(clampedX, clampedY, 0);
    }

    /// <summary>
    /// Được gọi khi va chạm với một collider vật lý khác (KHÔNG phải trigger).
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // --- ĐÂY LÀ DÒNG ĐÃ SỬA LỖI (DÙNG "||" VÀ TRÊN CÙNG MỘT DÒNG) ---
        // Kiểm tra xem chúng ta đã va chạm với Thiên thạch (Asteroid) HOẶC Tàu địch (Enemy)
        if (collision.gameObject.CompareTag("Asteroid") || collision.gameObject.CompareTag("Enemy"))
        {
            // Trò chơi kết thúc
            if (gameManager != null)
            {
                gameManager.GameOver();
            }

            // Phá hủy tàu vũ trụ
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Được gọi khi đi vào một Trigger collider khác (KHÔNG phải vật lý).
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem chúng ta đã thu thập được Ngôi sao (Star)
        if (other.gameObject.CompareTag("Star"))
        {
            // Thêm điểm
            if (gameManager != null)
            {
                gameManager.AddScore(gameManager.pointsPerStar);
            }

            // Phá hủy ngôi sao đã thu thập
            Destroy(other.gameObject);
        }
        // Logic mới để kiểm tra đạn địch
        else if (other.gameObject.CompareTag("EnemyLaser"))
        {
            // Bị trúng đạn địch!
            if (gameManager != null)
            {
                gameManager.GameOver(); // Gọi Game Over
            }

            Destroy(other.gameObject); // Phá hủy viên đạn
            Destroy(this.gameObject);  // Phá hủy Người chơi
        }
    }
}