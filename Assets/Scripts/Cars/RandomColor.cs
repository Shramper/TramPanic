using UnityEngine;
using System.Collections;

public class RandomColor : MonoBehaviour {

	// Use this for initialization

	public SpriteRenderer spriteRen;

	private float chooseColor;
	void Start () 
	{
		spriteRen = this.gameObject.GetComponent<SpriteRenderer>();
		chooseColor = Random.value;
		//Debug.Log (chooseColor);
		carColor ();

	}

	// Update is called once per frame
	void Update () 
	{

	}


	void carColor()
	{
		if (chooseColor <= 0.34) {
			spriteRen.color = Color.white;

		} 
		else if (chooseColor <= 0.55 && chooseColor >= 0.35)
		{
			spriteRen.color = Color.grey;

		}
		else if (chooseColor <= 0.76 && chooseColor >= 0.56)
		{	
			// Red
			spriteRen.color = new Color(0.603f,0.15f,0.12f);

		} 
		else if (chooseColor <= 0.93 && chooseColor >= 0.77)
		{	
			// Blue
			spriteRen.color = new Color(0.196f,0.203f,0.596f);

		}
		else if (chooseColor <= 1.0 && chooseColor >= 0.94) 
		{
			// Army Greenish
			spriteRen.color = new Color(0.295f,0.325f,0.125f);
		}
		else
		{
			/// Army Greenish
			spriteRen.color = new Color(0.295f,0.325f,0.125f);
		}
	}
}
