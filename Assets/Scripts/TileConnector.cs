using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TileConnector
{
    public Color selectedColor = Color.red;
    public Color unselectedColor = Color.green;
    public float delayOfDisappearingOfTiles = 0.07f;
    bool IsConnecting = false;
    bool areWeCurrentlyRemovingTiles = false;
    TraitsOfTile colorsFlagsOfFirstSelectedTile = TraitsOfTile.None;
    Tilemap tilemap;
    TilemapManager tilemapManager;
    Camera orthographicCamera;
    LineRenderer lineRenderer;

    Match3TileMapInformation mapInfo;
    List<Vector3Int> selectedTilesPositions = new List<Vector3Int>();
    #region Properties  

    Vector3Int GetLastSelectedPosition { get => selectedTilesPositions[selectedTilesPositions.Count - 1]; }

    Vector3Int GetPenultimateSelectedPosition { get => selectedTilesPositions[selectedTilesPositions.Count - 2]; }

    bool AreAtLeastThreeTilesSelected { get => selectedTilesPositions.Count > 2; }

    #endregion

    #region Public methods and properties

    public void InitConnector(Tilemap tilemap, Camera orthographicCamera, MapDimentions mapDimentions)
    {
        this.tilemap = tilemap;
        tilemapManager = tilemap.GetComponent<TilemapManager>();
        this.orthographicCamera = orthographicCamera;
        this.lineRenderer = tilemap.GetComponent<LineRenderer>();
        this.mapInfo = tilemap.GetComponent<Match3TileMapInformation>();
    }

    public void OnSelectFirstTile()
    {
        if (!mapInfo.IsMapFull || areWeCurrentlyRemovingTiles)
            return;

        Vector3Int actualPositionOfCursor = GetPositionOfTileWhenCursorIsOver();

        if (!IsFirstTileSelectable(actualPositionOfCursor))
            return;

        ChoiceSelectionMode(actualPositionOfCursor);
    }

    public void OnSelectNextTiles()
    {
        if (!IsConnecting || areWeCurrentlyRemovingTiles)
            return;

        Vector3Int actualPositionOfCursor = GetPositionOfTileWhenCursorIsOver();
        if (!IsNextTileSelectable(actualPositionOfCursor) || !IsNextTileHaveSameColorAsFirstTile(actualPositionOfCursor))
            return;

        if (tilemap.AreNeighboring(actualPositionOfCursor, GetLastSelectedPosition))
        {
            ConnectTiles(actualPositionOfCursor);
        }
        ConnectPositionsWithLines(selectedTilesPositions);
    }

    public void OnEndSelection()
    {
        if (!AreAtLeastThreeTilesSelected || areWeCurrentlyRemovingTiles)
            return;
        DeleteOrUnselectTiles();
    }


    #endregion
    

    /// <summary>
    /// Method to draw lines with LineRenderer to show the order of selection
    /// </summary>
    /// <param name="selectedTilesPositions">List of points to connect</param>
    void ConnectPositionsWithLines(List<Vector3Int> selectedTilesPositions)
    {
        int count = selectedTilesPositions.Count;
        lineRenderer.positionCount = count;
        if (count > 0)
        {
            lineRenderer.SetPositions(tilemap.GetCentersOfTilesInWorldSpace(selectedTilesPositions.ToArray()));
        }
    }
    private bool IsTileSelected(Vector3Int pos)
    {
        return selectedTilesPositions.Contains(pos);
    }

    #region Add /Remove Tiles
    private void AddTileToSelected(Vector3Int actualPosition)
    {
        if (!selectedTilesPositions.Contains(actualPosition))
        {
            selectedTilesPositions.Add(actualPosition);
        }
    }


    private void RemoveLastSelectedPosition()
    {
        RestoreTileDefaultAppearance(GetLastSelectedPosition);
        selectedTilesPositions.Remove(GetLastSelectedPosition);
    }

    private void DeleteTilesIfMatch()
    {
        tilemapManager.StartCoroutine(DestroyTileWithDelay(delayOfDisappearingOfTiles));
    }

    IEnumerator DestroyTileWithDelay(float delay)
    {
        areWeCurrentlyRemovingTiles = true;
        if (selectedTilesPositions.Count > 2)
        {
            for (int i = 0; i < selectedTilesPositions.Count; i++)
            {
                if (!mapInfo.IsCellEmpty(selectedTilesPositions[i]))
                {
                    mapInfo.SetThatCellIsEmptyOrNot(selectedTilesPositions[i], true);
                }
                tilemap.RemoveTileAtPosition(selectedTilesPositions[i]);
                yield return new WaitForSeconds(delay);
            }
            yield return new WaitForSeconds(delay);
        }     
        selectedTilesPositions.Clear();
        mapInfo.IsMapFull = false;
        yield return null;
        areWeCurrentlyRemovingTiles = false;
    }

    private void UnselectAllTiles()
    {
        foreach (var position in selectedTilesPositions)
        {
            RestoreTileDefaultAppearance(position);
        }
    }
    #endregion
    

    #region Tile Apperance

    void ChangeAppearanceOfTile(Vector3Int position)
    {
        if (IsConnecting)
        {
            tilemap.SetColor(position, selectedColor);
        }
    }
    private void RestoreTileDefaultAppearance(Vector3Int position)
    {
        tilemap.SetColor(position, unselectedColor);
    }
    #endregion

    #region Other
    void DeleteOrUnselectTiles()
    {
        IsConnecting = false;
        UnselectAllTiles();
        DeleteTilesIfMatch();

        HideConnectingLines();
        colorsFlagsOfFirstSelectedTile = TraitsOfTile.None;
    }
    /// <summary>
    /// If we have already selected less than 3 tiles, we can release button and mark next tiles further until you press tap again.
    /// OR 
    /// We can select at least 3 tiles and at time of release button, we immediately delete selected tiles
    /// </summary>
    private void ChoiceSelectionMode(Vector3Int actualPositionOfCursor)
    {
        if (IsConnecting)
        {
            DeleteOrUnselectTiles();
        }
        else
        {
            IsConnecting = true;
            AddTileToSelected(actualPositionOfCursor);
            ChangeAppearanceOfTile(actualPositionOfCursor);
        }
    }
    private bool IsNextTileHaveSameColorAsFirstTile(Vector3Int actualPosition)
    {
        Match3Tile match3TileInstance = tilemap.GetTile<Match3Tile>(actualPosition);
        if (!match3TileInstance)
            return false;

        bool hasSameColor = colorsFlagsOfFirstSelectedTile.HasSameColor(match3TileInstance.traits.GetColorsFlags());

        return hasSameColor;
    }

    private bool IsFirstTileSelectable(Vector3Int actualPosition)
    {
        GetColorFlagsFromFirstTile(actualPosition);
        bool isSelectable = IsTileSelectable(actualPosition);
        IfNotSelectableThenResetColorFlags(isSelectable);
        return isSelectable;
    }
    private bool IsNextTileSelectable(Vector3Int actualPosition)
    {
        return IsTileSelectable(actualPosition);
    }
    private bool IsTileSelectable(Vector3Int actualPosition)
    {
        bool isSelectable = !colorsFlagsOfFirstSelectedTile.IsTileNotSelectable();
        return isSelectable;
    }

    private void GetColorFlagsFromFirstTile(Vector3Int actualPosition)
    {
        Match3Tile match3TileInstance = tilemap.GetTile<Match3Tile>(actualPosition);
        if (!match3TileInstance)
            return;

        colorsFlagsOfFirstSelectedTile = match3TileInstance.traits.GetColorsFlags();
    }

    private void IfNotSelectableThenResetColorFlags(bool isSelectable)
    {
        if (!isSelectable)
        {
            colorsFlagsOfFirstSelectedTile = TraitsOfTile.None;
        }
    }

    void HideConnectingLines()
    {
        // As a parameter, we add an empty list so that the line will disappear
        ConnectPositionsWithLines(new List<Vector3Int>(0));
    }

    private Vector3Int GetPositionOfTileWhenCursorIsOver()
    {
        Vector3Int position = GetPositionOfTileOverCursor(tilemap, orthographicCamera);
        return position;
    }
    private void UnselectIfTileIsLastSelected(Vector3Int newPosition)
    {

        if (GetPenultimateSelectedPosition == newPosition)
        {
            RemoveLastSelectedPosition();
        }
    }


    public void ConnectTiles(Vector3Int newPosition)
    {
        if (!IsConnecting)
            return;

        if (IsTileSelected(newPosition))
        {
            UnselectIfTileIsLastSelected(newPosition);
            return;
        }
        else
        {
            AddTileToSelected(newPosition);
            ChangeAppearanceOfTile(newPosition);
        }
    }

    bool IsCursorOverTile(Camera camera)
    {
        Ray r = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hitTile = Physics2D.GetRayIntersection(r);
        return hitTile;
    }

    Vector3Int GetPositionOfTileOverCursor(Tilemap tilemap, Camera camera)
    {
        return camera.ScreenToCellPoint(tilemap, Input.mousePosition);
    }
    #endregion
}