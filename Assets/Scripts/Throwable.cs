using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    [SerializeField]
    float delay = 3f;
    [SerializeField]
    float damageRadius = 20f;
    [SerializeField]
    float explosionForce = 1200f;
    [SerializeField]
    float fireDuration = 5f;
    [SerializeField]
    float damageInterval = 1f;
    public float timer = 0;


    public Vector3 spawnPosition;
    public Vector3 spawnRotation;
    public bool isActiveThrowable;
    public Animator animator;

    float countdown;
    
    bool hasExploded = false;
    public bool hasBeenThrown = false;

    public enum ThrowableType
    {
        None,
        Grenade,
        Incendiary
    }

    public ThrowableType throwableType;

    private void Start()
    {
        countdown = delay;
        animator = GetComponent<Animator>();

    }

    private void Update()
    {   
        if (isActiveThrowable)
        {
            gameObject.layer = LayerMask.NameToLayer("WeaponRender");
            foreach (Transform child in transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("WeaponRender");
            }

            GetComponent<Outline>().enabled = false;

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                ThrowableAnim();
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {

                animator.SetBool("open", false);
            }

        }
        if (hasBeenThrown)
        {
            switch (throwableType)
            {
                case ThrowableType.Grenade:
                    countdown -= Time.deltaTime;
                    if (countdown <= 0f && !hasExploded)
                    {
                        Explode();
                        hasExploded = true;
                    }
                    break;
                case ThrowableType.Incendiary:
                    break;

            }
        }
    }
    public void ThrowableAnim()
    {
        animator.SetBool("open", true);
        SoundManager.Instance.throwablesChannel.PlayOneShot(SoundManager.Instance.openSound);
    }

    private void Explode()
    {
        GetThrowableEffect();
    }

    private void GetThrowableEffect()
    {
        switch (throwableType)
        {
            case ThrowableType.Grenade:
                GrenadeEffect();
                break;

            case ThrowableType.Incendiary: 
                IncendiaryEffect(); 
                break;
        }
    }

    private void GrenadeEffect()
    {
        // Visual Effect
        GameObject explosionEffect = GlobalReferences.Instance.grenadeExplosionEffect;
        Instantiate(explosionEffect, transform.position, transform.rotation);

        // Play Sound
        SoundManager.Instance.throwablesChannel.PlayOneShot(SoundManager.Instance.grenadeSound);

        // Physical Effect
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);

        foreach (Collider objectInRange in colliders)
        {
            Rigidbody rb = objectInRange.GetComponent<Rigidbody>();
            if(rb != null )
            {
                rb.AddExplosionForce(explosionForce, transform.position, damageRadius);
            }

            if (objectInRange.gameObject.GetComponent<Enemy>())
            {
                objectInRange.gameObject.GetComponent<Enemy>().TakeDamage(100);
            }
        }
        Destroy(gameObject);

    }

    private void IncendiaryEffect()
    {
        // Visual Effect
        GameObject explosionEffect = GlobalReferences.Instance.incendiaryExplosionEffect;
        Instantiate(explosionEffect, transform.position, transform.rotation);

        // Play Sound
        SoundManager.Instance.throwablesChannel.PlayOneShot(SoundManager.Instance.incendiarySound);

        // Physical Effect
        StartCoroutine(FireDamage(transform.position, damageRadius, fireDuration));

    }

    private IEnumerator FireDamage(Vector3 position, float radius, float duration)
    {
        while (timer < duration)
        {
            Collider[] colliders = Physics.OverlapSphere(position, radius);
            foreach (Collider objectInRange in colliders)
            {
                if (objectInRange.gameObject.GetComponent<Enemy>())
                {
                    objectInRange.gameObject.GetComponent<Enemy>().TakeDamage(40);
                }
            }
            timer += damageInterval;
            yield return new WaitForSeconds(damageInterval);
        }
        if (timer == duration)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (throwableType == ThrowableType.Incendiary && !hasExploded && hasBeenThrown)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Explode();
                hasExploded = true;
            }
        }
    }

}
