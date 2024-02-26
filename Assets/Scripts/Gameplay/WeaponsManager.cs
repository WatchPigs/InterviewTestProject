using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsManager : MonoBehaviour
{
    public enum WeaponSwitchState
    {
        // A weapon is in the player's hands and ready
        Up,
        // No weapon is in the player's hands
        Down,
        // On putting down the previous one
        PutDownPrevious,
        // On putting up the new one
        PutUpNew,
    }

    [Header("References")]
    public Transform WeaponSocket;
    public Transform DefaultWeaponPosition;
    public Transform DownWeaponPosition;

    [Header("Configurations")]
    public float WeaponSwitchCoolDown = 1f;
    public List<WeaponController> StartingWeapons = new List<WeaponController>();

    [HideInInspector]
    public int ActiveWeaponIndex { get; private set; }

    WeaponController[] m_WeaponSlots = new WeaponController[9];

    Vector3 m_WeaponMainLocalPosition;
    InputHandler m_InputHandler;
    WeaponSwitchState m_WeaponSwitchState;
    float m_TimeStartedWeaponSwitch;
    int m_WeaponSwitchNewWeaponIndex;

    public bool AddWeapon(WeaponController weaponPrefab)
    {
        for (int i = 0; i < m_WeaponSlots.Length; i++)
        {
            // find empty slot
            if (m_WeaponSlots[i] == null)
            {
                // spawn the weapon prefab as child of the weapon socket
                WeaponController weaponInstance = Instantiate(weaponPrefab, WeaponSocket);
                weaponInstance.transform.localPosition = Vector3.zero;
                weaponInstance.transform.localRotation = Quaternion.identity;

                weaponInstance.ShowWeapon(false);
                m_WeaponSlots[i] = weaponInstance;

                return true;
            }
        }
        return false;
    }

    void Start()
    {
        ActiveWeaponIndex = -1;
        m_WeaponSwitchState = WeaponSwitchState.Down;
        m_InputHandler = GetComponent<InputHandler>();

        foreach (var weapon in StartingWeapons)
        {
            AddWeapon(weapon);
        }
        if (StartingWeapons.Count > 0)
        {
            SwitchToWeaponIndex(0);
        }
    }

    void Update()
    {
        HandleShooting();
        HandleWeaponSwitching();
    }

    void LateUpdate()
    {
        UpdateWeaponSwitching();
        WeaponSocket.localPosition = m_WeaponMainLocalPosition;
    }

    WeaponController GetWeaponAtIndex(int index)
    {
        if (index >= 0 &&
            index < m_WeaponSlots.Length)
        {
            return m_WeaponSlots[index];
        }
        return null;
    }

    void SwitchToWeaponIndex(int newWeaponIndex)
    {
        m_WeaponSwitchNewWeaponIndex = newWeaponIndex;
        m_TimeStartedWeaponSwitch = Time.time;

        if (newWeaponIndex != ActiveWeaponIndex && newWeaponIndex >= 0)
        {
            // If there is no active weapon on player's hand
            if (GetWeaponAtIndex(ActiveWeaponIndex) == null)
            {
                m_WeaponMainLocalPosition = DownWeaponPosition.localPosition;
                m_WeaponSwitchState = WeaponSwitchState.PutUpNew;
                ActiveWeaponIndex = m_WeaponSwitchNewWeaponIndex;

                WeaponController newWeapon = GetWeaponAtIndex(m_WeaponSwitchNewWeaponIndex);
                newWeapon.ShowWeapon(true);
            }
            else
            {
                m_WeaponSwitchState = WeaponSwitchState.PutDownPrevious;
            }
        }
    }
    void HandleShooting()
    {
        WeaponController activeWeapon = GetWeaponAtIndex(ActiveWeaponIndex);

        if (activeWeapon != null && m_WeaponSwitchState == WeaponSwitchState.Up)
        {
            activeWeapon.HandleShootInputs(m_InputHandler.GetFireInputDown(), m_InputHandler.GetFireInputHeld());
        }
    }

    void HandleWeaponSwitching()
    {
        if (m_WeaponSwitchState == WeaponSwitchState.Up || m_WeaponSwitchState == WeaponSwitchState.Down)
        {
            int switchWeaponInput = m_InputHandler.GetSelectWeaponInput();
            if (switchWeaponInput != 0)
            {
                if (GetWeaponAtIndex(switchWeaponInput - 1) != null) SwitchToWeaponIndex(switchWeaponInput - 1);
            }
        }
    }

    void UpdateWeaponSwitching()
    {
        // The time factor since weapon switch was triggered
        float switchingTimeFactor = 0.0f;
        if (WeaponSwitchCoolDown == 0.0f)
        {
            switchingTimeFactor = 1.0f;
        }
        else
        {
            switchingTimeFactor = Mathf.Clamp01((Time.time - m_TimeStartedWeaponSwitch) / (WeaponSwitchCoolDown / 2.0f));
        }

        // weapons up/down finished
        if (switchingTimeFactor >= 1.0f)
        {
            // Done putting down old weapon
            if (m_WeaponSwitchState == WeaponSwitchState.PutDownPrevious)
            {
                // Deactivate old weapon
                WeaponController oldWeapon = GetWeaponAtIndex(ActiveWeaponIndex);
                if (oldWeapon != null)
                {
                    oldWeapon.ShowWeapon(false);
                }
                ActiveWeaponIndex = m_WeaponSwitchNewWeaponIndex;
                switchingTimeFactor = 0.0f;

                // Activate new weapon
                WeaponController newWeapon = GetWeaponAtIndex(ActiveWeaponIndex);
                if (newWeapon != null)
                {
                    newWeapon.ShowWeapon(true);
                }
                if (newWeapon)
                {
                    m_TimeStartedWeaponSwitch = Time.time;
                    m_WeaponSwitchState = WeaponSwitchState.PutUpNew;
                }
                else
                {
                    m_WeaponSwitchState = WeaponSwitchState.Down;
                }
            }
            // Done putting up new weapon
            else if (m_WeaponSwitchState == WeaponSwitchState.PutUpNew)
            {
                m_WeaponSwitchState = WeaponSwitchState.Up;
            }
        }
        // handle move weapon down and up
        if (m_WeaponSwitchState == WeaponSwitchState.PutDownPrevious)
        {
            m_WeaponMainLocalPosition = Vector3.Lerp(DefaultWeaponPosition.localPosition, DownWeaponPosition.localPosition, switchingTimeFactor);
        }
        else if (m_WeaponSwitchState == WeaponSwitchState.PutUpNew)
        {
            Debug.Log("putting up");
            m_WeaponMainLocalPosition = Vector3.Lerp(DownWeaponPosition.localPosition, DefaultWeaponPosition.localPosition, switchingTimeFactor);
        }
    }


}
