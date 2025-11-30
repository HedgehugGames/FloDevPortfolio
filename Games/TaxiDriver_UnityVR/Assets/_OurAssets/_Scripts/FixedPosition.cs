using System;
using UnityEngine;

public class FixedPosition : MonoBehaviour
{
    [SerializeField] private Transform xrOrigin;
    [SerializeField] private Transform fixedPosition;


    void Start()
    {
        xrOrigin.position = fixedPosition.position;
    }

    void LateUpdate()
    {
        xrOrigin.position = fixedPosition.position;
    }
}