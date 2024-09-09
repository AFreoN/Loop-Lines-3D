using UnityEngine;

public class DataManager : MonoBehaviour
{
    [SerializeField] bool LoadCustomLevel = false;
    [SerializeField] int LevelToLoad = 1;

    const string levelKey = "Level";

    const int totalLevel = 10;
    public static int currentLevel = 0;

    private void Awake()
    {
        if(!PlayerPrefs.HasKey(levelKey))
        {
            PlayerPrefs.SetInt(levelKey, 0);
        }

        if (LoadCustomLevel)
            PlayerPrefs.SetInt(levelKey, (LevelToLoad-1) % totalLevel);

        currentLevel = LoadLevel;
    }

    public static void SaveLevel()
    {
        currentLevel = (currentLevel + 1) % totalLevel;
        PlayerPrefs.SetInt(levelKey, currentLevel);
    }

    public static int LoadLevel => PlayerPrefs.GetInt(levelKey);
}
