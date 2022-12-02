using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public AnimationPlayer animations;
    private Rigidbody2D rb;
    [SerializeField] private float realSpeed;
    private Transform skin;
    public Vector2 moveVector;

    private bool jumping;
    public bool shoot;
    public bool right;
    public bool inPortal;
    [Header("Прыжок")]
    public bool onGround;
    private int jumpIteration = 0;
    private float jumpForce;
    public float normalForce;
    public float crystallForce;
    public float boostCrystallForce;
    private bool crystallJump;
    private bool boostCrystallJump;
    private bool doubleJump;
    private bool tripleJump;
    private readonly List<JumpCrystall> jumpCrystalls = new();
    private readonly List<JumpBonus> bonusDoubleJumps = new();
    private readonly List<JumpBonus> bonusTripleJumps = new();
    [Header("Смена внешнего вида при повороте")]
    public SpriteRenderer[] changingObj;
    public Sprite[] leftSprites;
    public Sprite[] rightSprites;
    [Header("Бинды кнопок")]
    private KeyCode walkLeftKey;
    private KeyCode walkRightKey;
    private KeyCode jumpKey;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animations = GetComponentInChildren<AnimationPlayer>();
        skin = animations.transform;

        SetControll();
        ControllSettingsManager.OnButtonChange += SetControll;

        DialogueManager.dialogueManager.SetPlayer(this);
    }

    private void Update()
    {
        if (!ButtonFunctional.isGamePaused) {
            Walk();
            Jump();
        }
    }

    private void Walk()
    {
        float move = 0f;
        if (Input.GetKey(walkLeftKey) && moveVector.x <= 0)
            move = -1;
        else if (Input.GetKey(walkRightKey) && moveVector.x >= 0)
            move = 1;
        moveVector.x = move * realSpeed;
        rb.velocity = new Vector2(moveVector.x, rb.velocity.y);
        if ((right && moveVector.x < 0 || !right && moveVector.x > 0)) {
            if (shoot)
                animations.ReverseWalk();
            else
                Flip();
        }
    }

    public void Flip()
    {
        skin.localScale = new Vector2(skin.localScale.x * -1, skin.localScale.y);
        right = !right;
        if (right){
            for (int i=0; i < changingObj.Length; i++)
                changingObj[i].sprite = rightSprites[i];
        } else {
            for (int i = 0; i < changingObj.Length; i++)
                changingObj[i].sprite = leftSprites[i];
        }
    }

    private void Jump()
    {
        if (Input.GetKeyUp(jumpKey))
            jumping = false;
        if (Input.GetKeyDown(jumpKey))
            jumping = true;

        if (jumpIteration > 0) {
            if (!onGround)
                jumping = false;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpIteration -= 1;
        }

        if (jumping && (onGround || crystallJump || doubleJump || tripleJump)) {
            if (onGround) {
                animations.PlayJump();
                jumpForce = normalForce;
                jumpIteration = 10;
                rb.AddForce(1.2f * jumpForce * Vector2.up, ForceMode2D.Impulse);
            } else if (crystallJump) {
                float value = CalculateJumpForce();
                jumpForce = crystallForce / value;
                if (boostCrystallJump)
                    jumpForce = boostCrystallForce / value;
                jumpIteration = 10;
                jumpCrystalls[0].Active(true);
                animations.PlayCrystallJump(boostCrystallJump);
            }  else if (doubleJump || tripleJump) {
                float value = CalculateJumpForce();
                jumpForce = crystallForce / value;
                jumpIteration = 10;
                animations.PlayBonusJump(tripleJump);
                if (doubleJump) {
                    doubleJump = false;
                } else if (tripleJump) {
                    tripleJump = false;
                    doubleJump = true;
                }
                animations.UpdateJumpBonus();
                CheckJumpBonuses();
            }
        }
    }

    public void Update_Ground(GroundGet ground)
    {
        animations.colorGroundParticle = ground.color;
        animations.SetWalkSound(ground.walkAudio);
        if (rb.velocity.y < -14f && onGround) {
            ParticleSystem.MainModule main = animations.fall.main;
            var num = Mathf.Min((int)(8 * (rb.velocity.y * -1 - 14f)), 250);
            main.maxParticles = num;
            StartCoroutine(animations.PlayFall(num));
            animations.FallParticle();
        }
    }

    public void Death()
    {
        LevelManager.levelManager.AddToScore("Death");
        GetComponent<DeathEffect>().StartDeath(1f, new Color(4f / 256f, 4f / 256f, 221f / 256f));
        animations.Death();
        enabled = false;
    }

    public void StopWorking()
    {
        animations.portalGun.enabled = false;
        moveVector = new Vector2(0, 0);
        rb.velocity = new Vector2(0, 0);
        enabled = false;
    }

    private float CalculateJumpForce()
    {
        float value = 1.4f;
        if (rb.velocity.y > 1f)
            value = rb.velocity.y * 1.15f;
        else if (rb.velocity.y < -1f)
            value = 1 + (rb.velocity.y / 40f);
        return value;
    }

    public void CheckCrystallList(JumpCrystall crystall, bool removing)
    {
        if (removing)
            jumpCrystalls.Remove(crystall);
        else
            jumpCrystalls.Add(crystall);

        if (jumpCrystalls.Count != 0) {
            crystallJump = true;
            boostCrystallJump = jumpCrystalls[0].boostJump;
        } else {
            crystallJump = false;
            boostCrystallJump = false;
        }
    }

    public void AddToJumpBonusesLists(JumpBonus bonus, bool removing)
    {
        if (bonus.tripleJump) {
            if (removing)
                bonusTripleJumps.Remove(bonus);
            else
                bonusTripleJumps.Add(bonus);
        } else {
            if (removing)
                bonusDoubleJumps.Remove(bonus);
            else
                bonusDoubleJumps.Add(bonus);
        }
        CheckJumpBonuses();
    }

    private void CheckJumpBonuses()
    {
        if (bonusTripleJumps.Count > 0 && !tripleJump) {
            animations.PlayGetBonus(true);
            tripleJump = true;
            doubleJump = false;
            bonusTripleJumps[0].GetComponent<Animator>().SetBool("Death", true);
            bonusTripleJumps.RemoveAt(0);
            animations.UpdateJumpBonus();
        } else if (bonusDoubleJumps.Count > 0 && !doubleJump && !tripleJump) {
            animations.PlayGetBonus(false);
            doubleJump = true;
            bonusDoubleJumps[0].GetComponent<Animator>().SetBool("Death", true);
            bonusDoubleJumps.RemoveAt(0);
            animations.UpdateJumpBonus();
        }
    }

    private void SetControll()
    {
        walkLeftKey = Save.save.leftKey;
        walkRightKey = Save.save.rightKey;
        jumpKey = Save.save.jumpKey;
    }

    public bool GetJump(bool areTriple=false)
    {
        if (areTriple)
            return tripleJump;
        return doubleJump;
    }
}
