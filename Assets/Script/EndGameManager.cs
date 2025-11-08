using UnityEngine;
using UnityEngine.SceneManagement; // BẮT BUỘC để tải scene
using TMPro; // BẮT BUỘC để điều khiển TextMeshPro

/// <summary>
/// Quản lý scene EndGame.
/// Hiển thị điểm số cuối cùng và xử lý các nút.
/// </summary>
public class EndGameManager : MonoBehaviour
{
    // Kéo đối tượng Text (TMP) của bạn vào đây
    public TextMeshProUGUI finalScoreText;

    void Start()
    {
        // 1. ĐỌC ĐIỂM SỐ ĐÃ LƯU
        // Lấy điểm số được lưu bởi GameManager bằng khóa "FinalScore"
        int score = PlayerPrefs.GetInt("FinalScore", 0);

        // 2. HIỂN THỊ ĐIỂM SỐ
        // Cập nhật văn bản để hiển thị điểm số
        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + score.ToString(); 
        }
    }

    /// <summary>
    /// Hàm này sẽ được gọi bởi Nút Chơi Lại (Restart).
    /// Nó tải lại scene "Gameplay".
    /// </summary>
    public void RestartGame()
    {
        // --- DÒNG QUAN TRỌNG NHẤT ĐỂ SỬA LỖI ---
        // Đặt lại tốc độ thời gian về bình thường (1)
        Time.timeScale = 1f; 
        // ------------------------------------

        // Tải scene trò chơi
        SceneManager.LoadScene("Gameplay");
    }

    /// <summary>
    /// Hàm này sẽ được gọi bởi Nút "Main Menu".
    /// Tải scene "MainMenu".
    /// </summary>
    public void BackToMainMenu()
    {
        // Cũng đặt lại thời gian, để đề phòng
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Hàm này sẽ được gọi bởi Nút "Quit".
    /// Thoát khỏi trò chơi.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game!"); // (Để kiểm tra trong Editor)
    }
}