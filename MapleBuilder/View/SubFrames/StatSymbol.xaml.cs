using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MapleAPI.Enum;
using MapleBuilder.Control;
using MapleBuilder.View.SubObjects;

namespace MapleBuilder.View.SubFrames;

public partial class StatSymbol : UserControl
{
    private static StatSymbol? selfInstnace;

    private readonly List<ComboBox>? abilityComboBoxes;
    private readonly List<Slider>? abilitySliders;
    private readonly List<CheckBox?> abilityCheckboxes;
    private List<HyperStatSlot> hyperStatSlots;
    private readonly List<MapleStatus.StatusType> abilityTypes;
    private readonly Dictionary<string, MapleStatus.StatusType> displayToAbilityType;
    private static bool syncLock = false;
    
    public static void Update()
    {
        selfInstnace!.Dispatcher.BeginInvoke(() =>
        {
            int arcane = 0, arcaneStat = 0, authentic = 0, authenticStat = 0;
            bool isXenon = BuilderDataContainer.CharacterInfo!.Class == MapleClass.ClassType.XENON;
            bool isAvenger = BuilderDataContainer.CharacterInfo.Class == MapleClass.ClassType.DEMON_AVENGER;
            foreach (var pair in selfInstnace.symbolLevels)
            {
                int level = BuilderDataContainer.PlayerStatus!.lastSymbols.TryGetValue(pair.Key, out int val) ? val : 0;
                ((TextBox) pair.Value).Text = level.ToString();

                if (level == 0) continue;
                
                switch (pair.Key)
                {
                    case MapleSymbol.SymbolType.YEORO:
                    case MapleSymbol.SymbolType.CHUCHU:
                    case MapleSymbol.SymbolType.LACHELEIN:
                    case MapleSymbol.SymbolType.ARCANA:
                    case MapleSymbol.SymbolType.MORAS:
                    case MapleSymbol.SymbolType.ESFERA:
                        arcane += (level + 2) * 10;
                        arcaneStat += isXenon ? 48 * (level + 2) : isAvenger ? 2100 * (level + 2) : 100 * (level + 2);
                        break;
                    case MapleSymbol.SymbolType.CERNIUM:
                    case MapleSymbol.SymbolType.ARCS:
                    case MapleSymbol.SymbolType.ODIUM:
                    case MapleSymbol.SymbolType.DOWONKYUNG:
                    case MapleSymbol.SymbolType.ARTERIA:
                    case MapleSymbol.SymbolType.CARCION:
                        authentic += level * 10;
                        authenticStat += isXenon ? 48 * (2 * level + 3) : isAvenger ? 2100 * (2 * level + 3) : 100 * (2 * level + 3);
                        break;
                    case MapleSymbol.SymbolType.UNKNOWN:
                    default:
                        break;
                }
            }

            selfInstnace.ctArcaneForceDisplay.Content = $"ARC +{arcane:N0}";
            selfInstnace.ctArcaneStatDisplay.Content = isXenon ? $"STR, DEX, LUK +{arcaneStat:N0}" :
                isAvenger ? $"MAX HP +{arcaneStat:N0}" :
                $"{BuilderDataContainer.PlayerStatus!.PlayerStat.mainStatType} +{arcaneStat:N0}";
            
            selfInstnace.ctAuthenticForceDisplay.Content = $"AUT +{authentic:N0}";
            selfInstnace.ctAuthenticStatDisplay.Content = isXenon ? $"STR, DEX, LUK +{authenticStat:N0}" :
                isAvenger ? $"MAX HP +{authenticStat:N0}" :
                $"{BuilderDataContainer.PlayerStatus!.PlayerStat.mainStatType} +{authenticStat:N0}";
        });
    }

    public static void InitAbility(Dictionary<MapleStatus.StatusType, int> abilities)
    {
        selfInstnace!.Dispatcher.BeginInvoke(() =>
        {
            syncLock = true;
            int idx = 0;
            MaplePotentialGrade.GradeType topGrade = MaplePotentialGrade.GradeType.NONE;
            foreach (var pair in abilities)
            {
                selfInstnace.abilityTypes[idx] = pair.Key;
                if (idx == 0)
                {
                    topGrade = MapleAbility.DetectAbilityGrade(pair.Key, pair.Value);
                    if (topGrade < MaplePotentialGrade.GradeType.EPIC) topGrade = MaplePotentialGrade.GradeType.EPIC;
                }
                else
                {
                    MaplePotentialGrade.GradeType abGrade = MapleAbility.DetectAbilityGrade(pair.Key, pair.Value);
                    selfInstnace.abilityCheckboxes[idx]!.IsChecked = topGrade - abGrade == 1;
                }
                
                idx++;
            }
        
            selfInstnace.ApplyAbility();
            syncLock = false;
            idx = 0;
            foreach (var pair in abilities)
            {
                selfInstnace.abilitySliders![idx++].Value = pair.Value;
            }
        });
    }
    
    public static void InitHyperStat(Dictionary<MapleStatus.StatusType, int> charInfoHyperStatLevels)
    {
        selfInstnace!.Dispatcher.BeginInvoke(() =>
        {
            foreach (var pair in charInfoHyperStatLevels)
            {
                foreach (var hsSlot in selfInstnace.hyperStatSlots.Where(hsSlot => hsSlot.StatType == pair.Key))
                {
                    hsSlot.Level = pair.Value;
                }
            }
        });
    }

    private Dictionary<MapleSymbol.SymbolType, UIElement> symbolLevels;
    
    public StatSymbol()
    {
        selfInstnace = this;
        InitializeComponent();

        symbolLevels = new Dictionary<MapleSymbol.SymbolType, UIElement>
        {
            {MapleSymbol.SymbolType.YEORO, ctYeoroText},
            {MapleSymbol.SymbolType.CHUCHU, ctChuchuText},
            {MapleSymbol.SymbolType.LACHELEIN, ctLacheleinText},
            {MapleSymbol.SymbolType.ARCANA, ctArcanaText},
            {MapleSymbol.SymbolType.MORAS, ctMorasText},
            {MapleSymbol.SymbolType.ESFERA, ctEsferaText},
            {MapleSymbol.SymbolType.CERNIUM, ctCerniumText},
            {MapleSymbol.SymbolType.ARCS, ctArcsText},
            {MapleSymbol.SymbolType.ODIUM, ctOdiumText},
            {MapleSymbol.SymbolType.DOWONKYUNG, ctDowonkyungText},
            {MapleSymbol.SymbolType.ARTERIA, ctArteriaText},
            {MapleSymbol.SymbolType.CARCION, ctCarsionText},
        };

        abilityComboBoxes = new List<ComboBox> {ctAbility1, ctAbility2, ctAbility3};
        abilitySliders = new List<Slider> {ctAbilitySlider1, ctAbilitySlider2, ctAbilitySlider3};
        abilityCheckboxes = new List<CheckBox?> {null, ctAbilityOver2, ctAbilityOver3};
        abilityTypes = new List<MapleStatus.StatusType>
            {MapleStatus.StatusType.OTHER, MapleStatus.StatusType.OTHER, MapleStatus.StatusType.OTHER};
        
        displayToAbilityType = new Dictionary<string, MapleStatus.StatusType>();
        foreach (MapleStatus.StatusType abType in Enum.GetValues(typeof(MapleStatus.StatusType)))
        {
            string str = MapleAbility.GetAbilityString(abType).Replace("%d", "[ ]").Trim();
            if (!displayToAbilityType.TryAdd(str, abType)) continue;
            ctAbility1.Items.Add(str);
            ctAbility2.Items.Add(str);
            ctAbility3.Items.Add(str);
        }

        foreach (MaplePotentialGrade.GradeType grade in Enum.GetValues(typeof(MaplePotentialGrade.GradeType)))
        {
            string str = MaplePotentialGrade.GetPotentialGradeString(grade);
            if (str.Equals("")) continue;
            ctGradeAbility.Items.Add(str);
        }

        ApplyAbility();

        hyperStatSlots = new List<HyperStatSlot>();
        foreach(var child in ParentGrid.GetChildren<HyperStatSlot>())
            hyperStatSlots.Add(child);

        new Thread(LoadSymbolIcons).Start();
    }

    #region 심볼
    private void LoadSymbolIcons()
    {
        while(!ResourceManager.itemIconLoaded) { }

        Dispatcher.Invoke(() =>
        {
            ctYeoroSlot.ctEditLabel.Content = "소멸의 여로";
            ctYeoroSlot.ctItemRenderer.Source = ResourceManager.GetItemIcon("아케인심볼 : 소멸의 여로")!.IconRaw;

            ctChuchuSlot.ctEditLabel.Content = "츄츄 포레스트";
            ctChuchuSlot.ctItemRenderer.Source = ResourceManager.GetItemIcon("아케인심볼 : 츄츄 아일랜드")!.IconRaw;

            ctLacheleinSlot.ctEditLabel.Content = "레헬른";
            ctLacheleinSlot.ctItemRenderer.Source = ResourceManager.GetItemIcon("아케인심볼 : 레헬른")!.IconRaw;

            ctArcanaSlot.ctEditLabel.Content = "아르카나";
            ctArcanaSlot.ctItemRenderer.Source = ResourceManager.GetItemIcon("아케인심볼 : 아르카나")!.IconRaw;

            ctMorasSlot.ctEditLabel.Content = "모라스";
            ctMorasSlot.ctItemRenderer.Source = ResourceManager.GetItemIcon("아케인심볼 : 모라스")!.IconRaw;

            ctEsferaSlot.ctEditLabel.Content = "에스페라";
            ctEsferaSlot.ctItemRenderer.Source = ResourceManager.GetItemIcon("아케인심볼 : 에스페라")!.IconRaw;
            
            ctCerniumSlot.ctEditLabel.Content = "세르니움";
            ctCerniumSlot.ctItemRenderer.Source = ResourceManager.GetItemIcon("어센틱심볼 : 세르니움")!.IconRaw;
            
            ctArcsSlot.ctEditLabel.Content = "아르크스";
            ctArcsSlot.ctItemRenderer.Source = ResourceManager.GetItemIcon("어센틱심볼 : 아르크스")!.IconRaw;
            
            ctOdiumSlot.ctEditLabel.Content = "오디움";
            ctOdiumSlot.ctItemRenderer.Source = ResourceManager.GetItemIcon("어센틱심볼 : 오디움")!.IconRaw;
            
            ctDowonkyungSlot.ctEditLabel.Content = "도원경";
            ctDowonkyungSlot.ctItemRenderer.Source = ResourceManager.GetItemIcon("어센틱심볼 : 도원경")!.IconRaw;
            
            ctArteriaSlot.ctEditLabel.Content = "아르테리아";
            ctArteriaSlot.ctItemRenderer.Source = ResourceManager.GetItemIcon("어센틱심볼 : 아르테리아")!.IconRaw;
            
            ctCarsionSlot.ctEditLabel.Content = "카르시온";
            ctCarsionSlot.ctItemRenderer.Source = ResourceManager.GetItemIcon("어센틱심볼 : 카르시온")!.IconRaw;
        });
    }

    private void CheckTextNumberOnly(object sender, TextCompositionEventArgs e)
    {
        e.Handled = e.Text.Length < 1 || e.Text[0] < '0' || e.Text[0] > '9';
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            int level = int.Parse(((TextBox) sender).Text);
            string name = ((TextBox) sender).Name;
            MapleSymbol.SymbolType sType = MapleSymbol.SymbolType.UNKNOWN;

            foreach (var pair in symbolLevels)
                if (((TextBox) pair.Value).Name.Equals(name))
                    sType = pair.Key;

            if (sType == MapleSymbol.SymbolType.UNKNOWN) return;

            int maxLevel = sType < MapleSymbol.SymbolType.CERNIUM ? 20 : 11;
            if (level > maxLevel)
            {
                ((TextBox) sender).Text = maxLevel.ToString();
                return;
            }

            Dictionary<MapleSymbol.SymbolType, int> newSymbolTable =
                symbolLevels.ToDictionary(pair => pair.Key, pair => int.Parse(((TextBox) pair.Value).Text));
            BuilderDataContainer.PlayerStatus!.ApplySymbolData(newSymbolTable);
        }
        catch (Exception ex)
        {
            // ignored
        }
    }
    #endregion
    
    #region 어빌리티
    private void ApplyAbilityToPlayerInfo()
    {
        if (BuilderDataContainer.PlayerStatus == null) return;
        Dictionary<MapleStatus.StatusType, int> abilityPair = new Dictionary<MapleStatus.StatusType, int>();
        for (int idx = 0; idx < 3; idx++)
            abilityPair.TryAdd(abilityTypes[idx], (int) abilitySliders![idx].Value);
        BuilderDataContainer.PlayerStatus.ApplyAbility(abilityPair);
    }
    
    private void OnSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (syncLock) return;
        
        string name = ((Slider) sender).Name;
        int index = name[^1]-'1';
        if (abilityComboBoxes == null || index < 0 || index >= abilityComboBoxes.Count) return;
        ComboBox targetBox = abilityComboBoxes[index];
        targetBox.Text = MapleAbility.GetAbilityString(abilityTypes[index])
            .Replace("%d", e.NewValue.ToString(CultureInfo.CurrentCulture));
        
        ApplyAbilityToPlayerInfo();
    }
    
    private void ApplyAbility()
    {
        if (abilitySliders == null || abilityComboBoxes == null) return;
        
        MaplePotentialGrade.GradeType topGrade = MaplePotentialGrade.GetPotentialGrade(ctGradeAbility.Text);
        for (int slot = 0; slot < 3; slot++)
        {
            MaplePotentialGrade.GradeType applyType = slot == 0 ? topGrade :
                (bool) abilityCheckboxes[slot]!.IsChecked! ? topGrade - 1 : topGrade - 2;
            
            int[] abV = MapleAbility.GetMinMax(abilityTypes[slot], applyType);
            abilitySliders[slot].Minimum = abV[0];
            abilitySliders[slot].Maximum = abV[1];
            abilitySliders[slot].SmallChange = abV.Length == 2 ? 1 : abV[2];
            abilitySliders[slot].Value = Math.Clamp(abilitySliders[slot].Value, abV[0], abV[1]);
            abilityComboBoxes[slot].Text = MapleAbility.GetAbilityString(abilityTypes[slot])
                .Replace("%d", abilitySliders[slot].Value.ToString(CultureInfo.CurrentCulture));
        }
        
        ApplyAbilityToPlayerInfo();
    }

    private void OnAbilitySelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (syncLock) return;
        
        string name = ((ComboBox) sender).Name;
        int index = name[^1] - '1';
        if (abilitySliders == null || abilityComboBoxes == null || index < 0 || index >= abilitySliders.Count) return;
        object selectedItem = abilityComboBoxes[index].SelectedItem;
        if (selectedItem != null && !selectedItem.ToString()!.Equals(""))
            abilityTypes[index] = displayToAbilityType[abilityComboBoxes[index].SelectedItem.ToString()!];
        ApplyAbility();
    }

    private void OnCheckboxChecked(object sender, RoutedEventArgs e)
    {
        ApplyAbility();
    }

    private void OnCheckboxUnchecked(object sender, RoutedEventArgs e)
    {
        ApplyAbility();
    }

    private void OnAbilityGradeChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplyAbility();
    }
    
    #endregion

    #region 하이퍼스탯

    private void OnHyperStatLevelChanged(object sender, RoutedEventArgs e)
    {
        if (e.Source is not HyperStatSlot slot) return;
        if (BuilderDataContainer.PlayerStatus == null) return;
        BuilderDataContainer.PlayerStatus.ApplyHyperStat(slot.StatType, slot.Delta);
    }

    #endregion
}