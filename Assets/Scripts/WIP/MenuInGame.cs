using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class MenuInGame : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private bool isMenuActive = false;

    private bool wasPrimaryButtonPressedLastFrame = false;

    // Detecta si s'ha presionat el boto primary button (A del mando de les Metaquest 3, en aquest cas)
    private void Update()
    {
        InputDevice rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        bool isPressed = false;
        bool isValidPress = InputHelpers.IsPressed(
            rightHandDevice,
            InputHelpers.Button.PrimaryButton,
            out isPressed
        );

        if (!isValidPress)
        {
            return;
        }

        if (isPressed && !wasPrimaryButtonPressedLastFrame)
        {
            TogglePauseMenu();
        }

        wasPrimaryButtonPressedLastFrame = isPressed;
    }

    public void TogglePauseMenu()
    {
        isMenuActive = !isMenuActive;

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(isMenuActive);
        }
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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