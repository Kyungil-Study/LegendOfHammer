using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPageFlowManageable
{
    public void SwapPage(UIPageType nextPageType);
}

public enum UIPageType
{
    BattlePage,
    ClassAumgentSelection,
    CommonAugmentSelection,
    ClearPage,
    GameOverPage,
    PausePage,
    LobbyPage,
}

public abstract class UIPage : MonoBehaviour
{
    public abstract UIPageType UIPageType { get; }
    public abstract void Initialize(IPageFlowManageable owner);
    public abstract void Enter();
    public abstract void Exit();
}
