using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization.Scripts;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TextAsset creditsNames;

    [SerializeField] private string sceneToPlay;

    private void Awake()
    {
        SetPlayButton();
        SetCredits();
        SetCollaboration();
    }

    private void SetCredits()
    {
        transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = creditsNames.text;
        transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = LocalizationManager.Localize("MadeBy");
    }

    private void SetPlayButton()
    {
        transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = LocalizationManager.Localize("PlayButton");
        transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
        {
            SceneManager.LoadScene(sceneToPlay);
        });
    }

    private void SetCollaboration()
    {
        transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = LocalizationManager.Localize("AProjectOf");   
        transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = LocalizationManager.Localize("InCollaboration");   
        transform.GetChild(2).GetChild(0).GetChild(2).GetComponent<TMP_Text>().text = LocalizationManager.Localize("SpecialThanks");   
    }    
}
