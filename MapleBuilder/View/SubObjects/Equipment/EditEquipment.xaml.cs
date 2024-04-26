using System.Windows;
using System.Windows.Controls;
using MapleAPI.Enum;
using MapleBuilder.Control.Data.Item;

namespace MapleBuilder.View.SubObjects.Equipment;

public partial class EditEquipment : UserControl
{
    public class SaveEquipmentEvent : RoutedEventArgs
    {
        public ItemBase? NewItem;
        protected internal SaveEquipmentEvent(RoutedEvent routedEvent, ItemBase? item) : base(routedEvent)
        {
            NewItem = item;
        }
    }
    
    public EditEquipment()
    {
        InitializeComponent();
    }

    private ItemBase? defaultItem;
    
    
    #region XAML Property
    
    public static readonly RoutedEvent SAVED_EVENT = EventManager.RegisterRoutedEvent(nameof(SavedEvent),
        RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EditEquipment));
    
    public event RoutedEventHandler SavedEvent
    {
        add => AddHandler(SAVED_EVENT, value);
        remove => RemoveHandler(SAVED_EVENT, value);
    }
    
    public static readonly DependencyProperty TargetItemProperty = DependencyProperty.Register(
        nameof(TargetItem), typeof(ItemBase), typeof(EditEquipment), new PropertyMetadata(null, TargetItemChanged)
        );

    private static void TargetItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == null) return;
        var control = (EditEquipment) d;
        control.defaultItem = (ItemBase?) e.NewValue;

        control.WeaponOnly.Visibility = control.defaultItem!.EquipType == MapleEquipType.EquipType.WEAPON
            ? Visibility.Visible
            : Visibility.Collapsed;
        control.Update();
    }

    public ItemBase? TargetItem
    {
        get => (ItemBase?) GetValue(TargetItemProperty);
        set => SetValue(TargetItemProperty, value);
    }
    
    #endregion

    private void Update()
    {
        if (defaultItem == null) return;

        if (defaultItem.EquipData != null) Thumbnail.Source = defaultItem.EquipData.Image;
        ItemName.Text = defaultItem.DisplayName;

        var status = defaultItem.GetUpStatus();
        StrLabel.Content = $"{(int) status[MapleStatus.StatusType.STR]:N0}";
        DexLabel.Content = $"{(int) status[MapleStatus.StatusType.DEX]:N0}";
        IntLabel.Content = $"{(int) status[MapleStatus.StatusType.INT]:N0}";
        LukLabel.Content = $"{(int) status[MapleStatus.StatusType.LUK]:N0}";
        HpLabel.Content = $"{(int) status[MapleStatus.StatusType.HP]:N0}";
        MpLabel.Content = $"{(int) status[MapleStatus.StatusType.MP]:N0}";
        
        AtkLabel.Content = $"{(int) status[MapleStatus.StatusType.ATTACK_POWER]:N0} / {(int) status[MapleStatus.StatusType.MAGIC_POWER]}";
        AllLabel.Content = $"{(int) status[MapleStatus.StatusType.ALL_STAT_RATE]:N0} %";
        DmgLabel.Content = $"{(int) status[MapleStatus.StatusType.DAMAGE]:N0} %";
        BdmgLabel.Content = $"{(int) status[MapleStatus.StatusType.BOSS_DAMAGE]:N0} %";
    }
    
    private void OnSaveClicked(object sender, RoutedEventArgs e)
    {
        SaveEquipmentEvent args = new SaveEquipmentEvent(SAVED_EVENT, TargetItem)
        {
            NewItem = TargetItem
        };
        RaiseEvent(args);
        TargetItem = null;
    }

    private void OnCancelClicked(object sender, RoutedEventArgs e)
    {
        SaveEquipmentEvent args = new SaveEquipmentEvent(SAVED_EVENT, defaultItem)
        {
            NewItem = defaultItem
        };
        RaiseEvent(args);
        defaultItem = null;
    }
}