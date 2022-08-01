using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    public Vector2 VectorForce;
    public float Force;
    public Collider2D itemToTeleport;

    public GameObject BlueLight;
    public GameObject OrangeLight;

    [Header("Бинды кнопок")]
    private KeyCode leftPortalKey;
    private KeyCode rightPortalKey;

    private void Start()
    {
        leftPortalKey = Save.save.portalGunLeftKey;
        rightPortalKey = Save.save.portalGunRightKey;
    }
    
    private void Update()
    {
        if ((Input.GetKeyDown(leftPortalKey) || Input.GetKeyDown(rightPortalKey)) && !player.InPortal && !menuActive && !CanvasManager.isGamePaused)
        {
            Shoot();

            HandRotation();

            HeadRotation();

            player.Animations.animator.SetBool("IsShoot", true);
            transform.parent = NewPosition;
            player.Shoot = true;
            player.Animations.animator.SetFloat("Speed", 1);
        }
    }
    void Barrier(float num)
    {
        if (Math.Abs(num - rotateZ) > Math.Abs(-num - rotateZ))
            Hand.rotation = Quaternion.Euler(0f, 0f, -num + offset);
        else
            Hand.rotation = Quaternion.Euler(0f, 0f, num + offset);
    }

    public void CheckPortals(bool Right)
    {
        if (!Right)
            ShootOrange = null;
        else
            ShootBlue = null;
        if (OrangePortal && BluePortal)
        {
            OrangePortal.Particles.Play();
            BluePortal.Particles.Play();
            OrangePortal.trigger.gameObject.SetActive(true);
            BluePortal.trigger.gameObject.SetActive(true);
            if (BluePortal.Collider != OrangePortal.Collider) {
                SliceColliders(BluePortal);
                SliceColliders(OrangePortal);
            } else {
                SliceBothColliders();
            }
            BluePortal.Active = true;
            OrangePortal.Active = true;
        } else
        {
            if (OrangePortal)
            {
                OrangePortal.Particles.Stop();
                OrangePortal.Active = false;
                SliceColliders(OrangePortal);
            }
            if (BluePortal)
            {
                BluePortal.Particles.Stop();
                BluePortal.Active = false;
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
                Set_Collider(portal.Collider1, points[0], portals[1], points[0], points[3], portals[1]);
                Set_Collider(portal.Collider2, portals[0], points[1], points[0], points[3], points[1]);
                break;
            case "Down":
                Set_Collider(portal.Collider1, portals[1], points[1], points[0], points[1], points[0]);
                Set_Collider(portal.Collider2, points[2], portals[0], points[1], points[0], portals[0]);
                break;
            case "Right":
                Set_Collider(portal.Collider1, points[3], portals[0], points[3], points[0], portals[0]);
                Set_Collider(portal.Collider2, portals[1], points[2], points[0], points[3], points[1]);
                break;
            case "Up":
                Set_Collider(portal.Collider1, portals[0], points[0], points[0], points[1], points[0]);
                Set_Collider(portal.Collider2, points[3], portals[1], points[0], points[1], portals[1]);
                break;
        }
        portal.Collider1.GetComponent<GroundGet>().color = portal.Collider.GetComponent<GroundGet>().color;
        portal.Collider2.GetComponent<GroundGet>().color = portal.Collider.GetComponent<GroundGet>().color;
    }

    void Set_Collider(BoxCollider2D collider, Vector2 x1, Vector2 x2, Vector2 y1, Vector2 y2, Vector2 center)
    {
        collider.size = new Vector2(Vector2.Distance(x1, x2), Vector2.Distance(y1, y2));
        collider.transform.position = center + ((x1 - x2) - (y1 - y2)) / 2;
    }

    public void Shoot()
    {
        if (player.Animations.animator.GetBool("IsShoot") && !player.Animations.animator.GetBool("RestartShooting"))
            player.Animations.animator.SetBool("RestartShooting", true);
        RightButton = Input.GetKeyDown(rightPortalKey);
        BlueLight.SetActive(false);
        OrangeLight.SetActive(false);


        if (RightButton)
        {
            if (ShootBlue) {
                ShootBlue.GetComponent<Animator>().enabled = true;
                ShootBlue = null;
            }
            if (!InWall) {
                ShootBlue = Instantiate(PrefabShoot, null);
                ShootBlue.Blue.SetActive(true);
                ShootBlue.gun = this;
                ShootBlue.Right = RightButton;
            }
            BlueLight.SetActive(true);
        }
        else
        {
            if (ShootOrange) {
                ShootOrange.GetComponent<Animator>().enabled = true;
                ShootOrange = null;
            }
            if (!InWall) {
                ShootOrange = Instantiate(PrefabShoot, null);
                ShootOrange.Orange.SetActive(true);
                ShootOrange.gun = this;
                ShootOrange.Right = RightButton;
            }
            OrangeLight.SetActive(true);
        }

        if (ShootOrange && ShootBlue)
        {
            if (ShootOrange.Speed == ShootBlue.Speed)
            {
                if (RightButton)
                    ShootBlue.GetComponent<Animator>().enabled = true;
                else
                    ShootOrange.GetComponent<Animator>().enabled = true;
            }
        }
    }

    public void HandRotation()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Hand.position;
        rotateZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        Hand.rotation = Quaternion.Euler(0f, 0f, rotateZ + offset);

        if (player.Right)
        {
            if (-90 >= rotateZ || rotateZ >= 90)
            {
                if ((rotateZ <= -90 && rotateZ < 0) || (rotateZ >= 90 && rotateZ > 0))
                {
                    player.Flip();
                    Hand.rotation = Quaternion.Euler(0f, 0f, rotateZ + offset);
                    if (-90 <= rotateZ && rotateZ <= 90)
                        Barrier(90);
                }
                else
                    Barrier(90);
            }
        }
        else
        {
            if (-90 <= rotateZ && rotateZ <= 90)
            {
                if ((rotateZ >= -90 && rotateZ < 0) || (rotateZ <= 90 && rotateZ > 0))
                {
                    player.Flip();
                    Hand.rotation = Quaternion.Euler(0f, 0f, rotateZ + offset);
                    if (-90 >= rotateZ || rotateZ >= 90)
                        Barrier(90);
                }
                else
                    Barrier(90);

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
        if (player.Right)
            off = -off;
        Head.rotation = Quaternion.Euler(0f, 0f, Hand.rotation.eulerAngles.z + off);
        if (player.Right)
        {
            if (Mathf.RoundToInt(Head.rotation.eulerAngles.z) % 90 + offset == Mathf.RoundToInt(Hand.rotation.eulerAngles.z) && Mathf.RoundToInt(Head.rotation.eulerAngles.z) % 90 > 3)
                Head.rotation = Quaternion.Euler(0f, 0f, 3f);
            else if (Mathf.RoundToInt(Head.rotation.eulerAngles.z) % 90 == Mathf.RoundToInt(Hand.rotation.eulerAngles.z) && Mathf.RoundToInt(Head.rotation.eulerAngles.z) % 90 - offset < -2)
                Head.rotation = Quaternion.Euler(0f, 0f, -2f);
            else if (Mathf.RoundToInt(Head.rotation.eulerAngles.z) % 90 == 0)
                Head.rotation = Quaternion.Euler(0f, 0f, 3f);
        }
        else
        {
            if (Mathf.RoundToInt(Head.rotation.eulerAngles.z) - offset == Mathf.RoundToInt(Hand.rotation.eulerAngles.z) && offset - Mathf.RoundToInt(Head.rotation.eulerAngles.z) % 90 > 3)
                Head.rotation = Quaternion.Euler(0f, 0f, -3f);
            else if (Mathf.RoundToInt(Hand.rotation.eulerAngles.z) % 90 == Mathf.RoundToInt(Head.rotation.eulerAngles.z) % 90 && Mathf.RoundToInt(Head.rotation.eulerAngles.z) > 2)
                Head.rotation = Quaternion.Euler(0f, 0f, 2f);
        }
    }

    void SliceBothColliders()
    {
        var portalFirst = OrangePortal;
        var portalSecond = BluePortal;

        Vector2[] points = portalFirst.Collider.points;
        for (int i = 0; i < points.Length; i++)
            points[i] = portalFirst.Collider.transform.TransformPoint(points[i]);

        switch (OrangePortal.side) {
            case "Left":
                if (portalFirst.transform.position.y < portalSecond.transform.position.y) {
                    portalFirst = BluePortal;
                    portalSecond = OrangePortal;
                }
                break;
            case "Down":
                if (portalFirst.transform.position.x > portalSecond.transform.position.x)
                {
                    portalFirst = BluePortal;
                    portalSecond = OrangePortal;
                }
                break;
            case "Right":
                if (portalFirst.transform.position.y < portalSecond.transform.position.y) {
                    portalFirst = BluePortal;
                    portalSecond = OrangePortal;
                }
                break;
            case "Up":
                if (portalFirst.transform.position.x > portalSecond.transform.position.x)
                {
                    portalFirst = BluePortal;
                    portalSecond = OrangePortal;
                }
                break;
        }

        Vector2[] firstPortalPoints = portalFirst.GetComponent<PolygonCollider2D>().points;
        Vector2[] secondPortalPoints = portalFirst.GetComponent<PolygonCollider2D>().points;
        for (int i = 0; i < firstPortalPoints.Length; i++)
            firstPortalPoints[i] = portalFirst.transform.TransformPoint(firstPortalPoints[i]);
        for (int i = 0; i < secondPortalPoints.Length; i++)
            secondPortalPoints[i] = portalSecond.transform.TransformPoint(secondPortalPoints[i]);

        switch (OrangePortal.side)
        {
            case "Left":
                Set_Collider(portalFirst.Collider1, points[0], firstPortalPoints[1], points[0], points[3], firstPortalPoints[1]);
                Set_Collider(portalFirst.Collider2, firstPortalPoints[3], secondPortalPoints[2], points[0], points[3], secondPortalPoints[1]);
                Set_Collider(portalSecond.Collider1, firstPortalPoints[3], secondPortalPoints[2], points[0], points[3], secondPortalPoints[1]);
                Set_Collider(portalSecond.Collider2, secondPortalPoints[0], points[1], points[0], points[3], points[1]);
                break;
            case "Down":
                Set_Collider(portalFirst.Collider1, firstPortalPoints[1], points[1], points[0], points[1], points[0]);
                Set_Collider(portalFirst.Collider2, firstPortalPoints[0], secondPortalPoints[1], points[1], points[0], secondPortalPoints[1]);
                Set_Collider(portalSecond.Collider1, firstPortalPoints[0], secondPortalPoints[1], points[1], points[0], secondPortalPoints[1]);
                Set_Collider(portalSecond.Collider2, points[2], secondPortalPoints[0], points[1], points[0], secondPortalPoints[0]);
                break;
            case "Right":
                Set_Collider(portalFirst.Collider1, points[3], firstPortalPoints[0], points[3], points[0], firstPortalPoints[0]);
                Set_Collider(portalFirst.Collider2, firstPortalPoints[1], secondPortalPoints[0], points[3], points[0], secondPortalPoints[0]);
                Set_Collider(portalSecond.Collider1, firstPortalPoints[1], secondPortalPoints[0], points[3], points[0], secondPortalPoints[0]);
                Set_Collider(portalSecond.Collider2, secondPortalPoints[1], points[2], points[0], points[3], points[1]);
                break;
            case "Up":
                Set_Collider(portalFirst.Collider1, firstPortalPoints[0], points[0], points[0], points[1], points[0]);
                Set_Collider(portalFirst.Collider2, firstPortalPoints[1], secondPortalPoints[0], points[0], points[1], secondPortalPoints[0]);
                Set_Collider(portalSecond.Collider1, firstPortalPoints[1], secondPortalPoints[0], points[0], points[1], secondPortalPoints[0]);
                Set_Collider(portalSecond.Collider2, points[3], secondPortalPoints[1], points[0], points[1], secondPortalPoints[1]);
                break;
        }
    }

    public void Move_To_Portal(Portal Exit, Portal Enter, Collider2D item)
    {
        Enter.trigger.inPortal = false;
        Enter.Teleport = false;

        float x = item.bounds.extents.x;
        float y = item.bounds.extents.y;
        switch (Exit.side)
        {
            case "Left": item.transform.position = new Vector2(Exit.transform.position.x + x + 1f, Exit.transform.position.y); break;
            case "Down": item.transform.position = new Vector2(Exit.transform.position.x, Exit.transform.position.y + y + 1f); break;
            case "Right": item.transform.position = new Vector2(Exit.transform.position.x - x - 1f, Exit.transform.position.y); break;
            case "Up": item.transform.position = new Vector2(Exit.transform.position.x, Exit.transform.position.y - y - 1f); break;
        }

        if (BluePortal.Collider != OrangePortal.Collider)
            Enter.Update_Portal();

        Exit.trigger.inPortal = true;
        Exit.Teleport = false;
        if (BluePortal.Collider != OrangePortal.Collider)
            Exit.Update_Portal();
        Exit.ChangePregrads(false);
        Exit.Mask.SetActive(true);
        Exit.AnimatorPortal();


        var rb = item.GetComponent<Rigidbody2D>();
        float velX = (Mathf.Abs(rb.velocity.x) + 1f) / 10f;
        if (velX < 1.2f)
            velX = 1.2f;
        float velY = (Mathf.Abs(rb.velocity.y) + 1.4f) / 10f;
        if (velY < 1.2f)
            velY = 1.2f;
        float massCompY = rb.mass;
        if (massCompY > 1)
            massCompY *= 1.03f;
        float massCompX = rb.mass;
        if (massCompX > 1)
            massCompX *= 0.3f;
        switch (Exit.side)
        {
            case "Left": VectorForce = new Vector2(-Force * velX * massCompX * 2.5f, 0); break;
            case "Down": break;
            case "Right": VectorForce = new Vector2(Force * massCompX * velX * 2.5f, 0); break;
            case "Up":
                VectorForce = new Vector2(0, Force * massCompY * velY);
                rb.velocity = new Vector2(rb.velocity.x, 0);
                break;
        }


        if (item.TryGetComponent(out Player player))
            player.Animations.From_Portal(Exit.Blue.activeSelf);
        else
            item.transform.rotation = Quaternion.Euler(0f, 0f, Exit.transform.rotation.eulerAngles.z);
        ForceIteration = 60f;
        StartCoroutine(GiveForce());
    }

    IEnumerator GiveForce()
    {
        while (ForceIteration > 0)
        {
            itemToTeleport.GetComponent<Rigidbody2D>().AddForce(VectorForce / ForceIteration);
            ForceIteration -= 1f;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        if (!OrangePortal.trigger.inPortal && !BluePortal.trigger.inPortal)
            itemToTeleport = null;
    }
}
