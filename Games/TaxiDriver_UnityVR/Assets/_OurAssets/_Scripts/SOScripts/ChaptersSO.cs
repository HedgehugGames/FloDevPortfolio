using UnityEngine;

[CreateAssetMenu(fileName = "Chapter", menuName = "VoiceSystem/Chapter")]
public class ChapterSO : ScriptableObject
{
    [Tooltip("The Object that will trigger the chapter.")]
    public GameObject chapterPrefab;
    [Tooltip("All the voice lines that will be played in this chapter.")]
    public VoiceLinesSO[] voiceLines;   // array because each chapter can have multiple lines
    [Tooltip("Set to true if the chapter does not require an object to be picked up in order to trigger this chapter.")]
    public bool chapterTriggered;
}