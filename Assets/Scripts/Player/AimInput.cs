using UnityEngine;
using UnityEngine.InputSystem;

// Central aim helper so weapons and the player work with both mouse and gamepad.
// Priority: recently-used right stick > currently-held left stick > mouse.
public static class AimInput
{
    private const float STICK_DEADZONE_SQ = 0.04f;
    private const float STICK_AIM_HOLD_SECONDS = 3f; // keep aiming where the stick last pointed

    private static Vector2 lastStickAim = Vector2.right;
    private static float lastStickTime = float.NegativeInfinity;

    // world-space direction from the given origin toward where the player is aiming
    public static Vector2 GetAimDirection(Vector3 origin)
    {
        Gamepad gamepad = Gamepad.current;
        if (gamepad != null)
        {
            Vector2 rightStick = gamepad.rightStick.ReadValue();
            if (rightStick.sqrMagnitude > STICK_DEADZONE_SQ)
            {
                lastStickAim = rightStick.normalized;
                lastStickTime = Time.unscaledTime;
            }
            if (Time.unscaledTime - lastStickTime <= STICK_AIM_HOLD_SECONDS)
            {
                return lastStickAim;
            }

            // no recent right-stick input: aim along the movement stick if it is held
            Vector2 leftStick = gamepad.leftStick.ReadValue();
            if (leftStick.sqrMagnitude > STICK_DEADZONE_SQ)
            {
                return leftStick.normalized;
            }
        }

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mouseWorld - origin;
        return direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
    }
}
