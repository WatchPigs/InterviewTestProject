using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Configurations")]
    public float MaxLifeTime = 3.0f;
    public float Speed = 20.0f;

    Rigidbody m_rigidbody;
    Vector3 m_velocity;

    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        Destroy(gameObject, MaxLifeTime);
        m_velocity = transform.forward * Speed;
        m_rigidbody.velocity = m_velocity;
    }
}
