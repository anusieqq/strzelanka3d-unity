using UnityEngine;
using UnityEngine.UI;

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

    void Start()
    {
        reserveAmmo = 20;
        UpdateAmmoText();
    }

    void Update()
    {
        if (isReloading)
            return;

        if (Input.GetMouseButtonDown(0) && Time.time >= nextTimeToFire && ammoCount > 0)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
            ammoCount--;
            Debug.Log("Ammo left in pistol: " + ammoCount);

            if (ammoCount <= 0 && reserveAmmo > 0 && !isReloading)
            {
                Reload();
            }
        }

        UpdateCrosshair();
        UpdateAmmoText();
    }

    void Shoot()
    {
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

            int ammoNeeded = maxAmmo - ammoCount;
            int ammoToReload = Mathf.Min(ammoNeeded, reserveAmmo);
            ammoCount += ammoToReload;
            reserveAmmo -= ammoToReload;

            Invoke("FinishReload", 2f);
        }
    }

    void FinishReload()
    {
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

            if (Physics.Raycast(ray, out hit, range, ~LayerMask.GetMask("Crosshair"))) // Ignorujemy kolizjê celownika
            {
                // Ustaw celownik na miejscu trafienia, ale trochê cofniêty, by unikn¹æ kolizji
                crosshairTransform.position = hit.point - (fpsCam.transform.forward * 0.05f);

                // Ustaw celownik tak, aby patrzy³ w stronê kamery
                crosshairTransform.rotation = Quaternion.LookRotation(-fpsCam.transform.forward);
            }
            else
            {
                // Jeœli nic nie trafiono, ustaw celownik w maksymalnej odleg³oœci
                crosshairTransform.position = fpsCam.transform.position + fpsCam.transform.forward * range;
                crosshairTransform.rotation = Quaternion.LookRotation(-fpsCam.transform.forward);
            }
        }
    }


}
