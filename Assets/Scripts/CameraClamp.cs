using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClamp : MonoBehaviour
{
   [SerializeField] private Transform _targetToFollow;

    void Update()
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, 0f, 10f), 
            Mathf.Clamp(transform.position.y, 0f, 0f), transform.position.z);
    }
}
