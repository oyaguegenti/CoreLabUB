using System;
using Assets.SimpleLocalization.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectLanguage : MonoBehaviour
{
    // Base Flag (Button) + Name  (Text)
    [SerializeField] private GameObject languagePrefab;

    private void Awake()
    {
        // Read Spreadsheet
        LocalizationManager.Read();

        // Select Default Language in Device
        LocalizationManager.AutoLanguage();

        // Select Language Text
        transform.GetChild(0).GetComponent<TMP_Text>().text = LocalizationManager.Localize("Instruction");

        // Complete Spreadsheet
        var temp = LocalizationManager.Dictionary; 

        // Instantiate and Configure language prefab for each Translation available (Catalan, Spanish and English)
        foreach (var language in temp.Keys)
        {
            GameObject prefab = Instantiate(languagePrefab, transform.GetChild(1));

            prefab.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Flags/"+language+"Flag");

            prefab.transform.GetChild(1).GetComponent<TMP_Text>().text = LocalizationManager.Localize(String.Concat("LanguageNames",language));

            // Button Onclick, Activate MainMenu and disable Language Selection
            prefab.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => 
            {
                LocalizationManager.Language = language; 
                transform.parent.GetChild(1).gameObject.SetActive(true);  
                gameObject.SetActive(false); 
            });
        }
    }
}
