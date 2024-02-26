using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    bool m_FireInputWasHeld;

    void LateUpdate()
    {
        m_FireInputWasHeld = GetFireInputHeld();
    }

    public int GetSelectWeaponInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            return 1;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            return 2;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            return 3;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            return 4;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            return 5;
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            return 6;
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            return 7;
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            return 8;
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            return 9;
        else
            return 0;
    }

    public bool GetFireInputHeld()
    {
        return Input.GetButton("Fire1");
    }

    public bool GetFireInputDown()
    {
        return GetFireInputHeld() && !m_FireInputWasHeld;
    }

    public bool GetFireInputReleased()
    {
        return !GetFireInputHeld() && m_FireInputWasHeld;
    }
}
