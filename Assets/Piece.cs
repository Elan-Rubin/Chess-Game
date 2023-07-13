using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Piece : MonoBehaviour
{
    bool _dragging, _draggingFirstCheck;
    public bool Dragging { get { return _dragging; } }
    Tile _tile;
    Vector2 _prevPos;

    //pawn, bishop, rook, knight, queen, king
    [SerializeField] List<Sprite> _pieceSprites = new List<Sprite>();

    Camera _virtualCamera;

    [SerializeField] PieceType _type = PieceType.Pawn;
    public PieceType Type { get { return _type; } set { _type = value; } }

    [SerializeField] GameObject bloodSplatter;

    void Start()
    {
        _virtualCamera = GameObject.Find("Virtual Camera").GetComponent<Camera>();
        transform.parent = GridManager.Instance.transform.GetChild(1);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) ChangeType(_type.Equals(PieceType.King) ? 0 : (PieceType)((int)_type + 1));
        if (_draggingFirstCheck)
        {
            AudioManager.Instance.PlaySound(SoundType.ImpactLight);
            _draggingFirstCheck = false;
            List<List<Tile>> tiles = GridManager.Instance.Tiles;
            foreach(List<Tile> tiles1 in tiles)
            {
                foreach(Tile tile in tiles1)
                {
                    if (CheckTile(_tile, tile)) tile.Flash(this);
                }
            }
        }
        if (_dragging)
        {
            Hand.Instance.GoTo(transform.position);

            var currentPos = transform.position;
            //dont use cam main call
            var targetPos = _virtualCamera.ScreenToWorldPoint(Input.mousePosition);

            transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = targetPos.x > currentPos.x;

            transform.position = Vector2.Lerp(currentPos, targetPos, 10 * Time.deltaTime);
            //transform.position = Vector3.Slerp((Vector2)currentPos, (Vector2)targetPos, 10 * Time.deltaTime);
            if (Input.GetMouseButtonUp(0))
            {
                _dragging = false;
                var hits = new List<Collider2D>(Physics2D.OverlapCircleAll(transform.position, 1.5f));
                hits.Sort((h1, h2) => Vector2.Distance(transform.position, h1.transform.position).CompareTo(Vector2.Distance(transform.position, h2.transform.position)));

                //while (hits[counter].gameObject.tag.Equals("Piece") && counter < hits.Count - 1)
                //    counter++;

                List<Collider2D> hits2 = new List<Collider2D>();
                foreach (Collider2D hit in hits)
                {
                    if (hit.GetComponent<Tile>())
                    {
                        if (_tile != null)
                        {
                            if (CheckTile(_tile, hit.GetComponent<Tile>()))
                                hits2.Add(hit);
                        }
                        else
                        {
                            hits2.Add(hit);
                        }
                    }
                }

                if (hits2.Count > 0)
                {
                    transform.DOMove(hits2[0].transform.position, 0.15f).OnComplete(() =>
                    {
                        AudioManager.Instance.PlaySound(SoundType.ImpactMedium);
                        if (_tile!=null) _tile.MyPiece = null;
                        _tile = hits2[0].gameObject.GetComponent<Tile>();
                        _tile.MyPiece = this;
                        Boing();
                    });
                }
                else
                {
                    transform.DOMove(_prevPos, 0.15f);
                }
                transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = false;
                //_tile.MyPiece = this;
            }
        }
    }

    void ChangeType(PieceType newType)
    {
        if (_type.Equals(newType)) return;
        _type = newType;
        //pawn, bishop, rook, knight, queen, king
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = _pieceSprites[(int)_type];
        //play animation for switching
        
        Debug.Log($"<color=yellow> Piece type changed to: </color>" + newType);

        Boing();
    }

    bool CheckTile(Tile currentTile, Tile newTile)
    {
        return CheckTile(currentTile, newTile, _type);
    }

    bool CheckTile(Tile currentTile, Tile newTile, PieceType type)
    {
        if (currentTile == newTile) return false;
        if (newTile.MyPiece != null) return false;
        var currentPos = currentTile.Position;
        var newPos = newTile.Position;
        var absValX = Mathf.Abs(currentPos.x - newPos.x);
        var absValY = Mathf.Abs(currentPos.y - newPos.y);
        switch (type)
        {
            case PieceType.Pawn:
                return true; //haven't worked this one out yet
            case PieceType.Bishop:
                if (absValX == absValY)
                    return CheckTileCast(currentTile, newTile, PieceType.Bishop);
                break;
            case PieceType.Rook:
                if (absValX == 0 || absValY == 0)
                    return CheckTileCast(currentTile, newTile, PieceType.Rook);
                break;
            case PieceType.Knight:
                return (absValX == 1 && absValY == 2) || (absValX == 2 && absValY == 1);
            case PieceType.Queen:
                if ((absValX == absValY) || (absValX == 0 || absValY == 0))
                    return CheckTileCast(currentTile, newTile, PieceType.Queen);
                break;
            case PieceType.King:
                return absValX <= 1 && absValY <= 1;
        }
        return false;
    }

    bool CheckTileCast(Tile currentTile, Tile newTile, PieceType type)
    {
        Vector2Int currentPos = currentTile.Position, newPos = newTile.Position;
        List<List<Tile>> tiles = GridManager.Instance.Tiles;
        int deltaX = Mathf.Abs(newPos.x - currentPos.x), deltaY = Mathf.Abs(newPos.y - currentPos.y);
        Vector2 dir = (newPos - currentPos);
        var dirN = dir.normalized;
        switch (type)
        {
            case PieceType.Bishop: //diagonal
                //delta x and y will be equal
                for (int i = 0; i < deltaX; i++)
                {
                    if (tiles[currentPos.x + (dir.x > 0 ? 1 : -1)][currentPos.y + (dir.y > 0 ? 1 : -1)].GetComponent<Tile>().MyPiece != null) return false;
                }
                break;
            case PieceType.Rook: //horizontal or vertical
                if (deltaX > 0 && deltaY == 0)
                {
                    for (int i = 1; i < deltaX; i++)
                    {
                        if (tiles[currentPos.x + (int)(i * dirN.x)][currentPos.y].GetComponent<Tile>().MyPiece != null) return false;
                    }
                }
                else if(deltaY > 0 && deltaX == 0)
                {
                    for (int i = 1; i < deltaY; i++)
                    {
                        if (tiles[currentPos.x][currentPos.y + (int)(i * dirN.y)].GetComponent<Tile>().MyPiece != null) return false;
                    }
                }
                return true;
            case PieceType.Queen: //both diagonal and horizontal or vertical
                return CheckTileCast(currentTile, newTile, PieceType.Bishop) || CheckTileCast(currentTile, newTile, PieceType.Rook);
        }
        return true;
    }

    //bool CheckTileCast(Tile currentTile, Tile newTile)
    //{
    //    Debug.Log($"<color=red> CheckTileCast called. </color>");
    //    Vector2 currentPos = currentTile.transform.position;
    //    Vector2 newPos = newTile.transform.position;
    //    //Debug.DrawLine(currentPos, new Vector3(0, 0, Vector2.Angle(currentPos, newPos)), Color.red, Vector2.Distance(currentPos, newPos));
    //    Debug.DrawLine(currentPos, newPos, Color.red, 1f);
    //    //var hits = new List<RaycastHit2D>(Physics2D.RaycastAll(currentPos, (newPos - currentPos).normalized * 10, Vector2.Distance(currentPos, newPos)));
    //    var hits = new List<RaycastHit2D>(Physics2D.RaycastAll(currentPos, Quaternion.Euler(0, 0, Vector2.Angle(currentPos, newPos)) * Vector2.right, Vector2.Distance(currentPos, newPos)));

    //    foreach (RaycastHit2D hit in hits)
    //    {
    //        //Debug.Log(hit.transform.gameObject);
    //        if (hit.transform.GetComponent<Piece>() && !hit.transform.Equals(transform))
    //        {
    //            Debug.Log($"<color=yellow>"+hit.transform.gameObject+"</color>");
    //            return false;
    //        }
    //    }
    //    return true;
    //}

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _dragging = _draggingFirstCheck = true;
            _prevPos = transform.position;
        }
    }

    private void Boing()
    {
        transform.GetChild(0).DOPunchScale(transform.localScale * 1.5f, 0.35f);
        //transform.GetChild(1).DOScale(Vector2.one * 3, 0.25f).OnComplete(() => transform.GetChild(1).DOScale(Vector2.zero, 0.25f));
        for (int i = 0; i < Random.Range(1,4); i++)
        {
            Instantiate(bloodSplatter, transform.position, Quaternion.identity);
        }

        Color previousColor = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
        transform.GetChild(0).GetComponent<SpriteRenderer>().DOColor(Color.white, 0.1f).SetEase(Ease.InExpo).OnComplete(() =>
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().DOColor(previousColor, 0.1f).SetEase(Ease.OutExpo);
        });
    }
}

public enum PieceType
{
    Pawn = 0,
    Bishop = 1,
    Rook = 2,
    Knight = 3,
    Queen = 4,
    King = 5
}
