using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    public AudioSource ShootingChannel;



    public AudioClip Pistol1Shot;
    public AudioClip AK47Shot;
    public AudioClip UziShot;

    public AudioSource reloadingSoundPistol1;
    public AudioSource reloadingSoundAK47;

    public AudioSource emptySoundPistol1;

    public AudioSource throwablesChannel;
    public AudioClip grenadeSound;
    public AudioClip incendiarySound;
    public AudioClip openSound;

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

    public void PlayShootingSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Pistol1:
                ShootingChannel.PlayOneShot(Pistol1Shot); 
                break;

            case WeaponModel.AK47:
                ShootingChannel.PlayOneShot(AK47Shot);
                break;
            case WeaponModel.Uzi:
                ShootingChannel.PlayOneShot(UziShot);
                break;
        }
    }

    public void PlayReloadSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Pistol1:
                reloadingSoundPistol1.Play();
                break;

            case WeaponModel.AK47:
                reloadingSoundAK47.Play();
                break;
            case WeaponModel.Uzi:
                reloadingSoundAK47.Play();
                break;
        }
    }
}
