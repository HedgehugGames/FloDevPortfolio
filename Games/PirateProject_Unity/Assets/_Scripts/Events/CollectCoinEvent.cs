using GameEvents;

public class CollectCoinEvent : GameEvent
{
    public readonly int Amount;
    public CollectCoinEvent(int amount) { this.Amount = amount; }
}