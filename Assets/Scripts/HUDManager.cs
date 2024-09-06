using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; set; }

    // UI
    [Header("Ammo")]
    public TextMeshProUGUI magazineAmmoUI;
    public TextMeshProUGUI totalAmmoUI;
    public Image ammoTypeUI;

    [Header("Weapon")]
    public Image activeWeaponUI;
    public Image unActiveWeaponUI;

    [Header("Throwables")]
    public Image lethalUI;
    public TextMeshProUGUI lethalAmountUI;

    [Header("Medkit")]
    public Image medkitUI;
    public TextMeshProUGUI medkitAmountUI;

    public Sprite emptySlot;
    public Sprite greySlot;

    [Header("Wave")]
    public TextMeshProUGUI currentWaveUI;
    public TextMeshProUGUI cooldownCouterUI;
    public TextMeshProUGUI waveOverUI;
    public TextMeshProUGUI EnemyRemainUI;

    public GameObject WinUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        Weapon activeWeapon = WeaponManager.Instance.activeWeaponSlot.GetComponentInChildren<Weapon>();
        Weapon unActiveWeapon = GetUnActiveWeaponSlot().GetComponentInChildren<Weapon>();

        if (activeWeapon)
        {
            Weapon.WeaponModel model = activeWeapon.thisWeaponModel;
            ammoTypeUI.sprite = GetAmmoSprite(model);

            activeWeaponUI.sprite = GetWeaponSprite(model);

            magazineAmmoUI.text = $"{activeWeapon.bulletsLeft / activeWeapon.bulletPerBurst}";
            totalAmmoUI.text = $"{WeaponManager.Instance.CheckAmmoLeftFor(model) / activeWeapon.bulletPerBurst}";

            if (unActiveWeapon)
            {
                unActiveWeaponUI.sprite = GetWeaponSprite (unActiveWeapon.thisWeaponModel);
            }
        }
        else
        {
            magazineAmmoUI.text = "";
            totalAmmoUI.text = "";

            ammoTypeUI.sprite = emptySlot;

            activeWeaponUI.sprite = emptySlot;
            unActiveWeaponUI.sprite = emptySlot;

        }

        if (WeaponManager.Instance.lethalsCount <=0) 
        {
            lethalUI.sprite = greySlot;
        }

        if (WeaponManager.Instance.medkitCount <= 0)
        {
            medkitUI.sprite = greySlot;
        }
    }

    private Sprite GetWeaponSprite(Weapon.WeaponModel model)
    {
        switch (model)
        {
            case Weapon.WeaponModel.Pistol1:
                return Resources.Load<GameObject>("UI/Pistol1_Weapon").GetComponent<SpriteRenderer>().sprite;
            case Weapon.WeaponModel.AK47:
                return Resources.Load<GameObject>("UI/AK47_Weapon").GetComponent<SpriteRenderer>().sprite;
            case Weapon.WeaponModel.Uzi:
                return Resources.Load<GameObject>("UI/Uzi_Weapon").GetComponent<SpriteRenderer>().sprite;
            default:
                return null;

        }
    }

    private Sprite GetAmmoSprite(Weapon.WeaponModel model)
    {
        switch (model)
        {
            case Weapon.WeaponModel.Pistol1:
                return Resources.Load<GameObject>("UI/Pistol_Ammo").GetComponent<SpriteRenderer>().sprite;
            case Weapon.WeaponModel.AK47:
                return Resources.Load<GameObject>("UI/Refile_Ammo").GetComponent<SpriteRenderer>().sprite;
            case Weapon.WeaponModel.Uzi:
                return Resources.Load<GameObject>("UI/SMG_Ammo").GetComponent<SpriteRenderer>().sprite;
            default:
                return null;
        }
    }

    private GameObject GetUnActiveWeaponSlot()
    {
        foreach (GameObject weaponSlot in WeaponManager.Instance.weaponSlots)
        {
            if (weaponSlot != WeaponManager.Instance.activeWeaponSlot)
            {
                return weaponSlot;
            }
        }
        return null; 
    }

    internal void UpdateThrowablesUI()
    {
        lethalAmountUI.text = $"{WeaponManager.Instance.lethalsCount}";

        switch (WeaponManager.Instance.equippedLethalType)
        {
            case Throwable.ThrowableType.Grenade:
                lethalUI.sprite = Resources.Load<GameObject>("UI/Grenade").GetComponent<SpriteRenderer>().sprite;
                break;

            case Throwable.ThrowableType.Incendiary:
                lethalUI.sprite = Resources.Load<GameObject>("UI/Incendiary").GetComponent<SpriteRenderer>().sprite;
                break;
        }
    }

    internal void UpdateMedkitUI()
    {
        medkitAmountUI.text = $"{WeaponManager.Instance.medkitCount}";
        medkitUI.sprite = Resources.Load<GameObject>("UI/Medkit").GetComponent<SpriteRenderer>().sprite;
    }

    public void EndGame()
    {
        WinUI.gameObject.SetActive(true);

    }
}
