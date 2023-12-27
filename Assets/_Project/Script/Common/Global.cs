using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public static class Global
{
    public const float SlotSize = 1;
    
    public const float BlockCreateDuration = 0.1f;
    public const Ease BlockCreateEaseType = Ease.InOutBack;
    
    public const float BlockMoveDuration = 0.3f;
    public const Ease BlockMoveEaseType = Ease.OutCubic;
    
    public const float BlockMergeAnimationPunchSize = 0.1f;
    public const Ease BlockMergeAnimationEaseType = Ease.InCirc;
    public const float BlockMergeAnimationDuration = 0.2f;
}

public static class Extensions
{
    public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
    {
        var rnd = new System.Random();
        return source.OrderBy((item) => rnd.Next());
    }
    
    public static bool IsPowerOf2(int x)
    {
        if (x < 0)
            return false;
        
        return (x != 0) && (x & (x - 1)) == 0;
    }
    
    public static int GetExponentPowerOf2(int num)
    {
        double logResult = Math.Log(num, 2);

        if (Math.Abs(logResult - Math.Round(logResult)) < double.Epsilon)
        {
            return (int)logResult;
        }
        else
        {
            return -1;
        }
    }
}