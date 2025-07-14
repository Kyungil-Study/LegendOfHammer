using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using TouchPhase = UnityEngine.TouchPhase;

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

    public InputActionAsset inputActionAsset;
    public InputActionReference pointerPress;
    public InputActionReference pointerPosition;
    private bool mb_IsPointerPressed = false;
    
    private void Start()
    {
        m_Camera = Camera.main;

        m_LeverRadius = (outerCircle.bounds.size / 2).x;
        float middleRadius = (middleCircle.bounds.size / 2).x;
        m_LeverThreshold = middleRadius / m_LeverRadius;
        
        inputActionAsset.Enable();

        pointerPress.action.performed += StartMove;
        pointerPress.action.canceled += context =>
        {
            mb_IsPointerPressed = false;
            innerCircle.transform.position = lever.transform.position;
            m_DisappearTime = disappearTime;
        };
    }

    private void StartMove(InputAction.CallbackContext context)
    {
        mb_IsPointerPressed = true;
        lever.SetActive(true);
        var inputPosition = ReadPointerPosition();
        if (Vector2.Distance(inputPosition, lever.transform.position) > m_LeverRadius)
        {
            m_TouchStartPosition = inputPosition;
            lever.transform.position = inputPosition;
        }
        // Check for multi-tap
        if (m_LastTapTime + multiTapGap > Time.time)
        {
            Vector3 direction = inputPosition - lever.transform.position;
            warrior.ChargeAttack(direction);
        }
        else
        {
            m_LastTapTime = Time.time;
        }
    }

    private Vector3 ReadPointerPosition()
    {
        var reVal = m_Camera.ScreenToWorldPoint(pointerPosition.action.ReadValue<Vector2>());
        reVal.z = 0;
        return reVal;
    }
    
    private void Move()
    {
        var inputPosition = ReadPointerPosition();
        Vector3 dir = inputPosition - m_TouchStartPosition;
                
        dir = dir.normalized * Mathf.InverseLerp(0, m_LeverRadius, dir.magnitude);
                
        innerCircle.transform.position = lever.transform.position + dir * m_LeverRadius;
        
        if (dir.magnitude > m_LeverThreshold)
        {
            squad.transform.position += dir * (Squad.STANDARD_DISTANCE * squad.stats.MoveSpeed * Time.deltaTime);
        }
    }

    public float disappearTime = 1f;
    private float m_DisappearTime;
    private void Update()
    {
        if (mb_IsPointerPressed)
        {
            Move();
        }
        else
        {
            m_DisappearTime -= Time.deltaTime;
            if (m_DisappearTime <= 0)
            {
                lever.SetActive(false);
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