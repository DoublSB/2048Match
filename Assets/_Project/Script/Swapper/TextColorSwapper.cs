using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TextColorSwapper : Swapper
{
    [SerializedDictionary("Type", "Color")]
    [SerializeField] private SerializedDictionary<Type, Color> colors;

    private TMP_Text text;

    protected override void Awake()
    {
        text = GetComponent<TMP_Text>();
        base.Awake();
    }

    protected override void OnSwap(Type type)
    {
        if (!colors.ContainsKey(type))
            type = defaultType;

        text.color = colors[type];
    }
}
