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
    private Squad m_Squad;
    public Rigidbody2D squadRigidbody;
    public Warrior warrior;
    private Vector3 m_TouchStartPosition;
    
    private Camera m_Camera;
    public float multiTapGap = 0.2f;
    private float m_LastTapTime;
    
    public GameObject lever;
    //public SpriteRenderer outerCircle;
    //public SpriteRenderer middleCircle;
    public RectTransform outerCircle;
    public RectTransform middleCircle;
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
        m_Squad = Squad.Instance;
        
        m_LeverRadius = (outerCircle.rect.width * 0.5f) * outerCircle.GetComponentInParent<Canvas>().scaleFactor;
        float middleRadius = m_LeverRadius * middleCircle.transform.localScale.x;
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
            Vector3 direction = inputPosition - m_Camera.WorldToScreenPoint(m_Squad.transform.position);
            warrior.ChargeAttack(direction);
        }
        else
        {
            m_LastTapTime = Time.time;
        }
    }

    private Vector3 ReadPointerPosition()
    {
        return pointerPosition.action.ReadValue<Vector2>();
    }
    
    private void Move()
    {
        var inputPosition = ReadPointerPosition();
        Vector3 dir = inputPosition - m_TouchStartPosition;
                
        dir = dir.normalized * Mathf.InverseLerp(0, m_LeverRadius, dir.magnitude);
                
        innerCircle.transform.position = lever.transform.position + dir * m_LeverRadius;
        
        if (dir.magnitude > m_LeverThreshold)
        {
            m_Squad.transform.position += dir * (Squad.STANDARD_DISTANCE * m_Squad.stats.MoveSpeed * Time.deltaTime);
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
        float x = Mathf.Clamp(m_Squad.transform.position.x, min.position.x, max.position.x);
        float y = Mathf.Clamp(m_Squad.transform.position.y, min.position.y, max.position.y);
        
        m_Squad.transform.position = new Vector3(x, y, m_Squad.transform.position.z);
    }
}