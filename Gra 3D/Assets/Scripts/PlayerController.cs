using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Statystyki gracza")]
    public float currentHP = 100f;
    public float maxHP = 100f;

    public float currentShield;
    public float maxShield;

    [Header("Ruch gracza")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;

    private Rigidbody rb;
    private Camera mainCamera;
    private Vector3 movementInput;

    public Slider healthSlider;
    public Slider shieldSlider;

    private float shieldDrainRate = 5f;
    private float shieldRegenDelay = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;

        LoadPlayerData();
        StartCoroutine(ShieldDrainCoroutine());
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void GetInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        movementInput = new Vector3(horizontal, 0f, vertical).normalized;
    }

    void MovePlayer()
    {
        if (movementInput.magnitude >= 0.1f)
        {
            Vector3 moveDirection = mainCamera.transform.TransformDirection(movementInput);
            moveDirection.y = 0f;

            Vector3 newPosition = rb.position + moveDirection.normalized * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    void LoadPlayerData()
    {
        if (PlayerPrefs.HasKey("PlayerPosX"))
        {
            float x = PlayerPrefs.GetFloat("PlayerPosX");
            float y = PlayerPrefs.GetFloat("PlayerPosY");
            float z = PlayerPrefs.GetFloat("PlayerPosZ");
            transform.position = new Vector3(x, y, z);
        }

        currentHP = PlayerPrefs.GetFloat("PlayerHealth", currentHP);
        maxHP = PlayerPrefs.GetFloat("MaxHealth", maxHP);

        currentShield = PlayerPrefs.GetFloat("PlayerShield", 0);
        maxShield = PlayerPrefs.GetFloat("MaxShield", 100);

        if (healthSlider != null)
        {
            healthSlider.value = currentHP;
            healthSlider.maxValue = maxHP;
        }

        if (shieldSlider != null)
        {
            shieldSlider.value = currentShield;
            shieldSlider.maxValue = maxShield;
        }
    }

    void SavePlayerData()
    {
        PlayerPrefs.SetFloat("PlayerHealth", currentHP);
        PlayerPrefs.SetFloat("PlayerShield", currentShield);
        PlayerPrefs.SetFloat("MaxHealth", maxHP);
        PlayerPrefs.SetFloat("MaxShield", maxShield);
        PlayerPrefs.SetFloat("PlayerPosX", transform.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", transform.position.y);
        PlayerPrefs.SetFloat("PlayerPosZ", transform.position.z);
        PlayerPrefs.Save();
    }

    IEnumerator ShieldDrainCoroutine()
    {
        while (true)
        {
            if (currentShield > 0)
            {
                currentShield -= shieldDrainRate;
                currentShield = Mathf.Clamp(currentShield, 0, maxShield);

                if (shieldSlider != null)
                {
                    shieldSlider.value = currentShield;
                }
            }
            yield return new WaitForSeconds(shieldRegenDelay);
        }
    }

    public void SetHealth(float hp, float maxHp)
    {
        currentHP = hp;
        maxHP = maxHp;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHp;
            healthSlider.value = hp;
        }
    }

    public void SetShield(float shield, float maxShieldValue)
    {
        currentShield = shield;
        maxShield = maxShieldValue;

        if (shieldSlider != null)
        {
            shieldSlider.maxValue = maxShieldValue;
            shieldSlider.value = shield;
        }
    }


    public void TakeDamage(float damage)
    {
        if (currentShield > 0)
        {
            float leftoverDamage = damage - currentShield;

            currentShield -= damage;
            currentShield = Mathf.Clamp(currentShield, 0, maxShield);

            if (shieldSlider != null)
                shieldSlider.value = currentShield;

            if (leftoverDamage > 0)
            {
                currentHP -= leftoverDamage;
                currentHP = Mathf.Clamp(currentHP, 0, maxHP);

                if (healthSlider != null)
                    healthSlider.value = currentHP;
            }
        }
        else
        {
            currentHP -= damage;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);

            if (healthSlider != null)
                healthSlider.value = currentHP;
        }

        Debug.Log($"[PlayerController] HP: {currentHP}, Shield: {currentShield}");
    }


    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}