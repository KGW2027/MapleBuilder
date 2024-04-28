using System.Collections.Generic;
using System.Windows.Controls;
using MapleAPI.DataType;
using MapleAPI.Enum;
using MapleBuilder.Control;
using MapleBuilder.Control.Data;

namespace MapleBuilder.View.SubFrames;

#pragma warning disable CS8604

public partial class CashEquips : UserControl
{
    private Dictionary<string, MapleStatContainer> setOptions;
    
    public CashEquips()
    {
        InitializeComponent();

        MapleStatContainer none = new MapleStatContainer();
        
        MapleStatContainer etc5 = new MapleStatContainer();
        etc5[MapleStatus.StatusType.ALL_STAT] += 5;
        etc5[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 3;
        etc5.Flush();
        
        MapleStatContainer m3 = new MapleStatContainer();
        m3[MapleStatus.StatusType.ALL_STAT] += 5;
        m3[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 3;
        m3[MapleStatus.StatusType.HP_AND_MP] += 250;
        m3.Flush();
        
        MapleStatContainer m5 = new MapleStatContainer();
        m5[MapleStatus.StatusType.ALL_STAT] += 15;
        m5[MapleStatus.StatusType.ATTACK_AND_MAGIC] += 10;
        m5[MapleStatus.StatusType.HP_AND_MP] += 750;
        m5.Flush();

        setOptions = new Dictionary<string, MapleStatContainer>
        {
            {"없음", none}, {"기타 5세트", etc5}, {"마라벨 3세트", m3}, {"마라벨 5세트", m5}
        };
        foreach (string key in setOptions.Keys) ctCashSetOption.Items.Add(key);

    }

    private void UpdateLabel(string selected)
    {
        switch (selected)
        {
            case "없음":
                ctCashSetOptionLabel.Content = "-";
                break;
            case "기타 5세트":
                ctCashSetOptionLabel.Content = "올스텟 +5\n공격력/마력 +3";
                break;
            case "마라벨 3세트":
                ctCashSetOptionLabel.Content = "올스텟 +5\n공격력/마력 +3\n최대 HP/MP +250";
                break;
            case "마라벨 5세트":
                ctCashSetOptionLabel.Content = "올스텟 +15\n공격력/마력 +10\n최대 HP/MP +750";
                break;
        }
    }

    private void OnSetOptionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0) return;
        UpdateLabel(e.AddedItems[0]!.ToString()!);
        if (GlobalDataController.Instance.PlayerInstance == null) return;
        
        string before = ((ComboBox) sender).Text;
        string next = e.AddedItems[0]!.ToString()!;

        GlobalDataController.Instance.PlayerInstance[PlayerData.StatSources.OTHER] ??= new MapleStatContainer();
        
        if (setOptions.TryGetValue(before, out var prev) && setOptions.TryGetValue(next, out var now))
            GlobalDataController.Instance.PlayerInstance[PlayerData.StatSources.OTHER] += (now - prev);
    }
}