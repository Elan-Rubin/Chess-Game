using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private static GridManager _instance;
    public static GridManager Instance { get { return _instance; } }

    List<List<Tile>> _tiles = new List<List<Tile>>();
    public List<List<Tile>> Tiles { get { return _tiles; } }

    public Vector2Int Dimensions = new Vector2Int(5, 5);
    public float Spacing = 1;
    [SerializeField] GameObject _tile;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    void Start()
    {
        var alternate = false;
        Transform pivot = new GameObject("Pivot").GetComponent<Transform>();
        Vector2 instPos = new Vector2();

        float waitTime = 0;
        int counter = 0;

        int total = Dimensions.x * Dimensions.y;

        for (int x = 0; x < Dimensions.x; x++)
        {
            List<Tile> column = new List<Tile>();
            for (int y = 0; y < Dimensions.y; y++)
            {
                GameObject newTile = Instantiate(_tile, instPos, Quaternion.identity);
                newTile.name = "Tile (" + x + "," + y + ")";
                newTile.transform.parent = pivot;
                Tile newTile2 = newTile.GetComponent<Tile>();
                newTile2.Number = counter;
                newTile2.Position = new Vector2Int(x, y);
                List<Color> palette = GameManager.Instance.Palette;
                newTile2.Appear(waitTime += 0.01f, alternate ? palette[0] : palette[1], alternate ? palette[2] : palette[3]);
                instPos += new Vector2(0, Spacing);

                column.Add(newTile2);

                alternate = !alternate;
                counter++;
            }
            instPos = new Vector2(instPos.x + Spacing, 0);
            if (Dimensions.y % 2 == 0) alternate = !alternate;
            _tiles.Add(column);
        }

        pivot.Translate(new Vector2(-(Dimensions.x - 1) / 2f, -(Dimensions.y - 1) / 2f));

        while (pivot.childCount > 0)
            pivot.GetChild(0).SetParent(transform.GetChild(0));
        Destroy(pivot.gameObject);
    }

    void Update()
    {

    }
}
