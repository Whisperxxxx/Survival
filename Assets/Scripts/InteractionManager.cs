using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; set; }

    public Weapon hoveredWeapon = null;
    public AmmoBox hoveredAmmoBox = null;
    public Throwable hoveredThrowable = null;
    public Medkit hoveredMedkit = null;


    Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;

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
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject target = hit.transform.gameObject;

            if (target.GetComponent<Weapon>() && target.GetComponent<Weapon>().isActiveWeapon == false)
            { 
                if (hoveredWeapon)
                {
                    hoveredWeapon.GetComponent<Outline>().enabled = false;
                }

                // Highlight the weapon
                hoveredWeapon = target.gameObject.GetComponent<Weapon>();
                hoveredWeapon.GetComponent<Outline>().enabled = true;

                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaponManager.Instance.PickupWeapon(target.gameObject);
                }
            }
            else
            {
                // Close the highlight
                if (hoveredWeapon)
                {
                    hoveredWeapon.GetComponent<Outline>().enabled = false;                 }
            }

            // AmmoBox      
            if (target.GetComponent<AmmoBox>())
            {
                if (hoveredAmmoBox)
                {
                    hoveredAmmoBox.GetComponent<Outline>().enabled = false;
                }
                // Highlight the weapon
                hoveredAmmoBox = target.gameObject.GetComponent<AmmoBox>();
                hoveredAmmoBox.GetComponent<Outline>().enabled = true;

                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaponManager.Instance.PickupAmmo(hoveredAmmoBox);
                    Destroy(target.gameObject);
                }
            }
            else
            {
                // Close the highlight
                if (hoveredAmmoBox)
                {
                    hoveredAmmoBox.GetComponent<Outline>().enabled = false;
                }
            }

            // Throwable      
            if (target.GetComponent<Throwable>())
            {
                if (hoveredThrowable)
                {
                    hoveredThrowable.GetComponent<Outline>().enabled = false;
                }
                // Highlight the weapon
                hoveredThrowable = target.gameObject.GetComponent<Throwable>();
                
                if (!hoveredThrowable.hasBeenThrown)
                {
                    hoveredThrowable.GetComponent<Outline>().enabled = true;

                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        WeaponManager.Instance.PickupThrowable(hoveredThrowable);
                    }

                }

            }
            else
            {
                // Close the highlight
                if (hoveredThrowable)
                {
                    hoveredThrowable.GetComponent<Outline>().enabled = false;
                }
            }

            // Medkit     
            if (target.GetComponent<Medkit>())
            {
                if (hoveredMedkit)
                {
                    hoveredMedkit.GetComponent<Outline>().enabled = false;
                }
                // Highlight the weapon
                hoveredMedkit = target.gameObject.GetComponent<Medkit>();
                hoveredMedkit.GetComponent<Outline>().enabled = true;

                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaponManager.Instance.PickupMedkit(hoveredMedkit);
                }
            }
            else
            {
                // Close the highlight
                if (hoveredMedkit)
                {
                    hoveredMedkit.GetComponent<Outline>().enabled = false;
                }
            }
        }
    }
}
