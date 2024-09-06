using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; set; }

    [Header("WeaponSlots")]
    public List<GameObject> weaponSlots;
    public GameObject activeWeaponSlot;

    [Header("Ammo")]
    public int totalRifleAmmo = 0;
    public int totalPistolAmmo = 0;
    public int totalSMGAmmo = 0;


    [Header("Throwables General")]
    public float throwForce = 40f;
    public GameObject throwableSpawn;
    public float forceMultiplier = 0f;
    public float forceMultiplierLimit = 2f;

    [Header("Lethals")]
    public int lethalsCount = 0;
    public int maxLethals = 2;
    public Throwable.ThrowableType equippedLethalType;
    public GameObject grenadePrefab;
    public GameObject incendiaryPrefab;
    public GameObject currentLethalObject;

    [Header("Medkit")]
    public int medkitCount = 0;
    public int maxMedkit = 1;
    public GameObject medkitPrefab;
    public GameObject currentMedkit;

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

    private void Start()
    {
        activeWeaponSlot = weaponSlots[0];
        equippedLethalType = Throwable.ThrowableType.None;
    }

    private void Update()
    {
        foreach(GameObject weaponSlot in weaponSlots)
        {
            if (weaponSlot == activeWeaponSlot)
            {
                weaponSlot.SetActive(true);
            }
            else
            {
                weaponSlot.SetActive(false);
            }
            UpdateTotalAmmoUsed();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchActiveSlot(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchActiveSlot(1);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            SwitchActiveSlot(2);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            SwitchActiveSlot(3);
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            forceMultiplier += Time.deltaTime;
            if (forceMultiplier > forceMultiplierLimit)
            {
                forceMultiplier = forceMultiplierLimit;
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (lethalsCount > 0 && activeWeaponSlot == weaponSlots[2])
            {
                ThrowLethal();
            }
            forceMultiplier = 0;
        }
    }
    public void PickupWeapon(GameObject pickedupWeapon)
    {
        if (activeWeaponSlot != weaponSlots[2])
        {
            AddWeaponIntoActiveSlot(pickedupWeapon);

        }
    }

    private void AddWeaponIntoActiveSlot(GameObject pickedupWeapon)
    {
        DropCurrentWeapon(pickedupWeapon);

        pickedupWeapon.transform.SetParent(activeWeaponSlot.transform, false);

        Weapon weapon = pickedupWeapon.GetComponent<Weapon>();

        pickedupWeapon.transform.localPosition = new Vector3(weapon.spawnPosition.x, weapon.spawnPosition.y, weapon.spawnPosition.z);
        pickedupWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation.x, weapon.spawnRotation.y, weapon.spawnRotation.z);

        weapon.isActiveWeapon = true;
        weapon.animator.enabled = true;
    }

    private void DropCurrentWeapon(GameObject pickedupWeapon)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            var weaponToDrap = activeWeaponSlot.transform.GetChild(0).gameObject;

            weaponToDrap.GetComponent<Weapon>().isActiveWeapon = false;
            weaponToDrap.GetComponent<Weapon>().animator.enabled = false;

            weaponToDrap.transform.SetParent(pickedupWeapon.transform.parent);
            weaponToDrap.transform.localPosition = pickedupWeapon.transform.localPosition;
            weaponToDrap.transform.localRotation = pickedupWeapon.transform.localRotation;
        }
    }
    
    private void SwitchActiveSlot(int slotNumber)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon currentWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            Throwable currentThrowable = activeWeaponSlot.transform.GetChild(0).GetComponent<Throwable>();
            Medkit currentMedkit = activeWeaponSlot.transform.GetChild(0).GetComponent<Medkit>();
            if (currentWeapon != null)
            {
                currentWeapon.isActiveWeapon = false;
            }
            else if (currentThrowable != null)
            {
                currentThrowable.isActiveThrowable = false;
            }
            else if (currentMedkit != null)
            {
                currentMedkit.isActiveMedkit = false;
            }
        }

        activeWeaponSlot = weaponSlots[slotNumber];

        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon newWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            Throwable currentThrowable = activeWeaponSlot.transform.GetChild(0).GetComponent<Throwable>();
            Medkit currentMedkit = activeWeaponSlot.transform.GetChild(0).GetComponent<Medkit>();
            if (newWeapon != null)
            {
                newWeapon.isActiveWeapon = true;
            }
            else if (currentThrowable != null)
            {
                currentThrowable.isActiveThrowable = true;
            }
            else if (currentMedkit != null)
            {
                currentMedkit.isActiveMedkit = true;
            }
        }
    }

    public void PickupAmmo(AmmoBox ammo)
    {
        switch (ammo.ammoType)
        {
            case AmmoBox.AmmoType.PistolAmmo:
                totalPistolAmmo += ammo.ammoAmount;
                break;
            case AmmoBox.AmmoType.RifleAmmo:
                totalRifleAmmo += ammo.ammoAmount;
                break;
            case AmmoBox.AmmoType.SMGAmmo:
                totalSMGAmmo += ammo.ammoAmount;
                break;
        }
    }

    internal void DecreaseTotalAmmo(int bulletsToDecrease, Weapon.WeaponModel thisWeaponModel)
    {
        switch (thisWeaponModel)
        {
            case Weapon.WeaponModel.Pistol1:
                totalPistolAmmo -= bulletsToDecrease; 
                break;
            case Weapon.WeaponModel.AK47:
                totalRifleAmmo -= bulletsToDecrease;
                break;
            case Weapon.WeaponModel.Uzi:
                totalSMGAmmo -= bulletsToDecrease;
                break;
        }
    }

    public int CheckAmmoLeftFor(Weapon.WeaponModel thisWeaponModel)
    {
        switch (thisWeaponModel)
        {
            case Weapon.WeaponModel.Pistol1:
                return totalPistolAmmo;
            case Weapon.WeaponModel.AK47:
                return totalRifleAmmo;
            case Weapon.WeaponModel.Uzi:
                return totalSMGAmmo;

            default:
                return 0;
        }
    }

    public void PickupThrowable(Throwable throwable)
    {
        switch (throwable.throwableType)
        {
            case Throwable.ThrowableType.Grenade :
                PickupThrowableAsLethal(Throwable.ThrowableType.Grenade);
                break;
            case Throwable.ThrowableType.Incendiary:
                PickupThrowableAsLethal(Throwable.ThrowableType.Incendiary);
                break;
        }
    }

    private void PickupThrowableAsLethal(Throwable.ThrowableType lethal)
    {
        if (equippedLethalType == lethal || equippedLethalType == Throwable.ThrowableType.None)
        {
            equippedLethalType = lethal;

            if (lethalsCount < maxLethals)
            {
                lethalsCount += 1;
                Destroy(InteractionManager.Instance.hoveredThrowable.gameObject);
                HUDManager.Instance.UpdateThrowablesUI();

                if (lethalsCount ==1)
                {
                    GameObject throwablePrefab = GetThrowablePrefab(lethal);
                    currentLethalObject = Instantiate(throwablePrefab, weaponSlots[2].transform);
                    SetupThrowableInSlot(currentLethalObject, throwablePrefab.GetComponent<Throwable>());
                    Animator animator = currentLethalObject.GetComponent<Animator>();
                    animator.enabled = true;
                }
                if (activeWeaponSlot == weaponSlots[2])
                {
                    Throwable throwable = currentLethalObject.GetComponent<Throwable>();
                    if (throwable != null)
                    {
                        throwable.isActiveThrowable = true;
                    }
                }
            }
        }
    }

    private void SetupThrowableInSlot(GameObject throwableObject, Throwable throwable)
    {
        throwableObject.transform.SetParent(weaponSlots[2].transform, false);
        throwableObject.transform.localPosition = new Vector3(throwable.spawnPosition.x, throwable.spawnPosition.y, throwable.spawnPosition.z);
        throwableObject.transform.localRotation = Quaternion.Euler(throwable.spawnRotation.x, throwable.spawnRotation.y, throwable.spawnRotation.z);
    }

    private void ThrowLethal()
    {
        if (currentLethalObject != null)
        {
            MonitoringMode.Instance.throwableUsed++;
            GameObject lethalPrefab = GetThrowablePrefab(equippedLethalType);
            GameObject throwable = Instantiate(lethalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
            Rigidbody rb = throwable.AddComponent<Rigidbody>();
            rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);
            rb.mass = 3;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            throwable.GetComponent<Throwable>().hasBeenThrown = true;

            lethalsCount -= 1;

            if (lethalsCount <=0)
            {
                Destroy(currentLethalObject );
                currentLethalObject = null;
                equippedLethalType = Throwable.ThrowableType.None;
            }

            HUDManager.Instance.UpdateThrowablesUI();
        }
    }

    private GameObject GetThrowablePrefab(Throwable.ThrowableType throwableType)
    {
        switch (throwableType)
        {
            case Throwable.ThrowableType.Grenade:
                return grenadePrefab;
            case Throwable.ThrowableType.Incendiary:
                return incendiaryPrefab;
        }
        return new();
    }

    public void UpdateTotalAmmoUsed()
    {
        int totalAmmoUsed = 0;
        foreach (GameObject weaponSlot in weaponSlots)
        {
            if (weaponSlot.transform.childCount > 0)
            {
                Weapon weapon = weaponSlot.transform.GetChild(0).GetComponent<Weapon>();
                if (weapon != null)
                {
                    totalAmmoUsed += weapon.totalAmmoConsumed;
                }
            }
            MonitoringMode.Instance.ammoUsed = totalAmmoUsed; 
        }
    }

    public void PickupMedkit(Medkit mediket)
    {
        if (medkitCount < maxMedkit)
        {
            medkitCount += 1;
            Destroy(InteractionManager.Instance.hoveredMedkit.gameObject);
            HUDManager.Instance.UpdateMedkitUI();

            if (medkitCount == 1)
            {
                currentMedkit = Instantiate(medkitPrefab, weaponSlots[3].transform);
                SetupMedkitInSlot(currentMedkit, medkitPrefab.GetComponent<Medkit>());
                Animator animator = currentMedkit.GetComponent<Animator>();
                animator.enabled = true;
            }
            if (activeWeaponSlot == weaponSlots[3])
            {
                Medkit medkit = currentMedkit.GetComponent<Medkit>();
                if (medkit != null)
                {
                    medkit.isActiveMedkit = true;
                }
            }
        }
    }

    private void SetupMedkitInSlot(GameObject medkitObject, Medkit medkit)
    {
        medkitObject.transform.SetParent(weaponSlots[3].transform, false);
        medkitObject.transform.localPosition = new Vector3(medkit.spawnPosition.x, medkit.spawnPosition.y, medkit.spawnPosition.z);
        medkitObject.transform.localRotation = Quaternion.Euler(medkit.spawnRotation.x, medkit.spawnRotation.y, medkit.spawnRotation.z);
    }

}

