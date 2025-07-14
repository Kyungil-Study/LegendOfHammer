using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPositioner : MonoBehaviour
{
    public Transform worldTarget;
    private Camera m_Camera;

    private void Start()
    {
        m_Camera = Camera.main;
    }

    private void Update()
    {
        transform.position = m_Camera.WorldToScreenPoint(worldTarget.position);
    }
}
