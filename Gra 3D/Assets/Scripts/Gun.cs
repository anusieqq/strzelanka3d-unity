using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;  // Dodajemy przestrze� nazw EventSystems

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


    void Start()
    {
        reserveAmmo = 20;
        UpdateAmmoText();
        audioSource = GetComponent<AudioSource>();

    }

    void Update()
    {
        if (isReloading || EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0) && Time.time >= nextTimeToFire)
        {
            if (ammoCount > 0)
            {
                nextTimeToFire = Time.time + fireRate;
                Shoot();
                ammoCount--;
                Debug.Log("Ammo left in pistol: " + ammoCount);

                if (ammoCount <= 0 && reserveAmmo > 0 && !isReloading)
                {
                    Reload(); // Automatyczne prze�adowanie
                }
            }
            else
            {
                if (reserveAmmo > 0 && !isReloading)
                {
                    Reload(); // Prze�aduj automatycznie
                }
                else
                {
                    PlayEmptyGunSound(); // D�wi�k pustego magazynka
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
            Debug.Log("Hit: " + hit.transform.name);

            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
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
            Debug.Log("Reloading...");

            // D�wi�k prze�adowania
            if (gunreloadSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(gunreloadSound);
            }

            // Wywo�aj zako�czenie prze�adowania po 2 sekundach
            Invoke("FinishReload", 2f);
        }
    }


    void FinishReload()
    {
        int ammoNeeded = maxAmmo - ammoCount;
        int ammoToReload = Mathf.Min(ammoNeeded, reserveAmmo);
        ammoCount += ammoToReload;
        reserveAmmo -= ammoToReload;

        isReloading = false;
        Debug.Log("Reload finished.");
    }


    public void UpdateAmmoText()
    {
        ammoText.text = "Ammo: " + ammoCount.ToString() + "/" + reserveAmmo.ToString();
    }

    void UpdateCrosshair()
    {
        if (crosshairTransform != null)
        {
            Ray ray = new Ray(fpsCam.transform.position, fpsCam.transform.forward);
            RaycastHit hit;

            // Sprawdzamy, czy raycast nie trafia w UI
            if (Physics.Raycast(ray, out hit, range, ~LayerMask.GetMask("UI"))) // U�ywamy maski, aby zignorowa� UI
            {
                // Ustaw celownik na miejscu trafienia, ale troch� cofni�ty, by unikn�� kolizji
                crosshairTransform.position = hit.point - (fpsCam.transform.forward * 0.05f);

                // Ustaw celownik tak, aby patrzy� w stron� kamery
                crosshairTransform.rotation = Quaternion.LookRotation(-fpsCam.transform.forward);
            }
            else
            {
                // Je�li nic nie trafiono, ustaw celownik w maksymalnej odleg�o�ci
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
        Debug.Log("Click! No ammo.");
    }

}
