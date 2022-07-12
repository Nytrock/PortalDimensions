using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public AnimationPlayer Animations;
    public Rigidbody2D rb;
    public Collider2D cl;
    public float RealSpeed;
    public Transform Skin;
    public GroundTrigger trigger;
    public bool Shoot;
    public bool Right;
    public bool InPortal;
    public Vector2 moveVector;
    [Header("Прыжок")]
    public bool Jumping;
    public bool onGround;
    public int jumpIteration = 0;
    public float jumpForce;
    public float NormalForce;
    public float CrystallForce;
    public float BoostCrystallForce;
    public bool CrystallJump;
    public bool BoostCrystallJump;
    public bool DoubleJump;
    public bool TripleJump;
    public JumpCrystall jumpCrystall;
    [Header("Смена внешнего вида при повороте")]
    public List<SpriteRenderer> ChangingObj;
    public List<Sprite> LeftSprites;
    public List<Sprite> RightSprites;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cl = GetComponent<Collider2D>();
    }

    void Update()
    {
        Walk();
        Jump();
    }

    void Walk()
    {
        moveVector.x = Input.GetAxis("Horizontal") * RealSpeed;
        rb.velocity = new Vector2(moveVector.x, rb.velocity.y);
        if ((Right && moveVector.x < 0 || !Right && moveVector.x > 0))
            if (Shoot)
                Animations.animator.SetFloat("Speed", -1);
            else
                Flip();
    }

    public void Flip()
    {
        Skin.localScale = new Vector2(Skin.localScale.x * -1, Skin.localScale.y);
        Right = !Right;
        if (Right)
        {
            for (int i=0; i < ChangingObj.Count; i++)
            {
                ChangingObj[i].sprite = RightSprites[i];
            }
        } else
        {
            for (int i = 0; i < ChangingObj.Count; i++)
            {
                ChangingObj[i].sprite = LeftSprites[i];
            }
        }
    }

    void Jump()
    {
        if (Input.GetKeyUp(KeyCode.Space))
            Jumping = false;
        if (Input.GetKeyDown(KeyCode.Space))
            Jumping = true;

        if (jumpIteration > 0)
        {
            if (!onGround)
                Jumping = false;
            rb.AddForce(Vector2.up * jumpForce / jumpIteration);
            jumpIteration -= 1;
        }

        if (Jumping && (onGround || CrystallJump || DoubleJump || TripleJump))
        {
            if (onGround)
            {
                jumpForce = NormalForce;
                jumpIteration = 10;
            } else if (CrystallJump)
            {
                float Const = 1.3f;
                if (rb.velocity.y > 1f)
                    Const = rb.velocity.y * 1.1f;
                else if (rb.velocity.y < -1f)
                    Const = 1 + (rb.velocity.y / 50f);
                jumpForce = CrystallForce / Const;
                if (BoostCrystallJump)
                    jumpForce = BoostCrystallForce / Const;
                jumpIteration = 60;
                jumpCrystall.Active(true);
            }  else if (DoubleJump || TripleJump) {
                float Const = 1.3f;
                if (rb.velocity.y > 1f)
                    Const = rb.velocity.y * 1.1f;
                else if (rb.velocity.y < -1f)
                    Const = 1 + (rb.velocity.y / 50f);
                jumpForce = CrystallForce / Const;
                jumpIteration = 60;
                if (DoubleJump)
                    DoubleJump = false;
                if (TripleJump)
                {
                    TripleJump = false;
                    DoubleJump = true;
                }
                Animations.UpdateJumpBonus();
            }
        }
    }

    public void Update_Ground(GroundGet ground)
    {
        Animations.colorGroundParticle = ground.color;
        if (rb.velocity.y < -14f && onGround)
        {
            var main = Animations.Fall.main;
            main.maxParticles = (int)(20 * (rb.velocity.y * -1 - 14f) / 1.5f);
            Animations.FallParticle();
        }
    }

    public void Death()
    {
        GetComponent<DeathEffect>().StartDeath(1f, new Color(4f / 256f, 4f / 256f, 221f / 256f));
        Animations.portalGun.enabled = false;
        Animations.animator.SetBool("IsDeath", true);
        enabled = false;
    }

    public void Dialogue()
    {
        Animations.portalGun.enabled = false;
        moveVector = new Vector2(0, 0);
        rb.velocity = new Vector2(0, 0);
        this.enabled = false;
    }
}
