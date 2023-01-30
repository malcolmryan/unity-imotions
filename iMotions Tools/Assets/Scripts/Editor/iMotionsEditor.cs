using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
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

        if (GUILayout.Button("Write XML")) 
        {
            WriteXML(iMotions);
        }
    }

//   <EventSource Id="Unity" Version="1" Name="UnityXYZ">
//       <Sample Id="Sample" Name="SampleXYZ">
//           <Field Id="Milliseconds" Range="Variable"></Field>
//           <Field Id="Seconds" Range="Variable"></Field>
//       </Sample>
//   </EventSource>

    private void WriteXML(IMotions iMotions)
    {
        string path = EditorUtility.SaveFilePanel("Save event source definition as XML",
            "", $"{iMotions.eventID}.xml", "xml");

        using (XmlTextWriter writer = new XmlTextWriter(new StreamWriter(path)))  
        {  
            writer.WriteStartElement("EventSource");
            writer.WriteAttributeString("Id", iMotions.eventID);
            writer.WriteAttributeString("Version", iMotions.version.ToString());
            writer.WriteAttributeString("Name", iMotions.eventID);

            writer.WriteStartElement("Sample");
            writer.WriteAttributeString("Id", iMotions.sampleID);
            writer.WriteAttributeString("Name", iMotions.sampleID);

            for (int i = 0; i < iMotions.sensors.Length; i++) 
            {
                writer.WriteStartElement("Field");
                writer.WriteAttributeString("Id", iMotions.sensors[i]);
                writer.WriteAttributeString("Range", "Variable");
                writer.WriteEndElement();                
            }
            writer.WriteEndElement();                
            writer.WriteEndElement();                
        } 
    }
}
