using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    private const string m_Horizontal = "Horizontal";
    private const string m_Vertical = "Vertical";

    public float speed = 6f;

    private void Update()
    {
        float k_horiz = Input.GetAxisRaw(m_Horizontal);
        float k_verti = Input.GetAxisRaw(m_Vertical);
        
        Vector3 direction = new Vector3(k_horiz, 0, k_verti).normalized;

        if (direction.magnitude >= 0.1f)
        {
            transform.position += (direction * (speed * Time.deltaTime));
        }
        
    }
}
