using System;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    [SerializeField] private ShipData data; 

    void Update()
    {
        float move = Input.GetAxis("Vertical") * data.speed * Time.deltaTime;
        float turn = Input.GetAxis("Horizontal") * data.turnSpeed * Time.deltaTime;

        transform.Translate(Vector3.forward * move);
        transform.Rotate(Vector3.up * turn);
    }
}
