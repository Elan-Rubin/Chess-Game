using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance;
    public static CameraManager Instance { get { return _instance; } }

    [SerializeField] private Camera _mainCamera;
    public Camera MainCamera { get { return _mainCamera; } }

    Vector2 _centerPoint = Vector2.zero;
    Vector2 _targetPosition;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    void Start()
    {

    }

    void Update()
    {
        Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        _targetPosition = Vector2.Lerp(_centerPoint, mousePosition, 0.05f);
        transform.position = Vector2.Lerp(transform.position, _targetPosition, Time.deltaTime * 5f);
    }

    public void ScreenShake()
    {
        transform.GetChild(0).DOShakePosition(0.1f, 0.1f, 10, 90);
        transform.GetChild(0).DOShakeRotation(0.1f, 0.1f, 10, 90).OnComplete(() =>
        {
            transform.GetChild(0).localPosition = new Vector3(0, 0, -10);
            transform.GetChild(0).localRotation = Quaternion.Euler(Vector3.zero);
        });
    }
}
