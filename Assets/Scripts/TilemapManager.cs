using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap), typeof(TilemapCollider2D))]
[RequireComponent(typeof(LineRenderer), typeof(Match3TileMapInformation))]
public class TilemapManager : MonoBehaviour
{
    public Tilemap tilemap;
    public MapDimentions mapDimentions = new MapDimentions(8, 8);
    public TileConnector tileConnector = new TileConnector();
    public TileCreatorAndMover tileCreatorAndMover = new TileCreatorAndMover();

    #region MouseEvents   

    private void OnMouseEnter()
    {
        tileConnector.OnSelectNextTiles();
    }

    private void Update()
    {
        tileCreatorAndMover.CreateOrMoveDownTile();

        if (Input.GetMouseButtonDown(0))
        {
            tileConnector.OnSelectFirstTile();
        }
        if (Input.GetMouseButtonUp(0))
        {
            tileConnector.OnEndSelection();
        }          
    }
  
    #endregion

    #region Initialization

    private void Reset()
    {
        tilemap.gameObject.AddComponent<TilemapCollider2D>();
        InitLineRenderer();
    }

    private void Start()
    {
        Input.multiTouchEnabled = false;
        tilemap = this.GetComponent<Tilemap>();       

        InitLineRenderer();

        tileConnector.InitConnector(tilemap, Camera.main, mapDimentions);
        tileCreatorAndMover.InitCreatorAndMover(tilemap, mapDimentions);
    }

    private void InitLineRenderer()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.widthMultiplier = 0.2f;
        lineRenderer.sortingLayerName = "LineRenderer"; 
    }

    #endregion
}



