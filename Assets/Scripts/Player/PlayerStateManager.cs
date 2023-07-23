using UnityEngine;
using static UnityEditor.Progress;
using static UnityEditor.Recorder.OutputPath;

public class PlayerStateManager : MonoBehaviour
{
    #region States Settings
    public PlayerBaseState currentState;
    public PlayerCalmState calmState = new();
    public PlayerWalkState walkState = new();
    public PlayerJumpState jumpState = new();
    public PlayerDisabledState disabledState = new();
    public PlayerDeathState deathState = new();
    #endregion

    [HideInInspector] public AnimationPlayer animations;
    public PortalGun portalGun;
    private Transform skin;
    private Rigidbody2D rb;
    [HideInInspector] public bool right;
    [HideInInspector] public bool shoot;
    [HideInInspector] public bool inPortal;

    [SerializeField] private float realSpeed;
    private float velocityAdd;
    private bool velocityWork;
    private Vector2 moveVector;

    [Header("Смерть")]
    [SerializeField] private Material deathMaterial;

    [Header("Прыжок")]
    public float normalForce;
    [HideInInspector] public bool onGround;

    [Header("Бинды кнопок")]
    [HideInInspector] public KeyCode walkLeftKey;
    [HideInInspector] public KeyCode walkRightKey;
    [HideInInspector] public KeyCode jumpKey; 

    [Header("Смена внешнего вида при повороте")]
    public SpriteRenderer[] changingObj;
    public Sprite[] leftSprites;
    public Sprite[] rightSprites;

    [Header("Цвета порталов и их спрайты")]
    public Color leftColor;
    public Color rightColor;
    public Sprite leftPortal;
    public Sprite rightPortal;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        portalGun.player = this;

        animations = GetComponentInChildren<AnimationPlayer>();
        animations.player = this;
    }

    private void Start()
    {
        currentState = calmState;
        currentState.EnterState(this);

        skin = animations.transform;
        DialogueManager.dialogueManager.SetPlayer(this);
        deathMaterial.SetFloat("_DissolveAmount", 0);

        SetControll();
        ControllSettingsManager.OnButtonChange += SetControll;
    }

    private void Update()
    {
        currentState.UpdateState();
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState();
    }

    public void SwitchState(PlayerBaseState state)
    {
        currentState.ExitState();
        portalGun.enabled = false;

        state.EnterState(this);
        currentState = state;
    }

    public void Flip()
    {
        skin.localScale = new Vector2(skin.localScale.x * -1, skin.localScale.y);
        right = !right;
        if (right) {
            for (int i = 0; i < changingObj.Length; i++)
                changingObj[i].sprite = rightSprites[i];
        } else {
            for (int i = 0; i < changingObj.Length; i++)
                changingObj[i].sprite = leftSprites[i];
        }
    }

    private void SetControll()
    {
        walkLeftKey = Save.save.leftKey;
        walkRightKey = Save.save.rightKey;
        jumpKey = Save.save.jumpKey;
    }

    public Rigidbody2D GetRigidbody()
    {
        return rb;
    }

    public Material GetDeathMaterial()
    {
        return deathMaterial;
    }


    public void SetVelocityAdd(float newValue)
    {
        velocityAdd = newValue;
        velocityWork = true;
    }

    public void Glide()
    {
        if (velocityAdd != 0) {
            rb.velocity = new Vector2(velocityAdd * 0.4f, 1);
            velocityAdd = 0;
        }
    }

    public void Walk()
    {
        float move = 0f;
        if (Input.GetKey(walkLeftKey) && moveVector.x <= 0)
            move = -1;
        else if (Input.GetKey(walkRightKey) && moveVector.x >= 0)
            move = 1;

        moveVector.x = move * realSpeed;
        if (!velocityWork)
            rb.velocity = new Vector2(moveVector.x, rb.velocity.y);

        if (move != 0)
            velocityWork = false;

        if ((right && moveVector.x < 0 || !right && moveVector.x > 0)) {
            if (shoot)
                animations.ReverseWalk();
            else
                Flip();
        }
    }
}
