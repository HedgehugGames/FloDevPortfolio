using GameEvents;

public class OpenChestEvent : GameEvent
{
    public readonly OpenChest ChestScript;
    public readonly bool IsOpening;

    public OpenChestEvent(OpenChest chestScript, bool isOpening)
    {
        this.ChestScript = chestScript;
        this.IsOpening = isOpening;
    }
}

