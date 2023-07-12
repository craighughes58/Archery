using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [Tooltip("The sound buttons make when they are pressed")]
    public AudioClip ButtonNoise;

    /// <summary>
    /// change the scene to the scene given by name to this method
    /// </summary>
    /// <param name="name">the name of the scene that must be loaded</param>
    public void MoveScene(string name)
    {
        //the noise that plays when changing scene
        AudioSource.PlayClipAtPoint(ButtonNoise, Camera.main.transform.position);
        //change the scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(name);
    }

    /// <summary>
    /// exit the game
    /// called from a button
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
