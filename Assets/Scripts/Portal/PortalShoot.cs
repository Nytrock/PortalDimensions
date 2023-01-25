using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PortalShoot : MonoBehaviour
{
    public float Speed;
    public float boost;
    public Light2D shootLight;
    public ParticleSystem shootParticle;
    public Portal portalPrefab;
    public bool right;

    public PortalGun gun;

    private Color mainColor;
    public float destroyTime;

    public bool isUsed = false;

    private void Start()
    {
        StartCoroutine(AddSpeed());
        Invoke(nameof(DestroyAmmo), destroyTime);

        var main = shootParticle.main;
        shootLight.color = mainColor;
        main.startColor = mainColor;
    }

    public void Update()
    {
        transform.Translate(Speed * Time.deltaTime * new Vector2(0, -1));
    }

    private void DestroyAmmo()
    {
        GetComponent<Animator>().enabled = true;
    }

    private IEnumerator AddSpeed()
    {
        while (true) {
            Speed += boost;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D obj)
    {
        var animator = GetComponent<Animator>();
        if (gun.RightButton == right && gun.InWall) {
            isUsed = true;
            animator.enabled = true;
        } else if (!isUsed && (obj.CompareTag("ForPortal"))) {
            if (obj.TryGetComponent(out PortalCollider portalCollider)) {
                SpawnPortal(portalCollider.portal.Collider);
                isUsed = true;
                animator.enabled = true;
            } else if (obj.TryGetComponent(out PolygonCollider2D polygon)) {
                SpawnPortal(polygon);
                isUsed = true;
                animator.enabled = true;
            }
        } else if ((isUsed && !(obj.CompareTag("Player"))) || (!isUsed && obj.CompareTag("NotForPortal"))) {
            animator.enabled = true;
        }
    }

    public void Destroy_Shoot()
    {
        Destroy(gameObject);
    }

    public void SpawnPortal(PolygonCollider2D other)
    {
        Portal portal = Instantiate(portalPrefab, null);
        portal.SetPortal(right, gun);

        Vector2[] points = other.points;
        Array.Resize(ref points, points.Length + 1);
        points[^1] = points[0];
        for (int i = 0; i < points.Length; i++)
            points[i] = other.transform.TransformPoint(points[i]);

        FindSideAndAlign(portal, points, other);

        portal.transform.parent = other.gameObject.transform;
        ParticleSystem.MainModule main = portal.Particles.main;
        main.startColor = mainColor;
        gun.CheckPortals(right);
    }

    private void Horizontal_Alignment(Vector2 portal0, Vector2 portal1, Vector2 pointI, Vector2 pointII, Portal portal)
    {
        if (portal1.x > pointI.x && portal0.x < pointII.x)
            Destroy_Portal(portal);
        else if (portal1.x > pointI.x)
            portal.transform.position = new Vector2(portal.transform.position.x - (portal1.x - pointI.x), portal.transform.position.y);
        else if (portal0.x < pointII.x)
            portal.transform.position = new Vector2(portal.transform.position.x + (pointII.x - portal0.x), portal.transform.position.y);
    }

    private void Vertical_Alignment(Vector2 portal0, Vector2 portal1, Vector2 pointI, Vector2 pointII, Portal portal)
    {
        if (portal1.y > pointI.y && portal0.y < pointII.y)
            Destroy_Portal(portal);
        else if (portal1.y > pointI.y)
            portal.transform.position = new Vector2(portal.transform.position.x, portal.transform.position.y - (portal1.y - pointI.y));
        else if (portal0.y < pointII.y)
            portal.transform.position = new Vector2(portal.transform.position.x, portal.transform.position.y + (pointII.y - portal0.y));
    }

    private bool Horizontal_Check(Vector2[] center, Vector2 side1, Vector2 side2)
    {
        return center[0].x > side2.x && center[0].x < side1.x || center[1].x > side2.x && center[1].x < side1.x ||
                                            center[0].x == side1.x && center[1].x == side2.x;
    }

    private bool Vertical_Check(Vector2[] center, Vector2 side1, Vector2 side2)
    {
        return center[0].y > side2.y && center[0].y < side1.y || center[1].y > side2.y && center[1].y < side1.y ||
                                            center[0].y == side1.y && center[1].y == side2.y;
    }

    private void VerticalPortalsAligment(Vector2 center1, Vector2[] Points1, Vector2[] Points2, Portal portal, int num1, int num2)
    {
        if (center1.y >= Points2[num1].y && center1.y <= Points2[num2].y)
            Destroy_Portal(portal);
        else if (center1.y < Points2[num1].y)
            portal.transform.position = new Vector2(center1.x - (Points1[num2].x - Points2[num1].x) * 1.1f, center1.y - (Points1[num2].y - Points2[num1].y) * 1.1f);
        else if (center1.y > Points2[num2].y)
            portal.transform.position = new Vector2(center1.x + (Points2[num2].x - Points1[num1].x) * 1.1f, center1.y + (Points2[num2].y - Points1[num1].y) * 1.1f);
    }

    private void HorizontalPortalsAligment(Vector2 center1, Vector2[] Points1, Vector2[] Points2, Portal portal, int num1, int num2)
    {
        if (center1.x >= Points2[num2].x && center1.x <= Points2[num1].x)
            Destroy_Portal(portal);
        else if (center1.x < Points2[num2].x)
            portal.transform.position = new Vector2(center1.x - (Points1[num1].x - Points2[num2].x) * 1.1f, center1.y + (Points2[num2].y - Points1[num1].y) * 1.1f);
        else if (center1.x > Points2[num1].x)
            portal.transform.position = new Vector2(center1.x + (Points2[num1].x - Points1[num2].x) * 1.1f, center1.y - (Points1[num2].y - Points2[num1].y) * 1.1f);
    }

    private void Destroy_Portal(Portal portal)
    {
        Destroy(portal.gameObject);
        if (right)
            gun.BluePortal = null;
        else 
            gun.OrangePortal = null;
    }

    private void SetPortalScaleAndRotation(Portal portal, PolygonCollider2D other, int i)
    {
        float rotat = other.transform.rotation.eulerAngles.z;
        switch (i) {
            case 0: rotat += -90f; break;
            case 1: rotat += 0f; break;
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
    }

    private void FindSideAndAlign(Portal portal, Vector2[] points, PolygonCollider2D other)
    {
        while (true) {
            for (int i = 0; i < points.Length - 1; i++) {
                foreach (RaycastHit2D item in Physics2D.LinecastAll(points[i], points[i + 1])) {
                    if (item.collider.CompareTag("Shoot")) {
                        if (right) {
                            if (gun.BluePortal != null)
                                gun.BluePortal.DestroyPortalAnimation();
                            gun.BluePortal = portal;
                        } else {
                            if (gun.OrangePortal != null)
                                gun.OrangePortal.DestroyPortalAnimation();
                            gun.OrangePortal = portal;
                        }

                        if (i % 2 == 0)
                            portal.transform.position = new Vector2(item.point.x, item.transform.position.y);
                        else
                            portal.transform.position = new Vector2(item.transform.position.x, item.point.y);

                        SetPortalScaleAndRotation(portal, other, i);

                        portal.Collider = other;
                        if (gun.OrangePortal && gun.BluePortal) {
                            if (gun.OrangePortal.Collider == gun.BluePortal.Collider) {
                                Vector2[] Points1 = portal.blue.GetComponent<PolygonCollider2D>().points;
                                Vector2[] Points2 = gun.BluePortal.blue.GetComponent<PolygonCollider2D>().points;
                                if (right)
                                    Points2 = gun.OrangePortal.blue.GetComponent<PolygonCollider2D>().points;
                                for (int j = 0; j < Points1.Length; j++)
                                    Points1[j] = portal.blue.transform.TransformPoint(Points1[j]);
                                for (int j = 0; j < Points2.Length; j++) {
                                    if (right)
                                        Points2[j] = gun.OrangePortal.blue.transform.TransformPoint(Points2[j]);
                                    else
                                        Points2[j] = gun.BluePortal.blue.transform.TransformPoint(Points2[j]);
                                }
                                Vector3 center1 = portal.transform.position;

                                switch (i) {
                                    case 0:
                                        if (Vertical_Check(Points1, Points2[1], Points2[0]))
                                            VerticalPortalsAligment(center1, Points1, Points2, portal, 0, 1);
                                        break;
                                    case 1:
                                        if (Horizontal_Check(Points1, Points2[0], Points2[1]))
                                            HorizontalPortalsAligment(center1, Points1, Points2, portal, 0, 1);
                                        break;
                                    case 2:
                                        if (Vertical_Check(Points1, Points2[0], Points2[1]))
                                            VerticalPortalsAligment(center1, Points1, Points2, portal, 1, 0);
                                        break;
                                    case 3:
                                        if (Horizontal_Check(Points1, Points2[1], Points2[0]))
                                            HorizontalPortalsAligment(center1, Points1, Points2, portal, 1, 0);
                                        break;
                                }
                            }
                        }
                        Vector2[] portals = portal.blue.GetComponent<PolygonCollider2D>().points;
                        for (int j = 0; j < portals.Length; j++)
                            portals[j] = portal.blue.transform.TransformPoint(portals[j]);
                        switch (i) {
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
                        if (gun.OrangePortal && gun.BluePortal) {
                            if (gun.OrangePortal.Collider == gun.BluePortal.Collider) {
                                Vector2[] Points1 = portal.blue.GetComponent<PolygonCollider2D>().points;
                                Vector2[] Points2 = gun.BluePortal.blue.GetComponent<PolygonCollider2D>().points;
                                if (right)
                                    Points2 = gun.OrangePortal.blue.GetComponent<PolygonCollider2D>().points;
                                for (int j = 0; j < Points1.Length; j++)
                                    Points1[j] = portal.blue.transform.TransformPoint(Points1[j]);
                                for (int j = 0; j < Points2.Length; j++) {
                                    if (right)
                                        Points2[j] = gun.OrangePortal.blue.transform.TransformPoint(Points2[j]);
                                    else
                                        Points2[j] = gun.BluePortal.blue.transform.TransformPoint(Points2[j]);
                                }
                                Vector3 center1 = portal.transform.position;

                                switch (i) {
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
                        switch (i) {
                            case 0: portal.side = "Left"; break;
                            case 1: portal.side = "Down"; break;
                            case 2: portal.side = "Right"; break;
                            case 3: portal.side = "Up"; break;
                        }
                        return;
                    }
                }
            }
            GetComponent<CircleCollider2D>().radius += 0.1f;
        }
    }

    public void SetColor(Color newColor)
    {
        mainColor = newColor;
    }
}
