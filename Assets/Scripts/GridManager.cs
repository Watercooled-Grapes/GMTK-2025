using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
 
public class GridManager : MonoBehaviour {
    [SerializeField] private int _width, _height;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Transform _cam;
 
    private Dictionary<Vector2, Tile> _tiles;
    public enum TileType
    {
        EmptyTile = 0,
        WallTile = 1,
        StartTile = 2,
        EndTile = 3,
        AppTile = 5,
    }
    
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
                bool isWall = mapData[x, y] == (int)TileType.WallTile;

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

    public List<Tile> GetReachableTiles(Vector2 startPos, int steps)
    {
        List<Tile> reachableTiles = new List<Tile>();

        Vector2[] directions = new Vector2[] {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };

        foreach (Vector2 dir in directions) {
            for (int i = 1; i <= steps; i++) {
                Vector2 nextPos = startPos + dir * i;
                Tile tile = GetTileAtPosition(nextPos);
                if (tile == null || tile.IsWall) break;
                reachableTiles.Add(tile);
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
    
    
    public static Vector2Int? FindFirstPositionOfType(int[,] mapData, GridManager.TileType tileType)
    {
        int width = mapData.GetLength(0);
        int height = mapData.GetLength(1);
        int tileValue = (int)tileType;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapData[x, y] == tileValue)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return null;
    }        

    public static List<Vector2Int> FindAllPositionOfType(int[,] mapData, TileType tileType)
    {
        int width = mapData.GetLength(0);
        int height = mapData.GetLength(1);
        int tileValue = (int)tileType;
        List<Vector2Int> positions = new List<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapData[x, y] == tileValue)
                {
                    positions.Add(new Vector2Int(x, y));
                }
            }
        }

        return positions;
    }

    public void GenerateApps(int[,] mapData)
    {
        List<Vector2Int> appPositions = FindAllPositionOfType(mapData, GridManager.TileType.AppTile);

        foreach (var appPosition in appPositions)
        {
            
        }
    }
}
