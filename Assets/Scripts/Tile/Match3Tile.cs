using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Reflection;
using System.ComponentModel;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Tilemaps
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Match3Tile", menuName = "Tiles/Match3Tile")]
    public class Match3Tile : Tile
    {
        public TraitsOfTile traits = TraitsOfTile.None;
        public Sprite normalSprite;
        public Sprite hoverSprite;      

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);
            tileData.sprite = normalSprite; 
            
        }
    }
}

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(Match3Tile))]
public class Match3TileEditor : Editor
{
    private Match3Tile tile { get { return (target as Match3Tile); } }

    SerializedProperty sprite; SerializedProperty normalSprite; SerializedProperty hoverSprite; SerializedProperty traits;

    SerializedProperty color; SerializedProperty colliderType;
    SerializedProperty flags;

    private void OnEnable()
    {     
        sprite = serializedObject.FindProperty("m_Sprite");
        normalSprite = serializedObject.FindProperty("normalSprite");
        hoverSprite = serializedObject.FindProperty("hoverSprite");
        traits = serializedObject.FindProperty("traits");
        color = serializedObject.FindProperty("m_Color");
        flags = serializedObject.FindProperty("m_Flags");
        colliderType = serializedObject.FindProperty("m_ColliderType");
    }

    public override void OnInspectorGUI()
    {
        Match3Tile e = (Match3Tile)target;
        serializedObject.Update();
        DrawSpritePreview(normalSprite, tile.normalSprite);
        DrawSpritePreview(hoverSprite, tile.hoverSprite);
        e.traits = (TraitsOfTile) EditorGUILayout.EnumFlagsField(e.traits);       
 
        EditorGUILayout.PropertyField(color);
       // EditorGUILayout.PropertyField(flags);
        EditorGUILayout.PropertyField(colliderType);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSpritePreview(SerializedProperty propertyOfSpriteType, Sprite sprite)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(propertyOfSpriteType);
        Texture2D normalTexture = sprite ? AssetPreview.GetAssetPreview(sprite): AssetPreview.GetMiniTypeThumbnail(typeof(Sprite));
        GUILayout.Label(normalTexture, GUILayout.Width(32), GUILayout.Height(32)); 
        EditorGUILayout.EndHorizontal();
    }
}
#endif








