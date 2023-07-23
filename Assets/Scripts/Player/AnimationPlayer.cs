using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AnimationPlayer : MonoBehaviour
{
    public Animator animator;
    private readonly int[] calm = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 3, 3, 4 };
    private int PreviosChoose;
    public ParticleSystem sleepEffect;
    public ParticleSystem walk1;
    public ParticleSystem walk2;
    public ParticleSystem fall;
    public ParticleSystem shoot;
    public PlayerStateManager player;
    public Color colorGroundParticle;

    [Header("Связанное с портальной пушкой")]
    public Transform body;
    public SpriteRenderer gunSprite;
    public Sprite gunOrigSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;
    private PortalGun portalGun;
    private Color firstColor;
    private Color secondColor;
    public ParticleSystem teleport;

    [Header("Усиления прыжка")]
    public SpriteRenderer eye;
    public Sprite normalEye;
    public Sprite doubleEye;
    public Sprite tripleEye;
    [SerializeField] private Light2D eyeLight;
    public ParticleSystem powerParticle;
    public Gradient doubleGradient;
    public Gradient tripleGradient;

    private Color firstPowerColor;
    private Color secondPowerColor;
    private Color addFirstPowerColor;
    private Color addSecondPowerColor;

    [Header("Звуки")]
    [SerializeField] private AudioSource[] walkSounds;
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource crystallJump;
    [SerializeField] private AudioSource bonusGet;
    [SerializeField] private AudioSource bonusJump;

    void Start()
    {
        animator = GetComponent<Animator>();
        firstColor = player.leftColor;
        secondColor = player.rightColor;
        portalGun = player.portalGun;
    }

    public void SetAnimation(string name, bool activate)
    {
        animator.SetBool(name, activate);
    }

    public void SetCalm()
    {
        if (animator.GetBool("IsCalm")) {
            int choose;
            while (true) {
                choose = calm[Random.Range(0, calm.Length)];
                if (choose == 0 || PreviosChoose != choose)
                    break;
            }
            animator.SetInteger("CalmIndex", choose);
            PreviosChoose = choose;
        }
    }

    public void GroundParticleLeft()
    {
        var main = walk2.main;
        main.startColor = colorGroundParticle;
        walk2.Play();
        PlayWalk();
    }

    public void GroundParticleRight()
    {
        var main = walk1.main;
        main.startColor = colorGroundParticle;
        walk1.Play();
        PlayWalk();
    }

    public void PlayWalk()
    {
        if (walkSounds[0].gameObject.activeSelf) {
            ChangeWalkPitch();
            walkSounds[0].Play();
        }
    }

    public IEnumerator PlayFall(int number)
    {
        var oldVolume = walkSounds[0].volume;
        foreach (AudioSource walk in walkSounds)
            walk.volume = Mathf.Max(number / 35f, walk.volume);
        ChangeWalkPitch(0);
        ChangeWalkPitch(1);
        yield return new WaitForSeconds(Time.deltaTime);
        walkSounds[0].Play();
        yield return new WaitForSeconds(Time.deltaTime);
        walkSounds[1].Play();
        foreach (AudioSource walk in walkSounds)
            walk.volume = oldVolume;
        yield return null;
    }

    private void ChangeWalkPitch(int index = 0)
    {
        walkSounds[index].pitch = 1 + Random.Range(-0.2f, 0.2f);
    }

    public void FallParticle()
    {
        var main = fall.main;
        main.startColor = colorGroundParticle;
        fall.Play();
    }

    public void EndShoot()
    {
        animator.SetBool("IsShoot", false);
        portalGun.SetCalm();
        portalGun.transform.parent = body.transform;
        gunSprite.sprite = gunOrigSprite;
        player.shoot = false;
        animator.SetFloat("Speed", 1);
    }

    public void StartShoot()
    {
        shoot.Stop();
        var main = shoot.main;
        animator.SetBool("RestartShooting", false);
        if (portalGun.RightButton) {
            gunSprite.sprite = rightSprite;
            main.startColor = secondColor;
        } else {
            gunSprite.sprite = leftSprite;
            main.startColor = firstColor;
        }
        shoot.Play();
    }

    public void FreeShoot()
    {
        if (portalGun.RightButton && portalGun.ShootBlue != null) {
            portalGun.ShootBlue.transform.position = new Vector2(portalGun.ShootParent.position.x, portalGun.ShootParent.position.y);
            portalGun.ShootBlue.gameObject.SetActive(true);
        } else if (portalGun.ShootOrange != null) {
            portalGun.ShootOrange.transform.position = new Vector2(portalGun.ShootParent.position.x, portalGun.ShootParent.position.y);
            portalGun.ShootOrange.gameObject.SetActive(true);
        }
    }

    public void From_Portal(bool Right)
    {
        var main = teleport.main;
        if (Right)
            main.startColor = secondColor;
        else
            main.startColor = firstColor;
        teleport.Play();
    }

    //public void UpdateJumpBonus()
    //{
    //    bool doubleJump = player.GetJump();
    //    bool tripleJump = player.GetJump(true);

    //    Color doubleColor = doubleGradient.colorKeys[0].color;
    //    Color tripleColor = tripleGradient.colorKeys[0].color;

    //    if (tripleJump) {
    //        eye.sprite = tripleEye;
    //        eyeLight.color = doubleColor;
    //        eyeLight.intensity = 0.8f;
    //    } else if (doubleJump) {
    //        eye.sprite = doubleEye;
    //        eyeLight.color = tripleColor;
    //        eyeLight.intensity = 0.8f;
    //    } else {
    //        eye.sprite = normalEye;
    //        eyeLight.intensity = 0;
    //    }
        
    //    var main = powerParticle.main;
    //    if (doubleJump || tripleJump) {
    //        powerParticle.Play();
    //    } else {
    //        powerParticle.Stop();
    //        main.startColor = Color.white;
    //    }

    //    animator.SetBool("TriplePower", tripleJump);
    //    animator.SetBool("DoublePower", doubleJump);
    //    animator.SetBool("StartPower", main.startColor.color == Color.white);

    //    if (main.startColor.color == doubleColor && tripleJump) {
    //        firstPowerColor = doubleGradient.colorKeys[0].color;
    //        secondPowerColor = doubleGradient.colorKeys[1].color;
    //        addFirstPowerColor = (tripleGradient.colorKeys[0].color - doubleGradient.colorKeys[0].color) / 15f;
    //        addSecondPowerColor = (tripleGradient.colorKeys[1].color - doubleGradient.colorKeys[1].color) / 15f;
    //    } else if (main.startColor.color == tripleColor && doubleJump) {
    //        firstPowerColor = tripleGradient.colorKeys[0].color;
    //        secondPowerColor = tripleGradient.colorKeys[1].color;
    //        addFirstPowerColor = (doubleGradient.colorKeys[0].color - tripleGradient.colorKeys[0].color) / 15f;
    //        addSecondPowerColor = (doubleGradient.colorKeys[1].color - tripleGradient.colorKeys[1].color) / 15f;
    //    } else if (main.startColor.color == Color.white) {
    //        var gradient = powerParticle.colorOverLifetime;
    //        if (doubleJump)
    //            gradient.color = doubleGradient;
    //        else if (tripleJump)
    //            gradient.color = tripleGradient;
    //    }

    //    if (doubleJump)
    //        main.startColor = doubleColor;
    //    else if (tripleJump)
    //        main.startColor = tripleColor;
    //    else
    //        main.startColor = Color.white;
    //}

    //public void ChangePowerGradient()
    //{
    //    var gradient = powerParticle.colorOverLifetime;
    //    firstPowerColor += addFirstPowerColor;
    //    secondPowerColor += addSecondPowerColor;
    //    Gradient grad = new();
    //    grad.SetKeys(new GradientColorKey[] { new GradientColorKey(firstPowerColor, 0.0f), new GradientColorKey(secondPowerColor, 1.0f) }, 
    //        new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 0.8f), new GradientAlphaKey(0.0f, 1.0f) });
    //    gradient.color = grad;
    //}

    public void RestartLevel()
    {
        LevelManager.levelManager.RestartLevel();
    }

    public void StartShooting()
    {
        if (animator.GetBool("IsShoot") && !animator.GetBool("RestartShooting"))
            animator.SetBool("RestartShooting", true);
        animator.SetBool("IsShoot", true);
        animator.SetFloat("Speed", 1);
    }

    public void ReverseWalk()
    {
        animator.SetFloat("Speed", -1);
    }

    public void SetWalkSound(AudioClip source)
    {
        foreach (AudioSource walk in walkSounds)
            walk.clip = source;
    }

    public void PlayJump()
    {
        jumpSound.Play();
    }

    public void PlayCrystallJump(bool boost)
    {
        float standard = 1;
        if (boost)
            standard += 0.15f;
        crystallJump.pitch = standard + Random.Range(-0.2f, 0.2f);
        crystallJump.Play();
    }

    public void PlayGetBonus(bool boost)
    {
        if (boost)
            bonusGet.pitch = 1.5f;
        else
            bonusGet.pitch = 1f;
        bonusGet.Play();
    }

    public void PlayBonusJump(bool triple)
    {
        if (triple)
            bonusJump.pitch = 2f;
        else
            bonusJump.pitch = 1f;
        bonusJump.Play();
    }

    public void Update_Ground(Color color, AudioClip walkAudio, float y)
    {
        colorGroundParticle = color;
        SetWalkSound(walkAudio);
        if (y < -10f && player.onGround) {
            ParticleSystem.MainModule main = fall.main;
            var num = Mathf.Min((int)(8 * (y * -1 - 10f)), 250);
            main.maxParticles = num;
            StartCoroutine(PlayFall(num));
            FallParticle();
        }
    }
}
