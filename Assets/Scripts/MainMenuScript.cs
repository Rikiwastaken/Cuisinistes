using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{

    public AudioSource UISound;
    public void Start()
    {
        Application.targetFrameRate = 60;
        Cursor.lockState = CursorLockMode.None;
        DontDestroyOnLoad(UISound.gameObject);
    }


    public void Quit()
    {
        Application.Quit();
    }

    public void Play()
    {
        DataScript.instance.remainingHP = 2;
        SceneManager.LoadScene("Mapping");
    }
}
