// LuckyWheelPhysics.cs
using UnityEngine;

public class LuckyWheelPhysics : MonoBehaviour
{
    public Rigidbody2D rb;
    public float torqueForce = 500f;
    public float damping = 0.99f;

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    public void Spin(float force)
    {
        rb.AddTorque(force, ForceMode2D.Impulse);
    }

    void FixedUpdate()
    {
        rb.angularVelocity *= damping;
    }
}
