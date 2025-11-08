using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Quản lý các tương tác UI và chuyển đổi scene cho Main Menu.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    // TÔI ĐÃ THAY ĐỔI DÒNG NÀY:
    // "public" cũng sẽ làm cho nó hiển thị trong Inspector.
    // Đây là để buộc Unity phải cập nhật.
    public GameObject instructionsPanel;

    /// <summary>
    /// Tải scene Gameplay chính.
    /// </summary>
    public void PlayGame()
    {
        SceneManager.LoadScene("Gameplay");
    }

    /// <summary>
    /// Bật hoặc tắt bảng hướng dẫn.
    /// </summary>
    public void ToggleInstructionsPanel()
    {
        if (instructionsPanel != null)
        {
            // Đặt trạng thái hoạt động của panel thành giá trị ngược lại của nó.
            instructionsPanel.SetActive(!instructionsPanel.activeSelf);
        }
        else
        {
            Debug.LogError("Instructions Panel chưa được gán trong Inspector!");
        }
    }

    /// <summary>
    /// Thoát khỏi ứng dụng.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}