using Godot;

public partial class InventoryUI : CanvasLayer
{
    private const int HotbarCount = 6;
    private readonly TextureRect[] inventorySelection = new TextureRect[HotbarCount];
    private readonly TextureRect[] hotbarSelection = new TextureRect[HotbarCount];
    private Control inventory;
    private Control hotbar;
    private int selectedSlot;

    public bool BlocksAttack => inventory.Visible || hotbar.GetGlobalRect().HasPoint(hotbar.GetGlobalMousePosition());

    public override void _Ready()
    {
        AddToGroup("inventory_ui");
        inventory = GetNode<Control>("Inventory");
        hotbar = GetNode<Control>("Hotbar");
        GetNode<TextureButton>("Inventory/Close").Pressed += () => inventory.Hide();
        for (int i = 0; i < HotbarCount; i++)
        {
            int index = i;
            var inventorySlot = GetNode<TextureButton>($"Inventory/Grid/Slot{i + 1}");
            var hotbarSlot = GetNode<TextureButton>($"Hotbar/Slot{i + 1}");
            inventorySelection[i] = inventorySlot.GetNode<TextureRect>("Selection");
            hotbarSelection[i] = hotbarSlot.GetNode<TextureRect>("Selection");
            inventorySlot.Pressed += () => SelectSlot(index);
            hotbarSlot.Pressed += () => SelectSlot(index);
        }
        // Preview the full UI when this scene is run on its own.
        if (GetTree().CurrentScene != this)
            inventory.Hide();
        SelectSlot(0);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventKey key || !key.Pressed || key.Echo)
            return;

        if (key.Keycode == Key.I)
        {
            inventory.Visible = !inventory.Visible;
            GetViewport().SetInputAsHandled();
        }
        else if (key.Keycode == Key.Escape && inventory.Visible)
        {
            inventory.Hide();
            GetViewport().SetInputAsHandled();
        }
        else if (key.Keycode >= Key.Key1 && key.Keycode <= Key.Key6)
        {
            SelectSlot((int)key.Keycode - (int)Key.Key1);
            GetViewport().SetInputAsHandled();
        }
    }

    private void SelectSlot(int index)
    {
        selectedSlot = index;
        for (int i = 0; i < HotbarCount; i++)
        {
            inventorySelection[i].Visible = i == selectedSlot;
            hotbarSelection[i].Visible = i == selectedSlot;
        }
    }
}
