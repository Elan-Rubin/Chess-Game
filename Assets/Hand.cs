using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    private static Hand _instance;
    public static Hand Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    private float _currentRotation, _targetRotation;

    public void GoTo(Vector2 newPosition)
    {
        transform.position = newPosition;
    }

    void Start()
    {
        _currentRotation = _targetRotation = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
