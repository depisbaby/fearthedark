using UnityEngine;

public class OneBitFilter : MonoBehaviour
{
    public static OneBitFilter Instance;
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] Material screenSpaceMaterial;
    [SerializeField] Color normalColor;
    [SerializeField] Color huntedColor;


    Color targetColor;
    float targetEdge;
    Color color;
    float edge;


    // Update is called once per frame
    void Update()
    {
        color = Color.Lerp(color, targetColor, Time.deltaTime * 3);
        edge = Mathf.Lerp(edge, targetEdge, Time.deltaTime);
        screenSpaceMaterial.SetColor("_Color", color);
        screenSpaceMaterial.SetFloat("_Edge1", edge);
    }

    public void SetFilterNormal(bool jump)
    {
        if (jump)
        {
            screenSpaceMaterial.SetColor("_Color", normalColor);
            screenSpaceMaterial.SetFloat("_Edge1", 5.0f);
        }

        targetColor = normalColor;
        targetEdge = 5.0f;
    }

    public void SetFilterHunted(bool jump)
    {
        if (jump)
        {
            screenSpaceMaterial.SetColor("_Color", huntedColor);
            screenSpaceMaterial.SetFloat("_Edge1", 0.5f);
        }

        targetColor = huntedColor;
        targetEdge = 0.5f;
    }
}
