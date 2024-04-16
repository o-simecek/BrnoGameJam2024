using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform playerLocation;
    public float baseOffset;
    // Start is called before the first frame update
    void Start()
    {
        baseOffset = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, baseOffset + playerLocation.position.z);
    }
}
