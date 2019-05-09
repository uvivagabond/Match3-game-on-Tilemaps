using UnityEngine;
using UnityEngine.Tilemaps;

public class Match3TileMapInformation : GridInformation
{
    #region privates
    MapDimentions mapDimentions;
    Tilemap tilemap;
    readonly string isCellEmpty = "isCellEmpty";
    bool isAnyRockInMap = false;
    #endregion

   [HideInInspector] public Vector3Int actualPositionOfEmptyTile;
    public UnavailablePlacesInTilemap unavailablePlacesInTilemap;
    public RocksManager rocks;

    #region Rocks 
    public void InitMapToCreateAndMove(Vector3Int leftDownCornerOfMap, Tilemap tilemap, MapDimentions mapDimentions, Tile[] tiles)
    {
        this.tilemap = tilemap;
        this.mapDimentions = mapDimentions;
        this.FillEmptyPlacesInMapWithRandomTiles(
               new Vector3Int(0, 0, 0),
               tilemap,
               mapDimentions,
               tiles
              );
        this.GetPositionsOfRocks(mapDimentions, tilemap);
    }
    void GetPositionsOfRocks(MapDimentions mapDimentions, Tilemap tilemap)
    {
        rocks = new RocksManager(mapDimentions, tilemap);
        unavailablePlacesInTilemap = new UnavailablePlacesInTilemap(this);
        this.mapDimentions = mapDimentions;
    }

    #endregion

    #region Info about Cells    
    public void SetThatCellIsEmptyOrNot(Vector3Int position, bool IsCellEmpty)
    {
        this.SetPositionProperty(position, isCellEmpty, IsCellEmpty);
    }

    public bool IsCellEmpty(Vector3Int position)
    {
        return this.GetPositionProperty(position, isCellEmpty, false);
    }

    public bool HasTile(Vector3Int position)
    {
        return !this.GetPositionProperty(position, isCellEmpty, false);
    }

    public bool IsColumnFull(int columnId)
    {
        bool isColumnFull = true;
        for (int y = 0; y < mapDimentions.height; y++)
        {
            isColumnFull = isColumnFull
                && !this.IsCellEmpty(new Vector3Int(columnId, y, 0));
        }
        return isColumnFull;
    }

    public bool AreAllCellsFull()
    {
        bool areAllCellsFull = true;

        for (int j = 0; j < mapDimentions.height; j++)
        {
            for (int i = 0; i < mapDimentions.width; i++)
            {
                areAllCellsFull = areAllCellsFull && !IsCellEmpty(new Vector3Int(i, j, 0));
                if (!areAllCellsFull)
                {
                    return areAllCellsFull;
                }
            }
        }

        return areAllCellsFull;
    }

    void FillEmptyPlacesInMapWithRandomTiles(Vector3Int leftDownCornerOfMap, Tilemap tilemap, MapDimentions mapDimentions, Tile[] tiles)
    {
        this.mapDimentions = mapDimentions;
        for (int j = 0; j < this.mapDimentions.height; j++)
        {
            for (int i = 0; i < this.mapDimentions.width; i++)
            {
                if (!tilemap.HasTile(new Vector3Int(i, j, 0)))
                {
                    tilemap.SetTile(new Vector3Int(i, j, 0), tiles[Random.Range(0, tiles.Length)]);
                }
            }
        }
    }

    public bool IsVerticalShiftingPossible()
    {
        if (!this.rocks.IsAnyRockInMap)
        {
            return true;
        }

        return CanWeMoveTilesDownFromRockColumns();
    }

    bool CanWeMoveTilesDownFromRockColumns()
    {
        for (int columnId = 0; columnId < mapDimentions.width; columnId++)
        {
            if (!this.IsColumnFull(columnId))
            {
                if (this.rocks.IsThereRockInColumn(columnId))
                {
                    for (int y = mapDimentions.height - 1; y >= 1; y--)
                    {
                        Vector3Int potentialPositionOfRock = new Vector3Int(columnId, y, 0);
                        if (this.HasTile(potentialPositionOfRock) && !this.rocks.IsRockAtPosition(potentialPositionOfRock))
                        {
                            if (!this.HasTile(new Vector3Int(columnId, y - 1, 0)))
                            {
                                return true;
                            }                            
                        }
                    }
                }
            }
        }

        return false;
    }

    public bool IsColumnFullForNextRock(Vector3Int a)
    {
        for (int y = a.y; y >= 0; y--)
        {
            Vector3Int potentialPositionOfRock = new Vector3Int(a.x, y, 0);

            if (!this.HasTile(potentialPositionOfRock))
            {
                return false;
            }
            else if (this.rocks.IsRockAtPosition(potentialPositionOfRock))
            {
                return true;
            }
        }
        return true;
    }

    public bool IsItPossibleToMoveTileDown(Vector3Int a)
    {
        int xxx = a.x;
        int yyy = a.y;
        if (!this.rocks.IsThereRockInColumn(xxx))
        {
            for (int y = yyy; y >= 0; y--)
            {
                Vector3Int position = new Vector3Int(xxx, y, 0);

                if (!this.HasTile(position))
                {
                    return true;
                }
            }
        }
        else
        {
            if (!this.IsColumnFull(xxx))
            {              
                for (int y = yyy; y >= 0; y--)
                {
                    Vector3Int position = new Vector3Int(xxx, y, 0);
                    Vector3Int positionUp = new Vector3Int(xxx, y + 1, 0);

                    if (!this.HasTile(position)&& this.HasTile(positionUp) && !this.rocks.IsRockAtPosition(positionUp))
                    {
                        return true;
                    }
                    else if (this.HasTile(position) && this.rocks.IsRockAtPosition(position))
                    {
                        break;
                    }
                }
            }
        }
        return false;
    }

    public void EraseCellEmptinessInfo(Vector3Int position)
    {
        this.ErasePositionProperty(position, isCellEmpty);
    }
    bool isMapNotFull = false;
    public bool IsMapFull { get => isMapNotFull; set => isMapNotFull = value; }
    #endregion


 public  bool CanWeMoveTileFromTo(Vector3Int from, Vector3Int to)
    {      
        return IsCellWaitingToBeFilled(to) && CanWeMoveUpperTile(from);
    }

    public bool IsCellWaitingToBeFilled(Vector3Int positionOfTile)
    {
        return !tilemap.HasTile(positionOfTile) && this.IsCellEmpty(positionOfTile);
    }

     bool CanWeMoveUpperTile(Vector3Int positionOfTile)
    {
        return tilemap.HasTile(positionOfTile) && IsTileMovable(positionOfTile)
            && !this.IsCellEmpty(positionOfTile);
    }

    bool IsTileMovable(Vector3Int positionOfTile)
    {
        Match3Tile match3Tile = tilemap.GetTile<Match3Tile>(positionOfTile);
        if (!match3Tile)
            return false;

        return match3Tile.traits.IsTileMovable() || match3Tile.transform.GetColumn(3).y > 0.5f;
    }

    public bool CanWeMakeObliqueMoveToRight(ReplacementNeighbors neighborsPositions)
    {
        return CanWeMoveTileFromTo(neighborsPositions.topLeft, neighborsPositions.actualPosition)
                    && this.rocks.IsRockAtPosition(neighborsPositions.top);
    }

  public  bool CanWeMakeObliqueMoveToLeft(ReplacementNeighbors neighborsPositions)
    {
        return CanWeMoveTileFromTo(neighborsPositions.topRight, neighborsPositions.actualPosition)
                             && this.rocks.IsRockAtPosition(neighborsPositions.top);
    }
}
