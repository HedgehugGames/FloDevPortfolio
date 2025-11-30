using GameEvents;
public class CollectItemEvent : GameEvent
    {
        public string ItemName;
        public CollectItemEvent(string itemName) { this.ItemName = itemName; }
    }


