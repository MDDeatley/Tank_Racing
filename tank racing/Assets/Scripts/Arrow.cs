﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform target;
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.LookAt(target);
    }
}
