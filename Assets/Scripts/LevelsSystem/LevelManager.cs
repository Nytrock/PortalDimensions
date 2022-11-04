using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Cinemachine;
using TMPro;

public class LevelManager: MonoBehaviour
{
    public static LevelManager levelManager;
    private KeyCode restartButton;

    [Header("Настройка уровня")]
    public CinemachineVirtualCamera mainVirtualCamera;
    public List<Camera> othersVirtualCameras;
    public List<LevelStructure> levels;
    private Level levelMain;

    [Header("Переменные для счёта")]
    public int numShoots;
    public int numTeleports;
    public int restartsCoef;
    public int deathsCoef;
    public int timeCoef;

    [Header("Счёт")]
    public TextMeshProUGUI scoreText;
    public List<string> inscriptions;
    public List<Animator> stars;
    public int needScore;
    public int nowScore;

    [Header("Деньги")]
    public Animator moneyAnimator;
    public ParticleSystem coinsParticle;
    public int numCoins;
    public GameObject coinsBound;
    public Transform coinsTarget;

    [Header("Список кнопок")]
    public List<Button> completeButtons;

    private void Start()
    {
        levelManager = this;
        coinsBound.SetActive(false);
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

    public void RestartScene(bool death = false)
    {
        if (death)
            AddToScore("Death");
        else
            AddToScore("Restart");
        LevelInfoHolder.deathsCount = deathsCoef;
        LevelInfoHolder.restartsCount = restartsCoef;
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

    public void NextLevel()
    {
        var world = levelMain.world;
        world.completedLevels = levelMain.id + 1;
        LevelInfoHolder.levelId = (levelMain.id + 1) % world.countLevels;
        Save.save.SaveAll();
        RestartScene();
    }

    private void SetControll()
    {
        restartButton = Save.save.fastRestartKey;
    }

    private void SetLevel(int id)
    {
        levelMain = levels[id].levelMain;
        var level = levels[id];

        restartsCoef = LevelInfoHolder.restartsCount;
        deathsCoef = LevelInfoHolder.deathsCount;

        for (int i=0; i < levels.Count; i++)
            levels[i].gameObject.SetActive(false);
        level.gameObject.SetActive(true);

        var player = Instantiate(Save.save.players[0]);
        player.transform.position = new Vector2(level.spawnPoint.position.x, level.spawnPoint.position.y);
        player.animations.portalGun.gameObject.SetActive(levelMain.hasGun);

        mainVirtualCamera.Follow = player.transform;
        CinemachineConfiner confiner = mainVirtualCamera.gameObject.AddComponent<CinemachineConfiner>();
        confiner.m_BoundingShape2D = level.borderCollider;
        mainVirtualCamera.m_Lens.OrthographicSize = levelMain.cameraZoom;
        foreach (Camera camera in othersVirtualCameras)
            camera.orthographicSize = levelMain.cameraZoom;

        foreach (Transform square in level.collidersContainer)
            square.GetComponent<SpriteRenderer>().enabled = false;
        numCoins = levelMain.coinsNumber;
    }

    public void AddToScore(string type)
    {
        switch (type) {
            case "Shoot":
                numShoots += 1;
                break;
            case "Teleport":
                numTeleports += 1;
                break;
            case "Restart":
                restartsCoef += 5;
                break;
            case "Death":
                deathsCoef += 10;
                break;
        }
    }

    public void StartParticles()
    {
        if (!levelMain.wasCompleted) {
            coinsBound.SetActive(true);

            var emission = coinsParticle.emission;
            emission.rateOverTime = Mathf.Min(numCoins * 100, 20000);

            var main = coinsParticle.main;
            main.maxParticles = Mathf.Min(numCoins, 200);

            coinsParticle.Play();
            StartCoroutine(CoinsWay());
        } else {
            StartCoroutine(WaitBeforeScore());
        }
    }

    IEnumerator CoinsWay()
    {
        yield return new WaitForSeconds(3f);
        coinsBound.SetActive(false);
        float speed = 10f;
        var moneyManager = Save.save.moneyManager;
        int coinsGived = 0;
        int coinsToParticle = 1;
        if (numCoins > 200)
            coinsToParticle = Mathf.FloorToInt(numCoins / 200f);

        List<Vector2> velocities = new List<Vector2>();
        var main = coinsParticle.main;
        main.gravityModifier = 0;
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[coinsParticle.particleCount];
        int count = coinsParticle.GetParticles(particles);
        Vector2 target = coinsTarget.position;
        for (int i = 0; i < count; i++) {
            Vector2 a = new Vector2(1, 0);
            Vector2 b = new Vector2(target.x - particles[i].position.x, target.y - particles[i].position.y);
            float cosinus = (a.x * b.x + a.y * b.y) /
                (Mathf.Sqrt(a.x * a.x + a.y * a.y) * Mathf.Sqrt(b.x * b.x + b.y * b.y));
            float sinus = Mathf.Sqrt(1 - cosinus * cosinus);

            velocities.Add(new Vector2(cosinus * speed, sinus * speed));
        }

        List<int> used = new List<int>();
        while (used.Count != count) {
            particles = new ParticleSystem.Particle[coinsParticle.particleCount];
            count = coinsParticle.GetParticles(particles);
            for (int i = 0; i < count; i++) {
                if (target.x - particles[i].position.x < 0.01f && target.y - particles[i].position.y < 0.01f && !used.Contains(i)) {
                    particles[i].startColor = new Color(0f, 0f, 0f, 0f);
                    used.Add(i);
                    coinsGived += coinsToParticle;
                    if (used.Count == count)
                        moneyManager.AddCoins(numCoins - coinsGived + coinsToParticle);
                    else
                        moneyManager.AddCoins(coinsToParticle);
                    moneyAnimator.Play("AddMoney", 9, 0.1f);
                }
                particles[i].velocity = velocities[i];
            }
            coinsParticle.SetParticles(particles, count);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        SetScore();
    }

    IEnumerator WaitBeforeScore()
    {
        yield return new WaitForSeconds(2f);
        SetScore();
    }

    private void SetScore()
    {
        StopAllCoroutines();
    }

    public void ChangeButtonsWorking(bool value)
    {
        foreach (Button button in completeButtons)
            button.interactable = value;
    }

    public void FixTime()
    {
        timeCoef = (int)Time.timeSinceLevelLoad;
    }
}
