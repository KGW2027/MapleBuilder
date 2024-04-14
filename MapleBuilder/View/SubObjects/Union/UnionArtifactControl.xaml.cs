using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MapleAPI.Enum;

namespace MapleBuilder.View.SubObjects;

public partial class UnionArtifactControl : UserControl
{
    public class ArtifactSettingChangedEventArgs : RoutedEventArgs
    {
        public List<MapleStatus.StatusType> StatusTypes;
        public int Level;
        public bool IsChangedLevel;
        public int BeforeLevel;
        public MapleStatus.StatusType BeforeType;
        public MapleStatus.StatusType NewType;

        public ArtifactSettingChangedEventArgs(RoutedEvent levelChangedEvent) : base(levelChangedEvent)
        {
            StatusTypes = new List<MapleStatus.StatusType>();
            Level = 0;
            IsChangedLevel = false;
            BeforeLevel = 0;
            BeforeType = MapleStatus.StatusType.OTHER;
            NewType = MapleStatus.StatusType.OTHER;
        }
    }


    private readonly List<ComboBox> optionSelectors;
    
    public UnionArtifactControl()
    {
        InitializeComponent();

        optionSelectors = new List<ComboBox>
        {
            ctOptionSelector1, ctOptionSelector2, ctOptionSelector3
        };

        for (int idx = 0; idx <= 5; idx++)
            ctLevelSelector.Items.Add($"LV {idx}");

        foreach (ComboBox comboBox in optionSelectors)
        {
            foreach (string option in MapleArtifact.GetOptionList())
                comboBox.Items.Add(option);
            comboBox.Items.Add("잠김");
            comboBox.Text = "잠김";
        }

    }
    
    #region XAML Property
    
    private static readonly RoutedEvent SETTING_CHANGED_EVENT = EventManager.RegisterRoutedEvent("SettingChanged",
        RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UnionArtifactControl));
    
    public event RoutedEventHandler SettingChanged
    {
        add => AddHandler(SETTING_CHANGED_EVENT, value);
        remove => RemoveHandler(SETTING_CHANGED_EVENT, value);
    }

    #endregion

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0) return;
        
        ArtifactSettingChangedEventArgs eventArgs = new ArtifactSettingChangedEventArgs(SETTING_CHANGED_EVENT);
        ComboBox self = (ComboBox) sender;
        eventArgs.Level = (self == ctLevelSelector ? e.AddedItems[0]!.ToString()![^1] : ctLevelSelector.Text[^1]) - '0';

        foreach (ComboBox comboBox in optionSelectors)
        {
            string query = (self == comboBox ? e.AddedItems[0]!.ToString()! : comboBox.Text) + " 증가";
            MapleStatus.StatusType statusType = MapleArtifact.GetArtifactType(query);
            eventArgs.StatusTypes.Add(statusType);

            if (self == comboBox)
            {
                eventArgs.BeforeType = MapleArtifact.GetArtifactType($"{comboBox.Text} 증가");
                eventArgs.NewType = statusType;
            }
        }

        eventArgs.IsChangedLevel = self == ctLevelSelector;
        eventArgs.BeforeLevel = self.Text[^1] - '0';
        RaiseEvent(eventArgs);
    }
}