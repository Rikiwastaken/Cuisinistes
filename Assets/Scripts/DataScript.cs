
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataScript : MonoBehaviour
{

    [Serializable]
    public class OptionsClass
    {
        public float Mastervol;
        public float MusicVol;
        public float SFXVol;
        public float sensibility;
    }

    public OptionsClass Options;

    public List<GameObject> cluelist;

    public int currentPerpID;

    public List<int> cluesFound;

    public static DataScript instance;

    private int GameOverCD;

    public int remainingHP;


    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (SceneManager.GetActiveScene().name == "FirstScene")
        {
            SceneManager.activeSceneChanged += SceneChange;
            SceneManager.LoadScene("MainMenu");
        }

        LoadOptions();
    }


    private void Update()
    {
        if (GameOverCD > 0)
        {
            GameOverCD++;
            Debug.Log(GameOverCD);
            if (GameOverCD > 5f / Time.deltaTime)
            {
                SceneManager.LoadScene("MainMenu");
                GameOverCD = 0;
            }

        }

        EndCheck endCheck = EndCheck.instance;

        MovementController MvtController = MovementController.instance;

        PickUpObjects PUObj = null;

        if (MvtController != null)
        {
            PUObj = MvtController.GetComponent<PickUpObjects>();
        }

        if (endCheck != null && endCheck.playerinside && PUObj != null && PUObj.heldClues.Count >= 5)
        {
            cluesFound = new List<int>();
            foreach (int clue in PUObj.heldClues)
            {
                cluesFound.Add(clue);
            }
            SceneManager.LoadScene("FinalScene");
        }

        if (Options.sensibility == 0)
        {
            Options = new OptionsClass();
            Options.MusicVol = 1.000001f;
            Options.SFXVol = 1.000001f;
            Options.Mastervol = 1.000001f;
            Options.sensibility = 0.000001f;
        }

    }

    private void LoadOptions()
    {
        string path = Application.persistentDataPath;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        string fileName = "options.json";
        string fullPath = Path.Combine(path, fileName);

        try
        {
            string json = File.ReadAllText(fullPath);
            if (json != null)
            {
                Options = JsonUtility.FromJson<OptionsClass>(json);
            }
        }
        catch
        {
            Debug.Log("creating new options data");
            Options = new OptionsClass();
            Options.MusicVol = 1.000001f;
            Options.SFXVol = 1.000001f;
            Options.Mastervol = 1.000001f;
            Options.sensibility = 1.000001f;
        }
        SoundManager.instance.ChangeVolume();
    }

    public void SaveOptions()
    {
        if (Options == null)
        {
            Options = new OptionsClass();
        }
        string path = Application.persistentDataPath;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        string fileName = "options.json";
        string fullPath = Path.Combine(path, fileName);
        string json = JsonUtility.ToJson(Options, true);

        try
        {
            File.WriteAllText(fullPath, json);
            Debug.Log($"Options Saved : {fullPath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error when saving options : {e.Message}");
        }

        if (SoundManager.instance != null)
        {
            SoundManager.instance.ChangeVolume();

        }

    }


    void SceneChange(Scene arg0, Scene arg1)
    {
        if (arg1.name == "Mapping")
        {
            InitializeClues();
        }
    }

    public void TakeDamage()
    {
        if (remainingHP > 0)
        {
            MovementController.instance.transform.position = MovementController.instance.StartPos;
            MovementController.instance.justtookdamage = true;
            EnemyController.instance.transform.position = EnemyController.instance.Startpos;
            EnemyController.instance.chasing = false;
            EnemyController.instance.seeingplayer = false;
            remainingHP--;

        }
        else
        {
            SceneManager.LoadScene("GameOverScene");
            GameOverCD = 1;
        }

    }

    void InitializeClues()
    {
        GameObject ClueHolderGO = GameObject.Find("Clue");


        if (ClueHolderGO == null)
        {
            Debug.Log("Error : didn't find Clue GO");
            return;
        }
        Transform ClueHolder = GameObject.Find("Clue").transform;
        currentPerpID = UnityEngine.Random.Range(0, 5);

        List<GameObject> relevantclues = new List<GameObject>();

        foreach (GameObject clue in cluelist)
        {
            if (clue.GetComponent<ThrowObjectScript>().clueOwnerIndex.Contains(currentPerpID))
            {
                relevantclues.Add(clue);
            }
        }


        List<Vector3> cluespositions = new List<Vector3>();

        int foundcluesposition = 0;

        int safeguard = 0;

        while (foundcluesposition < 5 && safeguard < 200)
        {
            int clueholderID = UnityEngine.Random.Range(0, ClueHolder.childCount);
            Vector3 newpos = ClueHolder.GetChild(clueholderID).position;
            if (!cluespositions.Contains(newpos))
            {
                cluespositions.Add(newpos);
                foundcluesposition++;
            }
            safeguard++;
        }
        if (safeguard >= 200)
        {
            Debug.Log("loop could not end");
        }

        for (int i = 0; i < 5; i++)
        {
            GameObject newclue = Instantiate(relevantclues[i]);
            newclue.transform.position = cluespositions[i];
        }

        Debug.Log("Clues Initialized");
    }

}
