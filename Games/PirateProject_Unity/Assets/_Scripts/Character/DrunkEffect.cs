using System;
using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class DrunkEffect : MonoBehaviour
{
    public CinemachineCamera cam;

    private float _drunkTime = 5f;
    [SerializeField] private float swayIntensity = 10f;
    [SerializeField] private float swaySpeed = 2f;

    private float _timer = 0f;
    private bool _isDrunk = false;
    private float _originalDutch;

    [SerializeField] private GameObject drunkVolume;

    void Start()
    {
        if (cam != null)
            _originalDutch = cam.Lens.Dutch;
    }

    private void Update()
    {
        if (_isDrunk && cam != null)
        {
            _timer += Time.deltaTime;
            float sway = Mathf.Sin(Time.time * swaySpeed) * swayIntensity;
            cam.Lens.Dutch = _originalDutch + sway;

            if (_timer >= _drunkTime)
            {
                _isDrunk = false;
                cam.Lens.Dutch = _originalDutch;
            }
        }
    }

    public void TriggerDrunk(float duration)
    {
        drunkVolume.SetActive(true);
        StartCoroutine(EndDrunk(duration));
        _isDrunk = true;
        _timer = 0f;
        _drunkTime = duration;
    }

    private IEnumerator EndDrunk(float duration)
    {
        yield return new WaitForSeconds(duration);
        drunkVolume.SetActive(false);
        _isDrunk = false;
    }
}
