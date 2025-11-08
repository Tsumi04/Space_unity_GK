using UnityEngine;
using TMPro; // Thêm dòng này để chuẩn bị cho Phần 6
using UnityEngine.SceneManagement; // Thêm dòng này để chuẩn bị cho Phần 6

public class GameManager : MonoBehaviour
{
    // Điểm thưởng
    public int pointsPerStar = 5;
    public int pointsPerAsteroidKill = 10;
    public int pointsPerEnemyKill = 20;

    // Trạng thái điểm
    public int currentScore = 0;

    // Tham chiếu UI (kéo thả trong Gameplay scene nếu có)
    public TMP_Text scoreText;

    private void Awake()
    {
        // Đảm bảo tồn tại duy nhất trong scene Gameplay (không cần DontDestroyOnLoad)
    }

    private void Start()
    {
        // Nếu chưa có UI, tạo tạm thời một Canvas + TMP để hiển thị điểm
        if (scoreText == null)
        {
            TryCreateDefaultScoreUI();
        }
        UpdateScoreUI();
    }

    public void AddScore(int pointsToAdd)
    {
        currentScore += pointsToAdd;
        if (currentScore < 0) currentScore = 0;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");

        // --- ĐẢM BẢO BẠN CÓ DÒNG NÀY ---
        Time.timeScale = 0f; // Đóng băng trò chơi 
        // -----------------------------

        // Lưu điểm số cuối cùng
        PlayerPrefs.SetInt("FinalScore", currentScore);

        // Tải End Game Scene
        SceneManager.LoadScene("EndGame");
    }

    private void TryCreateDefaultScoreUI()
    {
        // Tạo Canvas + TMP mặc định nếu thiếu, để người chơi chạy ngay được
        var canvasGO = new GameObject("Canvas_Score");
        var canvas = canvasGO.AddComponent<UnityEngine.Canvas>();
        canvas.renderMode = UnityEngine.RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        var eventSystem = GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        var textGO = new GameObject("ScoreText");
        textGO.transform.SetParent(canvasGO.transform, false);
        var rect = textGO.AddComponent<UnityEngine.RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(16, -16);

        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.enableWordWrapping = false;
        tmp.fontSize = 32;
        tmp.text = "Score: 0";

        scoreText = tmp;
    }
}