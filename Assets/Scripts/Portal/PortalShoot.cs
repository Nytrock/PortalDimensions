using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using static UnityEditor.Progress;

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

    private MapManager mapManager;
    private Vector2 moveVector;
    private Animator animator;

    private List<ShootCollider> colliders = new();

    private void Start()
    {
        Invoke(nameof(DestroyAmmo), destroyTime);
        InvokeRepeating(nameof(AddSpeed), 0, 0.1f);

        var main = shootParticle.main;
        shootLight.color = mainColor;
        main.startColor = mainColor;

        shootParticle.Play();

        mapManager = MapManager.mapManager;
        animator = GetComponent<Animator>();
    }

    public void Update()
    {
        transform.Translate(Speed * Time.fixedDeltaTime * moveVector);
    }

    private void DestroyAmmo()
    {
        animator.enabled = true;
    }

    private void AddSpeed() {
        Speed += boost;
    }

    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out TilemapCollider2D _) && colliders.Count == 4) {
            TileData tile = mapManager.GetTileData(transform.position);
            moveVector = Vector2.zero;
            if (tile != null && tile.forPortal)
                SpawnPortal(transform.position);
            animator.enabled = true;
        }
    }

    public void Destroy_Shoot()
    {
        Destroy(gameObject);
    }

    public void SpawnPortal(Vector2 shootPos)
    {
        Portal portal = Instantiate(portalPrefab, null);
        portal.SetPortal(right, gun);

        Tilemap tilemap = mapManager.GetMap();
        var grid = tilemap.GetComponentInParent<GridLayout>();

        Vector3Int gridPosition = tilemap.WorldToCell(shootPos);
        TileBase tile = tilemap.GetTile(gridPosition);
        Vector3 tilePos = tilemap.GetCellCenterWorld(gridPosition);
        float tileSize = grid.transform.localScale.x;
        print(tilePos);

        portal.side = colliders[3].side;
        if (right) {
            if (gun.bluePortal != null)
                gun.bluePortal.DestroyPortalAnimation();
            gun.bluePortal = portal;
        } else {
            if (gun.orangePortal != null)
                gun.orangePortal.DestroyPortalAnimation();
            gun.orangePortal = portal;
        }

        SetPortalTransform(portal, shootPos, tilePos, tileSize);

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
            gun.bluePortal = null;
        else 
            gun.orangePortal = null;
    }

    private void SetPortalTransform(Portal portal, Vector2 shootPos, Vector2 tilePos, float tileSize)
    {
        float portalRotation = 0;
        float portalScale = 1f;
        Vector2 portalPosition = Vector2.zero;


        switch (portal.side) {
            case "Left": 
                portalRotation = -90f; 
                portalPosition = new Vector2(tilePos.x - tileSize, shootPos.y);
                portalScale = 0.93f;
                break;
            case "Down": 
                portalRotation = 0f; 
                portalPosition = new Vector2(shootPos.x, tilePos.y - tileSize);
                portalScale = 0.28f;
                break;
            case "Right": 
                portalRotation = 90f; 
                portalPosition = new Vector2(tilePos.x + tileSize, shootPos.y);
                portalScale = 0.93f;
                break;
            case "Up": 
                portalRotation = 180f; 
                portalPosition = new Vector2(shootPos.x, tilePos.y + tileSize);
                portalScale = 0.28f;
                break;
        }

        portal.transform.rotation = Quaternion.Euler(0f, 0f, portalRotation);
        portal.transform.position = portalPosition;

        portal.transform.localScale = new Vector2(portal.transform.localScale.x * portalScale, portal.transform.localScale.y);
        portal.Collider1.transform.localScale = new Vector2(1f / portalScale, 1f);
        portal.Collider2.transform.localScale = new Vector2(1f / portalScale, 1f);
    }

    private void FindSideAndAlign(Portal portal, Vector2[] points, PolygonCollider2D other)
    {
        while (true) {
            for (int i = 0; i < points.Length - 1; i++) {
                foreach (RaycastHit2D item in Physics2D.LinecastAll(points[i], points[i + 1])) {
                    if (item.collider.CompareTag("Shoot")) {
                        //if (right) {
                        //    if (gun.bluePortal != null)
                        //        gun.bluePortal.DestroyPortalAnimation();
                        //    gun.bluePortal = portal;
                        //} else {
                        //    if (gun.orangePortal != null)
                        //        gun.orangePortal.DestroyPortalAnimation();
                        //    gun.orangePortal = portal;
                        //}

                        //if (i % 2 == 0)
                        //    portal.transform.position = new Vector2(item.point.x, item.transform.position.y);
                        //else
                        //    portal.transform.position = new Vector2(item.transform.position.x, item.point.y);

                        // SetPortalScaleAndRotation(portal, other, i);

                        //if (gun.orangePortal && gun.bluePortal) {
                        //    if (gun.orangePortal.Collider == gun.bluePortal.Collider) {
                        //        Vector2[] Points1 = portal.portalSprite.GetComponent<PolygonCollider2D>().points;
                        //        Vector2[] Points2 = gun.bluePortal.portalSprite.GetComponent<PolygonCollider2D>().points;
                        //        if (right)
                        //            Points2 = gun.orangePortal.portalSprite.GetComponent<PolygonCollider2D>().points;
                        //        for (int j = 0; j < Points1.Length; j++)
                        //            Points1[j] = portal.portalSprite.transform.TransformPoint(Points1[j]);
                        //        for (int j = 0; j < Points2.Length; j++) {
                        //            if (right)
                        //                Points2[j] = gun.orangePortal.portalSprite.transform.TransformPoint(Points2[j]);
                        //            else
                        //                Points2[j] = gun.bluePortal.portalSprite.transform.TransformPoint(Points2[j]);
                        //        }
                        //        Vector3 center1 = portal.transform.position;

                        //        switch (i) {
                        //            case 0:
                        //                if (Vertical_Check(Points1, Points2[1], Points2[0]))
                        //                    VerticalPortalsAligment(center1, Points1, Points2, portal, 0, 1);
                        //                break;
                        //            case 1:
                        //                if (Horizontal_Check(Points1, Points2[0], Points2[1]))
                        //                    HorizontalPortalsAligment(center1, Points1, Points2, portal, 0, 1);
                        //                break;
                        //            case 2:
                        //                if (Vertical_Check(Points1, Points2[0], Points2[1]))
                        //                    VerticalPortalsAligment(center1, Points1, Points2, portal, 1, 0);
                        //                break;
                        //            case 3:
                        //                if (Horizontal_Check(Points1, Points2[1], Points2[0]))
                        //                    HorizontalPortalsAligment(center1, Points1, Points2, portal, 1, 0);
                        //                break;
                        //        }
                        //    }
                        //}
                        //Vector2[] portals = portal.portalSprite.GetComponent<PolygonCollider2D>().points;
                        //for (int j = 0; j < portals.Length; j++)
                        //    portals[j] = portal.portalSprite.transform.TransformPoint(portals[j]);
                        //switch (i) {
                        //    case 0:
                        //        Vertical_Alignment(portals[0], portals[1], points[i], points[i + 1], portal);
                        //        break;
                        //    case 1:
                        //        Horizontal_Alignment(portals[1], portals[0], points[i + 1], points[i], portal);
                        //        break;
                        //    case 2:
                        //        Vertical_Alignment(portals[1], portals[0], points[i + 1], points[i], portal);
                        //        break;
                        //    case 3:
                        //        Horizontal_Alignment(portals[0], portals[1], points[i], points[i + 1], portal);
                        //        break;
                        //}
                        //if (gun.orangePortal && gun.bluePortal) {
                        //    if (gun.orangePortal.Collider == gun.bluePortal.Collider) {
                        //        Vector2[] Points1 = portal.portalSprite.GetComponent<PolygonCollider2D>().points;
                        //        Vector2[] Points2 = gun.bluePortal.portalSprite.GetComponent<PolygonCollider2D>().points;
                        //        if (right)
                        //            Points2 = gun.orangePortal.portalSprite.GetComponent<PolygonCollider2D>().points;
                        //        for (int j = 0; j < Points1.Length; j++)
                        //            Points1[j] = portal.portalSprite.transform.TransformPoint(Points1[j]);
                        //        for (int j = 0; j < Points2.Length; j++) {
                        //            if (right)
                        //                Points2[j] = gun.orangePortal.portalSprite.transform.TransformPoint(Points2[j]);
                        //            else
                        //                Points2[j] = gun.bluePortal.portalSprite.transform.TransformPoint(Points2[j]);
                        //        }
                        //        Vector3 center1 = portal.transform.position;

                        //        switch (i) {
                        //            case 0:
                        //                if (Vertical_Check(Points1, Points2[1], Points2[0]))
                        //                    Destroy_Portal(portal);
                        //                break;
                        //            case 1:
                        //                if (Horizontal_Check(Points1, Points2[0], Points2[1]))
                        //                    Destroy_Portal(portal);
                        //                break;
                        //            case 2:
                        //                if (Vertical_Check(Points1, Points2[0], Points2[1]))
                        //                    Destroy_Portal(portal);
                        //                break;
                        //            case 3:
                        //                if (Horizontal_Check(Points1, Points2[1], Points2[0]))
                        //                    Destroy_Portal(portal);
                        //                break;
                        //        }
                        //    }
                        //}
                        //float Distance1 = Vector2.Distance(points[i], points[i + 1]);
                        //float Distance2 = Vector2.Distance(portals[0], portals[1]);
                        //if (Distance1 <= Distance2)
                        //    Destroy_Portal(portal);
                        //switch (i) {
                        //    case 0: portal.side = "Left"; break;
                        //    case 1: portal.side = "Down"; break;
                        //    case 2: portal.side = "Right"; break;
                        //    case 3: portal.side = "Up"; break;
                        //}
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

    public void ChangeListColiders(ShootCollider collider, bool adding=true)
    {
        if (adding)
            colliders.Add(collider);
        else
            colliders.Remove(collider);
    }

    public void SetMoveVector(float x, float y)
    {
        moveVector = new Vector2(x, y);
    }
}
