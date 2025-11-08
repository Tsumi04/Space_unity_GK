using UnityEngine;

/// <summary>
/// Điều khiển hành vi của một thiên thạch, bao gồm cả
/// chuyển động và xoay ngẫu nhiên khi được sinh ra.
/// </summary>
public class Asteroid : MonoBehaviour
{
    [Header("Movement")]
   
    private float minSpeed = 1.0f;
   
    private float maxSpeed = 3.0f;

   
   
    private float rotationSpeed = 200.0f; // Độ mỗi giây

    private Rigidbody2D rb;
    private Vector2 randomDirection;

    /// <summary>
    /// Khởi tạo, gán chuyển động và xoay ngẫu nhiên.
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Cho một hướng ngẫu nhiên, chủ yếu là đi xuống
        randomDirection = new Vector2(Random.Range(-0.5f, 0.5f), -1.0f);
        float speed = Random.Range(minSpeed, maxSpeed);

        // Áp dụng lực cho Rigidbody.
        // Sử dụng.velocity để đặt một tốc độ không đổi.[50]
        rb.linearVelocity = randomDirection.normalized * speed;

        // Áp dụng xoay ngẫu nhiên liên tục [51]
        rb.angularVelocity = Random.Range(-rotationSpeed, rotationSpeed);
    }
}