using System.Collections.Generic;
using UnityEngine;

public class TouchData
{
    public enum TouchType { Click, Swipe, LongPress }
    public TouchType type;
    public Vector2 position;
    public Vector2 lastPosition;
    public bool isHolding;
    public bool isJude;
    public TouchData(TouchType type, Vector2 position, Vector2 lastPosition, bool isHolding)
    {
        this.type = type;
        this.position = position;
        this.lastPosition = lastPosition;
        this.isHolding = isHolding;
    }
}
