using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [SerializeField] private AudioSource mainMenu;
    [SerializeField] private AudioSource testLevel;

    private AudioSource currentTrack;

    private void Awake()
    {
        if (instance != null) {
            Destroy(gameObject);
            return;
        } else {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += UpdateMusic;
    }

    public void UpdateMusic(Scene current, Scene next)
    {
        AudioSource newTrack = new();
        switch (next.buildIndex) {
            case <= 2: newTrack = mainMenu; break; 
            case 4: newTrack = testLevel; break;
        }

        if (currentTrack == null) {
            currentTrack = newTrack;
            currentTrack.Play();
            return;
        }

        if (currentTrack != newTrack) {
            currentTrack.Stop();
            currentTrack = newTrack;
            currentTrack.Play();
        }
    }
}
