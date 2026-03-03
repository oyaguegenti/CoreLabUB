using UnityEngine;

public class PopupsClosers : PopupsRaycast
{
    private void Update()
    {
        RaycastHit hit;

        // If raycast does not collide with player, returns. the player object has the layer Player
        if (!Physics.Raycast(ray, out hit, rayDistance, interactableLayerMask))
        { return; }

        PopupsManager.Instance.ClosePopup(popupId);

        DestroyOnTrigger();
    }
}
