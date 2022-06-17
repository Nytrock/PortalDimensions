using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlayer : MonoBehaviour
{
    public Animator animator;
    private List<int> Calm = new List<int> { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 3, 4, 4, 5};
    private int PreviosChoose;
    public ParticleSystem sleepEffect;
    public ParticleSystem Walk1;
    public ParticleSystem Walk2;
    public ParticleSystem Fall;
    public ParticleSystem Shoot;
    public Player player;
    public Color colorGroundParticle;

    [Header("Связанное с портальной пушкой")]
    public Transform Body;
    public SpriteRenderer GunSprite;
    public Sprite GunOrigSprite;
    public Sprite LeftSprite;
    public Sprite RightSprite;
    public PortalGun portalGun;
    public Transform Hand;
    public Transform Head;
    public Color FirstColor;
    public Color SecondColor;
    public ParticleSystem Teleport;

    [Header("Усиления прыжка")]
    public SpriteRenderer Eye;
    public Sprite NormalEye;
    public Sprite DoubleEye;
    public Sprite TripleEye;
    public GameObject DoubleLight;
    public GameObject TripleLight;
    public ParticleSystem PowerParticle;
    public Gradient DoubleGradient;
    public Gradient TripleGradient;

    private Color FirstPowerColor;
    private Color SecondPowerColor;
    private Color AddFirstPowerColor;
    private Color AddSecondPowerColor;

    private bool Sleep;

    void Start()
    {
        animator.Play("Calm-1");
    }

    void Update()
    {
        animator.SetBool("IsJump", !player.onGround);
        animator.SetBool("IsCalm", player.onGround && player.moveVector.x == 0);
        animator.SetBool("IsWalk", player.onGround && player.moveVector.x != 0);
    }

    public void SetCalm()
    {
        if (animator.GetBool("IsCalm"))
        {
            int choose = 0;
            while (true)
            {
                choose = Calm[Random.Range(0, Calm.Count)];
                if (choose == 1 || PreviosChoose != choose)
                {
                    break;
                }
            }
            animator.Play("Calm-" + choose);
            PreviosChoose = choose;
        }
    }

    public void Sleeping()
    {
        Sleep = !Sleep;
        if (Sleep)
            sleepEffect.Play();
        else
            sleepEffect.Stop();
    }

    public void GroundParticleLeft()
    {
        var main = Walk2.main;
        main.startColor = colorGroundParticle;
        Walk2.Play();
    }

    public void GroundParticleRight()
    {
        var main = Walk1.main;
        main.startColor = colorGroundParticle;
        Walk1.Play();
    }

    public void FallParticle()
    {
        var main = Fall.main;
        main.startColor = colorGroundParticle;
        Fall.Play();
    }

    public void EndShoot()
    {
        animator.SetBool("IsShoot", false);
        Hand.rotation = Quaternion.Euler(0f, 0f, 0f);
        Hand.rotation *= Quaternion.Euler(0f, 0f, 0f);
        Head.rotation = Quaternion.Euler(0f, 0f, 0f);
        Head.rotation *= Quaternion.Euler(0f, 0f, 0f);
        portalGun.transform.parent = Body.transform;
        portalGun.BlueLight.SetActive(false);
        portalGun.OrangeLight.SetActive(false);
        GunSprite.sprite = GunOrigSprite;
        player.Shoot = false;
        animator.SetFloat("Speed", 1);
    }

    public void StartShoot()
    {
        Shoot.Stop();
        var main = Shoot.main;
        animator.SetBool("RestartShooting", false);
        if (portalGun.RightButton)
        {
            GunSprite.sprite = RightSprite;
            main.startColor = SecondColor;
        }
        else
        {
            GunSprite.sprite = LeftSprite;
            main.startColor = FirstColor;
        }
        Shoot.Play();
    }

    public void FreeShoot()
    {
        if (portalGun.RightButton && portalGun.ShootBlue != null)
        {
            portalGun.ShootBlue.transform.position = new Vector2(portalGun.Shoot_parent.position.x, portalGun.Shoot_parent.position.y);
            portalGun.ShootBlue.gameObject.SetActive(true);
        } else if (portalGun.ShootOrange != null)
        {
            portalGun.ShootOrange.transform.position = new Vector2(portalGun.Shoot_parent.position.x, portalGun.Shoot_parent.position.y);
            portalGun.ShootOrange.gameObject.SetActive(true);
        }
    }

    public void From_Portal(bool Right)
    {
        var main = Teleport.main;
        if (Right)
            main.startColor = SecondColor;
        else
            main.startColor = FirstColor;
        Teleport.Play();
    }

    public void UpdateJumpBonus()
    {
        if (player.TripleJump)
            Eye.sprite = TripleEye;
        else if (player.DoubleJump)
            Eye.sprite = DoubleEye;
        else
            Eye.sprite = NormalEye;
        TripleLight.SetActive(player.TripleJump);
        DoubleLight.SetActive(player.DoubleJump);
        

        var main = PowerParticle.main;

        if (player.DoubleJump || player.TripleJump)
            PowerParticle.Play();
        else
        {
            PowerParticle.Stop();
            main.startColor = Color.white;
        }

        animator.SetBool("TriplePower", player.TripleJump);
        animator.SetBool("DoublePower", player.DoubleJump);
        animator.SetBool("StartPower", main.startColor.color == Color.white);

        if (main.startColor.color == SecondColor && player.TripleJump)
        {
            FirstPowerColor = DoubleGradient.colorKeys[0].color;
            SecondPowerColor = DoubleGradient.colorKeys[1].color;
            AddFirstPowerColor = (TripleGradient.colorKeys[0].color - DoubleGradient.colorKeys[0].color) / 15f;
            AddSecondPowerColor = (TripleGradient.colorKeys[1].color - DoubleGradient.colorKeys[1].color) / 15f;
        }
        else if (main.startColor.color == FirstColor && player.DoubleJump)
        {
            FirstPowerColor = TripleGradient.colorKeys[0].color;
            SecondPowerColor = TripleGradient.colorKeys[1].color;
            AddFirstPowerColor = (DoubleGradient.colorKeys[0].color - TripleGradient.colorKeys[0].color) / 15f;
            AddSecondPowerColor = (DoubleGradient.colorKeys[1].color - TripleGradient.colorKeys[1].color) / 15f;
        } else if (main.startColor.color == Color.white)
        {
            var gradient = PowerParticle.colorOverLifetime;
            if (player.DoubleJump)
                gradient.color = DoubleGradient;
            else if (player.TripleJump)
                gradient.color = TripleGradient;
        }
    }

    public void ChangePowerGradient()
    {
        var gradient = PowerParticle.colorOverLifetime;
        FirstPowerColor += AddFirstPowerColor;
        SecondPowerColor += AddSecondPowerColor;
        Gradient grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(FirstPowerColor, 0.0f), new GradientColorKey(SecondPowerColor, 1.0f) }, 
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 0.8f), new GradientAlphaKey(0.0f, 1.0f) });
        gradient.color = grad;
    }
}
