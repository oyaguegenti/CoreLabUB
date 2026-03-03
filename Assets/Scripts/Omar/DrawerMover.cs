using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerMover : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target; // Si es null, usa este transform

    [Header("Movement (solo X, local)")]
    [SerializeField] private float targetX = 0f;
    [SerializeField] private float moveSpeed = 1.5f;

    [Header("Physics children while moving")]
    [SerializeField] private bool disableChildrenPhysicsWhileMoving = true;

    private bool isMoving;

    private struct RBState
    {
        public Rigidbody rb;
        public bool wasKinematic;
        public bool hadGravity;
    }

    private readonly List<RBState> cachedStates = new List<RBState>();

    public void Open()
    {
        if (isMoving) return;
        StartCoroutine(MoveToXCoroutine(targetX));
    }

    private IEnumerator MoveToXCoroutine(float destX)
    {
        isMoving = true;

        Transform t = target != null ? target : transform;

        if (disableChildrenPhysicsWhileMoving)
            DisableChildrenPhysics(t);

        Vector3 start = t.localPosition;
        Vector3 end = new Vector3(destX, start.y, start.z);

        while (Mathf.Abs(t.localPosition.x - end.x) > 0.001f)
        {
            float step = moveSpeed * Time.deltaTime;
            float newX = Mathf.MoveTowards(t.localPosition.x, end.x, step);
            t.localPosition = new Vector3(newX, start.y, start.z);
            yield return null;
        }

        t.localPosition = end;

        if (disableChildrenPhysicsWhileMoving)
            RestoreChildrenPhysics();

        isMoving = false;
    }

    private void DisableChildrenPhysics(Transform root)
    {
        cachedStates.Clear();

        var rbs = root.GetComponentsInChildren<Rigidbody>(true);

        foreach (var rb in rbs)
        {
            // Ignoramos el Rigidbody del propio root
            if (rb.transform == root)
                continue;

            cachedStates.Add(new RBState
            {
                rb = rb,
                wasKinematic = rb.isKinematic,
                hadGravity = rb.useGravity
            });

            rb.isKinematic = true;
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void RestoreChildrenPhysics()
    {
        foreach (var s in cachedStates)
        {
            if (s.rb == null) continue;
            s.rb.isKinematic = s.wasKinematic;
            s.rb.useGravity = s.hadGravity;
        }

        cachedStates.Clear();
    }
}