using UnityEngine;
using UnityEngine.SceneManagement;

public class WinManager : MonoBehaviour
{
    public static WinManager Instance;

  
    public GameObject winPanel; 

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (winPanel != null)
            winPanel.SetActive(false); 
    }

    public void ShowWin()
    {
        if (winPanel != null)
            winPanel.SetActive(true);
    }

    public void NextLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        int nextScene = currentScene + 1;

        if (nextScene < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextScene); 
        }
        else
        {
            
            SceneManager.LoadScene(0); 
        }
    }


    public void QuitToMenu()
    {
        SceneManager.LoadScene(0); 
    }
}
