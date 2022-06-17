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
    public Transform Shoot_parent;
    public PortalShoot PrefabShoot;
    public PortalShoot ShootOrange;
    public PortalShoot ShootBlue;

    public Portal Orange;
    public Portal Blue;

    public float ForceIteration;
    public Vector2 VectorForce;
    public float Force;

    public GameObject BlueLight;
    public GameObject OrangeLight;

    void Update()
    {

        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && !player.InPortal)
        {
            if (player.Animations.animator.GetBool("IsShoot") && !player.Animations.animator.GetBool("RestartShooting"))
                player.Animations.animator.SetBool("RestartShooting", true);

            RightButton = Input.GetMouseButtonDown(1);
            BlueLight.SetActive(false);
            OrangeLight.SetActive(false);


            if (RightButton)
            {
                if (ShootBlue)
                    ShootBlue.GetComponent<Animator>().enabled = true;
                ShootBlue = Instantiate(PrefabShoot, null);
                ShootBlue.Blue.SetActive(true);
                ShootBlue.gun = this;
                ShootBlue.Right = RightButton;
                BlueLight.SetActive(true);
            }
            else
            {
                if (ShootOrange)
                    ShootOrange.GetComponent<Animator>().enabled = true;
                ShootOrange = Instantiate(PrefabShoot, null);
                ShootOrange.Orange.SetActive(true);
                ShootOrange.gun = this;
                ShootOrange.Right = RightButton;
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

            player.Animations.animator.SetBool("IsShoot", true);
            transform.parent = NewPosition;

            Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Hand.position;
            rotateZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            Hand.rotation = Quaternion.Euler(0f, 0f, rotateZ + offset);

            if (player.Right)
            {
                if (-70 >= rotateZ || rotateZ >= 70)
                {
                    if ((rotateZ <= -90 && rotateZ < 0) || (rotateZ >= 90 && rotateZ > 0))
                    {
                        player.Flip();
                        Hand.rotation = Quaternion.Euler(0f, 0f, rotateZ + offset);
                        if (-110 <= rotateZ && rotateZ <= 110)
                            Barrier(110);
                    }
                    else
                        Barrier(70);
                }
            }
            else
            {
                if (-110 <= rotateZ && rotateZ <= 110)
                {
                    if ((rotateZ >= -90 && rotateZ < 0) || (rotateZ <= 90 && rotateZ > 0))
                    {
                        player.Flip();
                        Hand.rotation = Quaternion.Euler(0f, 0f, rotateZ + offset);
                        if (-70 >= rotateZ || rotateZ >= 70)
                            Barrier(70);
                    } 
                    else
                        Barrier(110);

                }
            }
            if (RightButton)
                ShootBlue.transform.rotation = Quaternion.Euler(0f, 0f, Hand.rotation.eulerAngles.z);
            else
                ShootOrange.transform.rotation = Quaternion.Euler(0f, 0f, Hand.rotation.eulerAngles.z);
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
            }
            else
            {
                if (Mathf.RoundToInt(Head.rotation.eulerAngles.z) - offset == Mathf.RoundToInt(Hand.rotation.eulerAngles.z) && offset - Mathf.RoundToInt(Head.rotation.eulerAngles.z) % 90 > 3)
                    Head.rotation = Quaternion.Euler(0f, 0f, -3f);
                else if (Mathf.RoundToInt(Hand.rotation.eulerAngles.z) % 90 == Mathf.RoundToInt(Head.rotation.eulerAngles.z) % 90 && Mathf.RoundToInt(Head.rotation.eulerAngles.z) > 2)
                    Head.rotation = Quaternion.Euler(0f, 0f, 2f);
            }

            player.Shoot = true;
            player.Animations.animator.SetFloat("Speed", 1);
        }

        GiveForce();
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
        if (Orange && Blue)
        {
            Orange.Particles.Play();
            Blue.Particles.Play();
            Orange.trigger.gameObject.SetActive(true);
            Blue.trigger.gameObject.SetActive(true);
            SliceColliders(Blue);
            SliceColliders(Orange);
            Blue.Active = true;
            Orange.Active = true;
        } else
        {
            if (Orange)
            {
                Orange.Particles.Stop();
                Orange.Active = false;
                SliceColliders(Orange);
            }
            if (Blue)
            {
                Blue.Particles.Stop();
                Blue.Active = false;
                SliceColliders(Blue);
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
            case 0:
                Set_Collider(portal.Collider1, points[0], portals[1], points[0], points[3], portals[1]);
                Set_Collider(portal.Collider2, portals[0], points[1], points[0], points[3], points[1]);
                break;
            case 1:
                Set_Collider(portal.Collider1, portals[1], points[1], points[0], points[1], points[0]);
                Set_Collider(portal.Collider2, points[2], portals[0], points[1], points[0], portals[0]);
                break;
            case 2:
                Set_Collider(portal.Collider1, points[3], portals[0], points[3], points[0], portals[0]);
                Set_Collider(portal.Collider2, portals[1], points[2], points[0], points[3], points[1]);
                break;
            case 3:
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

    void GiveForce()
    {
        if (ForceIteration > 0)
        {
            player.rb.AddForce(VectorForce / ForceIteration);
            ForceIteration -= 1f;
        }
    }

    public void Move_To_Portal(Portal Exit, Portal Enter)
    {
        Enter.trigger.inPortal = false;
        Enter.Teleport = false;
        Enter.Update_Portal();

        float x = player.cl.bounds.extents.x;
        float y = player.cl.bounds.extents.y;
        switch (Exit.side)
        {
            case 0: player.transform.position = new Vector2(Exit.transform.position.x + x, Exit.transform.position.y); break;
            case 1: player.transform.position = new Vector2(Exit.transform.position.x, Exit.transform.position.y + y); break;
            case 2: player.transform.position = new Vector2(Exit.transform.position.x - x, Exit.transform.position.y); break;
            case 3: player.transform.position = new Vector2(Exit.transform.position.x, Exit.transform.position.y - y); break;
        }
        

        Exit.trigger.inPortal = true;
        Exit.Teleport = false;
        Exit.Update_Portal();
        Exit.ChangePregrads(false);
        Exit.Mask.SetActive(true);
        Exit.AnimatorPortal();
        float velX = (Mathf.Abs(player.rb.velocity.x) + 1f) / 10f;
        if (velX < 1.2f)
            velX = 1.2f;
        float velY = (Mathf.Abs(player.rb.velocity.y) + 1f) / 10f;
        if (velY < 1.2f)
            velY = 1.2f;
        player.rb.velocity = Vector2.zero;
        switch (Exit.side)
        {
            case 0: VectorForce = new Vector2(-Force * velX, 0); break;
            case 1: VectorForce = new Vector2(0, -Force * velY); break;
            case 2: VectorForce = new Vector2(Force * velX * 3f, 0); break;
            case 3: VectorForce = new Vector2(0, Force * velY); break;
        }
        ForceIteration = 60f;
        player.Animations.From_Portal(Exit.Blue.activeSelf);
    }
}
