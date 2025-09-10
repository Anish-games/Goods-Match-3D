using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic Instance;
    public AudioSource musicSource;

    private void Awake()
    {
        // Singleton: only one instance survives
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // keep alive between scenes
        }
        else
        {
            Destroy(gameObject); // prevent duplicates
            return;
        }

        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.loop = true;
            musicSource.Play();
        }
    }
}
