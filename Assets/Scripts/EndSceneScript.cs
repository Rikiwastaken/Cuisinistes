
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneScript : MonoBehaviour
{

    public Animator EnemyAnimator;

    public Transform ClueVisuals;

    public List<TextMeshProUGUI> CluesDescriptions;

    public TextMeshProUGUI DescrText;
    public bool fall;

    public bool escapes;

    private int TimeBeforeMainMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(DataScript.instance.cluesFound.Count);
        for (int i = 0; i < DataScript.instance.cluesFound.Count; i++)
        {
            Vector3 clueposition = ClueVisuals.GetChild(i).position;
            foreach (GameObject clue in DataScript.instance.cluelist)
            {
                if (clue.GetComponent<ThrowObjectScript>().clueID == DataScript.instance.cluesFound[i])
                {
                    GameObject newclue = Instantiate(clue);
                    newclue.transform.position = ClueVisuals.GetChild(i).position;
                    CluesDescriptions[i].text = clue.name;
                    break;
                }

            }
        }
    }

    public void Update()
    {
        EnemyAnimator.SetBool("Fall", fall);

        if (escapes)
        {
            EnemyAnimator.GetComponentInParent<Rigidbody>().linearVelocity = new Vector3(1.5f, 0, 0);
            EnemyAnimator.GetComponentInParent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
            EnemyAnimator.transform.forward = new Vector3(1, 0, 0);
            EnemyAnimator.SetFloat("speed", EnemyAnimator.GetComponentInParent<Rigidbody>().linearVelocity.magnitude);
        }

        if (escapes || fall)
        {
            TimeBeforeMainMenu++;
            if (TimeBeforeMainMenu > 10f / Time.deltaTime)
            {
                SceneManager.LoadScene("MainMenu");
            }
        }

    }

    public void ChoosePerp(int PerpID)
    {
        if (PerpID == DataScript.instance.currentPerpID)
        {
            DescrText.text = "You caught the perp !";
            fall = true;
        }
        else
        {
            DescrText.text = "The perp escaped...";
            escapes = true;
        }
    }
}
