using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetPopupsExample : MonoBehaviour
{
    [SerializeField] private List<GameObject> popupsRaycasts;

    private void Start()
    {
        transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
        {
            popupsRaycasts.ForEach(_ => { _.SetActive(true); });
        });
    }
}
