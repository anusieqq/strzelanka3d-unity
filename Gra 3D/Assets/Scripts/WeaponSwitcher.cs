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
        // Sprawdü, czy InputActionAsset jest przypisany
        if (inputActions == null)
        {
            Debug.LogError("InputActionAsset nie jest przypisany w WeaponSwitcher! Przypisz InputActionAsset w Inspectorze.");
            return;
        }

        // Znajdü akcje Fire Torch i Pistol
        torchAction = inputActions.FindAction("Player/Fire Torch");
        pistolAction = inputActions.FindAction("Player/Pistol");

        // Sprawdü, czy akcje zosta≥y znalezione
        if (torchAction == null)
        {
            Debug.LogError("Nie znaleziono akcji Player/Fire Torch w InputActionAsset! Sprawdü nazwÍ akcji.");
        }
        if (pistolAction == null)
        {
            Debug.LogError("Nie znaleziono akcji Player/Pistol w InputActionAsset! Sprawdü nazwÍ akcji.");
        }
    }

    void Start()
    {
        // Sprawdü, czy obiekty sπ przypisane
        if (torch == null || pistol == null || crouch == null)
        {
            Debug.LogError("Jeden lub wiÍcej obiektÛw (torch, pistol, crouch) nie jest przypisanych w WeaponSwitcher!");
            return;
        }

        EquipPistol();

        // W≥πcz akcje inputu
        if (inputActions != null)
        {
            inputActions.Enable();
            Debug.Log("InputActions w≥πczone w WeaponSwitcher.");
        }
    }

    void OnDestroy()
    {
        // Wy≥πcz akcje inputu
        if (inputActions != null)
        {
            inputActions.Disable();
            Debug.Log("InputActions wy≥πczone w WeaponSwitcher.");
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

        // Wy≥πcz komponent Gun i w≥πcz Torch
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

        // Wy≥πcz komponent Torch i w≥πcz Gun
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