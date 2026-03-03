using UnityEngine;

public class JiggleBone : MonoBehaviour
{
    [Header("Settings")]
    public float stiffness = 0.2f;     // Qué tanto intenta volver a su posición
    public float damping = 0.3f;       // Cuánto se frena el movimiento
    public Vector3 gravity = new Vector3(0, -1f, 0);

    private Vector3 velocity;
    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
    }

    void LateUpdate()
    {
        Vector3 force = gravity;
        velocity += force * Time.deltaTime;

        Vector3 targetPos = transform.parent.position;
        Vector3 currentPos = transform.position;

        velocity += (targetPos - currentPos) * stiffness;
        velocity *= (1f - damping);

        transform.position += velocity;

        lastPosition = transform.position;
    }
}
