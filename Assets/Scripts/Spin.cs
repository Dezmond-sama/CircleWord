using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField]private float speed = 10f;
    [SerializeField] private float startAngle = 10f;
    private void Start()
    {
        transform.Rotate(Vector3.back, startAngle);
    }
    private void Update()
    {
        transform.Rotate(Vector3.back, speed * Time.deltaTime);
    }
}
