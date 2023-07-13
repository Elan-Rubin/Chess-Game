using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BloodSplatter : MonoBehaviour
{
    [SerializeField] private List<Sprite> splatters;

    void Start()
    {
        CameraManager.Instance.ScreenShake();
        transform.parent = GridManager.Instance.transform.GetChild(2);
        Vector2 targetScale = transform.localScale * Random.Range(0.8f, 1.2f);
        transform.Translate(new Vector2(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f)));
        transform.Rotate(0, 0, Random.Range(0, 360f));
        transform.GetChild(0).GetComponent<SpriteMask>().sprite = splatters[Random.Range(0, splatters.Count - 1)];
        transform.localScale = Vector2.zero;
        transform.DOScale(targetScale, 0.05f);
        //transform.DOScale(targetScale, 0.05f).OnComplete(() => transform.DOPunchScale(transform.localScale, 0.1f, 1, 0.3f));
    }
}
