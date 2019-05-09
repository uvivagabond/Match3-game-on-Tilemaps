using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class MatchUtility
{
    /// <summary>
    /// Removes tile at position (sets it null)
    /// </summary>
    /// <param name="positionOfCell"></param>
    public static void RemoveTileAtPosition(this Tilemap t, Vector3Int positionOfCell)
    {
        t.SetTile(positionOfCell, null);
    }


    /// <summary>
    /// Method to change positions in grid space to positions of centers of grid in world space
    /// </summary>
    /// <param name="listOfPositionsOfCells"> List of positions in grid space</param>
    public static List<Vector3> GetCentersOfTilesInWorldSpace(this Tilemap t, List<Vector3Int> listOfPositionsOfCells)
    {
        int lenght = listOfPositionsOfCells.Count;
        List<Vector3> vlist = new List<Vector3>(lenght);
        for (int i = 0; i < lenght; i++)
        {
            vlist[i] = t.GetCenterOfTileInWorldSpace(listOfPositionsOfCells[i]);
        }
        return vlist;
    }


    /// <summary>
    /// Method to change positions in grid space to positions of centers of grid in world space
    /// </summary>
    /// <param name="arrayOfPositionsOfCells">Array of positions in grid space</param>  
    public static Vector3[] GetCentersOfTilesInWorldSpace(this Tilemap t, Vector3Int[] arrayOfPositionsOfCells)
    {
        int lenght = arrayOfPositionsOfCells.Length;
        
        Vector3[] vArray = new Vector3[lenght];
        for (int i = 0; i < lenght; i++)
        {
            vArray[i] = t.GetCenterOfTileInWorldSpace(arrayOfPositionsOfCells[i]);
        }
        return vArray;
    }
    

    /// <summary>
    /// Get center of tile in world space
    /// </summary>
    /// <param name="positionOfCell"> Position of cell in grid space </param>
    /// <returns></returns>
    public static Vector3 GetCenterOfTileInWorldSpace(this Tilemap t, Vector3Int positionOfCell)
    {
        Grid grid = t.layoutGrid;
        Vector3 localPosition = grid.CellToLocalInterpolated(new Vector3(positionOfCell.x+0.5f, positionOfCell.y+0.5f, positionOfCell.z+0.5f));
        Vector3 worldPosition = grid.LocalToWorld(localPosition);
        return worldPosition;
    }


    /// <summary>
    /// Are tiles neighboring?
    /// </summary>
    /// <param name="lhs"> First position</param>
    /// <param name="rhs"> Second position</param>
    /// <returns></returns>
    public static bool AreNeighboring(this Tilemap t, Vector3Int lhs, Vector3Int rhs)
    {
        if (lhs.x == rhs.x && lhs.y == rhs.y)
        {
            return false;
        }

        return ((lhs.x == rhs.x || lhs.x == rhs.x - 1 || lhs.x == rhs.x + 1) &&
                (lhs.y == rhs.y||lhs.y == rhs.y - 1 || lhs.y == rhs.y + 1)) 
                ? true : false;       
    }


    /// <summary>
    /// Lerp Tile by offset
    /// The tile position in tilemap is only changed when the tile reaches the target.
    /// </summary>
    /// <param name="startPosition"> Position of tile which we want move</param>
    /// <param name="offset"> Offset about which we want move tile </param>
    /// <param name="timeOfMove"> How long tile should move</param>
    /// <returns></returns>
    public static IEnumerator LerpTileByOffsetDiscrete(this Tilemap t, Vector3Int startPosition, Vector3Int offset, float timeOfMove)
    {
        float elapsedTime = 0;
        Vector3 endPosition = VectorExtensionsUB.ConvertToVector3(offset);
        TileBase lerpedTile = t.GetTile<TileBase>(startPosition);

        float lerpTimeNormalized = 0;
        while (lerpTimeNormalized <= 1)
        {
            yield return null;
            elapsedTime += Time.smoothDeltaTime;
            lerpTimeNormalized = elapsedTime / timeOfMove;

            Vector3 tempPosition = Vector3.Lerp(Vector3.zero, endPosition, lerpTimeNormalized);
            t.SetTransformMatrix(startPosition, Matrix4x4.Translate(tempPosition));
        }

        t.SetTile(startPosition + offset, lerpedTile);
        t.SetTile(startPosition, null);
    }


    /// <summary>
    /// Lerp Tile to position. 
    /// The tile position in tilemap is only changed when the tile reaches the target.
    /// </summary>
    /// <param name="tilePosition"> Position of tile which we want move</param>
    /// <param name="endPosition"> Position where we want to place tile </param>
    /// <param name="timeOfMove"> How long tile should move</param>
    /// <returns></returns>
    public static IEnumerator LerpTileToPositionDiscrete(this Tilemap t, Vector3Int startPosition, Vector3Int endPosition, float timeOfMove)
    {
        float elapsedTime = 0;

        Vector3 startPos = VectorExtensionsUB.ConvertToVector3(startPosition);
        Vector3 endPos = VectorExtensionsUB.ConvertToVector3(endPosition);
        TileBase lerpedTile = t.GetTile<TileBase>(startPosition);

        float lerpTimeNormalized = 0;
        while (lerpTimeNormalized <= 1)
        {
            yield return null;
            elapsedTime += Time.smoothDeltaTime;
            lerpTimeNormalized = elapsedTime / timeOfMove;

            Vector3 tempPosition = Vector3.Lerp(Vector3.zero, endPos - startPos, lerpTimeNormalized);
            t.SetTransformMatrix(startPosition, Matrix4x4.Translate(tempPosition));
        }
        yield return null;

        t.SetTile(endPosition, lerpedTile);
        t.SetTile(startPosition, null);
    }
    /// <summary>
    /// Lerp Tile to position. 
    /// The tile position in the tilemap is swapped immediately and then the movement from the initial position to the final one is performed.
    /// </summary>
    /// <param name="tilePosition"> Position of tile which we want move</param>
    /// <param name="endPosition"> Position where we want to place tile </param>
    /// <param name="timeOfMove"> How long tile should move</param>
    /// <returns></returns>
    public static IEnumerator LerpTileToPositionContinuous(this Tilemap t, Vector3Int startPosition, Vector3Int endPosition, float timeOfMove)
    {

        Vector3 startPos = VectorExtensionsUB.ConvertToVector3(startPosition);
        Vector3 endPos = VectorExtensionsUB.ConvertToVector3(endPosition);
        TileBase lerpedTile = t.GetTile<TileBase>(startPosition);

        t.SetTile(endPosition, lerpedTile);
        t.SetTile(startPosition, null);

        float elapsedTime =  0.1f * timeOfMove;
        float lerpTimeNormalized = 0;

        while (lerpTimeNormalized <= 1)
        {           
            elapsedTime += Time.smoothDeltaTime;
            lerpTimeNormalized = elapsedTime / timeOfMove;

            Vector3 tempPosition = Vector3.Lerp(startPos-endPos, Vector3.zero, lerpTimeNormalized);
            t.SetTransformMatrix(endPosition, Matrix4x4.Translate(tempPosition));
            yield return null;
        }
    }


    /// <summary>
    /// Lerp Tile by offset
    /// The tile position in the tilemap is swapped immediately and then the movement from the initial position to the final one is performed.
    /// </summary>
    /// <param name="startPosition"> Position of tile which we want move</param>
    /// <param name="offset"> Offset about which we want move tile </param>
    /// <param name="timeOfMove"> How long tile should move</param>
    /// <returns></returns>
    public static IEnumerator LerpTileByOffsetContinuous(this Tilemap t, Vector3Int startPosition, Vector3Int offset, float timeOfMove)
    {
        float elapsedTime = 0;
        Vector3 endPosition = VectorExtensionsUB.ConvertToVector3(offset);
        TileBase lerpedTile = t.GetTile<TileBase>(startPosition);

        t.SetTile(startPosition + offset, lerpedTile);
        t.SetTile(startPosition, null);

        float lerpTimeNormalized = 0;
        while (lerpTimeNormalized <= 1)
        {
         
            elapsedTime += Time.smoothDeltaTime;
            lerpTimeNormalized = elapsedTime / timeOfMove;

            Vector3 tempPosition = Vector3.Lerp(endPosition, Vector3.zero, lerpTimeNormalized);
            t.SetTransformMatrix(startPosition, Matrix4x4.Translate(tempPosition));
            yield return null;
        }       
    }

}
