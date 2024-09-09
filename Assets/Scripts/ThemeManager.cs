using UnityEngine;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{
    [SerializeField] int ThemeID = 1;

    [Header("For objects")]
    [SerializeField] Color[] colors = null;
    [SerializeField] [ColorUsage(false, true)] Color[] emissionColors = null;
    [SerializeField] Material[] reflectedMaterials = null;

    [Header("For Background")]
    [SerializeField] Image bgImage = null;
    [SerializeField] Sprite[] bgSprites = null;
    [SerializeField] Image fillerImg = null;

    //public static Material coverMat { get; private set; }

    private void Awake()
    {
        int id = ThemeID - 1;
        //coverMat = coverMaterials[id % coverMaterials.Length];
        //mainPointShower.material = sphereMaterials[id % sphereMaterials.Length];

        Color currentColor = colors[id % colors.Length];
        Color currentEmissionColor = emissionColors[id % emissionColors.Length];
        foreach(Material mat in reflectedMaterials)
        {
            mat.SetColor("_BaseColor", currentColor);
            mat.SetColor("_EmissionColor", currentEmissionColor);
        }

        bgImage.sprite = bgSprites[id % bgSprites.Length];
        fillerImg.color = currentColor;
    }
}
