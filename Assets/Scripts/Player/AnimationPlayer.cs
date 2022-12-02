using System.Collections;
using UnityEngine;

public class AnimationPlayer : MonoBehaviour
{
    private Animator animator;
    private readonly int[] calm = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 3, 4, 4, 5 };
    private int PreviosChoose;
    public ParticleSystem sleepEffect;
    public ParticleSystem walk1;
    public ParticleSystem walk2;
    public ParticleSystem fall;
    public ParticleSystem shoot;
    public Player player;
    public Color colorGroundParticle;

    [Header("Связанное с портальной пушкой")]
    public Transform body;
    public SpriteRenderer gunSprite;
    public Sprite gunOrigSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;
    public PortalGun portalGun;
    public Transform hand;
    public Transform head;
    public Color firstColor;
    public Color secondColor;
    public ParticleSystem teleport;

    [Header("Усиления прыжка")]
    public SpriteRenderer eye;
    public Sprite normalEye;
    public Sprite doubleEye;
    public Sprite tripleEye;
    public GameObject doubleLight;
    public GameObject tripleLight;
    public ParticleSystem powerParticle;
    public Gradient doubleGradient;
    public Gradient TripleGradient;

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
        animator.Play("Calm-1");
    }

    void Update()
    {
        animator.SetBool("IsJump", !player.onGround);
        animator.SetBool("IsCalm", player.onGround && player.moveVector.x == 0 && !player.shoot);
        animator.SetBool("IsWalk", player.onGround && player.moveVector.x != 0);
    }

    public void SetCalm()
    {
        if (animator.GetBool("IsCalm")) {
            int choose;
            while (true) {
                choose = calm[Random.Range(0, calm.Length)];
                if (choose == 1 || PreviosChoose != choose)
                    break;
            }
            animator.Play("Calm-" + choose);
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
        ChangeWalkPitch();
        walkSounds[0].Play();
    }

    public IEnumerator PlayFall(int number)
    {
        var oldVolume = walkSounds[0].volume;
        foreach (AudioSource walk in walkSounds)
            walk.volume = Mathf.Max(number / 35f, walk.volume);
        ChangeWalkPitch(0);
        ChangeWalkPitch(1);
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
        hand.rotation = Quaternion.Euler(0f, 0f, 0f);
        hand.rotation *= Quaternion.Euler(0f, 0f, 0f);
        head.rotation = Quaternion.Euler(0f, 0f, 0f);
        head.rotation *= Quaternion.Euler(0f, 0f, 0f);
        portalGun.transform.parent = body.transform;
        portalGun.BlueLight.SetActive(false);
        portalGun.OrangeLight.SetActive(false);
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
            portalGun.ShootBlue.transform.position = new Vector2(portalGun.Shoot_parent.position.x, portalGun.Shoot_parent.position.y);
            portalGun.ShootBlue.gameObject.SetActive(true);
        } else if (portalGun.ShootOrange != null) {
            portalGun.ShootOrange.transform.position = new Vector2(portalGun.Shoot_parent.position.x, portalGun.Shoot_parent.position.y);
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

    public void UpdateJumpBonus()
    {
        bool doubleJump = player.GetJump();
        bool tripleJump = player.GetJump(true);

        if (tripleJump)
            eye.sprite = tripleEye;
        else if (doubleJump)
            eye.sprite = doubleEye;
        else
            eye.sprite = normalEye;
        tripleLight.SetActive(tripleJump);
        doubleLight.SetActive(doubleJump);
        
        var main = powerParticle.main;
        if (doubleJump || tripleJump) {
            powerParticle.Play();
        } else {
            powerParticle.Stop();
            main.startColor = Color.white;
        }

        animator.SetBool("TriplePower", tripleJump);
        animator.SetBool("DoublePower", doubleJump);
        animator.SetBool("StartPower", main.startColor.color == Color.white);

        if (main.startColor.color == secondColor && tripleJump) {
            firstPowerColor = doubleGradient.colorKeys[0].color;
            secondPowerColor = doubleGradient.colorKeys[1].color;
            addFirstPowerColor = (TripleGradient.colorKeys[0].color - doubleGradient.colorKeys[0].color) / 15f;
            addSecondPowerColor = (TripleGradient.colorKeys[1].color - doubleGradient.colorKeys[1].color) / 15f;
        } else if (main.startColor.color == firstColor && doubleJump) {
            firstPowerColor = TripleGradient.colorKeys[0].color;
            secondPowerColor = TripleGradient.colorKeys[1].color;
            addFirstPowerColor = (doubleGradient.colorKeys[0].color - TripleGradient.colorKeys[0].color) / 15f;
            addSecondPowerColor = (doubleGradient.colorKeys[1].color - TripleGradient.colorKeys[1].color) / 15f;
        } else if (main.startColor.color == Color.white) {
            var gradient = powerParticle.colorOverLifetime;
            if (doubleJump)
                gradient.color = doubleGradient;
            else if (tripleJump)
                gradient.color = TripleGradient;
        }

        if (doubleJump)
            main.startColor = secondColor;
        else if (tripleJump)
            main.startColor = firstColor;
        else
            main.startColor = Color.white;
    }

    public void ChangePowerGradient()
    {
        var gradient = powerParticle.colorOverLifetime;
        firstPowerColor += addFirstPowerColor;
        secondPowerColor += addSecondPowerColor;
        Gradient grad = new();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(firstPowerColor, 0.0f), new GradientColorKey(secondPowerColor, 1.0f) }, 
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 0.8f), new GradientAlphaKey(0.0f, 1.0f) });
        gradient.color = grad;
    }

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

    public void Death()
    {
        portalGun.enabled = false;
        animator.SetBool("IsDeath", true);
    }

    public void SetWalkSound(AudioSource source)
    {
        foreach (AudioSource walk in walkSounds) {
            walk.clip = source.clip;
            walk.volume = source.volume;
        }
    }

    public void PlayJump()
    {
        if (!jumpSound.isPlaying)
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
}
