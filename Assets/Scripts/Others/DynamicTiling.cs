﻿using UnityEngine;
using System.Collections;

public class DynamicTiling : MonoBehaviour {

	// UNITY METHODES

	void Start ()
    {
        GetComponent<Renderer>().material.SetFloat("RepeatX", gameObject.transform.localScale.x);
        GetComponent<Renderer>().material.SetFloat("RepeatY", gameObject.transform.localScale.y);
    }
	
}
