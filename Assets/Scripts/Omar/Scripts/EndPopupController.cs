using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class EndPopupController : MonoBehaviour
{
    [SerializeField] private GameObject popupContent;

    private void Awake()
    {
        Hide();
    }

    public void Show()
    {
        if (popupContent != null)
        {
            popupContent.SetActive(true);
        }
    }

    public void Hide()
    {
        if (popupContent != null)
        {
            popupContent.SetActive(false);
        }
    }

    public bool IsVisible()
    {
        return popupContent != null && popupContent.activeInHierarchy;
    }
}