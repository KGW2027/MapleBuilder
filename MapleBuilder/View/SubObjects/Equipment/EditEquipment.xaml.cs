using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using MapleAPI.Enum;
using MapleBuilder.Control.Data.Item;
using MapleBuilder.View.SubObjects.Equipment.EditEquips;

namespace MapleBuilder.View.SubObjects.Equipment;

public partial class EditEquipment : UserControl
{
    public class SaveEquipmentEvent : RoutedEventArgs
    {
        public ItemBase? NewItem;
        public bool CloseWindow;
        protected internal SaveEquipmentEvent(RoutedEvent routedEvent, ItemBase? item, bool close) : base(routedEvent)
        {
            NewItem = item;
            CloseWindow = close;
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
        var control = (EditEquipment) d;
        if (control.defaultItem != null && e.OldValue is ItemBase oldItem)
            control.Cancel(oldItem);

        if (e.NewValue is not ItemBase newItem)
        {
            Console.WriteLine("new value is not itembase");
            return;
        }
        control.defaultItem = newItem.Clone();

        control.WeaponOnly.Visibility = control.defaultItem!.EquipType == MapleEquipType.EquipType.WEAPON
            ? Visibility.Visible
            : Visibility.Collapsed;

        if (e.NewValue is CommonItem cItem)
        {
            if (cItem.Starforce != null)
            {
                control.SfvEditor.MaxStarforce = cItem.EquipData!.IsSuperior
                    ? cItem.ItemLevel switch // 슈페리얼 아이템
                    {
                        < 95 => 3,
                        < 108 => 5,
                        < 118 => 8,
                        < 128 => 10,
                        < 138 => 12,
                        >= 138 => 15,
                    }
                    : cItem.ItemLevel switch // 일반 아이템
                    {
                        < 95 => 5,
                        < 108 => 8,
                        < 118 => 10,
                        < 128 => 15,
                        < 138 => 20,
                        >= 138 => 25,
                    };
                control.SfvEditor.Starforce = cItem.Starforce ?? 0;
                control.SfvEditor.Visibility = Visibility.Visible;
            } 
            else control.SfvEditor.Visibility = Visibility.Collapsed;

            if (cItem.AddOptions != null)
            {
                control.AddOptEditor.TargetItem = cItem;
                control.AddOptEditor.Visibility = Visibility.Visible;
            }
            else control.AddOptEditor.Visibility = Visibility.Collapsed;

            if (cItem.Potential != null)
            {
                control.PotentialEditor.TargetItem = cItem;
                control.AdditionalEditor.TargetItem = cItem;
                control.PotentialEditor.Visibility = Visibility.Visible;
                control.AdditionalEditor.Visibility = Visibility.Visible;
            }
            else
            {
                control.PotentialEditor.Visibility = Visibility.Collapsed;
                control.AdditionalEditor.Visibility = Visibility.Collapsed;
            }

            if (cItem.Upgrades != null)
            {
                control.UpgradePreview.TargetItem = cItem;
                control.UpgradeEditor.TargetItem = cItem;
                control.UpgradePreview.Visibility = Visibility.Visible;
            }
            else
            {
                control.UpgradeEditor.Visibility = Visibility.Collapsed;
                control.UpgradePreview.Visibility = Visibility.Collapsed;
            }

            if (cItem.SoulOption != null) control.SoulEditor.TargetItem = cItem;
        }
        
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
        if (defaultItem == null || TargetItem == null) return;

        if (defaultItem.EquipData != null) Thumbnail.Source = defaultItem.EquipData.Image;
        ItemName.Text = TargetItem.DisplayName;

        var status = TargetItem.GetUpStatus();
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
        SaveEquipmentEvent args = new SaveEquipmentEvent(SAVED_EVENT, TargetItem, true)
        {
            NewItem = TargetItem
        };
        RaiseEvent(args);
        defaultItem = null;
        TargetItem = null;
    }

    private void Cancel(ItemBase previousItem)
    {
        if (previousItem is CommonItem cItem && defaultItem is CommonItem defItem)
        {
            cItem.Starforce = defItem.Starforce; // 1
            cItem.AddOptions = defItem.AddOptions; // 1
            cItem.Potential = defItem.Potential; // 3
            cItem.TopGrade = defItem.TopGrade;
            cItem.BottomGrade = defItem.BottomGrade;
            cItem.Upgrades = defItem.Upgrades; // 4
            cItem.ChaosAverage = defItem.ChaosAverage;
            cItem.MaxUpgradeCount = defItem.MaxUpgradeCount;
            cItem.RemainUpgradeCount = defItem.RemainUpgradeCount;
            cItem.SoulName = defItem.SoulName; // 2
            cItem.SoulOption = defItem.SoulOption;
            defaultItem = null;
        }
        
        SaveEquipmentEvent args = new SaveEquipmentEvent(SAVED_EVENT, TargetItem, TargetItem != null && previousItem.ItemHash == TargetItem.ItemHash)
        {
            NewItem = TargetItem
        };
        RaiseEvent(args);
    }

    private void OnCancelClicked(object sender, RoutedEventArgs e)
    {
        Cancel(TargetItem!);
        TargetItem = null;
    }

    private void StarforceChanged(object sender, RoutedEventArgs e)
    {
        if (TargetItem is not CommonItem cItem || e is not StarforceEditor.StarforceChangedArgs args) return;
        cItem.Starforce = args.NewStarforce;
        Update();
    }

    private void OnAddOptionChanged(object sender, RoutedEventArgs e)
    {
        Update();
    }

    private void OpenUpgradeEditor(object sender, RoutedEventArgs e)
    {
        UpgradeEditor.Visibility = Visibility.Visible;
    }

    private void CloseUpgradeEditor(object sender, RoutedEventArgs e)
    {
        UpgradePreview.Update();
        UpgradeEditor.Visibility = Visibility.Collapsed;
    }
}