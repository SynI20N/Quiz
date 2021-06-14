using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Match : MonoBehaviour
{
    private Camera camera;
    void Awake()
    {
        camera = GetComponent<Camera>();
        camera.orthographicSize = Screen.height / 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
