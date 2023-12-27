using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Slot : MonoBehaviour, IPoolAble
{
    public Vector2Int pos { private set; get; }
    public Block block { private set; get; }
    public bool IsEmpty => block == null;
    public bool IsMerging { private set; get; }

    public void Initialize(Vector2Int pos)
    {
        gameObject.name = $"slot{pos.x}x{pos.y}";
        this.pos = pos;
        transform.position = new Vector3(pos.x, pos.y);
        block = null;
    }

    public async UniTask SetBlock(Block block, bool isAnimation = false)
    {
        if(block == null || block == this.block)
            return;

        if (isAnimation)
            block.transform.DOMove(transform.position, Global.BlockMoveDuration).SetEase(Global.BlockMoveEaseType);
        else
            block.transform.position = transform.position;
        
        if (!ReferenceEquals(this.block, null))
            MergeBlock(block).Forget();
        else
            this.block = block;
    }

    public void RemoveBlock()
    {
        this.block = null;
    }

    public bool IsMergeAble(Block block)
    {
        if (IsMerging)
            return false;
        
        if (ReferenceEquals(this.block, null))
            return true;
        
        return block.Count == this.block.Count;
    }
    
    private async UniTask MergeBlock(Block block)
    {
        if(ReferenceEquals(block, null))
            return;
        
        if(!IsMergeAble(block))
            Debug.LogError("You tried to merge block which has different count!");

        IsMerging = true;

        this.block.transform.DOMoveZ(-1, 0);
        
        await this.block.SetCount(block.Count + this.block.Count, Global.BlockMoveDuration);
        Game2048Manager.instance.Release(block);
        
        this.block.transform.DOMoveZ(0, 0);

        IsMerging = false;
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
