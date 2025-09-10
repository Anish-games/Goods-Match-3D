using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("UI Elements")]
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;

    private void Awake()
    {
        // Always set the instance for the current scene (don't try to persist between scenes)
        Instance = this;

        // Safety: ensure the panel is hidden at scene start
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        // Clear static reference if this instance is destroyed
        if (Instance == this)
            Instance = null;
    }

    /// <summary>
    /// Robust static entry point — will find an instance if one hasn't been assigned yet.
    /// Call this from other classes (e.g. Placeholder) to show the panel safely.
    /// </summary>
    public static void ShowGameOverStatic(int finalScore)
    {
        if (Instance == null)
        {
            // Try to find an instance in the scene (covers ordering issues)
            Instance = FindObjectOfType<GameOverManager>();
            if (Instance == null)
            {
                Debug.LogError("[GameOverManager] No GameOverManager instance found in scene. Cannot show GameOver panel.");
                return;
            }
        }

        Instance.ShowGameOverInternal(finalScore);
    }

    // Internal instance method that actually shows the UI
    private void ShowGameOverInternal(int finalScore)
    {
        if (gameOverPanel == null)
        {
            Debug.LogError("[GameOverManager] gameOverPanel is not assigned in the inspector.");
            return;
        }

        Debug.Log("[GameOverManager] Showing Game Over panel. Final score: " + finalScore);

        // Display final score text if assigned
        if (finalScoreText != null)
            finalScoreText.text = "Final Score: " + Mathf.Max(0, finalScore);

        // Show panel and pause the game
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // Called by UI Restart button
    public void RestartLevel()
    {
        Time.timeScale = 1f; // unpause before reloading
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Called by UI Quit button (back to main menu)
    public void QuitToMenu()
    {
        Time.timeScale = 1f; // unpause before loading menu
        SceneManager.LoadScene(0);
    }
}
