using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int bulletDamage;
    private int damageMultiplier = 1;

   private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            CreateBulletImpactEffect(collision);
            Destroy(gameObject);

        }
        if (collision.gameObject.CompareTag("Beer"))
        {
            collision.gameObject.GetComponent<BeerBottle>().Shatter();
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.GetComponent<Enemy>().isDead == false)
            {
                MonitoringMode.Instance.hitshot++;
                collision.gameObject.GetComponent<Enemy>().TakeDamage(bulletDamage * damageMultiplier );
            }

            CreateBloodSprayEffect(collision);
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Head"))

        {
            HitDetector hitDetector = collision.gameObject.GetComponent<HitDetector>();
            if (hitDetector != null)
            {
                MonitoringMode.Instance.headshot ++;
                MonitoringMode.Instance.hitshot++;

                hitDetector.ReceiveDamage(bulletDamage * 4); 
            }
            CreateBloodSprayEffect(collision);
            Destroy(gameObject);
        }


    }

    void CreateBulletImpactEffect(Collision collision)
    {
        // Get the information of the first contact point
        ContactPoint contact = collision.contacts[0];
        
        // instantiate the hole in the contact point
        GameObject hole = Instantiate(GlobalReferences.Instance.bulletImpactEffectPrefab, contact.point, Quaternion.LookRotation(contact.normal));

        // Make the position of hole follow the position of target
        hole.transform.SetParent(collision.gameObject.transform);
    }

    private void CreateBloodSprayEffect(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];

        GameObject bloodSprayPrefab = Instantiate(GlobalReferences.Instance.bloodSprayEffect, contact.point, Quaternion.LookRotation(contact.normal));

        bloodSprayPrefab.transform.SetParent(collision.gameObject.transform);
    }
}
