using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;  // Dodajemy przestrzeñ nazw EventSystems

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
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (!isReloading && Input.GetMouseButtonDown(0) && Time.time >= nextTimeToFire)
        {
            if (ammoCount > 0)
            {
                nextTimeToFire = Time.time + fireRate;
                Shoot();
                ammoCount--;
                Debug.Log("Ammo left in pistol: " + ammoCount);

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

        UpdateCrosshair(); // dzia³a zawsze
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
                enemy.TakeDamage(this.tag);  // przekazujemy tag broni zamiast damage
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

            // DŸwiêk prze³adowania
            if (gunreloadSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(gunreloadSound);
            }

            // Wywo³aj zakoñczenie prze³adowania po 2 sekundach
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
        Debug.Log("Reload finished.");

        if (crosshairTransform != null)
        {
            crosshairTransform.gameObject.SetActive(true);
        }

    }


    public void UpdateAmmoText()
    {
        ammoText.text = "Ammo: " + ammoCount.ToString() + "/" + reserveAmmo.ToString();
    }

    void UpdateCrosshair()
    {
        if (crosshairTransform != null)
        {
            if (isReloading)
            {
                // Trzyma celownik w sta³ym punkcie, np. œrodku ekranu
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
        Debug.Log("Click! No ammo.");
    }

}