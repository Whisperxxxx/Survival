using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public GameObject settingsPanel;
    public Slider sensitivitySlider;
    public MouseMovement mouseMovement;

    private bool isPaused = false;

    void Start()
    {
        sensitivitySlider.value = mouseMovement.mouseSensitivity;
        sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        settingsPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UpdateSensitivity(float sensitivity)
    {
        mouseMovement.mouseSensitivity = sensitivity;
    }


    public void CloseButton()
    {
        Resume();
    }

    public void Restart()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(currentSceneName);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void QuitGame()
    {
        // If running in the Unity 
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If running the built game
        Application.Quit();
#endif
    }
}
