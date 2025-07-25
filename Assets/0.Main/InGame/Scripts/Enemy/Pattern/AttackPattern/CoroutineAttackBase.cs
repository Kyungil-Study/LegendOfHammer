using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CoroutineAttackBase : IAttackBehaviour
{
    protected Monster mMonster;
    Coroutine mLoop;

    public virtual void Init(Monster monster) => mMonster = monster;
    public virtual void Start() { }
    public virtual void Tick(float time) { }
    public virtual void Stop()
    {
        if (mLoop != null)
        {
            mMonster.StopCoroutine(mLoop);
            mLoop = null;
        }
    }
    public virtual void OnTriggerEnter2D(Collider2D col) { }

    protected void StartLoop(IEnumerator routine)
    {
        Stop();
        mLoop = mMonster.StartCoroutine(routine);
    }
}