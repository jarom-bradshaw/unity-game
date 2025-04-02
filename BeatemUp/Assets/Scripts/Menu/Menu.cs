using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
    NoSQLDatabase database;

    void Start()
    {
        database = new NoSQLDatabase("GameDatabase", "HighScores");

        float? highScore = database.GetHighScore();
        Debug.Log($"Current High Score: {highScore}");
    }
    // Called when we click the "Play" button.
    public void OnPlayButton()
    {
        SceneManager.LoadScene(1);
    }
    // Called when we click the "Quit" button.
    public void OnQuitButton()
    {
        Application.Quit();
    }

}