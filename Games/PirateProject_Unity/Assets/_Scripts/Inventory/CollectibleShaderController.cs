using System;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class CollectibleShaderController : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float waitTime = 1.0f;
    [SerializeField] private string alpha = "_CutOff";

    private Material _mat;
    private float _timer = 0f;
    private bool _waiting = false;
    private bool _wasLow = false;

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        _mat = new Material(rend.material);
        rend.material = _mat;
    }

    void Update()
    {
        if (_waiting)
        {
            _timer += Time.deltaTime;
            if (_timer >= waitTime)
            {
                _waiting = false;
                _timer = 0f;
                _wasLow = false;
            }
            return;
        }

        float wave = Mathf.Sin(Time.time * speed) * 0.5f + 0.5f;
        _mat.SetFloat(alpha, wave);
        
        if (wave < 0.05f)
        {
            _wasLow = true;
        }

        if (_wasLow && wave > 0.99f)
        {
            _waiting = true;
            _timer = 0f;
        }
    }
}