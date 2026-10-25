using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;
    private void Awake()
    {
        Instance = this;
    }

    public GameObject baseObject;
    public TMPro.TMP_InputField connectionAddress;
    public TMPro.TMP_Text error;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Close()
    {
        baseObject.SetActive(false);
    }

    public void SoloClicked()
    {
        SessionManager.Instance.Solo();
    }

    public void HostClicked()
    {
        SessionManager.Instance.AttemptHosting();
    }

    public void JoinClicked()
    {
        SessionManager.Instance.AttemptJoining(connectionAddress.text);
    }

    public void DisplayConnectionError(string _error)
    {
        error.text = _error;
    }



}
