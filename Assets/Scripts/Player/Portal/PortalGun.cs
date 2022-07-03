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

    public Portal OrangePortal;
    public Portal BluePortal;

    public float ForceIteration;
    public Vector2 VectorForce;
    public float Force;

    public GameObject BlueLight;
    public GameObject OrangeLight;

    void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && !player.InPortal)
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
            SliceColliders(BluePortal);
            SliceColliders(OrangePortal);
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

        if (RightButton)
            ShootBlue.transform.rotation = Quaternion.Euler(0f, 0f, Hand.rotation.eulerAngles.z);
        else
            ShootOrange.transform.rotation = Quaternion.Euler(0f, 0f, Hand.rotation.eulerAngles.z);
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
        }
        else
        {
            if (Mathf.RoundToInt(Head.rotation.eulerAngles.z) - offset == Mathf.RoundToInt(Hand.rotation.eulerAngles.z) && offset - Mathf.RoundToInt(Head.rotation.eulerAngles.z) % 90 > 3)
                Head.rotation = Quaternion.Euler(0f, 0f, -3f);
            else if (Mathf.RoundToInt(Hand.rotation.eulerAngles.z) % 90 == Mathf.RoundToInt(Head.rotation.eulerAngles.z) % 90 && Mathf.RoundToInt(Head.rotation.eulerAngles.z) > 2)
                Head.rotation = Quaternion.Euler(0f, 0f, 2f);
        }
    }
}
