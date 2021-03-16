using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPool : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PoolManager.GetInstance().GetObj("Test/Cube", (o)=>
            {
                o.transform.localScale = Vector3.one * 2;
            });
        }
        if (Input.GetMouseButtonDown(1))
        {
            PoolManager.GetInstance().GetObj("Test/Sphere", (o) =>
            {
                o.transform.localScale = Vector3.one * 2;
            });
        }
    }
}
