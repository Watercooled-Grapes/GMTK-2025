using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class GridManager : MonoBehaviour {
    [SerializeField] private int _width, _height;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Transform _cam;
 
    private Dictionary<Vector2, Tile> _tiles;

    public void GenerateGrid(int[,] mapData)
    {
        int width = mapData.GetLength(0);
        int height = mapData.GetLength(1);
        _tiles = new Dictionary<Vector2, Tile>();

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";

                bool isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                bool isWall = mapData[x, y] == 1;

                spawnedTile.Init(isOffset, x, y, isWall);

                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        _cam.transform.position = new Vector3(width / 2f - 0.5f, height / 2f - 0.5f, -30);
    }

 
    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }

    public Dictionary<Tile, int> GetReachableTiles(Vector2 startPos, int turns)
    {
        Dictionary<Tile, int> reachableTiles = new Dictionary<Tile, int>();

        Vector2[] directions = new Vector2[] {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };

        foreach (Vector2 dir in directions) {
            for (int i = 1; i <= turns; i++) {
                Vector2 nextPos = startPos + dir * i;
                Tile tile = GetTileAtPosition(nextPos);
                if (tile == null || tile.IsWall) break;
                reachableTiles.Add(tile, i);
            }
        }

        return reachableTiles;
    }

    public Vector3 GetTileCenterPosition(Vector2 gridPos)
    {
        Tile tile = GetTileAtPosition(gridPos);
        return GetTileCenterPosition(tile);
    }

    public Vector3 GetTileCenterPosition(Tile tile)
    {
        if (tile != null)
        {
            return tile.transform.position;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public Tile GetTileByWorldCoordinate(Vector3 worldPos)
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 gridPos = new Vector2(Mathf.Round(mouseWorldPos.x), Mathf.Round(mouseWorldPos.y));

        return GetTileAtPosition(gridPos);
    }

    public void OnResetForLoop(int[,] mapData, Vector2 startPosition)
    {
        // TODO: Reset the map
        GenerateGrid(mapData);
    }
}
