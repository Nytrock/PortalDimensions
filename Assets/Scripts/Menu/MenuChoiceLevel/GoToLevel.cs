using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToLevel : MonoBehaviour
{
    public void LoadLevel()
    {
        SceneManager.LoadScene(5);
    }
}
