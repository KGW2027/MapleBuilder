using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MapleBuilder.Control.Data;

namespace MapleBuilder.View.SubObjects.Equipment;

public partial class NewItemSelect : UserControl
{
    
    public class NewItemEvent : RoutedEventArgs
    {
        public EquipmentData? ItemData;
        public NewItemEvent(RoutedEvent e, EquipmentData? data) : base(e)
        {
            ItemData = data;
        }
    }

    private readonly (string, string)[] slots = {
        ("Af", "얼굴장식"), ("Ay", "눈장식"), ("Ae", "귀고리"), ("Pe", "펜던트"), ("Be", "벨트"),
        ("Me", "훈장"), ("Sh", "어깨장식"), ("Po", "포켓 아이템"), ("Ba", "뱃지"), ("Si", "엠블렘/보조"),
        ("Tm", "안드로이드"), ("Cp", "모자"), ("Sr", "망토"), ("Ma", "상의"), ("Gv", "장갑"),
        ("MaPn", "한벌옷"), ("Pn", "하의"), ("Ri", "반지"), ("So", "신발"), ("Wp", "무기"), ("WpSi", "두손무기"),
        ("", "전체")
    };
    
    public NewItemSelect()
    {
        InitializeComponent();
        
        WzDatabase.OnWzDataLoadCompleted += OnWzDataLoadCompleted;
        
    }

    private void OnWzDataLoadCompleted(WzDatabase database)
    {
        WzDatabase.OnWzDataLoadCompleted -= OnWzDataLoadCompleted;

        Dispatcher.BeginInvoke(() =>
        {
            ItemList.Children.Clear();
            foreach (var data in database.EquipmentDataList.Values)
            {
                if (int.TryParse(data.Name, out _) || data.Level < 100 || data.IconPath == null || string.IsNullOrEmpty(data.ISlot)) continue;
                if (data.IconPath.Contains("Consume") || data.IconPath.Contains("Install") || !data.IconPath.EndsWith(".png")) continue;
                var slot = new CandidateItemSlot {TargetItem = data};
                slot.MouseDoubleClick += OnAddItem;
                ItemList.Children.Add(slot);
            }
            
            SortingType.Items.Clear();
            foreach (var pair in slots) SortingType.Items.Add(pair.Item2);
            SortingType.SelectedItem = "전체";
        });
    }

    #region XAML Property
    public static readonly RoutedEvent SelectEvent = EventManager.RegisterRoutedEvent(
        nameof(Select), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NewItemSelect));

    public event RoutedEventHandler Select
    {
        add => AddHandler(SelectEvent, value);
        remove => RemoveHandler(SelectEvent, value);
    }
    #endregion

    private void Search(string? text, string? cat)
    {
        text ??= SearchBox.Text;
        cat ??= SortingType.SelectedItem.ToString();
        foreach(var pair in slots)
            if (pair.Item2.Equals(cat))
                cat = pair.Item1;
        
        foreach (var item in ItemList.Children)
        {
            if (item is not CandidateItemSlot slot) continue;
            bool nameCompare = string.IsNullOrEmpty(text) || slot.TargetItem!.Name.Contains(text);
            bool catCompare = string.IsNullOrEmpty(cat) || slot.TargetItem!.ISlot.Equals(cat);
            slot.Visibility = nameCompare && catCompare ? Visibility.Visible : Visibility.Collapsed;
        }
        
    }
    
    private void OnSearchByName(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox) return;

        Search(textBox.Text, null);
    }

    private void OnSearchByCategory(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0 || e.AddedItems[0] == null) return;
        
        Search(null, e.AddedItems[0]!.ToString());
    }

    private void OnAddItem(object sender, MouseButtonEventArgs e)
    {
        if (sender is not CandidateItemSlot slot) return;
        RaiseEvent(new NewItemEvent(SelectEvent, slot.TargetItem!));
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new NewItemEvent(SelectEvent, null));
    }
}