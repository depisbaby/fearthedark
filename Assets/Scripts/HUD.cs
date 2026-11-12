using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static HUD Instance;
    private void Awake()
    {
        Instance = this;
    }

    public Image blindFold;
    public void SetBlind(bool blind)
    {
        blindFold.gameObject.SetActive(blind);
    }
}
