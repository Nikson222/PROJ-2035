using UnityEngine;

internal struct Segment
{
    public Vector2 start;
    public Vector2 end;

    public Segment(Vector2 start, Vector2 end)
    {
        this.start = start;
        this.end = end;
    }
}