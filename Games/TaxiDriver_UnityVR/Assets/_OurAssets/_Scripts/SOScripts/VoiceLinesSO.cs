using System.Collections.Generic;
using UnityEngine;

public enum EventAfterVoiceLines
{
    None,
    StopCar,
    StartRecordingVoice
}

[System.Serializable]
public class RecordingResponseLines
{
    public VoiceLinesSO yes;
    public VoiceLinesSO no;
    public VoiceLinesSO noAnswer;
    public VoiceLinesSO notRead;
    public VoiceLinesSO repeat;
}

[CreateAssetMenu(fileName = "VoiceLine", menuName = "VoiceSystem/VoiceLine")]
public class VoiceLinesSO : ScriptableObject
{
    public AudioClip voiceLine;
    public TextAsset subtitleText; 
    public EventAfterVoiceLines eventType;
    
    // Only used when eventType == StopCar
    public List<VoiceLinesSO> voiceLinesWhileStopped;
    
    // Only used when eventType == StartRecordingVoice
    public RecordingResponseLines recordingResponses;

}