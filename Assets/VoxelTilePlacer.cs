using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class VoxelTilePlacer : MonoBehaviour
{
    public VoxelTile[] TilePrefabs;
    public Vector2Int MapSize = new Vector2Int(10, 10);

    private VoxelTile[,] spawnedTiles;

    private void Start()
    {
        spawnedTiles = new VoxelTile[MapSize.x, MapSize.y];

        foreach (VoxelTile tilePrefab in TilePrefabs)
        {
            tilePrefab.CalculateSidesColors();
        }

        StartCoroutine(Generate());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            StopAllCoroutines();

            foreach (VoxelTile spawnedTile in spawnedTiles)
            {
                if (spawnedTile != null) Destroy(spawnedTile.gameObject);
            }

            StartCoroutine(Generate());
        }
    }

    public IEnumerator Generate()
    {
        for (int x = 1; x < MapSize.x - 1; x++)
        {
            for (int y = 1; y < MapSize.y - 1; y++)
            {
                yield return new WaitForSeconds(0.02f);

                PlaceTile(x, y);
            }
        }
        
        yield return new WaitForSeconds(0.8f);
        foreach (VoxelTile spawnedTile in spawnedTiles)
        {
            if (spawnedTile != null) Destroy(spawnedTile.gameObject);
        }

        StartCoroutine(Generate());
    }

    private void PlaceTile(int x, int y)
    {
        List<VoxelTile> availableTiles = new List<VoxelTile>();

        foreach (VoxelTile tilePrefab in TilePrefabs)
        {
            if (CanAppendTile(spawnedTiles[x - 1, y], tilePrefab, Vector3.left) &&
                CanAppendTile(spawnedTiles[x + 1, y], tilePrefab, Vector3.right) &&
                CanAppendTile(spawnedTiles[x, y - 1], tilePrefab, Vector3.back) &&
                CanAppendTile(spawnedTiles[x, y + 1], tilePrefab, Vector3.forward))
            {
                availableTiles.Add(tilePrefab);
            }
        }

        if (availableTiles.Count == 0) return;

        VoxelTile selectedTile = availableTiles[Random.Range(0, availableTiles.Count)];
        Vector3 position = selectedTile.VoxelSize * selectedTile.TileSideVoxels * new Vector3(x, 0, y);
        spawnedTiles[x, y] = Instantiate(selectedTile, position, selectedTile.transform.rotation);
    }

    private bool CanAppendTile(VoxelTile existingTile, VoxelTile tileToAppend, Vector3 direction)
    {
        if (existingTile == null) return true;

        if (direction == Vector3.right)
        {
            return Enumerable.SequenceEqual(existingTile.ColorsRight, tileToAppend.ColorsLeft);
        }
        else if (direction == Vector3.left)
        {
            return Enumerable.SequenceEqual(existingTile.ColorsLeft, tileToAppend.ColorsRight);
        }
        else if (direction == Vector3.forward)
        {
            return Enumerable.SequenceEqual(existingTile.ColorsForward, tileToAppend.ColorsBack);
        }
        else if (direction == Vector3.back)
        {
            return Enumerable.SequenceEqual(existingTile.ColorsBack, tileToAppend.ColorsForward);
        }
        else
        {
            throw new ArgumentException("Wrong direction value, should be Vector3.left/right/back/forward",
                nameof(direction));
        }
    }
}