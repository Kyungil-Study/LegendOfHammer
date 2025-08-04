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
    public IPageFlowManageable Owner { get; private set; }
    public abstract UIPageType UIPageType { get; }
    
    [RuleTile.DontOverride]
    public void Setup(IPageFlowManageable owner)
    {
        Owner = owner;
        Initialize(owner); // Initialize the page with the owner
    }

    protected abstract void Initialize(IPageFlowManageable owner);
    
    public abstract void Enter();
    public abstract void Exit();
}
