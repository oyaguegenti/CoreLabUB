using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPermissionSetter : MonoBehaviour
{
    [SerializeField] private MagneticCard targetCard;

    public void SetArcheolab()
    {
        SetCardPermission(CardPerms.Archeolab);
    }

    public void SetBiolab()
    {
        SetCardPermission(CardPerms.Biolab);
    }

    public void SetGeolab()
    {
        SetCardPermission(CardPerms.Geolab);
    }

    public void SetNanolab()
    {
        SetCardPermission(CardPerms.Nanolab);
    }

    public void SetEntrance()
    {
        SetCardPermission(CardPerms.Entrance);
    }

    public void SetCardPermission(CardPerms permission)
    {
        if (targetCard == null)
        {
            Debug.LogError("Target MagneticCard is not assigned.");
            return;
        }

        targetCard.SetCardPermission(permission);
    }

    public void SetCardPermissionByIndex(int permissionIndex)
    {
        if (permissionIndex < 0 || permissionIndex > 4)
        {
            Debug.LogWarning("Invalid CardPerms index: " + permissionIndex);
            return;
        }

        SetCardPermission((CardPerms)permissionIndex);
    }
}