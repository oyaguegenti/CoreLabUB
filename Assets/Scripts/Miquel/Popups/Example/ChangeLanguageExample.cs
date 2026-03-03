using System;
using System.Collections.Generic;
using Assets.SimpleLocalization.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeLanguageExample : MonoBehaviour
{
    [SerializeField] private GameObject languagePrefab;
    private List<string> languages = new List<string>();
    private void Start()
    {
        // Read Spreadsheet
        LocalizationManager.Read();

        // Complete Spreadsheet
        var temp = LocalizationManager.Dictionary;

        // Instantiate and Configure language prefab for each Translation available (Catalan, Spanish and English)
        foreach (var language in temp.Keys)
        {
            languages.Add(language);
            GameObject prefab = Instantiate(languagePrefab, transform.GetChild(1));

            prefab.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Flags/" + language + "Flag");

            prefab.transform.GetChild(1).GetComponent<TMP_Text>().text = LocalizationManager.Localize(String.Concat("LanguageNames", language));

            // Button Onclick, Changes Language
            prefab.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
            {
                LocalizationManager.Language = language;
            });
        }

        // Is called when LocalizationManager.Language changes
        LocalizationManager.OnLocalizationChanged += () =>
        {
            for (int i = 0; i < transform.GetChild(1).childCount; i++)
            {
                transform.GetChild(1).GetChild(i).GetChild(1).GetComponent<TMP_Text>().text = LocalizationManager.Localize(String.Concat("LanguageNames", languages[i]));
            }
        };
    }
}
