using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TMP_Text highScoreUI;
    string newGameScence = "SampleScene";

    void Start()
    {
        // Set the high score texxt
        int highScore = SaveLoadManager.Instance.LoadHighScore();
        highScoreUI.text = $"Top Wave Survived: {highScore}";

    }

    public void StartNewGame()
    {
        SceneManager.LoadScene(newGameScence);

    }

    public void ExitApplication()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
