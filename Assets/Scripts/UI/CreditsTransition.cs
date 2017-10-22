using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsTransition : MonoBehaviour {
	
	public Animator Slide;
	public bool tutorial;

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

	//HOW TO SLIDE

	public void HowToPressed() {
		Slide.SetTrigger ("SlideIn_How");
		tutorial = true;
		Slide.SetBool ("tutorial", true);
	}

	public void HowToExit() {
		Slide.SetTrigger ("SlideOut_How");
		tutorial = false;
		Slide.SetBool ("tutorial", false);
		//Debug.Log ("FALSE");
	}

}
