using UnityEngine;
using GameEvents;

public class Underwater : MonoBehaviour
{
    [SerializeField] private Transform player;

    [SerializeField] private GameObject underwaterVolume;
    [SerializeField] private AudioSource underwaterAudio;

    private bool _isUnderwater = false;
    public bool inWater = false;

    private void Update()
    {
        if (!_isUnderwater)
        {
            if (player.transform.position.y <= -4f)
            {
                _isUnderwater = true;
                GameEventManager.Raise(new UnderwaterEvent(this, true));
            }
        }
        else
        {
            if (player.transform.position.y > -4f)
            {
                _isUnderwater = false;
                GameEventManager.Raise(new UnderwaterEvent(this, false));
            }
        }
    }

    public void SetUnderwater(bool right)
    {
        Debug.Log("is Underwater: " + right);

        RenderSettings.fog = right;
        RenderSettings.fogColor = new Color(0.1f, 0.4f, 0.6f);
        RenderSettings.fogDensity = 0.04f;

        if (underwaterVolume != null)
            underwaterVolume.SetActive(true);

        if (underwaterAudio != null)
            underwaterAudio.enabled = right;
    }

    public void NotUnderwater(bool wrong)
    {
        RenderSettings.fog = wrong;
        RenderSettings.fogColor = Color.clear;
        RenderSettings.fogDensity = 0f;

        if (underwaterVolume != null)
            underwaterVolume.SetActive(false);

        if (underwaterAudio != null)
            underwaterAudio.enabled = wrong;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Is in Water");
            inWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inWater = false;
        }
    }
}