using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RocksManager {

    MapDimentions mapDimentions;
    public RocksManager(MapDimentions mapDimentions, Tilemap tilemap)
    {
       this.mapDimentions = mapDimentions;
        this.GetPositionsOfRocks( mapDimentions,  tilemap);
    }
    List<int> columnInWhichAreRocks = new List<int>();
    List<Vector3Int> positionsOfRocks = new List<Vector3Int>();

    public void GetPositionsOfRocks(MapDimentions mapDimentions, Tilemap tilemap)
    {
        for (int i = 0; i < mapDimentions.width; i++)
        {
            for (int j = 0; j < mapDimentions.height; j++)
            {
                Vector3Int pos = new Vector3Int(i, j, 0);
                Match3Tile m3 = tilemap.GetTile<Match3Tile>(pos);
                if (m3 && m3.traits.IsRock())
                {
                    this.SetPositionOfRock(pos);
                }
            }
        }
    }


    public List<Vector3Int> GetRocksPositions()
    {
        return positionsOfRocks;
    }

    public bool IsRockAtPosition(Vector3Int position)
    {
        return positionsOfRocks.Contains(position);
    }

    public bool IsThereRockInColumn(int columnId)
    {
        return columnInWhichAreRocks.Contains(columnId);
    }

    public void SetPositionOfRock(Vector3Int position)
    {
        this.positionsOfRocks.Add(position);

        if (!columnInWhichAreRocks.Contains(position.x))
        {
            columnInWhichAreRocks.Add(position.x);
        }
    }

    public void EraseRocksPositions(Vector3Int position)
    {
        positionsOfRocks.Clear();
    }

    public bool IsItPossibleToMoveTileOnlyOneWay(Vector3Int position)
    {
        Vector3Int left = new Vector3Int(position.x - 1, position.y, position.z);

        Vector3Int right = new Vector3Int(position.x + 1, position.y, position.z);

        return 
            !positionsOfRocks.Contains(left)
            && positionsOfRocks.Contains(right)
            ||
            positionsOfRocks.Contains(left)
            && !positionsOfRocks.Contains(right)
            ||
            !positionsOfRocks.Contains(left)
            && !positionsOfRocks.Contains(right); ;
    }

   public  bool IsAnyRockInMap { get { return positionsOfRocks.Count > 0; } }


 
}
