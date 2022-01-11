using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MoveObject : MonoBehaviour
{
    [SerializeField] Vector3 movePosition;
    [SerializeField] float speed = 0.3f;
    [SerializeField] [Range(0,1)] float moveProgress; // 0 - 1 0 = object not moved 1 = object moved
    // Start is called before the first frame update
    Vector3 startPosition;
    
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        moveProgress = Mathf.PingPong(Time.time, 1);
        
        
        Vector3 offset = movePosition * moveProgress * speed;
        transform.position = startPosition + offset;
    }
}
