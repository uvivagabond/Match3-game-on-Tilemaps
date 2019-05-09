using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class hold top neighbors positions and actual
/// </summary>
public class ReplacementNeighbors
{
    public Vector3Int actualPosition;
    public Vector3Int top;
    public Vector3Int topLeft;
    public Vector3Int topRight;

    public ReplacementNeighbors(Vector3Int actualPosition)
    {
        this.actualPosition = actualPosition;
        this.top = new Vector3Int(actualPosition.x, actualPosition.y + 1, 0);
        this.topLeft = new Vector3Int(actualPosition.x - 1, actualPosition.y + 1, 0);
        this.topRight = new Vector3Int(actualPosition.x + 1, actualPosition.y + 1, 0);
    }
}

