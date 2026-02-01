
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataScript : MonoBehaviour
{

    public bool finished;

    public List<GameObject> cluelist;

    public int currentPerpID;

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


    private void Update()
    {
        EndCheck endCheck = EndCheck.instance;

        MovementController MvtController = MovementController.instance;

        PickUpObjects PUObj = null;

        if (MvtController != null)
        {
            PUObj = MvtController.GetComponent<PickUpObjects>();
        }

        if (endCheck != null && endCheck.playerinside && PUObj != null && PUObj.heldClues.Count >= 5)
        {
            finished = true;
        }
        else
        {
            finished = false;
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
