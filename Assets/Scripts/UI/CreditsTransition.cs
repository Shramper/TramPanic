using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsTransition : MonoBehaviour {
	
	public Animator Slide;

	//CREDITS SLIDE

	public void CreditsPressed(){
		Slide.SetTrigger ("SlideIn");
	}

	public void MenuPressed(){
		Slide.SetTrigger ("SlideOut");
	}

	//LEVEL SELECT SLIDE

	public void LevelSelectPressed(){
		Slide.SetTrigger ("SlideIn_LS");
	}
	public void LSMenuPressed(){
		Slide.SetTrigger ("SlideOut_LS");
	}
}
