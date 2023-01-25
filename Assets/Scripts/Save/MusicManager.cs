using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource mainMenu;
    [SerializeField] private AudioSource testLevel;

    private void Awake()
    {
        GameObject[] musicsManagers = GameObject.FindGameObjectsWithTag("Music");
        if (musicsManagers.Length > 1) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += UpdateMusic;
    }

    public void UpdateMusic(Scene current, Scene next)
    {
        if (next.buildIndex <= 2 && !mainMenu.isPlaying) {
            mainMenu.Play();
            testLevel.Stop();
        } else if (next.buildIndex == 4 && !testLevel.isPlaying) {
            mainMenu.Stop();
            testLevel.Play();
        }
    }
}
