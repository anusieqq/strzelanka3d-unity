using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject torch;
    public GameObject pistol;
    public GameObject crouch;

    [Header("Input System")]
    public InputActionAsset inputActions; // Referencja do InputActionAsset
    private InputAction torchAction;
    private InputAction pistolAction;

    private enum EquippedItem { Torch, Pistol }
    private EquippedItem currentItem;

    void Awake()
    {
        // Sprawd�, czy InputActionAsset jest przypisany
        if (inputActions == null)
        {
            Debug.LogError("InputActionAsset nie jest przypisany w WeaponSwitcher! Przypisz InputActionAsset w Inspectorze.");
            return;
        }

        // Znajd� akcje Fire Torch i Pistol
        torchAction = inputActions.FindAction("Player/Fire Torch");
        pistolAction = inputActions.FindAction("Player/Pistol");

        // Sprawd�, czy akcje zosta�y znalezione
        if (torchAction == null)
        {
            Debug.LogError("Nie znaleziono akcji Player/Fire Torch w InputActionAsset! Sprawd� nazw� akcji.");
        }
        if (pistolAction == null)
        {
            Debug.LogError("Nie znaleziono akcji Player/Pistol w InputActionAsset! Sprawd� nazw� akcji.");
        }
    }

    void Start()
    {
        // Sprawd�, czy obiekty s� przypisane
        if (torch == null || pistol == null || crouch == null)
        {
            Debug.LogError("Jeden lub wi�cej obiekt�w (torch, pistol, crouch) nie jest przypisanych w WeaponSwitcher!");
            return;
        }

        EquipPistol();

        // W��cz akcje inputu
        if (inputActions != null)
        {
            inputActions.Enable();
            Debug.Log("InputActions w��czone w WeaponSwitcher.");
        }
    }

    void OnDestroy()
    {
        // Wy��cz akcje inputu
        if (inputActions != null)
        {
            inputActions.Disable();
            Debug.Log("InputActions wy��czone w WeaponSwitcher.");
        }
    }

    void Update()
    {
        // Odczytaj inputy z akcji
        if (torchAction != null && torchAction.WasPressedThisFrame())
        {
            EquipTorch();
        }
        else if (pistolAction != null && pistolAction.WasPressedThisFrame())
        {
            EquipPistol();
        }
    }

    void EquipTorch()
    {
        if (currentItem == EquippedItem.Torch) return;

        torch.SetActive(true);
        pistol.SetActive(false);
        crouch.SetActive(false);
        currentItem = EquippedItem.Torch;

        // Wy��cz komponent Gun i w��cz Torch
        Gun gunComponent = pistol.GetComponent<Gun>();
        Torch torchComponent = torch.GetComponent<Torch>();

        if (gunComponent != null)
            gunComponent.enabled = false;
        else
            Debug.LogWarning("Brak komponentu Gun na pistolecie!");

        if (torchComponent != null)
            torchComponent.enabled = true;
        else
            Debug.LogWarning("Brak komponentu Torch na pochodni!");

        Debug.Log($"Torch active: {torch.activeSelf}, Torch component enabled: {torchComponent != null && torchComponent.enabled}");
    }

    void EquipPistol()
    {
        if (currentItem == EquippedItem.Pistol) return;

        torch.SetActive(false);
        pistol.SetActive(true);
        crouch.SetActive(true);
        currentItem = EquippedItem.Pistol;

        // Wy��cz komponent Torch i w��cz Gun
        Torch torchComponent = torch.GetComponent<Torch>();
        Gun gunComponent = pistol.GetComponent<Gun>();

        if (torchComponent != null)
            torchComponent.enabled = false;
        else
            Debug.LogWarning("Brak komponentu Torch na pochodni!");

        if (gunComponent != null)
            gunComponent.enabled = true;
        else
            Debug.LogWarning("Brak komponentu Gun na pistolecie!");

        Debug.Log($"Pistol active: {pistol.activeSelf}, Gun component enabled: {gunComponent != null && gunComponent.enabled}");
    }
}