using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public static float speed = 5f;
    private Transform _rotator;
    
    void Start()
    {
        _rotator = GetComponent<Transform>();
    }

    
    public void Update()
    {  
            _rotator.Rotate(0, speed * Time.deltaTime, 0);
    }
}
