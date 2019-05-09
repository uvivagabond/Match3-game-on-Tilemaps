using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnavailablePlacesInTilemap
{
    Match3TileMapInformation mapInfo;
    List<Vector3Int> positionsOfRocks;
    public List<Vector3Int> positions = new List<Vector3Int>();

    public UnavailablePlacesInTilemap(Match3TileMapInformation mapInfo)
    {
        this.mapInfo = mapInfo;
        this.positionsOfRocks = mapInfo.rocks.GetRocksPositions();
        this.FindUnavailable();
    }
    public bool IsUnavailable(Vector3Int position)
    {
        int x = position.x;
        int y = position.y;

        int dxp = Mathf.Clamp(x + 1, 0, 7);
        int dxm = Mathf.Clamp(x - 1, 0, 7);

        if (IsTileUnavailableLevel1(position))
        {
            positions.Add(new Vector3Int(x, y - 1, 0));
        }
        if (IsTileUnavailableLevel2(position))
        {
            positions.Add(new Vector3Int(x, y - 2, 0));
        }

        return false;
    }
    public bool FindUnavailable()
    {
        foreach (var position in positionsOfRocks)
        {
            int x = position.x;
            int y = position.y;

            if (IsUnavailable(position))
            {
                positions.Add(position);
            }
        }
        return positions.Count > 0;
    }
    public void FillUnAvailableTiles(Tilemap tilemap, Tile empty)
    {
        if (mapInfo.unavailablePlacesInTilemap.positions.Count == 0)
            return;

        List<Vector3Int> toRemove = new List<Vector3Int>();

        foreach (var positionOfUnavailable in mapInfo.unavailablePlacesInTilemap.positions)
        {
            if (!tilemap.HasTile(positionOfUnavailable) && mapInfo.IsCellEmpty(positionOfUnavailable))
            {
                tilemap.SetTile(positionOfUnavailable, empty);
                // musimy dodać pusty element do kamieni bo po skosie można przemieszczać elementy tylko gdy są pod kamieniami
                // we have to add an empty element to the stones because diagonally you can move the elements only when they are under the stones
                mapInfo.rocks.SetPositionOfRock(positionOfUnavailable);
                mapInfo.SetThatCellIsEmptyOrNot(positionOfUnavailable, false);
            }
        }
        foreach (var item in toRemove)
        {
            mapInfo.unavailablePlacesInTilemap.positions.Remove(item);
        }
    }



    bool IsTileUnavailableLevel2(Vector3Int position)
    {
        int x = position.x;
        int y = position.y;

        int dxp1 = Mathf.Clamp(x + 1, 0, 7);
        int dxp2 = Mathf.Clamp(x + 2, 0, 7);
        int dxp3 = Mathf.Clamp(x + 3, 0, 7);

        int dxm1 = Mathf.Clamp(x - 1, 0, 7);
        int dxm2 = Mathf.Clamp(x - 2, 0, 7);
        int dxm3 = Mathf.Clamp(x - 3, 0, 7);

        return
        IsSymetricPart(x, y, dxp1, dxp2, dxm1, dxm2)
        && (
          IsSymetricPart2(x, y, dxp1, dxp2, dxm1, dxm2)
          || IsRightPart(y, dxp1, dxp2, dxm1)
          || IsLeftPart(y, dxp1, dxm1, dxm2)
        );
    }
    #region Unavailable combinations
    //Legend:
    //R - rock
    //E - empty
    //S - source of block
    #endregion
    bool IsLeftPart(int y, int dxp1, int dxm1, int dxm2)
    {
        // R_s__
        // _E_R_
        // _____
        return positionsOfRocks.Contains(new Vector3Int(dxm2, y, 0)) && //rock left 2
                  !positionsOfRocks.Contains(new Vector3Int(dxm1, y - 1, 0)) && //empty left 2
                  positionsOfRocks.Contains(new Vector3Int(dxp1, y - 1, 0));  //rock right 1;
    }

     bool IsRightPart(int y, int dxp1, int dxp2, int dxm1)
    {
        // __s_R
        // _R_E_
        // _____
        return positionsOfRocks.Contains(new Vector3Int(dxp2, y, 0)) && //rock right 2
                  !positionsOfRocks.Contains(new Vector3Int(dxp1, y - 1, 0)) && //empty right 1
                  positionsOfRocks.Contains(new Vector3Int(dxm1, y - 1, 0));//rock left 1
    }

     bool IsSymetricPart2(int x, int y, int dxp1, int dxp2, int dxm1, int dxm2)
    {
        // R_s_R
        // _E_E_
        // _____
        return positionsOfRocks.Contains(new Vector3Int(dxp2, y, 0)) &&//rock right 2
                  !positionsOfRocks.Contains(new Vector3Int(dxm1, y - 1, 0)) && //empty left 1
                  positionsOfRocks.Contains(new Vector3Int(dxm2, y, 0)) && //rock left 2
                  !positionsOfRocks.Contains(new Vector3Int(dxp1, y - 1, 0));//empty right 1
    }

    bool IsSymetricPart(int x, int y, int dxp1, int dxp2, int dxm1, int dxm2)
    {
        return
   !positionsOfRocks.Contains(new Vector3Int(x, y - 1, 0)) && // empty 2 down
    !positionsOfRocks.Contains(new Vector3Int(x, y - 2, 0)) && // empty 1 down
    positionsOfRocks.Contains(new Vector3Int(dxm1, y, 0)) && //left rock 1
    positionsOfRocks.Contains(new Vector3Int(dxp1, y, 0))  //right rock 1
   ;
    }

    bool IsTileUnavailableLevel1(Vector3Int position)
    {
        int x = position.x;
        int y = position.y;

        int dxp = Mathf.Clamp(x + 1, 0, 7);
        int dxm = Mathf.Clamp(x - 1, 0, 7);
        return !positionsOfRocks.Contains(new Vector3Int(x, y - 1, 0)) &&
          positionsOfRocks.Contains(new Vector3Int(dxm, y, 0)) &&
         positionsOfRocks.Contains(new Vector3Int(dxp, y, 0));
    }

  

}
