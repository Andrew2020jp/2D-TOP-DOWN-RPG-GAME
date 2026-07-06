using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    private void Update()
    {
        FaceMouse();
    }

    private void FaceMouse()
    {
        // aims with mouse or gamepad right stick
        transform.right = AimInput.GetAimDirection(transform.position);
    }
}