using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Cinemachine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager levelManager;
    private KeyCode restartButton;
    private bool completeAnimations;
    private int oldCoins;

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
    public TextMeshProUGUI message;
    public List<Animator> stars;
    public GameObject newRecordText;
    public ParticleSystem newRecordParticle;
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
        newRecordText.SetActive(false);
        SetControll();
        ControllSettingsManager.OnButtonChange += SetControll;
        SetLevel(LevelInfoHolder.levelId);
    }

    private void Update()
    {
        if (Input.GetKeyDown(restartButton) && restartButton != KeyCode.None)
            RestartScene();

        if (Input.GetKeyDown(KeyCode.Escape) && completeAnimations)
            BreakAllAnimations();
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

    public void DropRestart()
    {
        LevelInfoHolder.deathsCount = 0;
        LevelInfoHolder.restartsCount = 0;
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
        LevelInfoHolder.deathsCount = 0;
        LevelInfoHolder.restartsCount = 0;
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

        for (int i = 0; i < levels.Count; i++)
            levels[i].gameObject.SetActive(false);
        level.gameObject.SetActive(true);

        var player = Instantiate(Save.save.characters[0].prefab).GetComponent<Player>();
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
        completeAnimations = true;
        if (!levelMain.wasCompleted) {
            coinsBound.SetActive(true);
            levelMain.wasCompleted = true;
            oldCoins = Save.save.moneyManager.GetCoins();

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
        float speed = 11f;
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
        Save.save.SaveAll();
        coinsParticle.Stop();
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
        needScore = Mathf.Max(1, GetScore());
        StartCoroutine(AnimatedScoreText());
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
    IEnumerator AnimatedScoreText()
    {
        coinsParticle.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        int num = 13;
        int starsGet = 0;
        List<int> scoresForStars = new List<int>() { levelMain.scoreForOneStar, levelMain.scoreForTwoStar, levelMain.scoreForThreeStar, levelMain.maxScore + 1000 };
        string localize = LocalizationManager.GetTranslate("Счёт:");
        for (int i = 0; i <= (needScore / num) * num + num; i += num) {
            nowScore = Mathf.Min(i, needScore);
            scoreText.text = localize + " " + nowScore.ToString();
            if (nowScore >= scoresForStars[starsGet]) {
                stars[starsGet].Play("FillStar", 0, 0.1f);
                starsGet += 1;
            }
            if (nowScore > levelMain.bestScore && levelMain.wasCompleted) {
                newRecordText.SetActive(true);
                newRecordParticle.Play();
                levelMain.bestScore = needScore;
            }
            yield return new WaitForSeconds(Time.deltaTime / 1.1f);
        }
        if (starsGet == 0)
            stars[starsGet].Play("FillStar", 0, 0.1f);

        yield return new WaitForSeconds(1f);
        message.GetComponent<Animator>().SetBool("isActivate", true);
        var a = (int)((needScore / (levelMain.maxScore * 1f)) * 10) / 10f;
        message.text = LocalizationManager.GetTranslate(inscriptions[(int)(a * 10) - 1]);

        yield return new WaitForSeconds(1.1f);
        foreach (Button button in completeButtons)
            button.interactable = true;
        completeAnimations = false;
    }

    public void NullResrtartButton()
    {
        restartButton = KeyCode.None;
    }

    private void BreakAllAnimations()
    {
        StopAllCoroutines();
        completeAnimations = false;

        coinsParticle.gameObject.SetActive(false);
        Save.save.moneyManager.SetCoins(oldCoins + numCoins);
        moneyAnimator.Play("AddMoney", 9, 0.1f);

        needScore = GetScore();
        nowScore = needScore;
        scoreText.text = LocalizationManager.GetTranslate("Счёт:") + " " + needScore.ToString();
        List<int> scoresForStars = new List<int>() { levelMain.scoreForOneStar, levelMain.scoreForTwoStar, levelMain.scoreForThreeStar, levelMain.maxScore + 1000 };
        for (int i = 0; i <= scoresForStars.Count; i++) {
            if (scoresForStars[i] <= needScore)
                stars[i].Play("FillStar", 0, 0.1f);
            else
                break;
        }
        if (scoresForStars[0] > needScore)
            stars[0].Play("FillStar", 0, 0.1f);

        message.GetComponent<Animator>().Play("Show", 0, 0f);
        var a = (int)((needScore / (levelMain.maxScore * 1f)) * 10) / 10f;
        message.text = LocalizationManager.GetTranslate(inscriptions[(int)(a * 10) - 1]);

        foreach (Button button in completeButtons)
            button.interactable = true;
        completeAnimations = false;

        if (nowScore > levelMain.bestScore && levelMain.wasCompleted) {
            newRecordText.SetActive(true);
            newRecordParticle.Play();
            levelMain.bestScore = needScore;
        }

        Save.save.SaveAll();
        levelMain.wasCompleted = true;
    }

    private int GetScore()
    {
        var extraTime = timeCoef - levelMain.bestTime;
        var extraShoots = numShoots - levelMain.bestShootCount;
        var extraTeleport = numTeleports - levelMain.bestTeleportCount;
        var result = levelMain.maxScore - extraTime * 15 - restartsCoef * 20 - deathsCoef * 40 - extraShoots * 5 - extraTeleport * 10; // это ещё балансить нужно аааа
        return result;
    }

    public void SetOtherCameras() {
        
    }
}
