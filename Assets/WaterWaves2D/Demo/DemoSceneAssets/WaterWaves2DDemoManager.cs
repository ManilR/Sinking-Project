using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterWaves2DDemoManager:MonoBehaviour{

	public WaterWaves2D WW2D;
	public Text Prop1;
	public Text Prop2;
	public Text Prop3;
	public Text Prop4;

	void Start(){
		UpdateTexts();
	}

	void UpdateTexts(){
		Prop1.text=WW2D.springConstant.ToString();
		Prop2.text=WW2D.damping.ToString();
		Prop3.text=WW2D.springConstant2.ToString();
		Prop4.text=WW2D.damping2.ToString();
	}

	public void Prop1Minus(){
		WW2D.springConstant-=0.01f;
		if(WW2D.springConstant<0.01f) WW2D.springConstant=0.01f;
		UpdateTexts();
	}

	public void Prop1Plus(){
		WW2D.springConstant+=0.01f;
		if(WW2D.springConstant>1f) WW2D.springConstant=1f;
		UpdateTexts();
	}

	public void Prop2Minus(){
		WW2D.damping-=0.01f;
		if(WW2D.damping<0.01f) WW2D.damping=0.01f;
		UpdateTexts();
	}

	public void Prop2Plus(){
		WW2D.damping+=0.01f;
		if(WW2D.damping<0.1f) WW2D.damping=0.1f;
		UpdateTexts();
	}

	public void Prop3Minus(){
		WW2D.springConstant2-=0.01f;
		if(WW2D.springConstant2<0.01f) WW2D.springConstant2=0.01f;
		UpdateTexts();
	}

	public void Prop3Plus(){
		WW2D.springConstant2+=0.01f;
		if(WW2D.springConstant2>1f) WW2D.springConstant2=1f;
		UpdateTexts();
	}

	public void Prop4Minus(){
		WW2D.damping2-=0.01f;
		if(WW2D.damping2<0.01f) WW2D.damping2=0.01f;
		UpdateTexts();
	}

	public void Prop4Plus(){
		WW2D.damping2+=0.01f;
		if(WW2D.damping2<0.1f) WW2D.damping2=0.1f;
		UpdateTexts();
	}

}
