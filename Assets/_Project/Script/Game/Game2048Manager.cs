using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class Game2048Manager : MonoSingleton<Game2048Manager>
{
    [Header("Settings")] 
    [SerializeField] private Vector2Int size;
    
    [SerializeField] private Block blockOrigin;
    [SerializeField] private Slot slotOrigin;
    [SerializeField] private Transform parent;
    
    private ObjectPool<Block> blockPool;
    private ObjectPool<Slot> slotPool;

    private Dictionary<Vector2Int, Slot> slots = new();

    public bool IsMoving { set; get; } = false;

    private void Awake()
    {
        InitSlotPool();
        InitBlockPool();

        for (int i = 0; i < size.x; i++)
        for (int j = 0; j < size.y; j++)
            CreateSlot(new Vector2Int(i, j));

        CreateBlockInRandomSlot(8);
        
        for (int i = 0; i < 2; i++)
            CreateBlockInRandomSlot(4);
        
        for (int i = 0; i < 4; i++)
            CreateBlockInRandomSlot(2);
        
        parent.transform.position = new Vector3(size.x, size.y) * -0.5f
                                    + (Vector3)(Vector2.one * Global.SlotSize * 0.5f);
    }

    public Slot CreateSlot(Vector2Int pos)
    {
        if(slots.ContainsKey(pos))
            Debug.LogError("There is already created slot!");
        
        var slot = slotPool.Get();
        slot.Initialize(pos);
        
        slots.Add(pos, slot);

        return slot;
    }

    public Block CreateBlock(Slot slot, int count = 2)
    {
        if(!slot.IsEmpty)
            Debug.LogError("You tried to create block in full slot!");
        
        var block = blockPool.Get();
        block.Initialize(count);
        
        slot.SetBlock(block).Forget();

        return block;
    }
    
    public Block CreateBlock(Vector2Int pos, int count = 2)
    {
        if (!slots.ContainsKey(pos))
            Debug.LogError("You tried to create block in nowhere!");

        var slot = slots[pos];

        return CreateBlock(slot, count);
    }

    private void CreateBlockInRandomSlot(int count = 2)
    {
        var randomize = slots.Randomize();

        foreach (var e in randomize)
        {
            if (e.Value.IsEmpty)
            {
                CreateBlock(e.Value, count);
                return;
            }
        }
        
        GameOver();
    }

    public async UniTask MoveAllBlock(Directions.Type dir)
    {
        if(IsMoving)
            return;
        
        IsMoving = true;
        
        IOrderedEnumerable<KeyValuePair<Vector2Int, Slot>> orderedEnumerable = null;
        
        if (dir == Directions.Type.Up)
            orderedEnumerable = slots.OrderByDescending(e => e.Key.y);
        
        else if (dir == Directions.Type.Down)
            orderedEnumerable = slots.OrderBy(e => e.Key.y);
        
        else if (dir == Directions.Type.Left)
            orderedEnumerable = slots.OrderBy(e => e.Key.x);
        
        else if (dir == Directions.Type.Right)
            orderedEnumerable = slots.OrderByDescending(e => e.Key.x);

        if(orderedEnumerable == null)
            return;

        bool isMoved = false;

        foreach (var e in orderedEnumerable)
        {
            if (MoveBlock(e.Value, dir))
                isMoved = true;
        }

        if (isMoved)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Global.BlockMoveDuration));
            CreateBlockInRandomSlot();
        }

        IsMoving = false;
    }
    
    public bool MoveBlock(Slot slot, Directions.Type dir)
    {
        if (ReferenceEquals(slot.block, null))
            return false;
        
        var dirVector = dir.ToVector2Int();
        var mergeAbleSlot = slot;
        
        while (true)
        {
            var findVector = mergeAbleSlot.pos + dirVector;
            
            if(!slots.ContainsKey(findVector))
                break;

            var trySlot = slots[findVector];

            if (trySlot.IsMergeAble(slot.block))
                mergeAbleSlot = trySlot;
            else
                break;
        }

        if (mergeAbleSlot != slot)
        {
            mergeAbleSlot.SetBlock(slot.block, true).Forget();
            slot.RemoveBlock();

            return true;
        }
        else
            return false;
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
    }

    #region Initialize

    private void InitSlotPool()
    {
        slotPool = new ObjectPool<Slot>
        (
            () =>
            {
                var obj = Instantiate(slotOrigin, parent);
                return obj.GetComponent<Slot>();
            },
            (e) =>
            {
                e.OnGet();
            },
            (e) =>
            {
                e.OnRelease();
                slots.Remove(e.pos);
            }
        );
    }

    private void InitBlockPool()
    {
        blockPool = new ObjectPool<Block>
        (
            () =>
            {
                var obj = Instantiate(blockOrigin, parent);
                return obj.GetComponent<Block>();
            },
            (e) =>
            {
                e.OnGet();
            },
            (e) =>
            {
                e.OnRelease();
            }
        );
    }

    public void Release(Block block)
    {
        blockPool.Release(block);
    }
    
    #endregion

    private bool isDragging;
    
    private void Update()
    {
        if(isDragging)
            return;
        
        if (Input.GetMouseButtonDown(0))
            OnDrag(Input.mousePosition).Forget();

        if (Input.GetKeyDown(KeyCode.W))
            MoveAllBlock(Directions.Type.Up);
        else if (Input.GetKeyDown(KeyCode.S))
            MoveAllBlock(Directions.Type.Down);
        else if (Input.GetKeyDown(KeyCode.A))
            MoveAllBlock(Directions.Type.Left);
        else if (Input.GetKeyDown(KeyCode.D))
            MoveAllBlock(Directions.Type.Right);
    }

    private async UniTask OnDrag(Vector3 pos)
    {
        isDragging = true;
        
        var start = pos;

        while (Input.GetMouseButton(0))
        {
            var end = Input.mousePosition;
            var scrollDelta = end - start;

            if (scrollDelta.x == 0 && scrollDelta.y == 0)
            {
                await UniTask.WaitForFixedUpdate();
                continue;
            }

            if (MathF.Abs(scrollDelta.x) > MathF.Abs(scrollDelta.y))
                await MoveAllBlock(scrollDelta.x > 0 ? Directions.Type.Right : Directions.Type.Left);
            else
                await MoveAllBlock(scrollDelta.y > 0 ? Directions.Type.Up : Directions.Type.Down);
            
            break;
        }

        isDragging = false;
    }
}
