using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PortalGun : MonoBehaviour
{
    private float rotateZ;
    public float offset;
    public Player player;

    public Transform hand;
    public Transform head;
    public Transform NewPosition;
    public bool RightButton;
    public bool InWall;
    public static bool menuActive;
    public Transform ShootParent;
    public PortalShoot PrefabShoot;
    public PortalShoot ShootOrange;
    public PortalShoot ShootBlue;

    public Portal OrangePortal;
    public Portal BluePortal;

    public float ForceIteration;
    public float Force;

    private bool cooldown;

    [SerializeField] private Light2D[] gunLights;

    [Header("����� ������")]
    private KeyCode leftPortalKey;
    private KeyCode rightPortalKey;

    [Header("�������� �����")]
    private Color leftColor;
    private Color rightColor;


    [Header("�����")]
    [SerializeField] private AudioSource leftGunSound;
    [SerializeField] private AudioSource rightGunSound;

    private void Start()
    {
        SetControll();
        ControllSettingsManager.OnButtonChange += SetControll;
        leftColor = player.leftColor;
        rightColor = player.rightColor;
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
            OrangePortal.particles.Play();
            BluePortal.particles.Play();
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
                OrangePortal.particles.Stop();
                OrangePortal.Active = false;
                OrangePortal.trigger.gameObject.SetActive(false);
                SliceColliders(OrangePortal);
            }
            if (BluePortal) {
                BluePortal.particles.Stop();
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

        foreach (Light2D light2D in gunLights)
            light2D.intensity = 1.4f;
        gunLights[4].intensity = 0.86f;

        if (!RightButton) {
            leftGunSound.Play();
            foreach (Light2D light2D in gunLights)
                light2D.color = leftColor;
            if (ShootOrange) {
                ShootOrange.GetComponent<Animator>().enabled = true;
                ShootOrange = null;
            }
        } else {
            rightGunSound.Play();
            foreach (Light2D light2D in gunLights)
                light2D.color = rightColor;
            if (ShootBlue) {
                ShootBlue.GetComponent<Animator>().enabled = true;
                ShootBlue = null;
            }
        }

        if (!InWall) {
            var shoot = Instantiate(PrefabShoot, null);
            shoot.gun = this;
            shoot.right = RightButton;

            if (!RightButton) {
                shoot.SetColor(leftColor);
                shoot.isUsed = ShootBlue != null && cooldown;
                ShootOrange = shoot;
            } else {
                shoot.SetColor(rightColor);
                shoot.isUsed = ShootOrange != null && cooldown;
                ShootBlue = shoot;
            }
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
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - hand.position;
        rotateZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        float off = offset;
        hand.rotation = Quaternion.Euler(0f, 0f, rotateZ + off);

        if (player.right) {
            if (rotateZ + off + 5f < 0 || rotateZ + off - 5f > 180) {
                player.Flip();
                hand.rotation = Quaternion.Euler(0f, 0f, rotateZ + off);
            }
        } else {
            if (!((rotateZ + off >= 175f && rotateZ + off <= 275) || (rotateZ + off <= 5 && rotateZ + off >= -95))) {
                player.Flip();
                hand.rotation = Quaternion.Euler(0f, 0f, rotateZ + off);
            }
        }

        if (!InWall) {
            if (RightButton)
                ShootBlue.transform.rotation = Quaternion.Euler(0f, 0f, hand.rotation.eulerAngles.z);
            else
                ShootOrange.transform.rotation = Quaternion.Euler(0f, 0f, hand.rotation.eulerAngles.z);
        }
    }

    public void HeadRotation()
    {
        float off = offset;
        if (player.right) {
            off = -off;
        }

        if (player.right) {
            if (Mathf.RoundToInt(hand.rotation.eulerAngles.z + off) % 90 + 3 >= 3)
                head.localRotation = Quaternion.Euler(0f, 0f, 3f);
            else if (Mathf.RoundToInt(hand.rotation.eulerAngles.z + off) % 90 - 3 <= -2)
                head.localRotation = Quaternion.Euler(0f, 0f, -2f);
            else
                head.localRotation = Quaternion.Euler(0f, 0f, hand.rotation.eulerAngles.z + off);
        } else {
            if (Mathf.RoundToInt(hand.rotation.eulerAngles.z) % 180 + 3 <= 87)
                head.localRotation = Quaternion.Euler(0f, 0f, 3f);
            else if (Mathf.RoundToInt(hand.rotation.eulerAngles.z) % 180 + 3 >= 92)
                head.localRotation = Quaternion.Euler(0f, 0f, -2f);
            else
                head.localRotation = Quaternion.Euler(0f, 0f, -(hand.rotation.eulerAngles.z + off));
        }
    }

    public void Move_To_Portal(Portal exit, Portal Enter, Collider2D item)
    {
        exit.AnimatorPortal();
        exit.PlayTeleport();
        string layerEnd = "Orange";
        if (exit.GetRight() == true && exit.Collider == Enter.Collider)
            layerEnd = "Blue";
        item.GetComponent<ItemToteleport>().SetLayerEnd(layerEnd);
        item.GetComponent<ItemToteleport>().SetLayer("TeleportingItem" + layerEnd, true, exit.GetRight());
        item.GetComponent<ItemToteleport>().portal = exit;

        float x = item.bounds.extents.x;
        float y = item.bounds.extents.y;
        float yAdd = 0f;
        if (item.TryGetComponent(out Player _))
            yAdd = 0.5f;
        switch (exit.side)
        {
            case "Left": item.transform.position = new Vector2(exit.transform.position.x + x + 1f, exit.transform.position.y + yAdd); break;
            case "Down": item.transform.position = new Vector2(exit.transform.position.x, exit.transform.position.y + y + 1f); break;
            case "Right": item.transform.position = new Vector2(exit.transform.position.x - x - 1f, exit.transform.position.y + yAdd); break;
            case "Up": item.transform.position = new Vector2(exit.transform.position.x, exit.transform.position.y - y - 1f); break;
        }

        var rb = item.GetComponent<Rigidbody2D>();
        var upCoefY = 1.1f;
        if (Mathf.Abs(rb.velocity.y) >= 20f)
            upCoefY = 1.075f;
        var upCoefX = 1.1f;
        if (Mathf.Abs(rb.velocity.x) >= 20f)
            upCoefX = 1.075f;

        switch (exit.side) {
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
            player.animations.From_Portal(exit.GetRight());
        else
            item.transform.rotation = Quaternion.Euler(0f, 0f, exit.transform.rotation.eulerAngles.z);
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

    public void SetCalm()
    {
        foreach (Light2D light2D in gunLights)
            light2D.intensity = 0;
        hand.rotation = Quaternion.Euler(0f, 0f, 0f);
        hand.rotation *= Quaternion.Euler(0f, 0f, 0f);
        head.rotation = Quaternion.Euler(0f, 0f, 0f);
        head.rotation *= Quaternion.Euler(0f, 0f, 0f);
    }
}
