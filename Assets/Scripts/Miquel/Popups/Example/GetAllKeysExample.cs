using System.Collections.Generic;
using Assets.SimpleLocalization.Scripts;
using TMPro;
using UnityEngine;

public class GetAllKeysExample : MonoBehaviour
{
    [SerializeField] private GameObject textPrefab;

    private Dictionary<int, string> localizationKeys = new();
    // Start is called before the first frame update
    private void Start()
    {
        LocalizationManager.Read();
        LocalizationManager.AutoLanguage();

        var temp = LocalizationManager.Dictionary;
        int id = 0;

        foreach (var keys in temp["English"].Keys)
        {
            localizationKeys.Add(id, keys);
            id++;
        }

        for (int i = 0; i < localizationKeys.Count; i++)
        {
            GameObject keyText = Instantiate(textPrefab, transform);
            keyText.transform.GetChild(0).GetComponent<TMP_Text>().text = i + ": " + localizationKeys[i];
        }
    }
}
