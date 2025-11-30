using UnityEngine;
using System.Collections;
using TMPro;

public class LuigiVoiceManager : MonoBehaviour
{
    #region Singleton

    public static LuigiVoiceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #endregion 
    
    [Header("Dialogue")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ChapterSO[] chapters;
    [SerializeField] private TextMeshProUGUI subtitlesUI;
    
    [Header("Reference Scripts")]
    [SerializeField] private VoiceManager voiceManager;
    [SerializeField] private EnvironmentMovement carMove;

    private int currentChapter = 0;
    private int currentLine = 0;
    
    private void Start()
    { 
        StartIntro();
    }

    private void StartIntro()
    {
        PlayCurrentLine();
    }

    private void CheckChapter(ChapterTrigger trigger) // check each chapter at the beginning if it needs a trigger object
    {
        if (trigger) // trigger is true
        {
            // if the chapter is already active, start immediately
            PlayCurrentLine();
            Debug.LogWarning("Chapter is TRUE. Start with the dialogue");
        }
        else // wait until it is true
        {
            Debug.LogWarning("Chapter is FALSE. Trigger needed");
            StartCoroutine(WaitForChapterReady());
        }
    }
    
    
    private IEnumerator WaitForChapterReady()
    {
        var chapter = chapters[currentChapter];

        Debug.LogWarning("Initial trigger state: " + chapter.chapterTriggered);
        
        while (!chapter.chapterTriggered)
        {
            yield return null;
        }

        Debug.LogWarning($"Trigger detected for chapter {currentChapter}. Starting voice lines!");

        PlayCurrentLine(); 
    }


    private void PlayCurrentLine()
    {
        // local references to the current chapter and voice line
        var chapter = chapters[currentChapter];
        var line = chapter.voiceLines[currentLine];

        // start playing the current audio voice line
        audioSource.PlayOneShot(line.voiceLine);
        
        // show subtitles
        string subtitle = line.subtitleText ? line.subtitleText.text : "SUBTITLE MISSING";
        subtitlesUI.text = subtitle;
        subtitlesUI.enabled = true;

        // call event when line ends
        StartCoroutine(HandleEventAfter(line));
    }
    
    private IEnumerator HandleEventAfter(VoiceLinesSO line)
    {
        // wait for the clip to end before going further
        yield return new WaitForSeconds(line.voiceLine.length);
        // hide subtitles 
        subtitlesUI.enabled = false;

        IEnumerator eventRoutine = null;

        switch (line.eventType)
        {
            case EventAfterVoiceLines.StopCar:
                eventRoutine = StopCarEventRoutine(line);
                break;

            case EventAfterVoiceLines.StartRecordingVoice:
                eventRoutine = StartRecordingRoutine(line);
                break;
        }

        if (eventRoutine != null)
            yield return StartCoroutine(eventRoutine);

        NextLine();
    }

    private void NextLine()
    {
        // go to the next line in the chapter
        currentLine++;

        // local reference to the current chapter
        var chapter = chapters[currentChapter];

        // chapter end?
        if (currentLine >= chapter.voiceLines.Length)
        {
            Debug.LogWarning($"Chapter ended. Next Chapter can be played");
            // go to next chapter
            currentChapter++;
            currentLine = 0;

            // all chapters done?
            if (currentChapter >= chapters.Length)
            {
                Debug.Log("All chapters finished!");
                return;
            }

            // get next chapter
            var nextChapter = chapters[currentChapter];

            // check if next chapter needs trigger
            if (nextChapter.chapterTriggered)
            {
                PlayCurrentLine();
            }
            else
            {
                StartCoroutine(WaitForChapterReady());
            }

            return;
        }
        
        // if there are more lines in the chapter play the next one
        PlayCurrentLine();
    }
    
    public void OnChapterTriggered(ChapterTrigger trigger)
    {
        if (chapters[currentChapter] == trigger.chapterData)
        {
            Debug.LogWarning($"Trigger status:  + {trigger.triggered}");
            CheckChapter(trigger);
        }
    }
    
    private IEnumerator StopCarEventRoutine(VoiceLinesSO line)
    {
        //Stop the car
        carMove.StopCar();

        //Play all extra voice lines while the car is stopped
        foreach (var extraLine in line.voiceLinesWhileStopped)
        {
            if (extraLine == null)
                continue;

            // play audio
            audioSource.PlayOneShot(extraLine.voiceLine);
            
            // show subtitles
            string subtitle = line.subtitleText ? line.subtitleText.text : "SUBTITLE MISSING";
            subtitlesUI.text = subtitle;
            subtitlesUI.enabled = true;

            // wait until the clip ends
            yield return new WaitForSeconds(extraLine.voiceLine.length);

            subtitlesUI.enabled = false;
        }

        // 3. Start car again
        carMove.StartCar();
    }

    
    private IEnumerator StartRecordingRoutine(VoiceLinesSO line)
    {
        voiceManager.StartRecording();

        float time = voiceManager.timerUI + 4f;
        yield return new WaitForSeconds(time);

        RecordingResponse final = voiceManager.FinalizeResult();
        var responses = line.recordingResponses;

        VoiceLinesSO chosen = null;

        switch (final)
        {
            case RecordingResponse.Yes:
                chosen = responses.yes;
                break;

            case RecordingResponse.No:
                chosen = responses.no;
                break;

            case RecordingResponse.NoAnswer:
                chosen = responses.noAnswer;
                break;

            case RecordingResponse.NotRead:
                chosen = responses.notRead;
                break;
            
            case RecordingResponse.Repeat:
                chosen = responses.repeat;
                break;
        }

        Debug.LogWarning("Final recording result: " + final);
 
        if (chosen != null)
        {
            audioSource.PlayOneShot(chosen.voiceLine);
            subtitlesUI.text = chosen.subtitleText ? chosen.subtitleText.text : "";
            subtitlesUI.enabled = true;

            yield return new WaitForSeconds(chosen.voiceLine.length);
            subtitlesUI.enabled = false;

            if (chosen == responses.noAnswer || chosen == responses.notRead)
            {
                voiceManager.StartRecording();
            }

            if (chosen == responses.repeat)
            {
                // if voice line is assigned pla this else play voice line before that
            }
        }
    }

}
