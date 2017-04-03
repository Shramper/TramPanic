using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsTransition : MonoBehaviour {
	
	public Animator Slide;

	public void CreditsPressed(){
		Slide.SetTrigger ("SlideIn");
	}

	public void MenuPressed(){
		Slide.SetTrigger ("SlideOut");
	}
}
