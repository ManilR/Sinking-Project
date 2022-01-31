using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWaves2DBallScript:MonoBehaviour{
	private Vector3 dragpoint;
	private bool dragging=false;
	private Rigidbody2D rb;
	public float maxVelocity=20f;
	
	private void Awake(){
		rb=GetComponent<Rigidbody2D>();
	}

	void Update(){
		if(Input.GetMouseButtonDown(0)){
			Vector3 mousePosition=Camera.main.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D[] hits=Physics2D.RaycastAll(mousePosition,Vector2.zero);
			for(int i=0;i<hits.Length;i++){ 
				if(hits[i].collider!=null && hits[i].collider.gameObject==gameObject){
					dragpoint=mousePosition-transform.position;
					this.dragging=true;
					break;
				}
			}
		}else if(Input.GetMouseButtonUp(0) && this.dragging){
			this.dragging=false;
		}
		if(this.dragging){
			Vector3 dragPosition=Camera.main.ScreenToWorldPoint(Input.mousePosition)-dragpoint;
			dragPosition.z=transform.position.z;
			Vector2 force=(Vector2)(dragPosition-transform.position);
			rb.velocity+=force*2f;
			//Limiting maximum velocity so the object doesn't move unnaturally fast
			if(rb.velocity.magnitude>maxVelocity) rb.velocity*=maxVelocity/rb.velocity.magnitude;
			//Damping the velocity
			rb.velocity*=0.91f;
		}

		if(transform.position.x>20f || transform.position.x<-20f || transform.position.y<-20f){ 
			transform.position=new Vector3(0f,9f,transform.position.z);
			rb.velocity=Vector2.zero;
		}

	}
}