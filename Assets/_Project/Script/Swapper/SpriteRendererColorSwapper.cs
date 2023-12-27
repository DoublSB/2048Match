using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRendererColorSwapper : Swapper
{
    [SerializedDictionary("Type", "Color")]
    [SerializeField] private SerializedDictionary<Type, Color> colors;

    private SpriteRenderer image;

    protected override void Awake()
    {
        image = GetComponent<SpriteRenderer>();
        base.Awake();
    }

    protected override void OnSwap(Type type)
    {
        if (!colors.ContainsKey(type))
            type = defaultType;

        image.color = colors[type];
    }
}
