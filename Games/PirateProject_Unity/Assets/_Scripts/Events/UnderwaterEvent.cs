using UnityEngine;
using GameEvents;

public class UnderwaterEvent : GameEvent
{
    public readonly Underwater Water;
    public readonly bool IsUnderwater;

    public UnderwaterEvent(Underwater water, bool isUnderwater)
    {
        this.Water = water;
        this.IsUnderwater = isUnderwater;
    }

}
