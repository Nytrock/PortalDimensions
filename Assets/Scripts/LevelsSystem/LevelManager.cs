using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager: MonoBehaviour
{
    private KeyCode restartButton;

    private void Start()
    {
        SetControll();
        ControllSettingsManager.OnButtonChange += SetControll;
    }

    private void Update()
    {
        if (Input.GetKeyDown(restartButton))
            RestartScene();
    }

    public void RestartLevel()
    {
        if (Save.GetAutoRestart())
            StartCoroutine(Reload());
        else
            GameObject.Find("Canvases").GetComponent<Animator>().SetBool("isDeath", true);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(1.3f);
        RestartScene();
    }

    public void Exit()
    {
        SceneManager.LoadScene(2);
    }

    private void SetControll()
    {
        restartButton = Save.save.fastRestartKey;
    }
}
