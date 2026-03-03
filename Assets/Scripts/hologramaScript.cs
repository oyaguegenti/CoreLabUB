using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hologramaScript : MonoBehaviour
{
    [SerializeField] private float velocidadRotacion = 50f;

    void Update()
    {
        transform.Rotate(Vector3.up, velocidadRotacion * Time.deltaTime, Space.World);
    }
}
