using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponShootType
{
    Manual,
    Automatic,
}
public class WeaponController : MonoBehaviour
{
    [Header("Information")]
    public string WeaponName;

    [Header("Attributes")]
    public WeaponShootType ShootType;
    public Projectile ProjectilePrefab;
    public float FireCoolDown = 1.0f;
    public float BulletSpreadAngle = 0.0f;
    public int BulletsPerShot = 1;

    [Header("References")]
    public GameObject WeaponRoot;
    public Transform WeaponMuzzle;

    [HideInInspector]
    public bool IsWeaponActive { get; private set; }


    float m_LastTimeShot = Mathf.NegativeInfinity;

    public void ShowWeapon(bool show)
    {
        WeaponRoot.SetActive(show);
        IsWeaponActive = show;
    }

    public bool HandleShootInputs(bool inputDown, bool inputHeld)
    {
        switch (ShootType)
        {
            case WeaponShootType.Manual:
                if (inputDown)
                {
                    return TryShoot();
                }
                return false;

            case WeaponShootType.Automatic:
                if (inputHeld)
                {
                    return TryShoot();
                }
                return false;

            default:
                return false;
        }
    }
    bool TryShoot()
    {
        if (m_LastTimeShot + FireCoolDown < Time.time)
        {
            HandleShoot();
            return true;
        }

        return false;
    }
    Vector3 GetShotDirection(Transform shootTransform)
    {
        float spreadAngleRatio = BulletSpreadAngle / 180f;
        Vector3 spreadWorldDirection = Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere, spreadAngleRatio);

        return spreadWorldDirection;
    }

    void HandleShoot()
    {
        // Spawn bullets
        for (int i = 0; i < BulletsPerShot; i++)
        {
            Vector3 shotDirection = GetShotDirection(WeaponMuzzle);
            Instantiate(ProjectilePrefab, WeaponMuzzle.position, Quaternion.LookRotation(shotDirection));
        }
        m_LastTimeShot = Time.time;
    }

}
