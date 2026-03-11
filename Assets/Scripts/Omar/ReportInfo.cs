using UnityEngine;
using UnityEngine.UI;

public class ReportInfo : MonoBehaviour
{
    private Image substanceImage;
    private Image graphicImage;

    public void SetSubstanceAndGraphic(Sprite substanceName, Sprite graphicName)
    {
        substanceImage = transform.GetChild(0).GetChild(1).GetComponent<Image>();
        graphicImage = transform.GetChild(0).GetChild(2).GetComponent<Image>();

        substanceImage.sprite = substanceName;
        graphicImage.sprite = graphicName;
    }
}
