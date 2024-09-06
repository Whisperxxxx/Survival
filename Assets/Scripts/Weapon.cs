using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Weapon : MonoBehaviour
{
    public bool isActiveWeapon;
    public int weaponDamage;


    //Shooting state
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 0.2f;

    // Burst
    public int bulletPerBurst = 3;
    public int burstBulletsLeft;

    // Spread
    public float spreadIntensity;

    // Bullet
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 300;
    public float bulletPrefabLifeTime = 3f;

    public GameObject muzzleEffect;
    internal Animator animator; 

    // Reloading
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    public int totalAmmoConsumed; // Track the ammo consumed

    public enum WeaponModel
    {
        Pistol1,
        AK47,
        Uzi
    }

    public WeaponModel thisWeaponModel;

    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }
    
    public ShootingMode currentShootingMode;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletPerBurst;
        animator = GetComponent<Animator>();
        bulletsLeft = magazineSize;
    }

    void Update()
    {
        if (isActiveWeapon)
        {
            gameObject.layer = LayerMask.NameToLayer("WeaponRender");
            foreach (Transform child in transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("WeaponRender");
            }

            GetComponent<Outline>().enabled = false;

            if (bulletsLeft == 0 && isShooting && !isReloading)
            {
                SoundManager.Instance.emptySoundPistol1.Play();
            }

            if (currentShootingMode == ShootingMode.Auto)
            {
                // Continuous shooting
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }
            else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
            {
                //Shooting once
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloading == false && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0 )
            {
                Reload();
            }

            // Automatically reload the bullets.
            if (readyToShoot && isShooting && !isReloading && bulletsLeft <= 0 && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0)
            {
                Reload();
            }

            // Check if can shoot
            if (readyToShoot && isShooting && !isReloading && bulletsLeft > 0)
            {
                burstBulletsLeft = bulletPerBurst;
                FireWeapon();
            }


        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
            foreach (Transform child in transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }
    }
    

    private void FireWeapon()
    {
        bulletsLeft--;
        totalAmmoConsumed++; 

        muzzleEffect.GetComponent<ParticleSystem>().Play();
        animator.SetTrigger("recoil");

        SoundManager.Instance.PlayShootingSound(thisWeaponModel);

        readyToShoot = false;
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        // Instantiate the bullet  
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        Bullet bul = bullet.GetComponent<Bullet>();
        bul.bulletDamage = weaponDamage;

        // Point the bullet direction
        bullet.transform.forward = shootingDirection;

        // Shoot the bullet
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        // Destory the bullet
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        // Shooting cd
        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        // Burst Mode
        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }

    private void Reload()
    {
        SoundManager.Instance.PlayReloadSound(thisWeaponModel);


        animator.SetTrigger("reload");

        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }

    private void ReloadCompleted()
    {
        int bulletConsumption = magazineSize - bulletsLeft; // The number of ammo that needs to be reloading
        int ammoLeft = WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel);

        if (ammoLeft >= bulletConsumption)
        {
            WeaponManager.Instance.DecreaseTotalAmmo(bulletConsumption, thisWeaponModel);
            bulletsLeft = magazineSize;
        }
        else
        {
            WeaponManager.Instance.DecreaseTotalAmmo(ammoLeft, thisWeaponModel);
            bulletsLeft += ammoLeft;
        }
        isReloading = false;
    }


    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        // Emit a ray from the center of the camera
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point; // The position of the target that been hit
        }
        else
        {
            targetPoint = ray.GetPoint(100); // If no target is hit
        }

        // Calculate the direction
        Vector3 direction = targetPoint - bulletSpawn.position;

        // Bullet spread
        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(x, y, 0);
    }
    private IEnumerator DestroyBulletAfterTime(GameObject bullet , float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);

    }
}
