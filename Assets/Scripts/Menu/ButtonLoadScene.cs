using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonLoadScene : MonoBehaviour
{
    public void LoadScene(int number)
    {
        SceneManager.LoadScene(number);
    }
}
