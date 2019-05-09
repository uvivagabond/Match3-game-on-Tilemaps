using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
[InitializeOnLoad]
internal class CustomMatch3TileIcon
{
   static  string match3TileExtension = ".asset";
    static CustomMatch3TileIcon()
    {
        EditorApplication.projectWindowItemOnGUI += OnChangeMatch3TileIcon;
    }

    static void OnChangeMatch3TileIcon(string GUID, Rect iconRect)
    {
        string assetpath = AssetDatabase.GUIDToAssetPath(GUID);
        string extension = System.IO.Path.GetExtension(assetpath);

        if (extension == match3TileExtension)
        {
            Match3Tile m3tile = AssetDatabase.LoadAssetAtPath<Match3Tile>(assetpath);
            if (m3tile && m3tile.normalSprite)
            {
                iconRect = CalculateOffsetOfIcon(iconRect);

                Texture2D icon = m3tile.normalSprite.texture;
                iconRect.width = iconRect.height * 0.9f;
                iconRect.height = iconRect.height * 0.9f;
                if (icon != null)
                {
                    GUI.DrawTexture(iconRect, icon);
                }
            }
        }
    }

    private static Rect CalculateOffsetOfIcon(Rect rect)
    {
        float width = rect.width;
        float height = rect.height;
       // rect.height = rect.width;
        rect.width = rect.height;

        if (rect.width > 64)
        {
            rect.height = width;
            rect.width = width;
            AddOffsetToIconPosition(ref rect, new Rect(1, 1, 0, 0));
        }
        else if (rect.width > 37)
        {
            rect.height = width;
            rect.width = width;
            AddOffsetToIconPosition(ref rect, new Rect(1, 2, 0, 0));
        }
        else if (rect.width >= 32)
        {
            rect.height = width;
            rect.width = width;
            AddOffsetToIconPosition(ref rect, new Rect(2, 1f, 0, 0));
        }
        else if (rect.width == 16)
        {
            rect.height = height;
            rect.width = height;

            AddOffsetToIconPosition(ref rect, new Rect(4, 1, 0, 0));
        }
        return rect;
    }

    private static void AddOffsetToIconPosition(ref Rect rect, Rect offset)
    {
        rect = new Rect(rect.x + offset.x, rect.y + offset.y, rect.height + offset.width, rect.height + offset.height);
    }
}

#endif

