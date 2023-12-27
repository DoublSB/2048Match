using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Directions
{
    public enum Type
    {
        None,
        Up,
        UpRight,
        Right,
        DownRight,
        Down,
        DownLeft,
        Left,
        UpLeft,
    }

    private static Dictionary<Vector2Int, Type> vectorMap = new()
    {
        { new Vector2Int(0, 1), Type.Up },
        { new Vector2Int(1, 1), Type.UpRight },
        { new Vector2Int(1, 0), Type.Right },
        { new Vector2Int(1, -1), Type.DownRight },
        { new Vector2Int(0, -1), Type.Down },
        { new Vector2Int(-1, -1), Type.DownLeft },
        { new Vector2Int(-1, 0), Type.Left},
        { new Vector2Int(-1, 1), Type.UpLeft },
    };

    private static Dictionary<Type, Vector2Int> typeMap = new()
    {
        {Type.Up, new Vector2Int(0, 1)},
        {Type.UpRight, new Vector2Int(1, 1)},
        {Type.Right, new Vector2Int(1, 0)},
        {Type.DownRight, new Vector2Int(1, -1)},
        {Type.Down, new Vector2Int(0, -1)},
        {Type.DownLeft, new Vector2Int(-1, -1)},
        {Type.Left, new Vector2Int(-1, 0)},
        {Type.UpLeft, new Vector2Int(-1, 1)},
        {Type.None, Vector2Int.zero},
    };

    public static bool IsDiagonal(Type type)
    {
        return type
            is Type.UpRight or Type.DownRight or Type.DownLeft or Type.UpLeft;
    }

    public static bool IsStraight(Type type)
    {
        return type
            is Type.Up or Type.Right or Type.Down or Type.Left;
    }

    public static Vector2Int ToVector2Int(this Type type)
    {
        return typeMap.TryGetValue(type, out var i) ? i : Vector2Int.zero;
    }

    public static Type ToDirectionType(this Vector2Int pos)
    {
        pos.Clamp(typeMap[Type.DownLeft], typeMap[Type.UpRight]);
        return vectorMap.TryGetValue(pos, out var type) ? type : Type.None;
    }

    public static Type Opposite(this Type type)
    {
        var vector = typeMap[type];
        vector *= -1;

        return vectorMap[vector];
    }
    
    public static Vector2Int Next(this Vector2Int pos, Type type)
    {
        var vector = typeMap[type];
        pos += vector;

        return pos;
    }
}
