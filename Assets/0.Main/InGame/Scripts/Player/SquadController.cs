using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadController : MonoBehaviour
{
    public Squad squad;
    public Warrior warrior;
    private Vector2 m_TouchStartPosition;

    public float multiTapGap = 0.2f;
    private float m_LastTapTime;
    
    private void Awake()
    {
        squad = GetComponent<Squad>();
    }
    
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                m_TouchStartPosition = Input.touches[0].position;
                
                // Check for multi-tap
                if (m_LastTapTime + multiTapGap > Time.time)
                {
                    Vector3 direction = (Vector3)Input.touches[0].position - transform.position;
                    warrior.ChargeAttack(direction);
                }
                else
                {
                    m_LastTapTime = Time.time;
                }
            }
            else
            {
                Vector3 dir = Input.touches[0].position - m_TouchStartPosition;
                dir.Normalize();

                squad.transform.position += dir * (Squad.BASE_MOVE_SPEED * squad.stats.MoveSpeed * Time.deltaTime);
            }
        }
    }
}
