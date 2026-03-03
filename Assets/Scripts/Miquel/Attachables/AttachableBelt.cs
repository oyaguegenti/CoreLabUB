using UnityEngine;

public class AttachableBelt : AttachableInteractor
{
    [SerializeField] private GameObject playerCamera;
    private Vector3 beltOffset = new(-0.1f,-0.5f,0);
    
    private void Update()
    {
        // 0 Forwards, -1 Backwards, -0.5 Left and Right
        // Checks Camera for Left and right

        // Positive is Right, Negative is Left
        float cameraFacing = playerCamera.transform.rotation.y >= 0.0f ? 1.0f : -1.0f;

        transform.position = playerCamera.transform.position + beltOffset;
        transform.localRotation = Quaternion.Euler(0, playerCamera.transform.localRotation.y * 180.0f * cameraFacing + 180.0f, 0);
    }
}
