using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Swapper : MonoBehaviour
{
    [SerializeField] protected Type defaultType;
    protected Type CurrentType;
    
    public enum Type
    {
        None,
        
        Enable = 1,
        Disable = 2,
        
        _2 = 10,
        _4 = 11,
        _8 = 12,
        _16 = 13,
        _32 = 14,
        _64 = 15,
        _128 = 16,
        _256 = 17,
        _512 = 18,
        _1024 = 19,
        _2048 = 20,
        _4096 = 21,
        _8192 = 22,
        _16384 = 23,
        _32768 = 24,
        _65536 = 25,
    }

    public static Type CountToType(int count)
    {
        var exponent = Extensions.GetExponentPowerOf2(count);

        if (exponent <= 0)
            return Type.None;
        
        else if (exponent >= 16)
            return Type._65536;

        return (Type)((int)Type._2 + exponent - 1);
    }
    
    protected virtual void Awake()
    {
        Swap(defaultType);
    }

    public virtual void Swap(Type type)
    {
        if(CurrentType == type)
            return;
        
        CurrentType = type;
        OnSwap(CurrentType);
    }

    protected abstract void OnSwap(Type type);
}
