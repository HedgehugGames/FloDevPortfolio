using System;
using System.Collections;
using Oculus.Voice;
using UnityEngine;
using UnityEngine.UI;
public enum RecordingResponse
{
    Yes,
    No,
    NoAnswer,
    NotRead,
    Repeat,
    Unknown
}

public class VoiceManager : MonoBehaviour
{

    [SerializeField] private AppVoiceExperience appVoice;
    [SerializeField] private Image imageUI;
    public float timerUI = 3f;
    
    public TextAsset subtitleText;
    
    // flags for responses
    private bool heardAnything = false;
    private bool gotValidIntent = false;

    public RecordingResponse LastResult { get; private set; } = RecordingResponse.Unknown;
    
    private IEnumerator UIActiveCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        // hide UI after time
        imageUI.gameObject.SetActive(false);
        Debug.LogWarning("UI Image set inactive again");
    }

    public void StartRecording()
    {
        LastResult = RecordingResponse.Unknown;
        heardAnything = false;
        gotValidIntent = false;
        
        // show UI
        imageUI.gameObject.SetActive(true);
        // start timer to hide ui again
        StartCoroutine(UIActiveCoroutine(timerUI));
        
        // start Recording
        appVoice.Activate();
        Debug.LogWarning("Started Recording");
        
        // check if voice was recorded at all
        appVoice.VoiceEvents.OnMicLevelChanged.AddListener(OnMicLevelChanged);
        
    }
    
    private void OnMicLevelChanged(float level)
    {
        if (level > 0.01f) // tiny threshold
            heardAnything = true;
    }


    public void OnWitResponse(string resultText)
    {
        gotValidIntent = true; // yes or no was detected

        LastResult = ParseRecordingResult(resultText);
        Debug.LogWarning("Recording result parsed: " + LastResult);
    }

    public RecordingResponse FinalizeResult()
    {
        // If yes/no/repeat were detected → done already
        if (gotValidIntent)
            return LastResult;

        // If user said nothing → NoAnswer
        if (!heardAnything)
            return RecordingResponse.NoAnswer;

        // User made noise but no intent → NotRead
        return RecordingResponse.NotRead;
    }


    private RecordingResponse ParseRecordingResult(string result)
    {
        if (string.IsNullOrEmpty(result))
            return RecordingResponse.Unknown;

        result = result.ToLower();

        if (result.Contains("yes"))
            return RecordingResponse.Yes;

        if (result.Contains("no"))
            return RecordingResponse.No;
        
        if (result.Contains("repeat"))
            return RecordingResponse.Repeat;
        
        return RecordingResponse.Unknown;
    }

}
