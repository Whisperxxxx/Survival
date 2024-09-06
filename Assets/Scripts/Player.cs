using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public int HP = 100;
    public GameObject bloodyScreen;


    public TextMeshProUGUI playerHealthUI;

    public GameObject gameOverUI;
    public GameObject restartUI;
    public GameObject exitUI;

    public bool isDead;

    private void Start()
    {
        playerHealthUI.text = $"Health:{HP}";
    }
     
    public void TakeDamage(int damage)
    {

        HP -= damage;

        MonitoringMode.Instance.health = HP;

        if (HP <= 0)
        {
            isDead = true;
            PlayerDead();
        }
        else
        {
            StartCoroutine(BloodyScreenEffect());
            playerHealthUI.text = $"Health:{HP}";

        }
    }

    private IEnumerator BloodyScreenEffect()
    {
        if (!isDead)
        {
            if (bloodyScreen.activeInHierarchy == false)
            {
                bloodyScreen.SetActive(true);
            }

            var image = bloodyScreen.GetComponentInChildren<Image>();

            // Set the initial alpha value to 1.
            Color startColor = image.color;
            startColor.a = 1f;
            image.color = startColor;

            float duration = 2f;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                // Calculate the new alpha value using Lerp.
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);

                // Update the color with the new alpha value.
                Color newColor = image.color;
                newColor.a = alpha;
                image.color = newColor;

                // Increment the elapsed time.
                elapsedTime += Time.deltaTime;

                yield return null; ; // Wait for the next frame.
            }

            if (bloodyScreen.activeInHierarchy)
            {
                bloodyScreen.SetActive(false);
            }
        }
       
    }

    private void PlayerDead()
    {
        GetComponent<MouseMovement>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;

        GetComponentInChildren<Animator>().enabled = true;

        playerHealthUI.gameObject.SetActive(false);

        GetComponent<ScreenFader>().StartFade();
        StartCoroutine(ShowGameOverUI());
    }

    private IEnumerator ShowGameOverUI()
    {
        yield return new WaitForSeconds(1f);
        Cursor.lockState = CursorLockMode.None;
        gameOverUI.gameObject.SetActive(true);
        restartUI.gameObject.SetActive(true);
        exitUI.gameObject.SetActive(true);
    }


    public void Heal(int heal)
    {
        HP += heal;
        if (HP > 100) 
        {
            HP = 100;
        }

        MonitoringMode.Instance.health = HP;
        playerHealthUI.text = $"Health:{HP}";

    }
}
