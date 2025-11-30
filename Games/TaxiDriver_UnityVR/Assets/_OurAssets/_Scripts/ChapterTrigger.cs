using UnityEngine;

public class ChapterTrigger : MonoBehaviour
{
    public ChapterSO chapterData; // reference to the SO
    public bool triggered = false;

    public void Trigger() // Assing in the inspector on each prefab
    {
        Debug.LogWarning($"Chapter {chapterData.name} Triggered");
        triggered = true; // make the chapter accessible 
        LuigiVoiceManager.Instance.OnChapterTriggered(this); // call the function in teh Manager 
    }
}