using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PortalShoot : MonoBehaviour
{
    public float Speed;
    public float boost;
    public GameObject Blue;
    public GameObject Orange;
    public Portal portalPrefab;
    public bool Right;

    public PortalGun gun;

    public Color FirstColor;
    public Color SecondColor;
    public float destroyTime;

    void Start()
    {
        StartCoroutine(AddSpeed());
        Invoke("DestroyAmmo", destroyTime);
    }

    public void Update()
    {
        transform.Translate(new Vector2(0, -1) * Speed * Time.deltaTime);
    }

    void DestroyAmmo()
    {
        GetComponent<Animator>().enabled = true;
    }

    IEnumerator AddSpeed()
    {
        while (true)
        {
            Speed += boost;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out PolygonCollider2D polygon) && obj.tag != "Player")
        {
            if (obj.tag == "ForPortal")
                SpawnPortal(polygon);
            GetComponent<Animator>().enabled = true;
        }
        else if (obj.TryGetComponent(out Portal portal))
        {
            Debug.Log(portal);
            SpawnPortal(portal.Collider);
            GetComponent<Animator>().enabled = true;
        }
        else if(obj.TryGetComponent(out PortalCollider portalCollider))
        {
            SpawnPortal(portalCollider.portal.Collider);
            GetComponent<Animator>().enabled = true;
        }
    }

    public void Destroy_Shoot()
    {
        Destroy(gameObject);
    }

    public void SpawnPortal(PolygonCollider2D other)
    {
        var portal = Instantiate(portalPrefab, null);
        if (Right)
            portal.Blue.SetActive(true);
        else
            portal.Orange.SetActive(true);
        portal.gun = gun;

        Vector2[] points = other.points;
        Array.Resize(ref points, points.Length + 1);
        points[points.Length - 1] = points[0];
        for (int i = 0; i < points.Length; i++)
            points[i] = other.transform.TransformPoint(points[i]);

        bool Flag = true;
        for (int i=0; i < points.Length - 1; i++)
        {
            if (Flag)
            {
                foreach (RaycastHit2D item in Physics2D.LinecastAll(points[i], points[i + 1]))
                {
                    if (item.collider.tag == "Shoot")
                    {
                        if (Right)
                        {
                            if (gun.Blue != null)
                                Destroy(gun.Blue.gameObject);
                            gun.Blue = portal;
                        }
                        else
                        {
                            if (gun.Orange != null)
                                Destroy(gun.Orange.gameObject);
                            gun.Orange = portal;
                        }

                        portal.transform.position = new Vector2(item.point.x, item.point.y);
                        float rotat = other.transform.rotation.eulerAngles.z;
                        switch (i)
                        {
                            case 0: rotat += -90f; break;
                            case 2: rotat += 90f; break;
                            case 3: rotat += 180f; break;
                        }
                        portal.transform.rotation = Quaternion.Euler(0f, 0f, rotat);

                        float Scale = 1f;
                        if (0 >= rotat && rotat >= -90 || 0 <= rotat && rotat <= 90)
                            Scale = Math.Abs(rotat / 180f) + .5f;
                        else if (180 >= rotat && rotat >= 90 || -90 >= rotat && rotat >= -180)
                            Scale = Math.Abs(180f / rotat) / 2f;
                        portal.transform.localScale = new Vector2(portal.transform.localScale.x * Scale, portal.transform.localScale.y);
                        portal.Collider1.transform.localScale = new Vector2(1f / Scale, 1f);
                        portal.Collider2.transform.localScale = new Vector2(1f / Scale, 1f);

                        portal.Collider = other;
                        Flag = false;
                        if (gun.Orange && gun.Blue)
                        {
                            if (gun.Orange.Collider == gun.Blue.Collider)
                            {
                                Vector2[] Points1 = portal.Blue.GetComponent<PolygonCollider2D>().points;
                                Vector2[] Points2 = gun.Blue.Blue.GetComponent<PolygonCollider2D>().points;
                                if (Right)
                                    Points2 = gun.Orange.Blue.GetComponent<PolygonCollider2D>().points;
                                for (int j = 0; j < Points1.Length; j++)
                                    Points1[j] = portal.Blue.transform.TransformPoint(Points1[j]);
                                for (int j = 0; j < Points2.Length; j++) {
                                    if (Right)
                                        Points2[j] = gun.Orange.Blue.transform.TransformPoint(Points2[j]);
                                    else
                                        Points2[j] = gun.Blue.Blue.transform.TransformPoint(Points2[j]);
                                }
                                var center1 = portal.transform.position;

                                switch (i)
                                {
                                    case 0:
                                        if (Vertical_Check(Points1, Points2[1], Points2[0]))
                                            Vertical_Portals_Alig(center1, Points1, Points2, portal, 0, 1);
                                        break;
                                    case 1:
                                        if (Horizontal_Check(Points1, Points2[0], Points2[1]))
                                            Horizontal_Portals_Alig(center1, Points1, Points2, portal, 0, 1);
                                        break;
                                    case 2:
                                        if (Vertical_Check(Points1, Points2[0], Points2[1]))
                                            Vertical_Portals_Alig(center1, Points1, Points2, portal, 1, 0);
                                        break;
                                    case 3:
                                        if (Horizontal_Check(Points1, Points2[1], Points2[0]))
                                            Horizontal_Portals_Alig(center1, Points1, Points2, portal, 1, 0);
                                        break;
                                }
                            }
                        }
                        Vector2[] portals = portal.Blue.GetComponent<PolygonCollider2D>().points;
                        for (int j = 0; j < portals.Length; j++)
                            portals[j] = portal.Blue.transform.TransformPoint(portals[j]);
                        switch (i)
                        {
                            case 0:
                                Vertical_Alignment(portals[0], portals[1], points[i], points[i + 1], portal);
                                break;
                            case 1:
                                Horizontal_Alignment(portals[1], portals[0], points[i + 1], points[i], portal);
                                break;
                            case 2:
                                Vertical_Alignment(portals[1], portals[0], points[i + 1], points[i], portal);
                                break;
                            case 3:
                                Horizontal_Alignment(portals[0], portals[1], points[i], points[i + 1], portal);
                                break;
                        }
                        if (gun.Orange && gun.Blue)
                        {
                            if (gun.Orange.Collider == gun.Blue.Collider)
                            {
                                Vector2[] Points1 = portal.Blue.GetComponent<PolygonCollider2D>().points;
                                Vector2[] Points2 = gun.Blue.Blue.GetComponent<PolygonCollider2D>().points;
                                if (Right)
                                    Points2 = gun.Orange.Blue.GetComponent<PolygonCollider2D>().points;
                                for (int j = 0; j < Points1.Length; j++)
                                    Points1[j] = portal.Blue.transform.TransformPoint(Points1[j]);
                                for (int j = 0; j < Points2.Length; j++)
                                {
                                    if (Right)
                                        Points2[j] = gun.Orange.Blue.transform.TransformPoint(Points2[j]);
                                    else
                                        Points2[j] = gun.Blue.Blue.transform.TransformPoint(Points2[j]);
                                }
                                var center1 = portal.transform.position;

                                switch (i)
                                {
                                    case 0:
                                        if (Vertical_Check(Points1, Points2[1], Points2[0]))
                                            Destroy_Portal(portal);
                                        break;
                                    case 1:
                                        if (Horizontal_Check(Points1, Points2[0], Points2[1]))
                                            Destroy_Portal(portal);
                                        break;
                                    case 2:
                                        if (Vertical_Check(Points1, Points2[0], Points2[1]))
                                            Destroy_Portal(portal);
                                        break;
                                    case 3:
                                        if (Horizontal_Check(Points1, Points2[1], Points2[0]))
                                            Destroy_Portal(portal);
                                        break;
                                }
                            }
                        }
                        float Distance1 = Vector2.Distance(points[i], points[i + 1]);
                        float Distance2 = Vector2.Distance(portals[0], portals[1]);
                        if (Distance1 <= Distance2)
                            Destroy_Portal(portal);
                        portal.side = i;
                        break;
                    }
                }
            } else
            {
                break;
            }
        }
        portal.transform.parent = other.gameObject.transform;
        var main = portal.Particles.main;
        if (Right)
            main.startColor = SecondColor;
        else
            main.startColor = FirstColor;
        if (Flag)
        {
            Destroy_Portal(portal);
            if (!gun.InWall)
                Instantiate(this, null);
        }
        gun.CheckPortals(Right);
    }

    void Horizontal_Alignment(Vector2 portal0, Vector2 portal1, Vector2 pointI, Vector2 pointII, Portal portal)
    {
        if (portal1.x > pointI.x && portal0.x < pointII.x)
            Destroy_Portal(portal);
        else if (portal1.x > pointI.x)
            portal.transform.position = new Vector2(portal.transform.position.x - (portal1.x - pointI.x), portal.transform.position.y);
        else if (portal0.x < pointII.x)
            portal.transform.position = new Vector2(portal.transform.position.x + (pointII.x - portal0.x), portal.transform.position.y);
    }

    void Vertical_Alignment(Vector2 portal0, Vector2 portal1, Vector2 pointI, Vector2 pointII, Portal portal)
    {
        if (portal1.y > pointI.y && portal0.y < pointII.y)
            Destroy_Portal(portal);
        else if (portal1.y > pointI.y)
            portal.transform.position = new Vector2(portal.transform.position.x, portal.transform.position.y - (portal1.y - pointI.y));
        else if (portal0.y < pointII.y)
            portal.transform.position = new Vector2(portal.transform.position.x, portal.transform.position.y + (pointII.y - portal0.y));
    }

    bool Horizontal_Check(Vector2[] center, Vector2 side1, Vector2 side2)
    {
        return center[0].x > side2.x && center[0].x < side1.x || center[1].x > side2.x && center[1].x < side1.x ||
                                            center[0].x == side1.x && center[1].x == side2.x;
    }

    bool Vertical_Check(Vector2[] center, Vector2 side1, Vector2 side2)
    {
        return center[0].y > side2.y && center[0].y < side1.y || center[1].y > side2.y && center[1].y < side1.y ||
                                            center[0].y == side1.y && center[1].y == side2.y;
    }

    void Vertical_Portals_Alig(Vector2 center1, Vector2[] Points1, Vector2[] Points2, Portal portal, int num1, int num2)
    {
        if (center1.y >= Points2[num1].y && center1.y <= Points2[num2].y)
            Destroy_Portal(portal);
        else if (center1.y < Points2[num1].y)
            portal.transform.position = new Vector2(center1.x - (Points1[num2].x - Points2[num1].x) * 1.1f, center1.y - (Points1[num2].y - Points2[num1].y) * 1.1f);
        else if (center1.y > Points2[num2].y)
            portal.transform.position = new Vector2(center1.x + (Points2[num2].x - Points1[num1].x) * 1.1f, center1.y + (Points2[num2].y - Points1[num1].y) * 1.1f);
    }

    void Horizontal_Portals_Alig(Vector2 center1, Vector2[] Points1, Vector2[] Points2, Portal portal, int num1, int num2)
    {
        if (center1.x >= Points2[num2].x && center1.x <= Points2[num1].x)
            Destroy_Portal(portal);
        else if (center1.x < Points2[num2].x)
            portal.transform.position = new Vector2(center1.x - (Points1[num1].x - Points2[num2].x) * 1.1f, center1.y + (Points2[num2].y - Points1[num1].y) * 1.1f);
        else if (center1.x > Points2[num1].x)
            portal.transform.position = new Vector2(center1.x + (Points2[num1].x - Points1[num2].x) * 1.1f, center1.y - (Points1[num2].y - Points2[num1].y) * 1.1f);
    }

    void Destroy_Portal(Portal portal)
    {
        Destroy(portal.gameObject);
        if (Right)
            gun.Blue = null;
        else
            gun.Orange = null;
    }
}
