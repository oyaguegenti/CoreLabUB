using UnityEngine;

public class PopupDetectors : PopupsRaycast
{
    private void Update()
    {
        RaycastHit hit;

        // If raycast does not collide with player, returns. the player object has the layer Player
        if (!Physics.Raycast(ray, out hit, rayDistance, interactableLayerMask))
        { return; }

        Transform child = transform.GetChild(0);

        // Creates a Popup at the Child position and rotation
        PopupsManager.Instance.CreatePopup(popupId, child.position, child.rotation);

        DestroyOnTrigger();
    }
}
