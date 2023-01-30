using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IMotions))]
public class iMotionsEditor : Editor
{
    private SerializedProperty sensorsProp;

    public void OnEnable()
    {
        sensorsProp = serializedObject.FindProperty("sensors");
    }
 
    public override void OnInspectorGUI()
    {
        IMotions iMotions = (IMotions) target;  

        iMotions.hostname = EditorGUILayout.TextField("Host",iMotions.hostname);
        iMotions.port = EditorGUILayout.IntField("Port", iMotions.port);
        iMotions.eventID = EditorGUILayout.TextField("Event ID",iMotions.eventID);
        iMotions.version = EditorGUILayout.IntField("Event version", iMotions.version);
        iMotions.instance = EditorGUILayout.TextField("Instance",iMotions.instance);
        iMotions.sampleID = EditorGUILayout.TextField("Sample ID",iMotions.sampleID);;    
        
        EditorGUILayout.PropertyField(sensorsProp); 
        serializedObject.ApplyModifiedProperties();
    }
}
