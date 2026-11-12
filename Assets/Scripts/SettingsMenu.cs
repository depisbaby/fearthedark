using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public static SettingsMenu Instance;
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private GameObject baseElement;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Toggle();
        }
    }

    void Toggle()
    {
        if (baseElement.activeSelf)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    public void Open()
    {
        baseElement.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Close()
    {
        baseElement.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ExitToMainMenuPressed()
    {
        GameManager.Instance.ExitToMainMenu("");
    }
}
