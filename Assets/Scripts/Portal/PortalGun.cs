using System.Collections;
using UnityEngine;

public class PortalGun : MonoBehaviour
{
    private float rotateZ;
    public float offset;
    public Player player;

    public Transform Hand;
    public Transform Head;
    public Transform NewPosition;
    public bool RightButton;
    public bool InWall;
    public static bool menuActive;
    public Transform Shoot_parent;
    public PortalShoot PrefabShoot;
    public PortalShoot ShootOrange;
    public PortalShoot ShootBlue;

    public Portal OrangePortal;
    public Portal BluePortal;

    public float ForceIteration;
    public float Force;

    private bool cooldown;

    public GameObject BlueLight;
    public GameObject OrangeLight;

    [Header("Бинды кнопок")]
    private KeyCode leftPortalKey;
    private KeyCode rightPortalKey;

    [Header("Звуки")]
    [SerializeField] private AudioSource leftGunSound;
    [SerializeField] private AudioSource rightGunSound;

    private void Start()
    {
        SetControll();
        ControllSettingsManager.OnButtonChange += SetControll;
    }
    
    private void Update()
    {
        if ((Input.GetKeyDown(leftPortalKey) || Input.GetKeyDown(rightPortalKey)) && !player.inPortal && !menuActive && !ButtonFunctional.isGamePaused)
        {
            Shoot();

            HandRotation();

            HeadRotation();

            LevelManager.levelManager.AddToScore("Shoot");
            transform.parent = NewPosition;
            player.shoot = true;
            player.animations.StartShooting();
        }
    }

    public void CheckPortals(bool Right)
    {
        if (!Right)
            ShootOrange = null;
        else
            ShootBlue = null;
        if (OrangePortal && BluePortal) {
            OrangePortal.Particles.Play();
            BluePortal.Particles.Play();
            OrangePortal.trigger.gameObject.SetActive(true);
            BluePortal.trigger.gameObject.SetActive(true);
            OrangePortal.SetPortalLayer();
            BluePortal.SetPortalLayer();
            SliceColliders(BluePortal);
            SliceColliders(OrangePortal);
            BluePortal.Active = true;
            OrangePortal.Active = true;
        } else {
            if (OrangePortal) {
                OrangePortal.Particles.Stop();
                OrangePortal.Active = false;
                OrangePortal.trigger.gameObject.SetActive(false);
                SliceColliders(OrangePortal);
            }
            if (BluePortal) {
                BluePortal.Particles.Stop();
                BluePortal.Active = false;
                BluePortal.trigger.gameObject.SetActive(false);
                SliceColliders(BluePortal);
            }
        }
    }

    public void SliceColliders(Portal portal)
    {
        Vector2[] points = portal.Collider.points;
        Vector2[] portals = portal.GetComponent<PolygonCollider2D>().points;
        for (int i = 0; i < points.Length; i++)
            points[i] = portal.Collider.transform.TransformPoint(points[i]);
        for (int i = 0; i < portals.Length; i++)
            portals[i] = portal.transform.TransformPoint(portals[i]);
        switch (portal.side)
        {
            case "Left":
                SetColliderAndMask(portal.Collider1, points[0], portals[1], points[0], points[3], portals[1]);
                SetColliderAndMask(portal.Collider2, portals[0], points[1], points[0], points[3], points[1]);
                break;
            case "Down":
                SetColliderAndMask(portal.Collider1, portals[1], points[1], points[0], points[1], points[0]);
                SetColliderAndMask(portal.Collider2, points[2], portals[0], points[1], points[0], portals[0]);
                break;
            case "Right":
                SetColliderAndMask(portal.Collider1, points[3], portals[0], points[3], points[0], portals[0]);
                SetColliderAndMask(portal.Collider2, portals[1], points[2], points[0], points[3], points[1]);
                break;
            case "Up":
                SetColliderAndMask(portal.Collider1, portals[0], points[0], points[0], points[1], points[0]);
                SetColliderAndMask(portal.Collider2, points[3], portals[1], points[0], points[1], portals[1]);
                break;
        }
        portal.Collider1.GetComponent<GroundGet>().color = portal.Collider.GetComponent<GroundGet>().color;
        portal.Collider1.GetComponent<GroundGet>().walkAudio = portal.Collider.GetComponent<GroundGet>().walkAudio;
        portal.Collider2.GetComponent<GroundGet>().color = portal.Collider.GetComponent<GroundGet>().color;
        portal.Collider2.GetComponent<GroundGet>().walkAudio = portal.Collider.GetComponent<GroundGet>().walkAudio;
    }

    void SetColliderAndMask(BoxCollider2D collider, Vector2 x1, Vector2 x2, Vector2 y1, Vector2 y2, Vector2 center)
    {
        collider.size = new Vector2(Vector2.Distance(x1, x2), Vector2.Distance(y1, y2));
        collider.transform.position = center + ((x1 - x2) - (y1 - y2)) / 2;
    }

    public void Shoot()
    {
        RightButton = Input.GetKeyDown(rightPortalKey);
        BlueLight.SetActive(false);
        OrangeLight.SetActive(false);

        if (RightButton) {
            rightGunSound.Play();
            if (ShootBlue) {
                ShootBlue.GetComponent<Animator>().enabled = true;
                ShootBlue = null;
            }
            if (!InWall) {
                ShootBlue = Instantiate(PrefabShoot, null);
                ShootBlue.gun = this;
                ShootBlue.right = RightButton;
                ShootBlue.isUsed = ShootOrange != null && cooldown;

            }
            BlueLight.SetActive(true);
        } else {
            leftGunSound.Play();
            if (ShootOrange) {
                ShootOrange.GetComponent<Animator>().enabled = true;
                ShootOrange = null;
            }
            if (!InWall) {
                ShootOrange = Instantiate(PrefabShoot, null);
                ShootOrange.gun = this;
                ShootOrange.right = RightButton;
                ShootOrange.isUsed = ShootBlue != null && cooldown;
            }
            OrangeLight.SetActive(true);
        }

        if (ShootOrange && ShootBlue) {
            if (ShootOrange.Speed == ShootBlue.Speed) {
                if (RightButton)
                    ShootBlue.GetComponent<Animator>().enabled = true;
                else
                    ShootOrange.GetComponent<Animator>().enabled = true;
            }
        }

        cooldown = true;
        StartCoroutine(CooldownTime());
    }

    public void HandRotation()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Hand.position;
        rotateZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        float off = offset;
        Hand.rotation = Quaternion.Euler(0f, 0f, rotateZ + off);

        if (player.right) {
            if (rotateZ + off + 5f < 0 || rotateZ + off - 5f > 180) {
                player.Flip();
                Hand.rotation = Quaternion.Euler(0f, 0f, rotateZ + off);
            }
        } else {
            if (!((rotateZ + off >= 175f && rotateZ + off <= 275) || (rotateZ + off <= 5 && rotateZ + off >= -95))) {
                player.Flip();
                Hand.rotation = Quaternion.Euler(0f, 0f, rotateZ + off);
            }
        }

        if (!InWall) {
            if (RightButton)
                ShootBlue.transform.rotation = Quaternion.Euler(0f, 0f, Hand.rotation.eulerAngles.z);
            else
                ShootOrange.transform.rotation = Quaternion.Euler(0f, 0f, Hand.rotation.eulerAngles.z);
        }
    }

    public void HeadRotation()
    {
        float off = offset;
        if (player.right) {
            off = -off;
        }

        if (player.right) {
            if (Mathf.RoundToInt(Hand.rotation.eulerAngles.z + off) % 90 + 3 >= 3)
                Head.localRotation = Quaternion.Euler(0f, 0f, 3f);
            else if (Mathf.RoundToInt(Hand.rotation.eulerAngles.z + off) % 90 - 3 <= -2)
                Head.localRotation = Quaternion.Euler(0f, 0f, -2f);
            else
                Head.localRotation = Quaternion.Euler(0f, 0f, Hand.rotation.eulerAngles.z + off);
        } else {
            if (Mathf.RoundToInt(Hand.rotation.eulerAngles.z) % 180 + 3 <= 87)
                Head.localRotation = Quaternion.Euler(0f, 0f, 3f);
            else if (Mathf.RoundToInt(Hand.rotation.eulerAngles.z) % 180 + 3 >= 92)
                Head.localRotation = Quaternion.Euler(0f, 0f, -2f);
            else
                Head.localRotation = Quaternion.Euler(0f, 0f, -(Hand.rotation.eulerAngles.z + off));
        }
    }

    public void Move_To_Portal(Portal Exit, Portal Enter, Collider2D item)
    {
        Exit.AnimatorPortal();
        Exit.PlayTeleport();
        if (Exit.Collider == Enter.Collider) {
            string layerEnd = "Orange";
            if (Exit.Blue.activeSelf == true)
                layerEnd = "Blue";
            item.GetComponent<ItemToteleport>().SetLayerEnd(layerEnd);
            item.GetComponent<ItemToteleport>().SetLayer("TeleportingItem" + layerEnd);
        }
        item.GetComponent<ItemToteleport>().portal = Exit;

        float x = item.bounds.extents.x;
        float y = item.bounds.extents.y;
        float yAdd = 0f;
        if (item.TryGetComponent(out Player _))
            yAdd = 0.5f;
        switch (Exit.side)
        {
            case "Left": item.transform.position = new Vector2(Exit.transform.position.x + x + 1f, Exit.transform.position.y + yAdd); break;
            case "Down": item.transform.position = new Vector2(Exit.transform.position.x, Exit.transform.position.y + y + 1f); break;
            case "Right": item.transform.position = new Vector2(Exit.transform.position.x - x - 1f, Exit.transform.position.y + yAdd); break;
            case "Up": item.transform.position = new Vector2(Exit.transform.position.x, Exit.transform.position.y - y - 1f); break;
        }

        var rb = item.GetComponent<Rigidbody2D>();
        var upCoefY = 1.1f;
        if (Mathf.Abs(rb.velocity.y) >= 20f)
            upCoefY = 1.075f;
        var upCoefX = 1.1f;
        if (Mathf.Abs(rb.velocity.x) >= 20f)
            upCoefX = 1.075f;

        switch (Exit.side) {
            case "Left":
                float force = -Mathf.Max(7f, Mathf.Abs(rb.velocity.y) * upCoefY, Mathf.Abs(rb.velocity.x) * upCoefX);
                if (item.TryGetComponent(out Player charachter))
                    charachter.SetVelocityAdd(force);
                else
                    rb.velocity = new Vector2(force, 0);
                break;
            case "Down":
                var downCoef = 0.7f;
                if (Mathf.Max(Mathf.Abs(rb.velocity.x) * downCoef, Mathf.Abs(rb.velocity.y) * downCoef) >= 17f)
                    downCoef = 0.9f;
                if (Mathf.Max(Mathf.Abs(rb.velocity.x) * downCoef, Mathf.Abs(rb.velocity.y) * downCoef) >= 35f)
                    downCoef = 1f;
                rb.velocity = new Vector2(0, -Mathf.Max(Mathf.Abs(rb.velocity.x) * downCoef, Mathf.Abs(rb.velocity.y) * downCoef));
                if (rb.velocity.y < -180f)
                    rb.velocity = new Vector2(rb.velocity.x, -180f);
                else if (rb.velocity.y > -5f)
                    rb.velocity = new Vector2(rb.velocity.x, -5f);
                break;
            case "Right":
                force = Mathf.Max(7f, Mathf.Abs(rb.velocity.y) * upCoefY, Mathf.Abs(rb.velocity.x) * upCoefX);
                if (item.TryGetComponent(out Player charachter1))
                    charachter1.SetVelocityAdd(force);
                else
                    rb.velocity = new Vector2(force, 0);
                break;
            case "Up":
                rb.velocity = new Vector2(0, Mathf.Max(12f, Mathf.Abs(rb.velocity.y) * upCoefY, Mathf.Abs(rb.velocity.x) * upCoefX));
                break;
        }

        if (item.TryGetComponent(out Player player))
            player.animations.From_Portal(Exit.Blue.activeSelf);
        else
            item.transform.rotation = Quaternion.Euler(0f, 0f, Exit.transform.rotation.eulerAngles.z);
        LevelManager.levelManager.AddToScore("Teleport");
    }

    IEnumerator CooldownTime()
    {
        yield return new WaitForSeconds(0.5f);
        cooldown = false;
    }

    private void SetControll() 
    {
        leftPortalKey = Save.save.portalGunLeftKey;
        rightPortalKey = Save.save.portalGunRightKey;
    }

    public void SetLevelSettings(bool working)
    {
        gameObject.SetActive(working);
    }
}
