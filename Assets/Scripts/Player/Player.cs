using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    public AnimationPlayer animations;
    private Rigidbody2D rb;
    public float RealSpeed;
    private Transform skin;
    public bool Shoot;
    public bool Right;
    public bool InPortal;
    public Vector2 moveVector;

    private bool Jumping;
    [Header("Прыжок")]
    public bool onGround;
    private int jumpIteration = 0;
    private float jumpForce;
    public float NormalForce;
    public float CrystallForce;
    public float BoostCrystallForce;
    private bool CrystallJump;
    private bool BoostCrystallJump;
    public bool DoubleJump;
    public bool TripleJump;
    private List<JumpCrystall> jumpCrystalls = new List<JumpCrystall>();
    private List<JumpBonus> bonusDoubleJumps = new List<JumpBonus>();
    private List<JumpBonus> bonusTripleJumps = new List<JumpBonus>();
    [Header("Смена внешнего вида при повороте")]
    public List<SpriteRenderer> ChangingObj;
    public List<Sprite> LeftSprites;
    public List<Sprite> RightSprites;
    [Header("Бинды кнопок")]
    private KeyCode walkLeftKey;
    private KeyCode walkRightKey;
    private KeyCode jumpKey;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animations = GetComponentInChildren<AnimationPlayer>();
        skin = animations.transform;

        SetControll();
        ControllSettingsManager.OnButtonChange += SetControll;
    }

    void Update()
    {
        if (!ButtonFunctional.isGamePaused) {
            Walk();
            Jump();
        }
    }

    void Walk()
    {
        var move = 0f;
        if (Input.GetKey(walkLeftKey) && moveVector.x <= 0)
            move = -1;
        else if (Input.GetKey(walkRightKey) && moveVector.x >= 0)
            move = 1;
        moveVector.x = move * RealSpeed;
        rb.velocity = new Vector2(moveVector.x, rb.velocity.y);
        if ((Right && moveVector.x < 0 || !Right && moveVector.x > 0)) {
            if (Shoot)
                animations.ReverseWalk();
            else
                Flip();
        }
    }

    public void Flip()
    {
        skin.localScale = new Vector2(skin.localScale.x * -1, skin.localScale.y);
        Right = !Right;
        if (Right){
            for (int i=0; i < ChangingObj.Count; i++)
                ChangingObj[i].sprite = RightSprites[i];
        } else {
            for (int i = 0; i < ChangingObj.Count; i++)
                ChangingObj[i].sprite = LeftSprites[i];
        }
    }

    void Jump()
    {
        if (Input.GetKeyUp(jumpKey))
            Jumping = false;
        if (Input.GetKeyDown(jumpKey))
            Jumping = true;

        if (jumpIteration > 0) {
            if (!onGround)
                Jumping = false;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpIteration -= 1;
        }

        if (Jumping && (onGround || CrystallJump || DoubleJump || TripleJump)) {
            if (onGround) {
                jumpForce = NormalForce;
                jumpIteration = 10;
                rb.AddForce(Vector2.up * jumpForce * 1.2f, ForceMode2D.Impulse);
            } else if (CrystallJump) {
                float value = CalculateJumpForce();
                jumpForce = CrystallForce / value;
                if (BoostCrystallJump)
                    jumpForce = BoostCrystallForce / value;
                jumpIteration = 10;
                jumpCrystalls[0].Active(true);
            }  else if (DoubleJump || TripleJump) {
                float value = CalculateJumpForce();
                jumpForce = CrystallForce / value;
                jumpIteration = 10;
                if (DoubleJump) {
                    DoubleJump = false;
                } else if (TripleJump) {
                    TripleJump = false;
                    DoubleJump = true;
                }
                animations.UpdateJumpBonus();
                CheckJumpBonuses();
            }
        }
    }

    public void Update_Ground(GroundGet ground)
    {
        animations.colorGroundParticle = ground.color;
        if (rb.velocity.y < -14f && onGround) {
            var main = animations.Fall.main;
            main.maxParticles = (int)(20 * (rb.velocity.y * -1 - 14f) / 1.5f);
            animations.FallParticle();
        }
    }

    public void Death()
    {
        GetComponent<DeathEffect>().StartDeath(1f, new Color(4f / 256f, 4f / 256f, 221f / 256f));
        animations.Death();
        enabled = false;
    }

    public void Dialogue()
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
            CrystallJump = true;
            BoostCrystallJump = jumpCrystalls[0].BoostJump;
        } else {
            CrystallJump = false;
            BoostCrystallJump = false;
        }
    }

    public void AddToJumpBonusesLists(JumpBonus bonus, bool removing)
    {
        if (bonus.TripleJump) {
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
        if (bonusTripleJumps.Count > 0 && !TripleJump) {
            TripleJump = true;
            DoubleJump = false;
            bonusTripleJumps[0].GetComponent<Animator>().SetBool("Death", true);
            bonusTripleJumps.RemoveAt(0);
            animations.UpdateJumpBonus();
        } else if (bonusDoubleJumps.Count > 0 && !DoubleJump && !TripleJump) {
            DoubleJump = true;
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
}
