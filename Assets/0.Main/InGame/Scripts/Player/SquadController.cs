using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class SquadController : MonoBehaviour
{
    public Squad squad;
    public Warrior warrior;
    private Vector3 m_TouchStartPosition;
    
    private Camera m_Camera;
    public float multiTapGap = 0.2f;
    private float m_LastTapTime;
    
    public GameObject lever;
    private float m_LeverRadius;
    private float m_LeverThreshold;
    public SpriteRenderer outerCircle;
    public SpriteRenderer middleCircle;
    public GameObject innerCircle;

    private void Start()
    {
        m_Camera = Camera.main;

        m_LeverRadius = (outerCircle.bounds.size / 2).x;
        float middleRadius = (middleCircle.bounds.size / 2).x;
        m_LeverThreshold = middleRadius / m_LeverRadius;
        Debug.Log($"{middleRadius} / {m_LeverRadius} = {m_LeverThreshold}");
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            var touchPosition = m_Camera.ScreenToWorldPoint(touch.position);
            touchPosition.z = 0;
            
            if (touch.phase == TouchPhase.Began)
            {
                m_TouchStartPosition = touchPosition;
                lever.transform.position = touchPosition;
                
                // Check for multi-tap
                if (m_LastTapTime + multiTapGap > Time.time)
                {
                    Vector3 direction = touchPosition - warrior.transform.position;
                    warrior.ChargeAttack(direction);
                }
                else
                {
                    m_LastTapTime = Time.time;
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                innerCircle.transform.position = lever.transform.position;
            }
            else
            {
                Vector3 dir = touchPosition - m_TouchStartPosition;
                
                dir = dir.normalized * Mathf.InverseLerp(0, m_LeverRadius, dir.magnitude);
                
                innerCircle.transform.position = lever.transform.position + dir * m_LeverRadius;

                if (dir.magnitude > m_LeverThreshold)
                {
                    squad.transform.position += dir * (Squad.BASE_MOVE_SPEED * squad.stats.MoveSpeed * Time.deltaTime);
                }
            }
        }
    }
}
