using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public float damage = 20f;
    public float range = 100f;
    public float fireRate = 0.1f;
    public Camera fpsCam;
    private float nextTimeToFire = 0f;
    public int ammoCount = 10;      // Pocz�tkowa ilo�� amunicji w pistolecie (max 10)
    public int maxAmmo = 10;         // Maksymalna ilo�� amunicji w magazynku (maksymalnie 10)
    public int maxReserveAmmo = 50;  // Zapasy amunicji (max 50)
    public int reserveAmmo = 20;         // Ilo�� zapasowej amunicji (zaczyna si� od maxReserveAmmo)
    private bool isReloading = false; // Flaga, kt�ra sprawdza, czy trwa prze�adowanie

    public Text ammoText; 

    void Start()
    {
        reserveAmmo = 20; 
        UpdateAmmoText();
    }

    void Update()
    {
        if (isReloading)
            return;

        // Sprawdzanie, czy mo�emy strzela�
        if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire && ammoCount > 0)
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
        UpdateAmmoText();
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);

            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    // Funkcja do prze�adowania
    void Reload()
    {
        if (reserveAmmo > 0)
        {
            isReloading = true;
            Debug.Log("Reloading...");

            // Prze�aduj 10 amunicji z zapas�w
            int ammoNeeded = maxAmmo - ammoCount;
            int ammoToReload = Mathf.Min(ammoNeeded, reserveAmmo); 
            ammoCount += ammoToReload;
            reserveAmmo -= ammoToReload;

            Invoke("FinishReload", 2f); // Czas prze�adowania: 2 sekundy
        }
    }

    // Funkcja, kt�ra ko�czy prze�adowanie
    void FinishReload()
    {
        isReloading = false;
        Debug.Log("Reload finished.");
    }
    public void UpdateAmmoText()
    {
        ammoText.text = "Ammo: " + ammoCount.ToString() + "/" + reserveAmmo.ToString();
    }
}
