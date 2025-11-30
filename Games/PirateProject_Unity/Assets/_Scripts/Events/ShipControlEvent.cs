using GameEvents;
public class ShipControlEvent : GameEvent
{
    public readonly bool IsControllingShip; // True if the player is controlling the ship
    public readonly ShipController Ship;   // Reference to the ship that is being controlled

    public ShipControlEvent(ShipController ship, bool isControllingShip)
    {
        this.Ship = ship;
        this.IsControllingShip = isControllingShip;
    }
}
