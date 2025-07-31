#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Collections.Generic;

public class TilemapSplitter : MonoBehaviour {
    [MenuItem("Tools/Split Tilemap Into Platforms")]
    static void SplitTilemapIntoPlatforms() {
        var selected = Selection.activeGameObject;
        if (!selected || !selected.TryGetComponent(out Tilemap sourceTilemap)) {
            Debug.LogError("Select a GameObject with a Tilemap to split.");
            return;
        }

        var tilemapBounds = sourceTilemap.cellBounds;
        var visited = new HashSet<Vector3Int>();
        var allTiles = new Dictionary<Vector3Int, TileBase>();

        // Gather all non-null tiles
        foreach (var pos in tilemapBounds.allPositionsWithin) {
            var tile = sourceTilemap.GetTile(pos);
            if (tile != null)
                allTiles[pos] = tile;
        }

        int platformCount = 0;

        foreach (var pos in allTiles.Keys) {
            if (visited.Contains(pos)) continue;

            // Flood fill to find connected tile group
            var platformTiles = new List<Vector3Int>();
            Queue<Vector3Int> q = new Queue<Vector3Int>();
            q.Enqueue(pos);

            while (q.Count > 0) {
                var current = q.Dequeue();
                if (visited.Contains(current) || !allTiles.ContainsKey(current)) continue;

                visited.Add(current);
                platformTiles.Add(current);

                // 4-directional check
                foreach (var offset in new[] {
                    Vector3Int.left, Vector3Int.right }) {
                    q.Enqueue(current + offset);
                }
            }

            if (platformTiles.Count == 0) continue;

            // Create new GameObject for the platform
            var platformGO = new GameObject($"Platform_{platformCount++}");
            platformGO.transform.position = selected.transform.position;
            platformGO.transform.parent = selected.transform.parent;
            platformGO.layer = LayerMask.NameToLayer("Platform");


            var newTilemap = platformGO.AddComponent<Tilemap>();
            var renderer = platformGO.AddComponent<TilemapRenderer>();

            foreach (var tilePos in platformTiles) {
                newTilemap.SetTile(tilePos, allTiles[tilePos]);
                sourceTilemap.SetTile(tilePos, null);
            }

            // Add physics components
            var tileCol = platformGO.AddComponent<TilemapCollider2D>();
            tileCol.usedByComposite = true;

            var body = platformGO.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Static;

            var compCol = platformGO.AddComponent<CompositeCollider2D>();
            compCol.geometryType = CompositeCollider2D.GeometryType.Polygons;
            compCol.usedByEffector = true;

            var effector = platformGO.AddComponent<PlatformEffector2D>();
            effector.useOneWay = true;
            effector.useOneWayGrouping = true;
            effector.surfaceArc = 160f;
        }

        Debug.Log($"✅ Split into {platformCount} platforms.");
    }
}
#endif
