﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using MapleAPI.Enum;
using MapleBuilder.Control;
using MapleBuilder.Control.Data;

namespace MapleBuilder.View.SubObjects;

public partial class UnionRaiderMap : UserControl
{
    private static readonly SolidColorBrush STROKE_BRUSH = new(Color.FromRgb(0x44, 0x44, 0x44));
    private static readonly SolidColorBrush BORDER_BRUSH = new(Colors.White);
    private static readonly SolidColorBrush CLAIM_BRUSH = new(Color.FromArgb(0xF0, 170, 136, 102));
    private static Stopwatch _doubleClickTester = new();
    
    private readonly UIElement?[,] claims;
    private readonly List<ComboBox> innerSelections;
    private readonly List<Label> displaysInner;
    private bool lockCombobox;

    
    public UnionRaiderMap()
    {
        claims = new UIElement?[20, 22];
        innerSelections = new List<ComboBox>();
            
        InitializeComponent();
        // 유니온 블럭 X,Y 선
        DrawBlockBorderLines();
        // 영역 분리선
        DrawUnionAreaBorderLines();
        
        displaysInner = new List<Label>
        {
            ctSelectOpt1, ctSelectOpt2, ctSelectOpt3, ctSelectOpt4,
            ctSelectOpt5, ctSelectOpt6, ctSelectOpt7, ctSelectOpt8,
        };
        // 유니온 스텟 선택
        DrawUnionInnerSelection();
        
        GlobalDataController.OnDataUpdated += OnDataUpdated;
    }

    private void OnDataUpdated(PlayerData pdata)
    {
        GlobalDataController.OnDataUpdated -= OnDataUpdated;

        Dispatcher.BeginInvoke(() =>
        {
            // Set Inner Stats
            lockCombobox = true;
            var innerTypes = GlobalDataController.Instance.PlayerInstance!.UnionClaim.InnerStatus;
            for (int idx = 0; idx < 8; idx++)
            {
                string label = innerTypes[idx] switch
                {
                    MapleStatus.StatusType.STR => "STR",
                    MapleStatus.StatusType.DEX => "DEX",
                    MapleStatus.StatusType.INT => "INT",
                    MapleStatus.StatusType.LUK => "LUK",
                    MapleStatus.StatusType.HP => "HP",
                    MapleStatus.StatusType.MP => "MP",
                    MapleStatus.StatusType.ATTACK_POWER => "공격력",
                    MapleStatus.StatusType.MAGIC_POWER => "마력",
                    _ => throw new Exception($"Expected StatusType from UnionRaider sync sequence. {innerTypes[idx]}")
                };

                innerSelections[idx].Text = label;
                displaysInner[idx].Content = label;
            }
            lockCombobox = false;
            
            var poses = pdata.UnionClaim.InitClaimPoses;
            for(int idx = 0 ; idx < poses.GetLength(0) ; idx++)
                SafeToggleClaimBlock(poses[idx, 0], poses[idx, 1]);
        });
    }
    
    #region Draw

    private void DrawBlockBorderLines()
    {
        for (int lineIndex = 1; lineIndex <= 21; lineIndex++)
        {
            double x = ctArealines.Width * (lineIndex / 22.0), y = ctArealines.Height * (lineIndex / 20.0);
            Polyline horizLine = new Polyline
            {
                Points = new PointCollection
                {
                    new(x, 1), new(x, ctArealines.Height-1)
                },
                Stroke = STROKE_BRUSH,
                StrokeThickness = 1
            };
            ctArealines.Children.Add(horizLine);
            if (lineIndex > 19) continue;
            Polyline vertLine = new Polyline
            {
                Points = new PointCollection
                {
                    new(1, y), new(ctArealines.Width-1, y)
                },
                Stroke = STROKE_BRUSH,
                StrokeThickness = 1
            };
            ctArealines.Children.Add(vertLine);
        }
    }

    private void DrawUnionAreaBorderLines()
    {
        double xUnit = ctArealines.Width / 22.0, yUnit = ctArealines.Height / 20.0;
        var points = new PointCollection
        {
            new(xUnit*1,       1),
            new(xUnit*1, yUnit*1), new(xUnit*2, yUnit*1),
            new(xUnit*2, yUnit*2), new(xUnit*3, yUnit*2),
            new(xUnit*3, yUnit*3), new(xUnit*4, yUnit*3),
            new(xUnit*4, yUnit*4), new(xUnit*5, yUnit*4),
            new(xUnit*5, yUnit*5), new(xUnit*6, yUnit*5),
            new(xUnit*6, yUnit*6), new(xUnit*7, yUnit*6),
            new(xUnit*7, yUnit*7), new(xUnit*8, yUnit*7),
            new(xUnit*8, yUnit*8), new(xUnit*9, yUnit*8),
            new(xUnit*9, yUnit*9), new(xUnit*10, yUnit*9),
            new(xUnit*10, yUnit*10), new(xUnit*11-4, yUnit*10), new(6, yUnit*10),
            new(xUnit*5, yUnit*10), new(xUnit*5, yUnit*5), 
            new(xUnit*11, yUnit*5), new(xUnit*11, 6), new(xUnit*11, yUnit*10)
            
        };

        for (int flip = 0; flip < 4; flip++)
        {
            int flipX = flip < 2 ? 1 : -1;
            int flipY = flip % 2 == 1 ? 1 : -1;
            ScaleTransform flipTransform = new ScaleTransform(flipX, flipY, xUnit*11, yUnit*10);
            
            Polyline polyLine = new Polyline
            {
                Points = points,
                Stroke = BORDER_BRUSH,
                StrokeThickness = 1,
                RenderTransform = flipTransform
            };

            ctArealines.Children.Add(polyLine);
        }

    }

    #endregion

    #region Union Blocks
    
    private void SafeToggleClaimBlock(int x, int y)
    {
        bool visible = claims[y, x] == null || claims[y, x]!.Visibility == Visibility.Collapsed;
        SafeToggleClaimBlock(x, y, visible);
    }

    private List<MapleStatus.StatusType> GetInnerStatus()
    {
        return displaysInner.Select(label => label.Content switch
            {
                "STR" => MapleStatus.StatusType.STR,
                "DEX" => MapleStatus.StatusType.DEX,
                "INT" => MapleStatus.StatusType.INT,
                "LUK" => MapleStatus.StatusType.LUK,
                "HP" => MapleStatus.StatusType.HP,
                "MP" => MapleStatus.StatusType.MP,
                "공격력" => MapleStatus.StatusType.ATTACK_POWER,
                "마력" => MapleStatus.StatusType.MAGIC_POWER,
                _ => MapleStatus.StatusType.OTHER
            })
            .ToList();
    }

    private void SafeToggleClaimBlock(int x, int y, bool beVisible)
    {
        bool isNull = claims[y, x] == null;
        if (isNull)
        {
            double unit = ctArealines.Width / 22.0;
            claims[y, x] = new Border
            {
                Margin = new Thickness(x * unit, y * unit, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Background = CLAIM_BRUSH,
                BorderBrush = BORDER_BRUSH,
                BorderThickness = new Thickness(0.1),
                Visibility = Visibility.Collapsed,
                Width = unit,
                Height = unit
            };
            ctUnionBlocks.Children.Add(claims[y, x]!);
        }

        var slotClaimType = MapleUnion.GetOptionBySlot(x-11, y+10);
        int delta = beVisible ? 1 : -1;
        claims[y, x]!.Visibility = beVisible ? Visibility.Visible : Visibility.Collapsed;

        if (GlobalDataController.Instance.PlayerInstance == null) return;
        GlobalDataController.Instance.PlayerInstance.UnionClaim[slotClaimType] += delta;
    }

    private void SafeToggleClaimBlockBulk(int x, int y, bool beVisible, MapleUnion.ClaimType targetType)
    {
        Visibility targetVisibility = beVisible ? Visibility.Visible : Visibility.Collapsed;
        Queue<int[]> queue = new Queue<int[]>();
        queue.Enqueue(new[]{x, y});

        bool[,] tested = new bool[claims.GetLength(0), claims.GetLength(1)];

        while (queue.TryDequeue(out int[]? poll))
        {
            int tx = poll[0], ty = poll[1];
            
            // Validate
            if (tx < 0 || tx >= claims.GetLength(1) || ty < 0 || ty >= claims.GetLength(0)) continue;
            if (tested[ty, tx]) continue;
            tested[ty, tx] = true;
            
            // Check Whitelist
            if (claims[ty, tx] != null && claims[ty, tx]!.Visibility == targetVisibility) continue;
            MapleUnion.ClaimType pollType = MapleUnion.GetOptionBySlot(tx - 11, ty + 10);
            if (pollType != targetType) continue;
            
            // Toggle Block
            SafeToggleClaimBlock(tx, ty, beVisible);
            
            queue.Enqueue(new[]{ tx - 1, ty });
            queue.Enqueue(new[]{ tx + 1, ty });
            queue.Enqueue(new[]{ tx, ty + 1 });
            queue.Enqueue(new[]{ tx, ty - 1 });
        }
    }
    
    private void OnUnionAreaClicked(object sender, MouseButtonEventArgs e)
    {
        Point mousePos = e.GetPosition((UIElement) sender);

        double gap = ctArealines.Width / 22.0;
        int x = (int) Math.Floor(mousePos.X / gap);
        int y = (int) Math.Floor(mousePos.Y / gap);
        
        _doubleClickTester.Stop();
        var ms = _doubleClickTester.ElapsedMilliseconds;
        if (ms is > 50 and < 150)
        {
            // Double Click Action
            bool beVisible = !(claims[y, x] == null || claims[y, x]!.Visibility == Visibility.Collapsed);
            SafeToggleClaimBlock(x, y, !beVisible);
            MapleUnion.ClaimType claimType = MapleUnion.GetOptionBySlot(x-11, y+10);
            SafeToggleClaimBlockBulk(x, y, beVisible, claimType);
            
            _doubleClickTester.Reset();
            return;
        }
        _doubleClickTester = Stopwatch.StartNew();
        
        // One Click Action
        SafeToggleClaimBlock(x, y);
    }
    
    #endregion
    
    #region Union Inner Selection

    private void DrawUnionInnerSelection()
    {
        List<string> items = new List<string>
        {
            "STR", "DEX", "INT", "LUK", "HP", "MP", "공격력", "마력"
        };

        for (int idx = 0; idx < 8; idx++)
        {
            Label label = new Label
            {
                Content = $"선택 스텟 {idx + 1}",
                Margin = new Thickness(532, 4 + 44 * idx, 0, 0),
                FontSize = 12,
                FontWeight = FontWeights.Thin
            };

            ComboBox comboBox = new ComboBox
            {
                Text = items[idx],
                Width = 56,
                Height = 24,
                Margin = new Thickness(536, 26 + 44 * idx, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Items = { "STR", "DEX", "INT", "LUK", "HP", "MP", "공격력", "마력" },
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Padding = new Thickness(0, 0, 4, 0),
            };
            comboBox.SelectionChanged += OnInnerSelectionChanged;

            displaysInner[idx].Content = items[idx];

            ctMainGrid.Children.Add(label);
            ctMainGrid.Children.Add(comboBox);
            innerSelections.Add(comboBox);
        }
    }

    private void OnInnerSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lockCombobox || e.AddedItems.Count == 0) return;
        
        
        ComboBox comboBox = (ComboBox) sender;
        int selfIdx = -1, targetIdx = -1;
        string target = e.AddedItems[0]!.ToString()!;
        for (int idx = 0; idx < innerSelections.Count; idx++)
        {
            if (comboBox == innerSelections[idx])
            {
                selfIdx = idx;
            }
            else if (innerSelections[idx].Text == target)
            {
                targetIdx = idx;
            }
        }

        if (selfIdx == -1 || targetIdx == -1) return;

        lockCombobox = true;
        innerSelections[targetIdx].Text = comboBox.Text;
        displaysInner[targetIdx].Content = comboBox.Text;

        innerSelections[selfIdx].Text = target;
        displaysInner[selfIdx].Content = target;
        lockCombobox = false;
        
        if (GlobalDataController.Instance.PlayerInstance == null) return;
        GlobalDataController.Instance.PlayerInstance.UnionClaim.SwapInners(targetIdx, selfIdx);
    }

    #endregion
}