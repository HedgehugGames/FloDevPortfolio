using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<ItemSound> itemSounds;

    private Dictionary<string, AudioClip> _soundMap;

    [System.Serializable]
    public class ItemSound
    {
        public string itemName;
        public AudioClip sound;
    }

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // Create dictionary from list
        _soundMap = new Dictionary<string, AudioClip>();
        foreach (var item in itemSounds)
        {
            if (!_soundMap.ContainsKey(item.itemName))
                _soundMap.Add(item.itemName, item.sound);
        }
    }

    public void PlayItemSound(string itemName)
    {
        if (_soundMap.ContainsKey(itemName))
        {
            audioSource.PlayOneShot(_soundMap[itemName]);
        }
        else
        {
            Debug.LogWarning($"No sound found for item: {itemName}");
        }
    }
}
