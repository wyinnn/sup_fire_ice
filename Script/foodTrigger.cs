﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foodTrigger : MonoBehaviour {

    public float cdTime;
    public float countDown;
    public float speed;

    void Update()
    {
        countDown -= Time.deltaTime;



        if (countDown < 0)
        {
            int childNum;
            countDown = cdTime;
            childNum = Random.Range(0, 4);

            GameObject randomChild = transform.GetChild(childNum).gameObject;
            GameObject newChild = Instantiate(randomChild, new Vector3(0f, 5.24f, -0.5f), new Quaternion(0f, 0f, 0f, 0f)) as GameObject;
            newChild.GetComponent<Rigidbody>().velocity = new Vector3(0, speed, 0);
            
            newChild.SetActive(true);
        }
    }
}
