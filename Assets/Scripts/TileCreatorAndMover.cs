using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TileCreatorAndMover
{
    public Tile[] tiles;
    public Match3Tile empty;
    public float lerpTime;
    float timer = 0;
    MapDimentions mapDimentions;
    Tilemap tilemap;
    TilemapManager tilemapManager;
    Match3TileMapInformation mapInfo;
    bool moveLeft = true;


    #region Public Methods

    public void InitCreatorAndMover(Tilemap tilemap, MapDimentions mapDimentions)
    {
        this.mapDimentions = mapDimentions;
        this.tilemap = tilemap;
        tilemapManager = tilemap.GetComponent<TilemapManager>();
        this.mapInfo = tilemap.GetComponent<Match3TileMapInformation>();

        this.mapInfo.InitMapToCreateAndMove(new Vector3Int(0, 0, 0), tilemap, mapDimentions, tiles);
        tilemapManager.StartCoroutine(ChangeObliqueMoveDirection());
    }

    IEnumerator ChangeObliqueMoveDirection()
    {
        while (true)
        {
            yield return new WaitForSeconds(lerpTime);
            moveLeft = !moveLeft;
        }
    }

    public void CreateOrMoveDownTile()
    {
        if (!mapInfo.IsMapFull)
        {
            CheckWhichCellsAreEmptyThenCreateOrMoveDownTiles();
        }
    }
    #endregion

    void CheckWhichCellsAreEmptyThenCreateOrMoveDownTiles()
    {
        for (int y = 0; y < mapDimentions.height; y++)
        {
            MoveDownAllTilesExceptThoseFromLastTopLayer(y);
            CreateNewTilesAndMoveThemDownToLastTopLayer(y);
        }
        mapInfo.unavailablePlacesInTilemap.FillUnAvailableTiles(tilemap, empty);
        mapInfo.IsMapFull = mapInfo.AreAllCellsFull();      
    }

    void CreateNewTilesAndMoveThemDownToLastTopLayer(int y)
    {
        if (y == mapDimentions.height - 1)
        {
            for (int x = 0; x < mapDimentions.width; x++)
            {
                Vector3Int actualPosition = new Vector3Int(x, mapDimentions.height - 1, 0);
                Vector3Int topPosition = new Vector3Int(x, mapDimentions.height, 0);

                if (!tilemap.HasTile(actualPosition))
                {
                    tilemap.SetTile(topPosition, tiles[Random.Range(0, tiles.Length)]);
                    tilemapManager.StartCoroutine(ChangeTilePositionWithLerp(actualPosition, topPosition));
                }
            }
        }
    }

    void MoveDownAllTilesExceptThoseFromLastTopLayer(int idOfHorizontalLayerOfMap)
    {
        for (int x = 0; x < mapDimentions.width; x++)
        {
            MoveTilesToEmptyPlacesInMap(x, idOfHorizontalLayerOfMap);
        }
    }

    IEnumerator ChangeTilePositionWithLerp(Vector3Int actualPosition, Vector3Int top)
    {
        mapInfo.SetThatCellIsEmptyOrNot(top, true);
        yield return tilemapManager.StartCoroutine(tilemap.LerpTileToPositionContinuous(top, actualPosition, lerpTime));
        mapInfo.SetThatCellIsEmptyOrNot(actualPosition, false);
    }
         
    void MoveTilesToEmptyPlacesInMap(int x, int idOfHorizontalLayerOfMap)
    {
        ReplacementNeighbors neighborsPositions = new ReplacementNeighbors(actualPosition: new Vector3Int(x, idOfHorizontalLayerOfMap, 0));
        mapInfo.actualPositionOfEmptyTile = neighborsPositions.actualPosition;

        if (mapInfo.IsItPossibleToMoveTileDown(neighborsPositions.actualPosition))
        {
            DoVerticalMove(neighborsPositions);
        }
        else
        {
            DoObliqueMove(neighborsPositions);
        }
    }

    void DoVerticalMove(ReplacementNeighbors neighborsPositions)
    {
        if (mapInfo.CanWeMoveTileFromTo(neighborsPositions.top, neighborsPositions.actualPosition))
        {
            tilemapManager.StartCoroutine(ChangeTilePositionWithLerp(neighborsPositions.actualPosition, neighborsPositions.top));
        }
    }

    void DoObliqueMove(ReplacementNeighbors neighborsPositions)
    {
        if (!moveLeft && mapInfo.CanWeMakeObliqueMoveToLeft(neighborsPositions) && !mapInfo.IsItPossibleToMoveTileDown(neighborsPositions.topRight))
        {
            tilemapManager.StartCoroutine(ChangeTilePositionWithLerp(neighborsPositions.actualPosition, neighborsPositions.topRight));
        }
        else if (moveLeft && mapInfo.CanWeMakeObliqueMoveToRight(neighborsPositions) && !mapInfo.IsItPossibleToMoveTileDown(neighborsPositions.topLeft))
        {
            tilemapManager.StartCoroutine(ChangeTilePositionWithLerp(neighborsPositions.actualPosition, neighborsPositions.topLeft));
        }
    }

   

  


}



