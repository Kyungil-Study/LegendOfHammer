using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadController : MonoBehaviour
{
    public Squad squad;
    private Vector2 m_TouchStartPosition;
    
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                m_TouchStartPosition = Input.touches[0].position;
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
