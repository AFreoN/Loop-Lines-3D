using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //For MainMenu
    public Text lvlText;
    public Text lvlTextShadow;

    //For InGamePanel
    public Text currentLvl;
    public Text nextLvl;

    //For StageFinishedPanel
    public Text currentLvlText;

    void Start()
    {
        string current = (DataManager.currentLevel+1).ToString();
        string next = (DataManager.currentLevel+2).ToString();

        lvlText.text = "Level   " + "<color=#F3FF00>"+ current +"</color>";
        lvlTextShadow.text = "Level   " + current;

        currentLvl.text = current;
        nextLvl.text = next;

        currentLvlText.text = "Level " + current;
    }
}
