using UnityEngine;
using UnityEngine.UI;

public class InGamePanel : MonoBehaviour
{
    public static InGamePanel instance;

    public Image ProgressionFillerImg;
    public Text MovesAvailableText;

    int totalPaths;
    [HideInInspector]
    public int currentPath;

    int temp;
    float lerpingValue;

    bool startLerping = false;

    private void Awake()
    {
        instance = this;
    }

    void OnEnable()
    {
        if(PathsController.instance != null)
        {
            StartSetter();
        }
    }

    void StartSetter()
    {
        totalPaths = PathsController.instance.allPaths.Count;

        currentPath = totalPaths;
        temp = currentPath;

        ProgressionFillerImg.fillAmount = 0;

        MovesAvailableText.text = GameManager.MovesAvailable.ToString();
    }

    void Update()
    {
        if(currentPath != temp)
        {
            temp = currentPath;

            lerpingValue = (float)(totalPaths - currentPath) / totalPaths;

            if(lerpingValue != 1)
            {
                startLerping = true;
            }
            else
            {
                ProgressionFillerImg.fillAmount = lerpingValue;
            }
        }

        if(startLerping)
        {
            ProgressionFillerImg.fillAmount = Mathf.Lerp(ProgressionFillerImg.fillAmount, lerpingValue, .1f);
            if(Mathf.Abs(ProgressionFillerImg.fillAmount - lerpingValue) < .01f)
            {
                startLerping = false;
                ProgressionFillerImg.fillAmount = lerpingValue;
            }
        }
    }

    public void UpdateMoves()
    {
        MovesAvailableText.text = GameManager.MovesAvailable.ToString();
        MovesAvailableText.GetComponent<Animator>().SetTrigger("popup");
    }
}
