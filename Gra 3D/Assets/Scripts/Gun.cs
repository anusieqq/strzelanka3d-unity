using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public float damage = 20f;
    public float range = 100f;
    public float fireRate = 0.1f;
    public Camera fpsCam;
    private float nextTimeToFire = 0f;
    public int ammoCount = 10;
    public int maxAmmo = 10;
    public int maxReserveAmmo = 50;
    public int reserveAmmo = 20;
    private bool isReloading = false;

    public Text ammoText;
    public GameObject muzzleFlashPrefab;
    public GameObject bulletHolePrefab;

    public Transform crosshairTransform;

    public AudioClip gunShotSound;
    public AudioClip gunreloadSound;
    public AudioClip gunemptySound;
    private AudioSource audioSource;

    [Header("Input System")]
    public InputActionAsset inputActions; // Referencja do InputActionAsset
    private InputAction shootAction;

    void Awake()
    {
        // Sprawdü, czy InputActionAsset jest przypisany
        if (inputActions == null)
        {
            Debug.LogError("InputActionAsset nie jest przypisany w Gun! Przypisz InputActionAsset w Inspectorze.");
            return;
        }

        // Znajdü akcjÍ Shoot
        shootAction = inputActions.FindAction("Player/Shoot");

        // Sprawdü, czy akcja zosta≥a znaleziona
        if (shootAction == null)
        {
            Debug.LogError("Nie znaleziono akcji Player/Shoot w InputActionAsset! Sprawdü nazwÍ akcji.");
        }

        // Sprawdü, czy tag jest ustawiony
        if (string.IsNullOrEmpty(gameObject.tag))
        {
            Debug.LogWarning("GameObject z Gun nie ma ustawionego tagu! Ustaw tag 'pistol' w Inspectorze.");
            gameObject.tag = "pistol"; // Domyúlny tag
        }
    }

    void Start()
    {
        reserveAmmo = 20;
        UpdateAmmoText();
        audioSource = GetComponent<AudioSource>();

        // W≥πcz akcje inputu
        if (inputActions != null)
        {
            inputActions.Enable();
            Debug.Log("InputActions w≥πczone w Gun.");
        }
    }

    void OnDestroy()
    {
        // Wy≥πcz akcje inputu
        if (inputActions != null)
        {
            inputActions.Disable();
            Debug.Log("InputActions wy≥πczone w Gun.");
        }
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (!isReloading && shootAction != null && shootAction.WasPressedThisFrame() && Time.time >= nextTimeToFire)
        {
            if (ammoCount > 0)
            {
                nextTimeToFire = Time.time + fireRate;
                Shoot();
                ammoCount--;
                Debug.Log($"Gun: Ammo left in pistol: {ammoCount}");

                if (ammoCount <= 0 && reserveAmmo > 0)
                {
                    Reload();
                }
            }
            else
            {
                if (reserveAmmo > 0)
                {
                    Reload();
                }
                else
                {
                    PlayEmptyGunSound();
                }
            }
        }

        UpdateCrosshair();
        UpdateAmmoText();
    }

    void Shoot()
    {
        if (gunShotSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gunShotSound);
        }

        if (muzzleFlashPrefab != null)
        {
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, fpsCam.transform.position + fpsCam.transform.forward * 0.5f, Quaternion.LookRotation(fpsCam.transform.forward));
            Destroy(muzzleFlash, 0.1f);
        }

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log($"Gun: Hit: {hit.transform.name}");

            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(gameObject.tag, damage); // Uøywa tagu i obraøeÒ
                Debug.Log($"Gun: Zadano {damage} obraøeÒ wrogowi {hit.transform.name} z tagiem {gameObject.tag}");
            }

            if (bulletHolePrefab != null && hit.collider.gameObject.CompareTag("wall"))
            {
                GameObject bulletHole = Instantiate(bulletHolePrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(bulletHole, 5f);
            }
        }
    }

    void Reload()
    {
        if (reserveAmmo > 0)
        {
            isReloading = true;
            Debug.Log("Gun: Reloading...");

            if (gunreloadSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(gunreloadSound);
            }

            Invoke("FinishReload", 2f);
        }

        if (crosshairTransform != null)
        {
            crosshairTransform.gameObject.SetActive(false);
        }
    }

    void FinishReload()
    {
        int ammoNeeded = maxAmmo - ammoCount;
        int ammoToReload = Mathf.Min(ammoNeeded, reserveAmmo);
        ammoCount += ammoToReload;
        reserveAmmo -= ammoToReload;

        isReloading = false;
        Debug.Log("Gun: Reload finished.");

        if (crosshairTransform != null)
        {
            crosshairTransform.gameObject.SetActive(true);
        }
    }

    public void UpdateAmmoText()
    {
        if (ammoText != null)
        {
            ammoText.text = $"Ammo: {ammoCount}/{reserveAmmo}";
        }
    }

    void UpdateCrosshair()
    {
        if (crosshairTransform != null)
        {
            if (isReloading)
            {
                crosshairTransform.position = fpsCam.transform.position + fpsCam.transform.forward * 2f;
                crosshairTransform.rotation = Quaternion.LookRotation(-fpsCam.transform.forward);
                return;
            }

            Ray ray = new Ray(fpsCam.transform.position, fpsCam.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, range, ~LayerMask.GetMask("UI")))
            {
                crosshairTransform.position = hit.point - (fpsCam.transform.forward * 0.05f);
                crosshairTransform.rotation = Quaternion.LookRotation(-fpsCam.transform.forward);
            }
            else
            {
                crosshairTransform.position = fpsCam.transform.position + fpsCam.transform.forward * range;
                crosshairTransform.rotation = Quaternion.LookRotation(-fpsCam.transform.forward);
            }
        }
    }

    void PlayEmptyGunSound()
    {
        if (gunemptySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gunemptySound);
        }
        Debug.Log("Gun: Click! No ammo.");
    }

    public void SetAmmo(int newAmmoCount, int newReserveAmmo)
    {
        ammoCount = Mathf.Clamp(newAmmoCount, 0, maxAmmo);
        reserveAmmo = Mathf.Clamp(newReserveAmmo, 0, maxReserveAmmo);
        UpdateAmmoText();
    }
}