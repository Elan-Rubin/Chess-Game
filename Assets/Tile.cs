using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    int _number;
    public int Number { get { return _number; } set { _number = value; } }

    Vector2Int _position;
    public Vector2Int Position { get { return _position; } set { _position = value; } }

    GameObject _tileRenderer;
    public GameObject TileRenderer { get { return _tileRenderer; } }
    SpriteRenderer _tileRendererSR { get { return _tileRenderer.GetComponent<SpriteRenderer>(); } }

    SpriteRenderer _dotSR { get { return transform.GetChild(1).GetComponent<SpriteRenderer>(); } }

    Piece _myPiece;
    public Piece MyPiece { get { return _myPiece; } set { _myPiece = value; } }

    //bool[,] _surroundings = new bool[3, 3];
    //public bool[,] Surroundings { get { return _surroundings; } set { _surroundings = value; } }

    Vector2 _scale = Vector2.one;

    void Start()
    {
        _tileRenderer = transform.GetChild(0).gameObject;
        _scale = _tileRenderer.transform.localScale;
        _tileRenderer.transform.localScale = Vector2.zero;
    }


    void Update()
    {

    }

    public void Appear(float waitTime, Color newColor, Color otherColor)
    {
        StartCoroutine(_Appear(waitTime, newColor, otherColor));
    }
    IEnumerator _Appear(float waitTime, Color newColor, Color otherColor)
    {
        yield return new WaitForSeconds(waitTime);
        _tileRenderer.transform.Rotate(0, 0, Random.Range(0, 360));
        _tileRenderer.GetComponent<SpriteRenderer>().color = newColor;
        transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = otherColor;
        _tileRenderer.transform.DORotate(Vector2.zero, 0.5f);
        _tileRenderer.transform.DOScale(_scale, 0.75f);
    }

    public void Flash(Piece piece)
    {
        StartCoroutine(nameof(Flash1), piece);
    }

    private IEnumerator Flash1(Piece piece)
    {
        Color previousColor = _dotSR.material.color;
        _dotSR.material.DOColor(Color.red, 0.2f);
        while (piece.Dragging)
            yield return null;
        _dotSR.material.DOColor(previousColor, 0.2f);
    }
}
