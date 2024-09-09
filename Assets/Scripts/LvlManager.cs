using UnityEngine;

public class LvlManager : MonoBehaviour
{ 
    [Header("Shapes")]
    public GameObject[] allShapes;

    [Header("Move Counts")]
    [SerializeField] int[] MoveCount = null;

    private void Awake()
    {
        int num = DataManager.currentLevel;

        gameObject.GetComponent<PathsController>().Shape = allShapes[num].transform;

        foreach (GameObject g in allShapes)
            g.SetActive(false);

        allShapes[num].SetActive(true);

        PlayerController.currentShape = allShapes[num].transform;
        GameManager.MovesAvailable = MoveCount[num];
    }
}
