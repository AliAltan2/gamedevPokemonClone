using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Source : https://www.youtube.com/watch?v=zc8ac_qUXQY also known as Barckleys 
public class MainMenuScript : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
    public void QuitGame()
    {
        Debug.Log("Successfuly Quited");
        Application.Quit();
    }
}
