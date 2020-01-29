using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelTile : MonoBehaviour
{
    public float VoxelSize = 0.1f;
    public int TileSideVoxels = 8;

    [HideInInspector] public byte[] ColorsRight;
    [HideInInspector] public byte[] ColorsForward;
    [HideInInspector] public byte[] ColorsLeft;
    [HideInInspector] public byte[] ColorsBack;

    public void CalculateSidesColors()
    {
        ColorsRight = new byte[TileSideVoxels * TileSideVoxels];
        ColorsForward = new byte[TileSideVoxels * TileSideVoxels];
        ColorsLeft = new byte[TileSideVoxels * TileSideVoxels];
        ColorsBack = new byte[TileSideVoxels * TileSideVoxels];
        
        for (int y = 0; y < TileSideVoxels; y++)
        {
            for (int i = 0; i < TileSideVoxels; i++)
            {
                ColorsRight[y * TileSideVoxels + i] = GetVoxelColor(y, i, Vector3.right);
                ColorsForward[y * TileSideVoxels + i] = GetVoxelColor(y, i, Vector3.forward);
                ColorsLeft[y * TileSideVoxels + i] = GetVoxelColor(y, i, Vector3.left);
                ColorsBack[y * TileSideVoxels + i] = GetVoxelColor(y, i, Vector3.back);
            }
        }
    }

    private byte GetVoxelColor(int verticalLayer, int horizontalOffset, Vector3 direction)
    {
        var meshCollider = GetComponentInChildren<MeshCollider>();

        float vox = VoxelSize;
        float half = VoxelSize / 2;

        Vector3 rayStart;
        if (direction == Vector3.right)
        {
            rayStart = meshCollider.bounds.min +
                       new Vector3(-half, 0, half + horizontalOffset * vox);
        }
        else if (direction == Vector3.forward)
        {
            rayStart = meshCollider.bounds.min +
                       new Vector3(half + horizontalOffset * vox, 0, -half);
        }
        else if (direction == Vector3.left)
        {
            rayStart = meshCollider.bounds.max +
                       new Vector3(half, 0, -half - (TileSideVoxels - horizontalOffset - 1) * vox);
        }
        else if (direction == Vector3.back)
        {
            rayStart = meshCollider.bounds.max +
                       new Vector3(-half - (TileSideVoxels - horizontalOffset - 1) * vox, 0, half);
        }
        else
        {
            throw new ArgumentException("Wrong direction value, should be Vector3.left/right/back/forward",
                nameof(direction));
        }

        rayStart.y = meshCollider.bounds.min.y + half + verticalLayer * vox;

        Debug.DrawRay(rayStart, direction * .1f, Color.blue, 2);

        if (Physics.Raycast(new Ray(rayStart, direction), out RaycastHit hit, vox))
        {
            Mesh mesh = meshCollider.sharedMesh;

            int hitTriangleVertex = mesh.triangles[hit.triangleIndex * 3];
            byte colorIndex = (byte) (mesh.uv[hitTriangleVertex].x * 256);
            return colorIndex;
        }

        return 0;
    }
}