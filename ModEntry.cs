using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewModdingAPI.Utilities;
using StardewValley.Menus;

namespace StarAttribute;

public class ModEntry : Mod
{
    // ====== UI
    private readonly PerScreen<Toolbar?> _toolbar = new();
    private readonly PerScreen<IList<ClickableComponent>?> _toolbarSlots = new();
    
    private readonly Rectangle _tooltipSourceRect = new(0, 256, 60, 60);
    private const int TooltipBorderSize = 12;
    private const int Padding = 5;
    private readonly Vector2 _tooltipOffset = new(Game1.tileSize / 2f);
    
    private readonly Dictionary<Item, AttributeData> _items = new();

    public override void Entry(IModHelper helper)
    {
        Helper.Events.Player.InventoryChanged += OnInventoryChanged;
        Helper.Events.Display.RenderedHud += OnRenderedHud;
        Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
    }
    
    private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
    {
        foreach (var item in e.Added)
        {
            if (_items.ContainsKey(item)) continue;

            var attribute = new AttributeData(item);
            _items.Add(item, attribute);
            Monitor.Log(
                $"[StarAttribute] Add item {item.DisplayName} with {attribute.GetDisplayDesc()}",
                LogLevel.Debug);
        }

        foreach (var item in e.Removed)
        {
            if (_items.ContainsKey(item))
            {
                var attribute = _items[item];
                attribute.DeActive();
                _items.Remove(item);
                Monitor.Log(
                    $"[StarAttribute] Remove item {item.DisplayName} with {attribute.GetDisplayDesc()}",
                    LogLevel.Debug);
            }
        }
    }
    
    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        // cache the toolbar & slots
        if (e.IsOneSecond)
        {
            if (Context.IsPlayerFree)
            {
                _toolbar.Value = Game1.onScreenMenus.OfType<Toolbar>().FirstOrDefault();
                _toolbarSlots.Value = this._toolbar.Value != null
                    ? Helper.Reflection.GetField<List<ClickableComponent>>(this._toolbar.Value, "buttons").GetValue()
                    : null;
            }
            else
            {
                _toolbar.Value = null;
                _toolbarSlots.Value = null;
            }
        }
    }

    
    private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
    {
        if (!Context.IsPlayerFree) return;

        Item? item = GetItemFromToolbar();
        if (item == null) return;
        
        DrawTooltip(Game1.spriteBatch, Game1.smallFont, item);
    }
    
    
    private void DrawTooltip(SpriteBatch spriteBatch, SpriteFont font, Item item)
        {
            if (!_items.TryGetValue(item, out var data)) return;
            
            // basic measurements
            const int borderSize = TooltipBorderSize;
            const int padding = Padding;
            Vector2 offsetFromCursor = _tooltipOffset;

            // prepare text
            string desc = data.GetDisplayDesc();
            
            // get dimensions
            Vector2 descSize = font.MeasureString(desc);
            Vector2 innerSize = new(descSize.X + padding + padding, descSize.Y);
            Vector2 outerSize = innerSize + new Vector2((borderSize + padding) * 2);

            // get position
            float x = Game1.getMouseX() - offsetFromCursor.X - outerSize.X - innerSize.X;
            float y = Game1.getMouseY() + offsetFromCursor.Y + borderSize;

            // adjust position to fit on screen
            Rectangle area = new((int)x, (int)y, (int)outerSize.X, (int)outerSize.Y);
            if (area.Right > Game1.uiViewport.Width)
                x = Game1.uiViewport.Width - area.Width;
            if (area.Bottom > Game1.uiViewport.Height)
                y = Game1.uiViewport.Height - area.Height;

            // draw tooltip box
            IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, _tooltipSourceRect, (int)x, (int)y, (int)outerSize.X, (int)outerSize.Y, Color.White);
            // draw text
            Utility.drawTextWithShadow(spriteBatch, desc, font, new Vector2(x + borderSize + padding, y + borderSize + padding), Game1.textColor);
        }
    
    
    private Item? GetItemFromToolbar()
    {
        if (!Context.IsPlayerFree || _toolbar.Value == null || _toolbarSlots.Value == null || !Game1.displayHUD)
            return null;

        // find hovered slot
        int x = Game1.getMouseX();
        int y = Game1.getMouseY();
        ClickableComponent? hoveredSlot = _toolbarSlots.Value.FirstOrDefault(slot => slot.containsPoint(x, y));
        if (hoveredSlot == null)
            return null;

        // get inventory index
        int index = _toolbarSlots.Value.IndexOf(hoveredSlot);
        if (index < 0 || index > Game1.player.Items.Count - 1)
            return null;

        // get hovered item
        return Game1.player.Items[index];
    }
    
}