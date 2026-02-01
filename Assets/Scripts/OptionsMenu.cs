using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{

    public Image SFXBar;
    public Image MusicBar;
    public Image MasterBar;


    private void Start()
    {
        DataScript Datascript = DataScript.instance;
        SFXBar.fillAmount = Datascript.Options.SFXVol / 2f;
        MusicBar.fillAmount = Datascript.Options.MusicVol / 2f;
        MasterBar.fillAmount = Datascript.Options.Mastervol / 2f;
    }

    public void Increasevol(int ID)
    {
        Debug.Log(ID);
        DataScript Datascript = DataScript.instance;
        if (ID == 0)
        {
            Datascript.Options.SFXVol += 0.1f;
            if (Datascript.Options.SFXVol > 2.000001f)
            {
                Datascript.Options.SFXVol = 2.000001f;
            }
            SFXBar.fillAmount = Datascript.Options.SFXVol / 2f;
            Datascript.SaveOptions();
        }
        else if (ID == 1)
        {
            Datascript.Options.MusicVol += 0.1f;
            if (Datascript.Options.MusicVol > 2.000001f)
            {
                Datascript.Options.MusicVol = 2.000001f;
            }
            MusicBar.fillAmount = Datascript.Options.MusicVol / 2f;
            Datascript.SaveOptions();
        }
        else if (ID == 2)
        {
            Datascript.Options.Mastervol += 0.1f;
            if (Datascript.Options.Mastervol > 2.000001f)
            {
                Datascript.Options.Mastervol = 2.000001f;
            }
            MasterBar.fillAmount = Datascript.Options.Mastervol / 2f;
            Datascript.SaveOptions();
        }
    }
    public void DecreaseVol(int ID)
    {
        Debug.Log(ID);
        DataScript Datascript = DataScript.instance;
        if (ID == 0)
        {
            Datascript.Options.SFXVol -= 0.1f;
            if (Datascript.Options.SFXVol < 0.000001f)
            {
                Datascript.Options.SFXVol = 0.000001f;
            }
            SFXBar.fillAmount = Datascript.Options.SFXVol / 2f;
            Datascript.SaveOptions();
        }
        else if (ID == 1)
        {
            Datascript.Options.MusicVol -= 0.1f;
            if (Datascript.Options.MusicVol < 0.000001f)
            {
                Datascript.Options.MusicVol = 0.000001f;
            }
            MusicBar.fillAmount = Datascript.Options.MusicVol / 2f;
            Datascript.SaveOptions();
        }
        else if (ID == 2)
        {
            Datascript.Options.Mastervol -= 0.1f;
            if (Datascript.Options.Mastervol < 0.000001f)
            {
                Datascript.Options.Mastervol = 0.000001f;
            }
            MasterBar.fillAmount = Datascript.Options.Mastervol / 2f;
            Datascript.SaveOptions();
        }
    }
}
