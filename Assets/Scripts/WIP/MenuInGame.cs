using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


public class MenuInGame : MonoBehaviour
{
    public GameObject pauseMenu; // Asigna tu menú en el Inspector
    [SerializeField]private bool isMenuActive = false;

    void Update()
    {
        // Detectar si se presiona el botón "PrimaryButton" (botón A en Oculus)
        if (InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(XRNode.RightHand), InputHelpers.Button.PrimaryButton, out bool isPressed) && isPressed)
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        isMenuActive = !isMenuActive;
        pauseMenu.SetActive(isMenuActive);
    }
    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void Salir()
    {
        Application.Quit();
    }
}
