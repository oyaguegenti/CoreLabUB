using System.Collections.Generic;
using Assets.SimpleLocalization.Scripts;
using TMPro;
using UnityEngine;


public enum Language { Catalan, Spanish, English }

public class PopupsManager : Singleton<PopupsManager>
{
    [SerializeField] private bool enableDebug = false;
    [SerializeField] private Language DEBUG_activeLanguage;

    private int currentPopupId = 0;
    private TMP_Text popupText;

    private Dictionary<int, string> localizationKeys = new();

    private void Start()
    {
        //LocalizationManager.Read();
        if (enableDebug)
        { LocalizationManager.Language = DEBUG_activeLanguage.ToString(); }
        

        // Get ALL spreadsheet Keys
        var temp = LocalizationManager.Dictionary;
        int count = 0;
        foreach ( var key in temp.Values )
        {
            foreach (var keys in key.Keys)
            {
                localizationKeys.Add(count, keys);
                count++;
            }
        }

        popupText = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
    }

    public void CreatePopup(int id, Vector3 pos, Quaternion rot)
    {
        if (enableDebug)
        { Debug.Log("CREATE | CURRENT ID: " + currentPopupId + " ID PARAMETER: " + id); }

        // Checks if the popup to create is the correct one, this prevents the creation of future popups.
        // If the player has not grabbed the Card, and walks down the hallway, the next popup will not appear
        if (currentPopupId != id) { return; }
       
        popupText.transform.parent.gameObject.SetActive(true);
        popupText.text = LocalizationManager.Localize(localizationKeys[id]);
        popupText.transform.parent.parent.SetPositionAndRotation(pos, rot);
    }

    public void ClosePopup(int id)
    {
        if (enableDebug)
        { Debug.Log("CLOSE | CURRENT ID: " + currentPopupId + " ID PARAMETER: " + id); }

        // Checks if the popup to close is the correct one, this prevents the closure of future popups.
        if (currentPopupId != id) { return;}

        popupText.transform.parent.gameObject.SetActive(false);

        currentPopupId++;
    }
}
