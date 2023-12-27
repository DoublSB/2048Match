using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour, IPoolAble
{
    [SerializeField] private TMP_Text txtCount;
    [SerializeField] private Swapper[] swappers;
    public int Count { private set; get; }

    public void Initialize(int count)
    {
        this.Count = count;
        RefreshText();
        
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, Global.BlockCreateDuration)
                 .SetEase(Global.BlockCreateEaseType);
    }

    public async UniTask SetCount(int count, float delay)
    {
        this.Count = count;

        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        
        transform.DOPunchScale(Vector3.one * Global.BlockMergeAnimationPunchSize, Global.BlockMergeAnimationDuration)
            .SetEase(Global.BlockMergeAnimationEaseType);
        RefreshText();
    }
    
    public void RefreshText()
    {
        txtCount.text = Count.ToString();

        var type = Swapper.CountToType(Count);
        
        foreach (var e in swappers)
            e.Swap(type);
    }

    public void OnGet()
    {
        gameObject.SetActive(true);
    }

    public void OnRelease()
    {
        gameObject.SetActive(false);
    }
}
