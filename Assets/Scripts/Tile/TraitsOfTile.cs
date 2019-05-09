using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[System.Flags]
public enum TraitsOfTile
{
    None = 0,
    [Description("Color - Red")]
    Red = 1,
    [Description("Color - Orange")]
    Orange = 2,
    [Description("Color - Yellow")]
    Yellow = 4,
    [Description("Color - Green")]
    Green = 8,
    [Description("Color - Blue")]
    Blue = 16,
    [Description("Color - Violet")]
    Violet = 32,
 
    [Description("Bonus - Horizontal")]
    BonusHorizontal = 256,
    [Description("Bonus - Vertical")]
    BonusVertical = 512,

    [Description("Not Selectable - Rock")]
    Rock = 2048,
    [Description("Not Selectable - Diamond")]
    Diamond = 4096,
}

public static class TraitsOfTileExtensions
{
    public static bool IsTileNotSelectable(this TraitsOfTile self)
    {
       return self.HasFlag(TraitsOfTile.Diamond) || self.HasFlag(TraitsOfTile.Rock) || self == TraitsOfTile.None;
    }
    public static bool IsTileMovable(this TraitsOfTile self)
    {
        return !self.IsRock();
    }
    public static bool IsRock(this TraitsOfTile self)
    {
        return self.HasFlag(TraitsOfTile.Rock);
    }
    public static bool IsNone(this TraitsOfTile self)
    {
        return self == TraitsOfTile.None;
    }
    public static bool IsBonus(this TraitsOfTile self)
    {
        return self.HasFlag(TraitsOfTile.BonusHorizontal) || self.HasFlag(TraitsOfTile.BonusVertical);
    }
    public static bool HasSameColor(this TraitsOfTile self, TraitsOfTile actualColorFlag)
    {
        return (!self.IsNone() && !actualColorFlag.IsNone()) && self.HasFlag(actualColorFlag.GetColorsFlags());//&& ((self & actualColorFlag) == actualColorFlag)
    }
    public static TraitsOfTile GetColorsFlags(this TraitsOfTile self)
    {
        return (self & TraitsOfTile.Red)
            | (self & TraitsOfTile.Orange)
            | (self & TraitsOfTile.Yellow)
            | (self & TraitsOfTile.Green)
            | (self & TraitsOfTile.Blue)
            | (self & TraitsOfTile.Violet);
    }

    public static TraitsOfTile GetBonusesFlags(this TraitsOfTile self)
    {
        return (self & TraitsOfTile.BonusHorizontal)
            | (self & TraitsOfTile.BonusVertical);
    }
}
