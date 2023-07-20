using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public AnimationPlayer animations;
    private Rigidbody2D rb;

    private bool jumping;
    public bool shoot;
    public bool inPortal;
    [SerializeField] private float realSpeed;
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

    [HideInInspector] public KeyCode jumpKey;

    [Header("Цвета порталов и их спрайты")]
    public Color leftColor;
    public Color rightColor;
    public Sprite leftPortal;
    public Sprite rightPortal;


    //private void Jump()
    //{
    //    if (Input.GetKeyUp(jumpKey)) {
    //        jumping = false;
    //    }

    //    if (Input.GetKeyDown(jumpKey)) {
    //        jumping = true;
    //        if (onGround) {
    //            animations.PlayJump();
    //            jumpForce = normalForce;
    //            jumpIteration = 60;
    //        } else if (crystallJump) {
    //            jumpForce = crystallForce;
    //            if (boostCrystallJump)
    //                jumpForce = boostCrystallForce;
    //            rb.velocity = new Vector2(rb.velocity.x, 0);
    //            jumpIteration = 60;

    //            // jumpCrystalls[0].Active(true);
    //            animations.PlayCrystallJump(boostCrystallJump);
    //        } else if (doubleJump || tripleJump) {
    //            jumpForce = 0.2780f * normalForce + 0.0975f;
    //            rb.velocity = new Vector2(rb.velocity.x, 0);
    //            jumpIteration = 60;

    //            animations.PlayBonusJump(tripleJump);
    //            if (doubleJump) {
    //                doubleJump = false;
    //            } else if (tripleJump) {
    //                tripleJump = false;
    //                doubleJump = true;
    //            }
    //            // animations.UpdateJumpBonus();
    //            // CheckJumpBonuses();
    //        }
    //    }
    //}

    //public void StopWorking()
    //{
    //    animations.portalGun.StopAllCoroutines();
    //    animations.portalGun.enabled = false;
    //    moveVector = new Vector2(0, 0);
    //    rb.velocity = new Vector2(0, 0);
    //    enabled = false;
    //}

    //public void CheckCrystallList(JumpCrystall crystall, bool removing)
    //{
    //    if (removing)
    //        jumpCrystalls.Remove(crystall);
    //    else
    //        jumpCrystalls.Add(crystall);

    //    if (jumpCrystalls.Count != 0) {
    //        crystallJump = true;
    //        boostCrystallJump = jumpCrystalls[0].boostJump;
    //    } else {
    //        crystallJump = false;
    //        boostCrystallJump = false;
    //    }
    //}

    //public void AddToJumpBonusesLists(JumpBonus bonus, bool removing)
    //{
    //    if (bonus.tripleJump) {
    //        if (removing)
    //            bonusTripleJumps.Remove(bonus);
    //        else
    //            bonusTripleJumps.Add(bonus);
    //    } else {
    //        if (removing)
    //            bonusDoubleJumps.Remove(bonus);
    //        else
    //            bonusDoubleJumps.Add(bonus);
    //    }
    //    CheckJumpBonuses();
    //}

    //private void CheckJumpBonuses()
    //{
    //    if (bonusTripleJumps.Count > 0 && !tripleJump) {
    //        animations.PlayGetBonus(true);
    //        tripleJump = true;
    //        doubleJump = false;
    //        bonusTripleJumps[0].GetComponent<Animator>().SetBool("Death", true);
    //        bonusTripleJumps.RemoveAt(0);
    //        // animations.UpdateJumpBonus();
    //    } else if (bonusDoubleJumps.Count > 0 && !doubleJump && !tripleJump) {
    //        animations.PlayGetBonus(false);
    //        doubleJump = true;
    //        bonusDoubleJumps[0].GetComponent<Animator>().SetBool("Death", true);
    //        bonusDoubleJumps.RemoveAt(0);
    //        // animations.UpdateJumpBonus();
    //    }
    //}

    //public bool GetJump(bool areTriple=false)
    //{
    //    if (areTriple)
    //        return tripleJump;
    //    return doubleJump;
    //}
}
