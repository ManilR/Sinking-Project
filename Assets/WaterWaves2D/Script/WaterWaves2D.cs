using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaterSplash2DExtension;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WaterWaves2D:MonoBehaviour{

	#region Variables
	[SerializeField]
	private Mesh mesh;
	[SerializeField]
	private List<Vector3> vertices=new List<Vector3>(200);
	[SerializeField]
	private List<Vector3> uvs=new List<Vector3>(200);
	[SerializeField]
	private List<Color> colors=new List<Color>(200);
	[SerializeField]
	private List<Vector3> normals=new List<Vector3>(200);
	[SerializeField]
	private int[] triangles;
	[SerializeField]
	private int triangleCount;

	public enum Shape{Surface,Circle};
	public Shape shape=Shape.Surface;
	[SerializeField]
	private bool loop=false; //If the shape is circular we need to loop the waves around

	public bool showColorProperties=true;
	public bool showWaveProperties=true;
	public bool showRandomRipplesProperties=true;
	public bool showAudioProperties=true;

	public Bounds bounds;
	public int resolution=50; //Number of surface points
	public float lineWidth=0.1f;

	public Color lineColor=new Color(1f,1f,1f,0.6f);
	public Color outColor=new Color(0.5f,0.5f,0.5f,0.2f);
	public Color inColor=new Color(0.5f,0.5f,0.5f,0.5f);
	#if UNITY_2018_1_OR_NEWER
	//HDR colors
	public bool HDRColors=false;
	#endif

	public float springConstant=0.03f; //Spring constant for the springs that attach the points to their origins
	public float damping=0.02f; //Damping constant for those springs
	public float springConstant2=0.1f; //Spring constant for the springs that keep the neighboring points together
	public float damping2=0.1f;  //Damping constant for those springs

	public float randomRipplesFrequency=0f; //Ripples per second
	public float randomRipplesSize=0.3f; //Initial width of a ripple
	public float randomRipplesForce=0.5f; //Force of virtual impact
	private float randomRipplesNextTime=0f; //To hold the time of next random ripple
	private float randomRippleTimeOffset=0.4f; //Defines by how much we can randomly offset ripples in time

	//private float sleepThreshold=0.0001f; //If the biggest change in position or speed is lower than this value we stop simulating

	public int sortingLayer=0;
	public int orderInLayer=0;

	[SerializeField]
	private MeshFilter mf;
	[SerializeField]
	private MeshRenderer mr;
	[SerializeField]
	private BoxCollider2D boxCol;
	[SerializeField]
	private CircleCollider2D circleCol;
	[SerializeField]
	private BuoyancyEffector2D buo;

	public enum MaterialTypes{Unlit,Lit}
	public MaterialTypes materialType=MaterialTypes.Unlit;
	public Material useMaterial=null;

	public bool audioEnabled=true;
	[SerializeField]
	private AudioSource[] channels=new AudioSource[3];
	public AudioClip[] enterSounds;
	public AudioClip[] exitSounds;
	[SerializeField]
	private int channelToUse=0;

	[System.Serializable]
	public class Point:System.Object{
		public Vector2 position;
		public Vector2 restPosition;
		public Vector2 velocity;
		public Vector2 axis=Vector2.zero; //If we want to lock point movements to an axis
		public float axisFreedom=45f; //How far from the axis we allowed to move in degrees
		public Point(Vector2 position){ 
			this.position=position;
			this.restPosition=position;
		}
		public Point(Vector2 position,Vector2 axis,float axisFreedom){ 
			this.position=position;
			this.restPosition=position;
			this.axis=axis.normalized;
			this.axisFreedom=axisFreedom;
		}
		public void SetPos(Vector2 position){ 
			this.restPosition=this.position=position;
		}
		//Adds velocity limiting it to the axis and allowing slight deviations using axisDegrees
		public void AddVelocity(Vector2 amount){ 
			float axisFreedom=Mathf.Max(0f,1f-(Mathf.Min(Vector2.Angle(axis,amount),Vector2.Angle(-axis,amount))/this.axisFreedom));
			Vector2 axisAmount=axis*(Vector2.Dot(axis,amount)/axis.magnitude);
			velocity+=axisAmount+(amount-axisAmount)*axisFreedom;
		}
	}

	public List<WaterWaves2D.Point> points=new List<Point>(100);

	#endregion
	
	#region Bootstrap

	private void Awake(){
		TakeCareOfComponenets();
		Generate();
	}

	public void TakeCareOfComponenets(){
		#if UNITY_EDITOR
		//Tools.pivotMode = PivotMode.Pivot;
		#endif
		//Get material to use
		if(useMaterial==null) RefreshMaterial();
		if(GetComponent<MeshFilter>()==null) gameObject.AddComponent<MeshFilter>();
		if(GetComponent<MeshRenderer>()==null) gameObject.AddComponent<MeshRenderer>();
		if(GetComponent<BoxCollider2D>()==null && shape==Shape.Surface){
			gameObject.AddComponent<BoxCollider2D>();
			#if UNITY_EDITOR
			DestroyImmediate(gameObject.GetComponent<CircleCollider2D>());
			#else
			Destroy(gameObject.GetComponent<CircleCollider2D>());
			#endif
		}
		if(GetComponent<CircleCollider2D>()==null && shape==Shape.Circle){
			gameObject.AddComponent<CircleCollider2D>();
			#if UNITY_EDITOR
			DestroyImmediate(gameObject.GetComponent<BoxCollider2D>());
			#else
			Destroy(gameObject.GetComponent<BoxCollider2D>());
			#endif
		}
		if(GetComponent<BuoyancyEffector2D>()==null) gameObject.AddComponent<BuoyancyEffector2D>();
		mf=GetComponent<MeshFilter>();
		mr=GetComponent<MeshRenderer>();
		mr.sharedMaterial=useMaterial;
		sortingLayer=mr.sortingLayerID;
		orderInLayer=mr.sortingOrder;
		boxCol=GetComponent<BoxCollider2D>();
		if(boxCol!=null){
			boxCol.isTrigger=true;
			boxCol.usedByEffector=true;
		}
		circleCol=GetComponent<CircleCollider2D>();
		if(circleCol!=null){
			circleCol.isTrigger=true;
			circleCol.usedByEffector=true;
		}
		buo=GetComponent<BuoyancyEffector2D>();
		////buo.density=1.5f;
		
		//Work with audio
		//Add array of audio chennels
		if(channels[0]==null && audioEnabled){
			if(gameObject.GetComponents<AudioSource>().Length==channels.Length){
				//If we already added the components but not set them, we jsut need to assign them
				channels=gameObject.GetComponents<AudioSource>();
			}else{
				//Destroy existing audiosources
				AudioSource[] AudioSources=gameObject.GetComponents<AudioSource>();
				for(int i=0;i<AudioSources.Length;i++){
					#if UNITY_EDITOR
					DestroyImmediate(AudioSources[i]);
					#else
					Destroy(AudioSources[i]);
					#endif
				}
				//Create new audiosources for each chennel
				for(int i=0;i<channels.Length;i++){
					channels[i]=gameObject.AddComponent<AudioSource>();
				}
			}
		}else if(!audioEnabled && gameObject.GetComponents<AudioSource>().Length>0){ 
			//Destroy existing audiosources
			AudioSource[] AudioSources=gameObject.GetComponents<AudioSource>();
			for(int i=0;i<AudioSources.Length;i++){
				#if UNITY_EDITOR
				DestroyImmediate(AudioSources[i]);
				#else
				Destroy(AudioSources[i]);
				#endif
			}
		}
		//Get default sounds
		if((enterSounds==null || enterSounds.Length==0) && (exitSounds==null || exitSounds.Length==0) && audioEnabled){
			Object[] audioClips=Resources.LoadAll("",typeof(AudioClip));
			List<AudioClip> enterSoundsList=new List<AudioClip>(3);
			List<AudioClip> exitSoundsList=new List<AudioClip>(3);
			for(int i=0;i<audioClips.Length;i++){
				if(Regex.IsMatch(((AudioClip)audioClips[i]).name,"^plop[0-9]+$")){ 
					enterSoundsList.Add((AudioClip)audioClips[i]);
				}else if(Regex.IsMatch(((AudioClip)audioClips[i]).name,"^squirt[0-9]+$")){ 
					exitSoundsList.Add((AudioClip)audioClips[i]);
				}
			}
			enterSounds=enterSoundsList.ToArray();
			exitSounds=exitSoundsList.ToArray();
		}

	}

	public void RefreshMaterial(){
		if(materialType==MaterialTypes.Lit){
			useMaterial=(Material)Resources.Load("WaterWaves2DLit",typeof(Material));
		}else{
			useMaterial=(Material)Resources.Load("WaterWaves2D",typeof(Material));
		}
		if(mr!=null) mr.sharedMaterial=useMaterial;
	}

	public void Generate(){ 
		points.Clear();
		//Build a line of points
		if(shape==WaterWaves2D.Shape.Surface){
			for(int i=0;i<resolution;i++){
				loop=false;
				points.Add(new Point(
					new Vector2(
						bounds.min.x+((bounds.size.x/(resolution-1))*i),
						bounds.size.y/2
					),
					Vector2.down,
					45f
				));
			}
		//Build a cicle of points
		}else if(shape==WaterWaves2D.Shape.Circle){ 
			for(int i=0;i<resolution;i++){
				loop=true;
				float a=(((360f/resolution)*i)*Mathf.Deg2Rad);
				points.Add(new Point(
					new Vector2(
						(float)(Mathf.Cos(a)*bounds.extents.x),
						(float)(Mathf.Sin(a)*bounds.extents.y)
					),
					new Vector2(
						(float)(Mathf.Cos(a)*bounds.extents.x),
						(float)(Mathf.Sin(a)*bounds.extents.y)
					),
					90f
				));
			}
		}
		if(boxCol==null) boxCol=GetComponent<BoxCollider2D>();
		if(circleCol==null) circleCol=GetComponent<CircleCollider2D>();
		if(buo==null) buo=GetComponent<BuoyancyEffector2D>();
		if(boxCol!=null){
			boxCol.offset=bounds.center;
			boxCol.size=bounds.size;
		}
		if(circleCol!=null){
			circleCol.offset=bounds.center;
			circleCol.radius=bounds.size.x/2;
		}
		buo.surfaceLevel=bounds.size.y/2;
		BuildMesh();
	}
	#endregion
	
	#region Calculate point positions

	void FixedUpdate(){
		//if(Time.time>=5f) Debug.Break();
		UpdatePoints();
		BuildMesh(true);
		//Add random ripples
		if(randomRipplesFrequency>0f){
			if(randomRipplesNextTime<Time.time){
				//Get random point on water's surface and the direction of the impact force depending on the shape of the water
				Vector2 randomContact=Vector2.zero;
				Vector2 contactForce=Vector2.zero;
				if(shape==WaterWaves2D.Shape.Surface){
					Vector2 sStart=new Vector2(boxCol.bounds.min.x,boxCol.bounds.max.y);
					Vector2 sEnd=new Vector2(boxCol.bounds.max.x,boxCol.bounds.max.y);
					randomContact=sStart+Random.value*(sEnd-sStart);
					//randomContact=sStart+0.5f*(sEnd-sStart); //Right in the center. For testing
					contactForce=Vector2.down*randomRipplesForce;
				}else if(shape==WaterWaves2D.Shape.Circle){ 
					randomContact=(Vector2)circleCol.transform.position+circleCol.offset+Random.insideUnitCircle.normalized*circleCol.radius;
					contactForce=(((Vector2)circleCol.transform.position+circleCol.offset)-randomContact).normalized*randomRipplesForce;
				}
				//This value offsetts the scheduled time for next ripple for certain percentage of desired frequency so it doesn't look like clockwork
				float randomTimeDiff=(Random.value*((1f/randomRipplesFrequency)*randomRippleTimeOffset))-(((1f/randomRipplesFrequency)*randomRippleTimeOffset)/2f);
				//Schedule next ripple
				ScheduleImpact(randomContact,Vector2.one*randomRipplesSize,contactForce,(1f/randomRipplesFrequency)+randomTimeDiff);
				//Save time for next scheduled random ripple so when it passes, we schedule another one
				randomRipplesNextTime=Time.time+((1f/randomRipplesFrequency)+randomTimeDiff);
			}
		}
	}

	private void UpdatePoints(){
		float dTime=(Time.deltaTime/0.02f);
		//Move points based on their velocities
		for(int i=0;i<points.Count;i++){
			points[i].position+=points[i].velocity*dTime;
		}
		//If not looped lock the x positions of first and last point
		if(!loop){
			points[0].position.x=points[0].restPosition.x;
			points[points.Count-1].position.x=points[points.Count-1].restPosition.x;
		}
		//Caclculate force for origin constraints
		for(int i=0;i<points.Count;i++){
			Vector2 direction=points[i].position-points[i].restPosition;
			Vector2 force=-(springConstant*direction)*dTime; //Spring constraint
			points[i].velocity+=force; //Adding full force to current velocity
			points[i].velocity-=((direction.normalized*(Vector2.Dot(points[i].velocity,direction.normalized)*damping)))*dTime; //Damping in the direction of the spring
		}
		//Calculate force for neighbor constrains
		for(int i=0;i<points.Count;i++){
			if(loop || i<points.Count-1){
				Vector2 direction=points[i].position-points.Loop(i+1).position;
				Vector2 directionRest=points[i].restPosition-points.Loop(i+1).restPosition;
				Vector2 force=-(springConstant2*(direction-directionRest))*dTime; //Spring constraint
				//Applying half of the force to each end of the spring
				points[i].velocity+=force/2f; 
				points.Loop(i+1).velocity-=force/2f;
				//Dampening each end of the spring 
				points[i].velocity-=(((direction.normalized/2)*(Vector2.Dot(points[i].velocity,direction.normalized)*springConstant2*damping2)))*dTime;
				points.Loop(i+1).velocity-=(((direction.normalized/2)*(Vector2.Dot(points.Loop(i+1).velocity,direction.normalized)*springConstant2*damping2)))*dTime;
			}
		}
	}

	#endregion
	
	#region Impact

	private void OnTriggerEnter2D(Collider2D other){
		Vector2 closest=new Vector2();
		#if UNIT2019_0_1_OR_NEWER
			closest=other.ClosestPoint(other.transform.position)
		#else
			closest=other.bounds.ClosestPoint(other.transform.position);
#endif
		if (other.GetComponent<Rigidbody2D>() != null)
		{
			StartCoroutine(DelayedImpact(

				closest,
				(Vector2)other.bounds.extents,
				other.GetComponent<Rigidbody2D>().velocity,
				0.1f
			));
		
		PlayEnterSound();
		}
	}
	
	private void OnTriggerExit2D(Collider2D other){
		Vector2 closest=new Vector2();
		#if UNIT2019_0_1_OR_NEWER
			closest=other.ClosestPoint(other.transform.position)
		#else
			closest=other.bounds.ClosestPoint(other.transform.position);
#endif
		if (other.GetComponent<Rigidbody2D>() != null)
        {
			Impact(
			closest,
			(Vector2)other.bounds.extents,
			other.GetComponent<Rigidbody2D>().velocity
		);
			PlayExitSound();
		}		
	}

	private void ScheduleImpact(Vector2 position,Vector2 size,Vector2 force,float delay){
		StartCoroutine(DelayedImpact(position,size,force,delay));
	}
	
	IEnumerator DelayedImpact(Vector2 position,Vector2 size,Vector2 force,float delay) {
		yield return new WaitForSeconds(delay);
		Impact(position,size,force);
	}
	

	void Impact(Vector2 position,Vector2 size,Vector2 force){ 
		//Get ditance between points depending on shape, size and resolution
		float minDist=0;
		if(shape==WaterWaves2D.Shape.Surface){
			minDist=bounds.size.x/(resolution-1);
		}else{ //circle
			minDist=(Mathf.PI*bounds.size.x)/resolution;
		}
		//Don't let the size be smaller that distance between points so we touch at least one point
		if(size.magnitude<minDist) size=size.normalized*minDist;
		for(int i=0;i<points.Count;i++){
			//If the point is roughly beneath the object
			if(Vector2.Distance(transform.TransformPoint(points[i].position),position)<size.magnitude){
				//Add velocity of the falling object to the point
				points[i].AddVelocity(force*0.05f*(1f-Vector2.Distance(transform.TransformPoint(points[i].position),position)/size.magnitude));
			}
		}
	}

	#endregion
	
	#region Build mesh

	public void BuildMesh(){
		BuildMesh(false);
	}

	public void BuildMesh(bool onlyVertices){
		//If it's an animation update, we don't rebuild everything from scratch, we only need to update vertices
		if(!onlyVertices){
			if(mesh==null) mesh=new Mesh{name="WaterBlob2DMesh"};
			mesh.Clear();
			uvs.Clear();
			colors.Clear();
			normals.Clear();
		}
		vertices.Clear();

		if(shape==WaterWaves2D.Shape.Surface){ 
			BuildRectengularMesh(onlyVertices);
		}else{ 
			BuildRadialMesh(onlyVertices);
		}

		mesh.MarkDynamic();
		mesh.SetVertices(vertices);

		if(!onlyVertices){
			mesh.SetColors(colors);
			mesh.SetUVs(0,uvs);
			mesh.SetNormals(normals);
			mesh.SetTriangles(triangles,0);
			if(mf==null) mf=GetComponent<MeshFilter>();
			mf.sharedMesh=mesh;
			if(mr!=null){
				mr.sortingLayerID=sortingLayer;
				mr.sortingOrder=orderInLayer;
			}
		}
	}

	private void BuildRectengularMesh(){
		BuildRectengularMesh(false);
	}

	private void BuildRectengularMesh(bool onlyVertices){
		int pX=points.Count;
		int pY=lineWidth>0?4:2;
		float lineWidthModifier=1f;
		//Add vertices
		for(int y=0;y<pY;y++){
			for(int x=0;x<pX;x++){
				if(y==0){
					vertices.Add(new Vector3(
						points[x].position.x,
						bounds.min.y,
						0f
					));
				}
				//If we have a line
				if(y>0 && lineWidth>0){
					//This thing is a sigmoid function of velocity of the point
					if(lineWidth>0 && y!=0){
						lineWidthModifier=1f-((1/(1+Mathf.Pow(30f,points[x].velocity.y*10f)))*1.6f-0.8f);
						lineWidthModifier/=2f;
					}
					if(y==1 || y==2){
						vertices.Add(new Vector3(
							points[x].position.x,
							(points[x].position.y-(lineWidth*lineWidthModifier))-(lineWidth/2),
							0f
						));
					}else if(y==3){
						vertices.Add(new Vector3(
							points[x].position.x,
							(points[x].position.y+(lineWidth*lineWidthModifier))-(lineWidth/2),
							0f
						));
					}
				//If we don't have a line
				}else if(y>0){ 
					vertices.Add(new Vector3(
						points[x].position.x,
						points[x].position.y,
						0f
					));
				}
			}
		}
		if(!onlyVertices){
			//Add UVs
			/*
			for(int i=0;i<vertices.Count;i++){ 
				uvs.Add(new Vector3(
					((float)x/(float)(pX-1)),
					(((float)y/(float)(pY-1))-0.5f),
					//colorPoints[y].position-0.5f,
					0f
				));
			}
			*/
			//Add colors and triangles
			int verticeNum=0;
			int squareNum=-1;
			triangleCount=((pX-1)*(pY-(lineWidth>0?2:1)))*2; //If we render the line then we skip one row between the water and the line
			if(triangles==null || triangles.Length!=triangleCount*3) triangles=new int[triangleCount*3];
			for(int y=0;y<pY;y++){
				for(int x=0;x<pX;x++){
					if(y==0) colors.Add(inColor);
					if(y==1) colors.Add(outColor);
					if(y==2 || y==3) colors.Add(lineColor);
					if(x>0 && y>0 && y!=2){ //We skip 2nd row of triangles, we don't need to render it
						verticeNum=x+(y*pX);
						squareNum++;
						triangles[squareNum*6]=verticeNum-pX-1;
						triangles[squareNum*6+1]=verticeNum-1;
						triangles[squareNum*6+2]=verticeNum;
						triangles[squareNum*6+3]=verticeNum;
						triangles[squareNum*6+4]=verticeNum-pX;
						triangles[squareNum*6+5]=verticeNum-pX-1;
					}
				}
			}
			//Add normals
			for(int i=0;i<vertices.Count;i++){
				normals.Add(Vector3.back);
			}
		}
	}

	private void BuildRadialMesh(){
		BuildRadialMesh(false);
	}

	private void BuildRadialMesh(bool onlyVertices){ 
		int circles=lineWidth>0?3:1;
		float lineWidthModifier=1f;
		Vector2 lineModifier=Vector2.zero;
		Vector2 direction=Vector2.zero;
		//Create vertices
		vertices.Add(Vector3.zero);
		for(int c=0;c<circles;c++){ 
			for(int p=0;p<points.Count;p++){
				//This thing is a sigmoid function of velocity of the point
				if(lineWidth>0){
					lineWidthModifier=1f-((1/(1+Mathf.Pow(30f,Vector2.Dot(points[p].velocity,points[p].axis)*10f)))*1.6f-0.8f);
					lineWidthModifier/=2f;
					if(points[p].velocity!=Vector2.zero) {
						lineModifier=points[p].axis.normalized*lineWidth*lineWidthModifier;
					}else{ 
						lineModifier=points[p].axis.normalized*lineWidth*lineWidthModifier;
					}
					direction=points[p].axis.normalized*(lineWidth/2);
				}
				if(c==0 || c==1){
					vertices.Add((points[p].position-lineModifier)-direction);
				}else if(c==2){
					vertices.Add((points[p].position+lineModifier)-direction);
				}
			}
		}
		if(!onlyVertices){
			triangleCount=resolution+(lineWidth>0?(1*resolution)*2:0);
			//UVs
			/*
			 * Not used yet
			 **/
			//Colors
			colors.Add(inColor);
			for(int c=0;c<circles;c++){
				for(int p=0;p<points.Count;p++){
					if(c==0) colors.Add(outColor);
					else if(c>0) colors.Add(lineColor);
				}
			}
			//Triangles
			triangles=new int[triangleCount*3];
			int si;
			for(int c=0;c<circles;c++){ 
				if(c==0){
					for(int p=0;p<points.Count;p++){
						triangles[0+p*3]=0;
						triangles[1+p*3]=(p+2>points.Count?1:p+2);
						triangles[2+p*3]=p+1;
					}
				}else if(c>1){ //Skipping c=1 so we don't add triangles between the water and the line
					for(int p=0;p<points.Count;p++){
						si=(points.Count*3)+(c-2)*(points.Count*6)+p*6;
						triangles[0+si]=((c-1)*points.Count)+p+1;
						triangles[1+si]=((c-1)*points.Count)+(p+1<points.Count?p+2:1);
						triangles[2+si]=((c-0)*points.Count)+p+1;
						triangles[3+si]=((c-1)*points.Count)+(p+1<points.Count?p+2:1);
						triangles[4+si]=((c-0)*points.Count)+(p+1<points.Count?p+2:1);
						triangles[5+si]=((c-0)*points.Count)+p+1;
					}
				}
			}
			
		}
	}

	public int getTriangleCount{
		get{return triangleCount;}
	}

	#endregion

	#region Audio

	public void PlayEnterSound(){ 
		if(audioEnabled && enterSounds!=null) PlayClip(enterSounds[Random.Range(0,enterSounds.Length)]);
	}

	public void PlayExitSound(){ 
		if(audioEnabled && exitSounds!=null) PlayClip(exitSounds[Random.Range(0,exitSounds.Length)]);
	}

	public void PlayClip(AudioClip clip){
		//Check if next channel isn't playing and skip it if it is
		for(int i=0;i<channels.Length && channels[channelToUse].isPlaying;i++){
			channelToUse++;
			if(channelToUse>channels.Length-1) channelToUse=0;
		}
		channels[channelToUse].clip=clip;
		channels[channelToUse].volume=1f;
		channels[channelToUse].Play();
		channelToUse++;
		if(channelToUse>channels.Length-1) channelToUse=0;
	}

	#endregion

}