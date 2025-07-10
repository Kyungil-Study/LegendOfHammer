using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class SquadController : MonoBehaviour
{
    public Squad squad;
    public Rigidbody2D squadRigidbody;
    public Warrior warrior;
    private Vector3 m_TouchStartPosition;
    
    private Camera m_Camera;
    public float multiTapGap = 0.2f;
    private float m_LastTapTime;
    
    public GameObject lever;
    public SpriteRenderer outerCircle;
    public SpriteRenderer middleCircle;
    public GameObject innerCircle;
    private float m_LeverRadius;
    private float m_LeverThreshold;

    public Transform min;
    public Transform max;
    
    private void Start()
    {
        m_Camera = Camera.main;

        m_LeverRadius = (outerCircle.bounds.size / 2).x;
        float middleRadius = (middleCircle.bounds.size / 2).x;
        m_LeverThreshold = middleRadius / m_LeverRadius;
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
                if (Vector2.Distance(touchPosition, lever.transform.position) > m_LeverRadius)
                {
                    m_TouchStartPosition = touchPosition;
                    lever.transform.position = touchPosition;
                }
                
                // Check for multi-tap
                if (m_LastTapTime + multiTapGap > Time.time)
                {
                    Vector3 direction = touchPosition - lever.transform.position;
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
                    squad.transform.position += dir * (Squad.STANDARD_DISTANCE * squad.stats.MoveSpeed * Time.deltaTime);
                }
            }
        }
    }

    private void LateUpdate()
    {
        float x = Mathf.Clamp(squad.transform.position.x, min.position.x, max.position.x);
        float y = Mathf.Clamp(squad.transform.position.y, min.position.y, max.position.y);
        
        squad.transform.position = new Vector3(x, y, squad.transform.position.z);
    }
}