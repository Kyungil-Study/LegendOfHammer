using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapEventExecutor<T> : MonoSingleton<MapEventExecutor<T>>
{
    public abstract void ExecuteMapEvent();
}
