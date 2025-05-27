using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject torch;
    public GameObject pistol;
    public GameObject crouch;

    private enum EquippedItem { Torch, Pistol }
    private EquippedItem currentItem;

    public void Start()
    {
        EquipPistol(); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            EquipTorch();
        }
        else if (Input.GetKeyDown(KeyCode.P))
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
        Debug.Log("Equipped Torch");
    }

    void EquipPistol()
    {
        if (currentItem == EquippedItem.Pistol) return;

        torch.SetActive(false);
        pistol.SetActive(true);
        crouch.SetActive(true);
        currentItem = EquippedItem.Pistol;
        Debug.Log("Equipped Pistol");
    }
}
