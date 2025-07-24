using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZigzagMove : IMoveBehaviour
{
    private Monster mMonster;
    private ZigzagMoveConfig mConfig;

    // 런타임용 내부 상태
    private float baseLineX;
    private float targetBaseLineX;
    private float newBaseLineX;
    private float zigzagTime;
    private float cycleLength;
    private int   nextCycle;
    private bool  hasHitted;
    private float shiftSpeed;

    public ZigzagMove(ZigzagMoveConfig cfg)
    {
        mConfig = cfg;
    }

    public void Init(Monster monster)
    {
        mMonster = monster;

        baseLineX       = mMonster.transform.position.x;
        targetBaseLineX = baseLineX;
        zigzagTime      = 0f;
        cycleLength     = 2f * Mathf.PI / mConfig.frequency;
        hasHitted       = false;
        shiftSpeed      = 0f;
    }

    public void Tick(float time)
    {
        zigzagTime += time;

        if (hasHitted)
        {
            int currentCycle = Mathf.FloorToInt(zigzagTime / cycleLength);
            
            if (currentCycle >= nextCycle)
            {
                hasHitted      = false;
                targetBaseLineX= newBaseLineX;
                float shiftDist= targetBaseLineX - baseLineX;
                shiftSpeed     = Mathf.Abs(shiftDist) / cycleLength;
            }
        }

        baseLineX = Mathf.MoveTowards(baseLineX, targetBaseLineX, shiftSpeed * time);

        float x = baseLineX + Mathf.Sin(zigzagTime * mConfig.frequency) * mConfig.amplitude;
        float y = mMonster.transform.position.y - mMonster.Stat.FinalStat.MoveSpeed * time;
        
        mMonster.transform.position = new Vector2(x, y);
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 8)
        {
            float middle = (MapManager.Instance.LeftBoundX + MapManager.Instance.RightBoundX) * 0.5f;
            float shift  = (mMonster.transform.position.x < middle) ? mConfig.offsetX : -mConfig.offsetX;

            hasHitted    = true;
            newBaseLineX = baseLineX + shift;
            nextCycle    = Mathf.FloorToInt(zigzagTime / cycleLength);
        }
    }
}