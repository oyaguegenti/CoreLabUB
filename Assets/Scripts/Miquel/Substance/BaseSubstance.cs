using Unity.VisualScripting;
using UnityEngine;

public enum SubstanceType { NULL, Pollen, Wine, Grass }
public class BaseSubstance : RaycastTarget
{
    // Add this Script and select the substanceType, it will automatically switch to the chosen substanceClass 

    [SerializeField] protected SubstanceType substanceType;

    [SerializeField] protected Material substanceMaterial;

    protected override void Awake()
    {
        base.Awake();

        switch (substanceType)
        {
            case SubstanceType.Pollen:
            {
                transform.AddComponent<Pollen>().ApplyMaterial(substanceType);

                break;
            }
            case SubstanceType.Wine:
            {
                transform.AddComponent<Wine>().ApplyMaterial(substanceType);

                break;
            }
            case SubstanceType.Grass:
            {
                transform.AddComponent<Grass>().ApplyMaterial(substanceType);
                
                break;
            }
            case SubstanceType.NULL:
                break;
        }
        Destroy(this);
    }

    public SubstanceType GetSubstanceType()
    {
        return substanceType;
    }

    protected void ApplyMaterial(SubstanceType substanceType)
    {
        substanceMaterial = (Material) Resources.Load("Materials/Substances/" + substanceType.ToString());
        GetComponent<Renderer>().material = substanceMaterial;
    }

    public Material GetSubstanceMaterial()
    {
        return substanceMaterial;
    }

    public override void OnRaycastEnter(GameObject emitter)
    {
        emitter.GetComponent<Stick>().ChangeHead(substanceMaterial);
        emitter.GetComponent<Stick>().SetSubstance(this);
            
        Destroy(gameObject);
    }
}
