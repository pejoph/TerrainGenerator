/// ================================
/// Peter Phillips, 2022
/// ================================


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 100f;
    //public float sprintFactor = 2f;

    private Rigidbody rb;
    private Vector3 direction = Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Set and normalise direction vector.
        direction.x = Input.GetAxisRaw("Horizontal");
        direction.z = Input.GetAxisRaw("Vertical");
        direction.Normalize();
        // Multiply direction vector by speed to get velocity, modify value if holding L-shift.
        rb.velocity = direction * speed;// * (Input.GetKey(KeyCode.LeftShift) ? sprintFactor : 1);
    }
}
