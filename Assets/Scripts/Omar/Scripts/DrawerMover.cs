using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerMover : MonoBehaviour
{
    [System.Serializable]
    private class ExtraMoveTarget
    {
        public Transform target;
        public float targetX;
    }

    [Header("Main Target")]
    [SerializeField] private Transform target;

    [Header("Main Movement (solo X, local)")]
    [SerializeField] private float targetX = 0f;
    [SerializeField] private float moveSpeed = 1.5f;

    [Header("Extra Targets To Move With Drawer")]
    [SerializeField] private List<ExtraMoveTarget> extraTargets = new List<ExtraMoveTarget>();

    private bool isMoving;

    public void Open()
    {
        if (isMoving)
        {
            return;
        }

        StartCoroutine(MoveToXCoroutine());
    }

    private IEnumerator MoveToXCoroutine()
    {
        isMoving = true;

        Transform mainTarget = target != null ? target : transform;

        Vector3 mainStart = mainTarget.localPosition;
        Vector3 mainEnd = new Vector3(targetX, mainStart.y, mainStart.z);

        List<Vector3> extraStarts = new List<Vector3>();
        List<Vector3> extraEnds = new List<Vector3>();

        for (int i = 0; i < extraTargets.Count; i++)
        {
            if (extraTargets[i] == null || extraTargets[i].target == null)
            {
                extraStarts.Add(Vector3.zero);
                extraEnds.Add(Vector3.zero);
                continue;
            }

            Vector3 extraStart = extraTargets[i].target.localPosition;
            Vector3 extraEnd = new Vector3(extraTargets[i].targetX, extraStart.y, extraStart.z);

            extraStarts.Add(extraStart);
            extraEnds.Add(extraEnd);
        }

        while (Mathf.Abs(mainTarget.localPosition.x - mainEnd.x) > 0.001f)
        {
            float step = moveSpeed * Time.deltaTime;

            float newMainX = Mathf.MoveTowards(mainTarget.localPosition.x, mainEnd.x, step);
            mainTarget.localPosition = new Vector3(newMainX, mainStart.y, mainStart.z);

            for (int i = 0; i < extraTargets.Count; i++)
            {
                if (extraTargets[i] == null || extraTargets[i].target == null)
                {
                    continue;
                }

                Transform extraTarget = extraTargets[i].target;
                Vector3 extraEnd = extraEnds[i];
                Vector3 extraStart = extraStarts[i];

                float newExtraX = Mathf.MoveTowards(extraTarget.localPosition.x, extraEnd.x, step);
                extraTarget.localPosition = new Vector3(newExtraX, extraStart.y, extraStart.z);
            }

            yield return null;
        }

        mainTarget.localPosition = mainEnd;

        for (int i = 0; i < extraTargets.Count; i++)
        {
            if (extraTargets[i] == null || extraTargets[i].target == null)
            {
                continue;
            }

            Transform extraTarget = extraTargets[i].target;
            Vector3 extraEnd = extraEnds[i];
            extraTarget.localPosition = extraEnd;
        }

        isMoving = false;
    }
}