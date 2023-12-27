using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolAble
{
    public void OnGet();
    public void OnRelease();
}
