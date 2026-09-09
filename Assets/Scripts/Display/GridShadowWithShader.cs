using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridShadowWithShader : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Material gridMat;
    public List<Vector2Int> activeCells;
    public List<Vector2Int> inactiveCells;

    public void Update()
    {
        Texture2D activeMask = new(6, 6, TextureFormat.R8, false);
        Texture2D inactiveMask = new(6, 6, TextureFormat.R8, false);

        // Initialize all pixels to black (transparent)
        for (int y = 0; y < 6; y++)
        {
            for (int x = 0; x < 6; x++)
            {
                activeMask.SetPixel(x, y, Color.black);
                inactiveMask.SetPixel(x, y, Color.black);
            }
        }

        // Fill active cells
        foreach (Vector2Int c in activeCells)
        {
            activeMask.SetPixel(c.x, c.y, Color.white);
        }

        // Fill inactive cells
        foreach (Vector2Int c in inactiveCells)
        {
            inactiveMask.SetPixel(c.x, c.y, Color.white);
        }

        // activeMask.wrapMode = TextureWrapMode.Clamp;
        // inactiveMask.wrapMode = TextureWrapMode.Clamp;
        activeMask.filterMode = FilterMode.Point;
        inactiveMask.filterMode = FilterMode.Point;

        activeMask.Apply();
        inactiveMask.Apply();

        gridMat.SetTexture("_ActiveMask", activeMask);
        gridMat.SetTexture("_InactiveMask", inactiveMask);
    }
}
