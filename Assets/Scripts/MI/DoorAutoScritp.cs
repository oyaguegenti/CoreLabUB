using UnityEngine;

public class DoorAutoScript : MonoBehaviour
{
    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;
    [SerializeField] private float velocidad = 1f;
    [SerializeField] private float distanciaObjetivo = 1f;

    private Vector3 leftClosedPos;
    private Vector3 rightClosedPos;
    private Vector3 leftOpenPos;
    private Vector3 rightOpenPos;

    private enum Estado { Closed, Opening, Opened, Closing }
    private Estado estado = Estado.Closed;

    void Start()
    {
        // Guardamos las posiciones cerradas
        leftClosedPos = leftDoor.position;
        rightClosedPos = rightDoor.position;

        // Calculamos posiciones abiertas
        // movemos leftDoor +X y rightDoor -X
        leftOpenPos = leftClosedPos + Vector3.right * distanciaObjetivo;
        rightOpenPos = rightClosedPos - Vector3.right * distanciaObjetivo;
    }

    void OnTriggerEnter(Collider other)
    {
        if (estado == Estado.Closed || estado == Estado.Closing)
        {
            estado = Estado.Opening;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (estado == Estado.Opened || estado == Estado.Opening)
        {
            estado = Estado.Closing;
        }
    }

    void Update()
    {
        float step = velocidad * Time.deltaTime;

        switch (estado)
        {
            case Estado.Opening:
                // Mover hacia openPos
                leftDoor.position = Vector3.MoveTowards(leftDoor.position, leftOpenPos, step);
                rightDoor.position = Vector3.MoveTowards(rightDoor.position, rightOpenPos, step);
                // Si llegaron, cambian a Opened
                if (leftDoor.position == leftOpenPos && rightDoor.position == rightOpenPos)
                    estado = Estado.Opened;
                break;

            case Estado.Closing:
                // Mover hacia closedPos
                leftDoor.position = Vector3.MoveTowards(leftDoor.position, leftClosedPos, step);
                rightDoor.position = Vector3.MoveTowards(rightDoor.position, rightClosedPos, step);
                // Si llegaron, cambian a Closed
                if (leftDoor.position == leftClosedPos && rightDoor.position == rightClosedPos)
                    estado = Estado.Closed;
                break;

            // En estados Closed u Opened no hacemos nada
            case Estado.Closed:
            case Estado.Opened:
            default:
                break;
        }
    }
}
