using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VoiceLinesSO))]
public class VoiceLinesSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Always show clip
        EditorGUILayout.PropertyField(serializedObject.FindProperty("voiceLine"));

        // Always show subtitle
        EditorGUILayout.PropertyField(serializedObject.FindProperty("subtitleText"));

        // Event selection
        var eventProp = serializedObject.FindProperty("eventType");
        EditorGUILayout.PropertyField(eventProp);

        // StopCar event
        if ((EventAfterVoiceLines)eventProp.enumValueIndex == EventAfterVoiceLines.StopCar)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Voice Lines While Car Is Stopped", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("voiceLinesWhileStopped"),
                true
            );
        }

        // StartRecording event
        if ((EventAfterVoiceLines)eventProp.enumValueIndex == EventAfterVoiceLines.StartRecordingVoice)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Recording Responses", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("recordingResponses"),
                true
            );
        }

        serializedObject.ApplyModifiedProperties();
    }
}