using UnityEngine;

public class Tile : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }
    private Match3Game gameController;

    private static Tile selectedTile;

    public void Initialize(int x, int y, Match3Game controller)
    {
        X = x;
        Y = y;
        gameController = controller;

        transform.localScale = new Vector3(135.3468f, 135.3468f, 135.3468f);
    }

    void OnMouseDown()
    {
        if (selectedTile == null)
        {
            selectedTile = this;
            HighlightTile(true);
        }
        else if (selectedTile == this)
        {
            HighlightTile(false);
            selectedTile = null;
        }
        else
        {
            if (IsAdjacentTo(selectedTile))
            {
                gameController.SwapTiles(this, selectedTile);
                selectedTile.HighlightTile(false);
                selectedTile = null;
            }
            else
            {
                selectedTile.HighlightTile(false);
                selectedTile = this;
                HighlightTile(true);
            }
        }
    }

    bool IsAdjacentTo(Tile other)
    {
        return Mathf.Abs(X - other.X) + Mathf.Abs(Y - other.Y) == 1;
    }

    void HighlightTile(bool highlight)
    {
        if (highlight)
            transform.GetComponent<SpriteRenderer>().color = Color.green;
        else
            transform.GetComponent<SpriteRenderer>().color = Color.white;
    }
}