using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetroEntranceReader : MonoBehaviour
{
    [Header("Valid card permission")]
    [SerializeField] private CardPerms requiredPerm = CardPerms.Entrance;

    [Header("Door pivots")]
    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;

    [Header("Open rotation (local, absolute)")]
    [SerializeField] private Vector3 leftDoorOpenLocalEuler;
    [SerializeField] private Vector3 rightDoorOpenLocalEuler;

    [Header("Timing")]
    [SerializeField] private float rotateSpeed = 2f;
    [SerializeField] private float autoCloseDelay = 3f;

    [Header("Door colliders")]
    [SerializeField] private List<Collider> collidersToDisable = new List<Collider>();
    [SerializeField] private bool setCollidersAsTriggerInstead = false;

    [Header("Feedback")]
    [SerializeField] private Renderer feedbackRenderer;
    [SerializeField] private string confirmationSoundName = "CardReaderConfirmation";
    [SerializeField] private string errorSoundName = "CardReaderError";
    [SerializeField] private int numBlinks = 10;

    private Quaternion leftDoorClosedRotation;
    private Quaternion rightDoorClosedRotation;

    private bool isBusy = false;
    private Coroutine doorRoutine;

    private readonly Dictionary<Collider, bool> cachedTriggerStates = new Dictionary<Collider, bool>();

    private void Awake()
    {
        if (leftDoor != null)
        {
            leftDoorClosedRotation = leftDoor.localRotation;
        }

        if (rightDoor != null)
        {
            rightDoorClosedRotation = rightDoor.localRotation;
        }

        if (feedbackRenderer == null && transform.parent != null)
        {
            feedbackRenderer = transform.parent.GetComponent<Renderer>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isBusy)
        {
            return;
        }

        MagneticCard card = other.GetComponent<MagneticCard>();
        if (card == null)
        {
            card = other.GetComponentInParent<MagneticCard>();
        }

        if (card == null)
        {
            return;
        }

        if (card.CheckPerm(requiredPerm))
        {
            if (doorRoutine != null)
            {
                StopCoroutine(doorRoutine);
            }

            doorRoutine = StartCoroutine(OpenCloseRoutine());
        }
        else
        {
            StartCoroutine(BlinkError());
            PlaySound(errorSoundName);
        }
    }

    private IEnumerator OpenCloseRoutine()
    {
        isBusy = true;

        StartCoroutine(BlinkConfirmation());
        PlaySound(confirmationSoundName);

        SetDoorCollidersOpenState(true);

        Quaternion leftOpenRotation = leftDoor != null
            ? Quaternion.Euler(leftDoorOpenLocalEuler)
            : Quaternion.identity;

        Quaternion rightOpenRotation = rightDoor != null
            ? Quaternion.Euler(rightDoorOpenLocalEuler)
            : Quaternion.identity;

        yield return StartCoroutine(RotateDoors(leftOpenRotation, rightOpenRotation));

        yield return new WaitForSeconds(autoCloseDelay);

        yield return StartCoroutine(RotateDoors(leftDoorClosedRotation, rightDoorClosedRotation));

        SetDoorCollidersOpenState(false);

        isBusy = false;
        doorRoutine = null;
    }

    private IEnumerator RotateDoors(Quaternion leftTargetRotation, Quaternion rightTargetRotation)
    {
        bool leftFinished = leftDoor == null;
        bool rightFinished = rightDoor == null;

        while (!leftFinished || !rightFinished)
        {
            if (leftDoor != null && !leftFinished)
            {
                leftDoor.localRotation = Quaternion.Lerp(
                    leftDoor.localRotation,
                    leftTargetRotation,
                    Time.deltaTime * rotateSpeed
                );

                if (Quaternion.Angle(leftDoor.localRotation, leftTargetRotation) <= 0.1f)
                {
                    leftDoor.localRotation = leftTargetRotation;
                    leftFinished = true;
                }
            }

            if (rightDoor != null && !rightFinished)
            {
                rightDoor.localRotation = Quaternion.Lerp(
                    rightDoor.localRotation,
                    rightTargetRotation,
                    Time.deltaTime * rotateSpeed
                );

                if (Quaternion.Angle(rightDoor.localRotation, rightTargetRotation) <= 0.1f)
                {
                    rightDoor.localRotation = rightTargetRotation;
                    rightFinished = true;
                }
            }

            yield return null;
        }
    }

    private void SetDoorCollidersOpenState(bool open)
    {
        for (int i = 0; i < collidersToDisable.Count; i++)
        {
            Collider currentCollider = collidersToDisable[i];
            if (currentCollider == null)
            {
                continue;
            }

            if (setCollidersAsTriggerInstead)
            {
                if (open)
                {
                    if (!cachedTriggerStates.ContainsKey(currentCollider))
                    {
                        cachedTriggerStates.Add(currentCollider, currentCollider.isTrigger);
                    }

                    currentCollider.isTrigger = true;
                }
                else
                {
                    if (cachedTriggerStates.ContainsKey(currentCollider))
                    {
                        currentCollider.isTrigger = cachedTriggerStates[currentCollider];
                    }
                }
            }
            else
            {
                currentCollider.enabled = !open;
            }
        }

        if (!open)
        {
            cachedTriggerStates.Clear();
        }
    }

    private IEnumerator BlinkConfirmation()
    {
        int counter = 0;

        while (counter <= numBlinks)
        {
            ChangeEmissionColor(Color.green);
            yield return new WaitForSeconds(0.05f);
            ChangeEmissionColor(Color.white);
            yield return new WaitForSeconds(0.05f);
            counter++;
        }
    }

    private IEnumerator BlinkError()
    {
        int counter = 0;

        while (counter <= numBlinks)
        {
            ChangeEmissionColor(Color.red);
            yield return new WaitForSeconds(0.08f);
            ChangeEmissionColor(Color.white);
            yield return new WaitForSeconds(0.08f);
            counter++;
        }
    }

    private void ChangeEmissionColor(Color color)
    {
        if (feedbackRenderer == null)
        {
            return;
        }

        feedbackRenderer.material.SetColor("_EmissionColor", color);
    }

    private void PlaySound(string soundName)
    {
        if (AudioManager.Instance == null)
        {
            return;
        }

        AudioManager.Instance.PlaySoundAt(soundName, transform.position);
    }
}