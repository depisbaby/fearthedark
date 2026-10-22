using UnityEngine;

public class MoveLight : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = new Vector3(Mathf.Sin(Time.time), transform.position.y, Mathf.Cos(Time.time));
        transform.position = pos;
        
    }
}
