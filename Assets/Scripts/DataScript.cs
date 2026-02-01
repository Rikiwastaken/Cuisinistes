using UnityEngine;
using UnityEngine.SceneManagement;

public class DataScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (SceneManager.GetActiveScene().name == "FirstScene")
        {
            SceneManager.activeSceneChanged += SceneChange;
            SceneManager.LoadScene("MainMenu");
        }
    }

    void SceneChange(Scene arg0, Scene arg1)
    {
        if (arg1.name == "Mapping")
        {
            InitializeClues();
        }
    }

    void InitializeClues()
    {
        Debug.Log("testInitializeClues");
    }

}
