using UnityEngine;

public class RotateOnAxis : MonoBehaviour
{
   
    public Vector3 axis = Vector3.up;

    
    public float speed = 45f;

    void Update()
    {
        transform.Rotate(axis.normalized * speed * Time.deltaTime, Space.Self);
    }
}
