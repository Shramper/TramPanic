﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlurryTextFix : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		GetComponent<Text> ().font.material.mainTexture.filterMode = FilterMode.Point;
		
	}

}
