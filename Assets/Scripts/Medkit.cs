using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : MonoBehaviour
{
    public Vector3 spawnPosition;
    public Vector3 spawnRotation;
    public bool isActiveMedkit;
    public int heal = 80;
    public bool isUsed;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

    }

    private void Update()
    {
        if (isActiveMedkit)
        {
            gameObject.layer = LayerMask.NameToLayer("WeaponRender");
            foreach (Transform child in transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("WeaponRender");
            }

            GetComponent<Outline>().enabled = false;

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Used();
            }
        }
    }

    private void Used()
    {
        animator.SetTrigger("using");
        isUsed = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isUsed == true)
        {
            other.GetComponent<Player>().Heal(heal);
            WeaponManager.Instance.medkitCount -= 1;
            HUDManager.Instance.UpdateMedkitUI();
            Destroy(gameObject);
        }
    }
}
