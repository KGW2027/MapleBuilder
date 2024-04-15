using System;
using System.Windows;
using System.Windows.Controls;
using MapleAPI.Enum;

namespace MapleBuilder.View.SubObjects;

public partial class PetEquipSlot : UserControl
{
    private int maxUpgrade;
    
    public PetEquipSlot()
    {
        InitializeComponent();

        foreach (MaplePetType.PetType petType in Enum.GetValues<MaplePetType.PetType>())
            ctSetName.Items.Add(MaplePetType.GetPetTypeString(petType));

        ctSetNum.Items.Add("세트 1");
        ctSetNum.Items.Add("세트 2");
        ctSetNum.Items.Add("세트 3");
        maxUpgrade = 0;
    }

    #region XAML Property
    
    public static readonly DependencyProperty PET_TYPE_PROPERTY =
        DependencyProperty.Register("PetType", typeof(MaplePetType.PetType), typeof(PetEquipSlot), new PropertyMetadata(MaplePetType.PetType.OTHER,
            (o, args) =>
            {
                var control = (PetEquipSlot) o;
                MaplePetType.PetType petType = (MaplePetType.PetType) args.NewValue;
                control.ctSetName.Text = MaplePetType.GetPetTypeString(petType);
                control.maxUpgrade = petType switch
                {
                    MaplePetType.PetType.LUNA_PETIT => 10,
                    MaplePetType.PetType.OTHER => 8,
                    _ => 9
                };
                control.ctUpgradeCount.Content = $"8 / {control.maxUpgrade}";
            }));
    
    public MaplePetType.PetType PetType
    {
        get => (MaplePetType.PetType) GetValue(PET_TYPE_PROPERTY);
        set => SetValue(PET_TYPE_PROPERTY, value);
    }
    
    
    public static readonly DependencyProperty PET_SET_NUM_PROPERTY =
        DependencyProperty.Register("PetSetNumber", typeof(int), typeof(PetEquipSlot), new PropertyMetadata(0,
            (o, args) =>
            {
                var control = (PetEquipSlot) o;
                control.ctSetNum.Text = $"세트 {(int) args.NewValue}";
            }));
    
    public int PetSetNumber
    {
        get => (int) GetValue(PET_SET_NUM_PROPERTY);
        set => SetValue(PET_SET_NUM_PROPERTY, value);
    }
    
    public static readonly DependencyProperty PET_ORDER_PROPERTY =
        DependencyProperty.Register("PetOrder", typeof(int), typeof(PetEquipSlot), new PropertyMetadata(0, OnPetOrderChanged));

    public int PetOrder
    {
        get => (int) GetValue(PET_ORDER_PROPERTY);
        set => SetValue(PET_ORDER_PROPERTY, value);
    }
    
    private static void OnPetOrderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (PetEquipSlot) d;
        control.ctTitle.Content = $"펫 슬롯 {(int) e.NewValue}";
    }
    
    private static readonly RoutedEvent SET_CHANGED_EVENT = EventManager.RegisterRoutedEvent("SetChanged",
        RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PetEquipSlot));
    
    public event RoutedEventHandler SetChanged
    {
        add => AddHandler(SET_CHANGED_EVENT, value);
        remove => RemoveHandler(SET_CHANGED_EVENT, value);
    }
    
    private static readonly RoutedEvent TYPE_CHANGED_EVENT = EventManager.RegisterRoutedEvent("TypeChanged",
        RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PetEquipSlot));
    
    public event RoutedEventHandler TypeChanged
    {
        add => AddHandler(TYPE_CHANGED_EVENT, value);
        remove => RemoveHandler(TYPE_CHANGED_EVENT, value);
    }
    
    
    #endregion

    private void OnPetTypeChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0) return;
        PetType = MaplePetType.GetPetType(e.AddedItems[0]!.ToString()!);

        maxUpgrade = PetType switch
        {
            MaplePetType.PetType.WONDER_BLACK => 10,
            MaplePetType.PetType.OTHER => 8,
            _ => 9
        };
        ctUpgradeCount.Content = $"8 / {maxUpgrade}";
        
        RoutedEventArgs newEventArgs = new RoutedEventArgs(TYPE_CHANGED_EVENT);
        RaiseEvent(newEventArgs);
        
    }

    private void OnPetSetNumChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0) return;
        PetSetNumber = e.AddedItems[0]!.ToString()![^1] - '0';
        
        RoutedEventArgs newEventArgs = new RoutedEventArgs(SET_CHANGED_EVENT);
        RaiseEvent(newEventArgs);
    }
}