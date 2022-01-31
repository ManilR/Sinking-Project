using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using System;

[CustomEditor(typeof(WaterWaves2D))]
public class WaterWaves2DEditor:Editor{

	WaterWaves2D script;

	#region Bootstrap

	[MenuItem("GameObject/2D Object/Water Waves 2D")]
	static void Create(){
		GameObject go=new GameObject{name="Water Waves 2D"}; //Create new GameObject
		//Place it in the center of current scene view
		SceneView sc=SceneView.lastActiveSceneView==null?SceneView.lastActiveSceneView:SceneView.sceneViews[0] as SceneView;
		go.transform.position=new Vector3(sc.pivot.x,sc.pivot.y,0f);
		//Set it to be a child of any selected object in hierarchy
		if(Selection.activeGameObject!=null) go.transform.parent=Selection.activeGameObject.transform;
		Selection.activeGameObject=go;
		//Add components and set some initial settings
		go.AddComponent<WaterWaves2D>();
		WaterWaves2D tScript=go.GetComponent<WaterWaves2D>();
		tScript.bounds=new Bounds(Vector3.zero,new Vector3(20f,4f,0f));
		tScript.TakeCareOfComponenets();
		UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(tScript.GetComponent<MeshFilter>(),false);
		UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(tScript.GetComponent<MeshRenderer>(),false);
		UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(tScript.GetComponent<BoxCollider2D>(),false);
		UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(tScript.GetComponent<CircleCollider2D>(),false);
		UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(tScript.GetComponent<BuoyancyEffector2D>(),false);
		UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(tScript.GetComponent<AudioSource>(),false);
		tScript.Generate();
		//Change current tool
		//Tools.current=Tool.Move;
	}

	#endregion

	#region Scene

	void OnSceneGUI(){
		script=(WaterWaves2D)target;
		if(script.transform.hasChanged){ 
			script.transform.localScale=Vector3.one;
		}
		Event e=Event.current;

		//Draw bounds
		if(script.shape==WaterWaves2D.Shape.Surface) {
			Handles.color=new Color(1f,1f,1f,0.5f);
			Handles.DrawLine(
				script.transform.TransformPoint(new Vector3(script.bounds.max.x,script.bounds.max.y,0f)),
				script.transform.TransformPoint(new Vector3(script.bounds.max.x,script.bounds.min.y,0f))
			);
			Handles.DrawLine(
				script.transform.TransformPoint(new Vector3(script.bounds.max.x,script.bounds.min.y,0f)),
				script.transform.TransformPoint(new Vector3(script.bounds.min.x,script.bounds.min.y,0f))
			);
			Handles.DrawLine(
				script.transform.TransformPoint(new Vector3(script.bounds.min.x,script.bounds.min.y,0f)),
				script.transform.TransformPoint(new Vector3(script.bounds.min.x,script.bounds.max.y,0f))
			);
		}

		/*
		//Draw surface
		Handles.color=Color.white;
		for(int i=0;i<script.points.Count;i++){
			Handles.DrawSolidDisc(
				script.transform.TransformPoint(script.points[i].restPosition),
				Vector3.back,
				0.03f
			);
			Handles.DrawLine(
				script.transform.TransformPoint(script.points[i].position),
				script.transform.TransformPoint(script.points[i].restPosition)
			);
			Handles.DrawSolidDisc(
				script.transform.TransformPoint(script.points[i].position),
				Vector3.back,
				0.05f
			);
			if(i>0 || script.loop){ 
				Handles.DrawLine(
					script.transform.TransformPoint(script.points.Loop(i-1).position),
					script.transform.TransformPoint(script.points[i].position)
				);
			}
		}
		*/

		float size=HandleUtility.GetHandleSize(Vector3.zero)*0.06f;
		//Left handle
		EditorGUI.BeginChangeCheck();
		Vector3 lh=Handles.FreeMoveHandle(
			script.transform.TransformPoint(new Vector3(script.bounds.min.x,script.bounds.center.y,0f)),
			script.transform.rotation,
			size,
			Vector3.zero,
			Handles.CubeHandleCap
		);
		if(EditorGUI.EndChangeCheck()){
			script.bounds.min=new Vector3(
				script.transform.InverseTransformPoint(lh).x,
				script.bounds.min.y,
				0f
			);
			if(!e.alt) script.transform.position=new Vector3(script.transform.position.x+script.bounds.center.x,script.transform.position.y,script.transform.position.z);
			script.bounds.center=Vector3.zero;
			script.Generate();
		}

		//Right handle
		EditorGUI.BeginChangeCheck();
		Vector3 rh=Handles.FreeMoveHandle(
			script.transform.TransformPoint(new Vector3(script.bounds.max.x,script.bounds.center.y,0f)),
			script.transform.rotation,
			size,
			Vector3.zero,
			Handles.CubeHandleCap
		);
		if(EditorGUI.EndChangeCheck()){
			script.bounds.max=new Vector3(
				script.transform.InverseTransformPoint(rh).x,
				script.bounds.max.y,
				0f
			);
			if(!e.alt) script.transform.position=new Vector3(script.transform.position.x+script.bounds.center.x,script.transform.position.y,script.transform.position.z);
			script.bounds.center=Vector3.zero;
			script.Generate();
		}

		//Bottom handle
		EditorGUI.BeginChangeCheck();
		Vector3 bh=Handles.FreeMoveHandle(
			script.transform.TransformPoint(new Vector3(script.bounds.center.x,script.bounds.min.y,0f)),
			script.transform.rotation,
			size,
			Vector3.zero,
			Handles.CubeHandleCap
		);
		if(EditorGUI.EndChangeCheck()){
			script.bounds.min=new Vector3(
				script.bounds.min.x,
				script.transform.InverseTransformPoint(bh).y,
				0f
			);
			if(!e.alt) script.transform.position=new Vector3(script.transform.position.x,script.transform.position.y+script.bounds.center.y,script.transform.position.z);
			script.bounds.center=Vector3.zero;
			script.Generate();
		}

		//Bottom handle
		EditorGUI.BeginChangeCheck();
		Vector3 th=Handles.FreeMoveHandle(
			script.transform.TransformPoint(new Vector3(script.bounds.center.x,script.bounds.max.y,0f)),
			script.transform.rotation,
			size,
			Vector3.zero,
			Handles.CubeHandleCap
		);
		if(EditorGUI.EndChangeCheck()){
			script.bounds.max=new Vector3(
				script.bounds.max.x,
				script.transform.InverseTransformPoint(th).y,
				0f
			);
			if(!e.alt) script.transform.position=new Vector3(script.transform.position.x,script.transform.position.y+script.bounds.center.y,script.transform.position.z);
			script.bounds.center=Vector3.zero;
			script.Generate();
		}
	}

	#endregion

	#region Inspector

	public override void OnInspectorGUI(){
		int labelWidth=120;
		EditorGUIUtility.labelWidth=labelWidth+4;

		script=(WaterWaves2D)target;
		if(script.transform.hasChanged){ 
			script.transform.localScale=Vector3.one;
		}
		//Shape of the water
		WaterWaves2D.Shape shape=(WaterWaves2D.Shape)EditorGUILayout.EnumPopup(new GUIContent("The shape of water","Shape of a mesh and configuration of surface points"),script.shape);
		if(shape!=script.shape){
			Undo.RecordObject(script,"Change the shape of water");
			if(shape==WaterWaves2D.Shape.Circle){
				script.bounds.size=(Vector3)new Vector2(Mathf.Min(script.bounds.size.x,script.bounds.size.y),Mathf.Min(script.bounds.size.x,script.bounds.size.y));
			}
			script.shape=shape;
			script.TakeCareOfComponenets();
			script.Generate();
			UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(script.GetComponent<BoxCollider2D>(),false);
			UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(script.GetComponent<CircleCollider2D>(),false);
			EditorUtility.SetDirty(script);
		}
		//Mesh properties
		if(shape==WaterWaves2D.Shape.Circle){
			float size=script.bounds.extents.x*2;
			float newSize=EditorGUILayout.FloatField("Diameter",size);
			if(size!=newSize){ 
				Undo.RecordObject(script,"Change size");
				script.bounds.size=(Vector3)new Vector2(newSize,newSize);
				script.Generate();
				EditorUtility.SetDirty(script);
			}
		}else{
			//EditorGUIUtility.wideMode=true;
			Vector2 size=new Vector2(script.bounds.extents.x*2,script.bounds.extents.y*2);
			Vector2 newSize=EditorGUILayout.Vector2Field("Size",size);
			if(size!=newSize){
				if(newSize.x<0) newSize.x=0;
				if(newSize.y<0) newSize.y=0;
				Undo.RecordObject(script,"Change size");
				script.bounds.size=(Vector3)newSize;
				script.Generate();
				EditorUtility.SetDirty(script);
			}
		}
		int newResolution=EditorGUILayout.IntField(new GUIContent("Resolution","Number of points"),script.resolution);
		if(newResolution!=script.resolution){ 
			if(newResolution<2) newResolution=2;
			Undo.RecordObject(script,"Change resolution");
			script.resolution=newResolution;
			script.Generate();
			EditorUtility.SetDirty(script);
		}
		float newLineWidth=EditorGUILayout.FloatField(new GUIContent("Line thickness","Width of the top water line"),script.lineWidth);
		if(newLineWidth!=script.lineWidth){ 
			if(newLineWidth<0) newLineWidth=0;
			if(newLineWidth>script.bounds.extents.y) newLineWidth=script.bounds.extents.y;
			Undo.RecordObject(script,"Change line width");
			script.lineWidth=newLineWidth;
			script.Generate();
			EditorUtility.SetDirty(script);
		}

		//Show triangle count
		GUILayout.Box(new GUIContent(script.getTriangleCount>1?"The mesh has "+script.getTriangleCount.ToString()+" triangles":(script.getTriangleCount==1?"The mesh is just one triangle":"The mesh has no triangles")),EditorStyles.helpBox);
		
		//Colors
		script.showColorProperties=EditorGUILayout.Foldout(script.showColorProperties,"Colors",true);
		if(script.showColorProperties){
			#if UNITY_2018_1_OR_NEWER
			Color lineColor=EditorGUILayout.ColorField(new GUIContent("Line color"),script.lineColor,true,true,script.HDRColors);
			#else
			Color lineColor=EditorGUILayout.ColorField("Line color",script.lineColor);
			#endif
			if(script.lineColor!=lineColor){
				Undo.RecordObject(script,"Change line color");
				script.lineColor=lineColor;
				script.Generate();
				EditorUtility.SetDirty(script);
			}
			#if UNITY_2018_1_OR_NEWER
			Color outColor=EditorGUILayout.ColorField(new GUIContent("Outer color"),script.outColor,true,true,script.HDRColors);
			#else
			Color outColor=EditorGUILayout.ColorField("Outer color",script.outColor);
			#endif
			if(script.outColor!=outColor){
				Undo.RecordObject(script,"Change outer color");
				script.outColor=outColor;
				script.Generate();
				EditorUtility.SetDirty(script);
			}
			#if UNITY_2018_1_OR_NEWER
			Color inColor=EditorGUILayout.ColorField(new GUIContent("Inner color"),script.inColor,true,true,script.HDRColors);
			#else
			Color inColor=EditorGUILayout.ColorField("Inner color",script.inColor);
			#endif
			if(script.inColor!=inColor){
				Undo.RecordObject(script,"Change inner color");
				script.inColor=inColor;
				script.Generate();
				EditorUtility.SetDirty(script);
			}
		}

		//Spring properties. Waves are controlled by the spring equations
		script.showWaveProperties=EditorGUILayout.Foldout(script.showWaveProperties,"Wave properties",true);
		if(script.showWaveProperties){
			float springConstant=EditorGUILayout.FloatField(new GUIContent("Wobble speed","Speed of surface oscilations once displaced. Usable values are from 0.001 to 1. Increasing the number increases the speed."),script.springConstant);
			if(springConstant!=script.springConstant){ 
				if(springConstant<0) springConstant=0;
				Undo.RecordObject(script,"Change wobble speed");
				script.springConstant=springConstant;
				EditorUtility.SetDirty(script);
			}
			float damping=EditorGUILayout.FloatField(new GUIContent("Wobble damping","Sets how fast the oscilations will receede. Usable values from 0.001 to 0.1. Increase value to dampen the oscilations quicker."),script.damping);
			if(damping!=script.damping){ 
				if(damping<0) damping=0;
				Undo.RecordObject(script,"Change wobble damping");
				script.damping=damping;
				EditorUtility.SetDirty(script);
			}
			float springConstant2=EditorGUILayout.FloatField(new GUIContent("Spread speed","Sets how fast the waves propagate. Usable values are from 0.001 to 1. Increasing the number increases the speed."),script.springConstant2);
			if(springConstant2!=script.springConstant2){ 
				if(springConstant2<0) springConstant2=0;
				Undo.RecordObject(script,"Change spread speed");
				script.springConstant2=springConstant2;
				EditorUtility.SetDirty(script);
			}
			float damping2=EditorGUILayout.FloatField(new GUIContent("Spread damping","Sets how fast the wave propagation slows down. Usable values from 0.001 to 0.1. Increase value to decrease the distance the waves travel."),script.damping2);
			if(damping2!=script.damping2){ 
				if(damping2<0) damping2=0;
				Undo.RecordObject(script,"Change spread damping");
				script.damping2=damping2;
				EditorUtility.SetDirty(script);
			}
		}

		//Random ripples
		script.showRandomRipplesProperties=EditorGUILayout.Foldout(script.showRandomRipplesProperties,"Random ripples",true);
		if(script.showRandomRipplesProperties){
			float randomRipplesFrequency=EditorGUILayout.FloatField(new GUIContent("Frequency","Sets how often to create random ripples per second. Setting it to 1 will result in 1 ripple per second."),script.randomRipplesFrequency);
			if(randomRipplesFrequency!=script.randomRipplesFrequency){ 
				if(randomRipplesFrequency<0) randomRipplesFrequency=0;
				Undo.RecordObject(script,"Change random ripple frequency");
				script.randomRipplesFrequency=randomRipplesFrequency;
				EditorUtility.SetDirty(script);
			}
			float randomRipplesSize=EditorGUILayout.FloatField(new GUIContent("Size","The starting width of random ripples."),script.randomRipplesSize);
			if(randomRipplesSize!=script.randomRipplesSize){ 
				if(randomRipplesSize<0) randomRipplesSize=0;
				Undo.RecordObject(script,"Change random ripple size");
				script.randomRipplesSize=randomRipplesSize;
				EditorUtility.SetDirty(script);
			}
			float randomRipplesForce=EditorGUILayout.FloatField(new GUIContent("Force","The starting width of random ripples."),script.randomRipplesForce);
			if(randomRipplesForce!=script.randomRipplesForce){ 
				if(randomRipplesForce<0) randomRipplesForce=0;
				Undo.RecordObject(script,"Change random ripple force");
				script.randomRipplesForce=randomRipplesForce;
				EditorUtility.SetDirty(script);
			}
		}

		//Audio
		script.showAudioProperties=EditorGUILayout.Foldout(script.showAudioProperties,"Audio",true);
		if(script.showAudioProperties){
			bool audioEnabled=EditorGUILayout.Toggle(new GUIContent("Enable audio","Whether to create audio sources and arrays for enter/exit sounds"),script.audioEnabled);
			if(audioEnabled!=script.audioEnabled){ 
				Undo.RecordObject(script,"Change enable audio");
				script.audioEnabled=audioEnabled;
				script.TakeCareOfComponenets();
				EditorUtility.SetDirty(script);
			}
			if(script.audioEnabled) {
				EditorGUI.indentLevel++;
				//Enter sounds
				SerializedProperty enterSounds=serializedObject.FindProperty("enterSounds");
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(enterSounds,new GUIContent("Enter water"),true);
				if(EditorGUI.EndChangeCheck()){ 
					serializedObject.ApplyModifiedProperties();
					EditorUtility.SetDirty(script);
				}
				//Exit sounds
				SerializedProperty exitSounds=serializedObject.FindProperty("exitSounds");
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(exitSounds,new GUIContent("Exit water"),true);
				if(EditorGUI.EndChangeCheck()){ 
					serializedObject.ApplyModifiedProperties();
					EditorUtility.SetDirty(script);
				}
			}
			EditorGUI.indentLevel--;
		}

		//Sprite sorting
		GUILayout.Space(10);

		//Allow to switch between lit and unlit material
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label(new GUIContent("Sprite material","The type of material to use for this object"),GUILayout.Width(labelWidth));
		string[] enumstrings=WaterWaves2D.MaterialTypes.GetNames(typeof(WaterWaves2D.MaterialTypes));
		GUIContent[] buttons=new GUIContent[enumstrings.Length];
		for(int i=0;i<buttons.Length;i++){
			buttons[i]=new GUIContent(enumstrings[i]);
		}
		int switchState=GUILayout.Toolbar((int)script.materialType,buttons);
		if(switchState!=(int)script.materialType){
			GUI.FocusControl(null);
			script.materialType=(WaterWaves2D.MaterialTypes)Enum.Parse(typeof(WaterWaves2D.MaterialTypes),enumstrings[switchState]);
			script.RefreshMaterial();
		}
		EditorGUILayout.EndHorizontal();

		//Get sorting layers
		int[] layerIDs=GetSortingLayerUniqueIDs();
		string[] layerNames=GetSortingLayerNames();
		//Get selected sorting layer
		int selected=-1;
		for(int i=0;i<layerIDs.Length;i++){
			if(layerIDs[i]==script.sortingLayer){
				selected=i;
			}
		}
		//Select Default layer if no other is selected
		if(selected==-1){
			for(int i=0;i<layerIDs.Length;i++){
				if(layerIDs[i]==0){
					selected=i;
				}
			}
		}
		//Sorting layer dropdown
		EditorGUI.BeginChangeCheck();
		GUIContent[] dropdown=new GUIContent[layerNames.Length+2];
		for(int i=0;i<layerNames.Length;i++){
			dropdown[i]=new GUIContent(layerNames[i]);
		}
		dropdown[layerNames.Length]=new GUIContent();
		dropdown[layerNames.Length+1]=new GUIContent("Add Sorting Layer...");
		selected=EditorGUILayout.Popup(new GUIContent("Sorting Layer","Name of the Renderer's sorting layer"),selected,dropdown);
		if(EditorGUI.EndChangeCheck()){
			Undo.RecordObject(script,"Change sorting layer");
			if(selected==layerNames.Length+1){
				#if UNIT2019_0_1_OR_NEWER
					SettingsService.OpenProjectSettings("Project/Tags and Layers");
				#else
					EditorApplication.ExecuteMenuItem("Edit/Project Settings/Tags and Layers");
				#endif
			}else{
				script.sortingLayer=layerIDs[selected];
				script.Generate();
			}
			EditorUtility.SetDirty(script);
		}
		//Order in layer field
		EditorGUI.BeginChangeCheck();
		int order=EditorGUILayout.IntField(new GUIContent("Order in Layer","Renderer's order within a sorting layer"),script.orderInLayer);
		if(EditorGUI.EndChangeCheck()){
			Undo.RecordObject(script,"Change order in layer");
			script.orderInLayer=order;
			script.Generate();
			EditorUtility.SetDirty(script);
		}
	}

	#endregion

	#region Utility functions
	//Get the sorting layer IDs
	public int[] GetSortingLayerUniqueIDs() {
		Type internalEditorUtilityType=typeof(InternalEditorUtility);
		PropertyInfo sortingLayerUniqueIDsProperty=internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs",BindingFlags.Static|BindingFlags.NonPublic);
		return (int[])sortingLayerUniqueIDsProperty.GetValue(null,new object[0]);
	}

	//Get the sorting layer names
	public string[] GetSortingLayerNames(){
		Type internalEditorUtilityType=typeof(InternalEditorUtility);
		PropertyInfo sortingLayersProperty=internalEditorUtilityType.GetProperty("sortingLayerNames",BindingFlags.Static|BindingFlags.NonPublic);
		return (string[])sortingLayersProperty.GetValue(null,new object[0]);
	}
	#endregion

}
