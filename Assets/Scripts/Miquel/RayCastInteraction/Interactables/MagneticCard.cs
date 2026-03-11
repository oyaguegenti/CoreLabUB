using System.Collections.Generic;
using UnityEngine;

public enum CardPerms { Archeolab = 0, Biolab = 1, Geolab = 2, Nanolab = 3, Entrance = 4 }

public class MagneticCard : AttachableObject
{
    [SerializeField] private CardPerms initialCardPerm = CardPerms.Entrance;

    private Dictionary<CardPerms, bool> cardPerms = new Dictionary<CardPerms, bool>();

    // Orden obligatorio:
    // 0 Archeolab
    // 1 Biolab
    // 2 Geolab
    // 3 Nanolab
    // 4 Entrance
    [SerializeField] private List<Material> cardMaterials = new List<Material>();

    protected override void Awake()
    {
        base.Awake();

        interactableType = InteractableType.MagneticCard;

        InitializePerms();
        SetCardPermission(initialCardPerm);
    }

    private void InitializePerms()
    {
        cardPerms.Clear();

        cardPerms.Add(CardPerms.Archeolab, false);
        cardPerms.Add(CardPerms.Biolab, false);
        cardPerms.Add(CardPerms.Geolab, false);
        cardPerms.Add(CardPerms.Nanolab, false);
        cardPerms.Add(CardPerms.Entrance, false);
    }

    public override void SelectEnter(GameObject interactor)
    {
        CardReader.toggleCardReaderRaycast.Invoke(true);
    }

    public override void SelectExit(GameObject interactor)
    {
        if (!isAttatch)
        {
            ReturnToPreviousPosition();
        }

        CardReader.toggleCardReaderRaycast.Invoke(false);
    }

    public override void OnAttach(Transform attachPosition)
    {
        base.OnAttach(attachPosition);

        PopupsManager.Instance.ClosePopup(0);
    }

    public bool CheckPerm(CardPerms cardPerm)
    {
        return cardPerms[cardPerm];
    }

    public void SetCardPermission(CardPerms labPerm)
    {
        InitializePerms();

        // Siempre Entrance
        cardPerms[CardPerms.Entrance] = true;

        // Si se elige un laboratorio, también se marca
        if (labPerm != CardPerms.Entrance)
        {
            cardPerms[labPerm] = true;
            ChangeMaterial(labPerm);
        }
        else
        {
            ChangeMaterial(CardPerms.Entrance);
        }

        Debug.Log("Card permission set to: Entrance + " + labPerm);
    }

    public void AddPerms(CardPerms cardPerm)
    {
        cardPerms[cardPerm] = true;
    }

    public void RemovePerms(CardPerms cardPerm)
    {
        cardPerms[cardPerm] = false;
    }

    private void ChangeMaterial(CardPerms cardPerm)
    {
        int materialIndex = (int)cardPerm;

        if (cardMaterials == null || cardMaterials.Count <= materialIndex || cardMaterials[materialIndex] == null)
        {
            Debug.LogWarning("Card material missing for perm: " + cardPerm);
            return;
        }

        Renderer rendererComponent = GetComponent<Renderer>();
        if (rendererComponent == null)
        {
            Debug.LogWarning("MagneticCard has no Renderer.");
            return;
        }

        rendererComponent.material = cardMaterials[materialIndex];
    }
}