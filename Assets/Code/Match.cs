using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Match : MonoBehaviour
{
    private Camera _camera;
    void Awake()
    {
        _camera = GetComponent<Camera>();
        _camera.orthographicSize = Screen.height / 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
