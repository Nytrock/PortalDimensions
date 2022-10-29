using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelManager: MonoBehaviour
{
    private KeyCode restartButton;
    public List<Level> levels;

    private void Start()
    {
        SetControll();
        ControllSettingsManager.OnButtonChange += SetControll;
        SetLevel(LevelInfoHolder.levelId);
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

    private void SetLevel(int id)
    {
        for (int i=0; i < levels.Count; i++)
            levels[i].gameObject.SetActive(false);
        levels[id].gameObject.SetActive(true);

        var player = Instantiate(Save.save.players[0]);
        player.transform.position = new Vector2(levels[id].spawnPoint.position.x, levels[id].spawnPoint.position.y);
    }
}
