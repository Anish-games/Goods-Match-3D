using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    [Header("Scoring")]
    public int currentScore = 0;
    public int pointsToNextLevel = 10;

    [Header("UI References")]
    public TMP_Text scoreText;
    public Slider progressBar; // assign in inspector

    void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreUI();

        // Update progress bar
        if (progressBar != null)
        {
            progressBar.value = (float)currentScore / pointsToNextLevel; // normalized 0–1
        }

        // Trigger win when score goal reached
        if (currentScore >= pointsToNextLevel)
        {
            TriggerWin();
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + currentScore;
    }

    private void TriggerWin()
    {
        Debug.Log("[ScoreSystem] Player reached target score. Triggering Win screen.");

        // Use WinManager to show Win UI
        if (WinManager.Instance != null)
        {
            WinManager.Instance.ShowWin();
        }
        else
        {
            Debug.LogError("[ScoreSystem] No WinManager instance found in scene!");
        }
    }
}
