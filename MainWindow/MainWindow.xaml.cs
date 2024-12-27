/*==============================================================================
 *  SectionSteelCalculationTool - A tool that assists Excel in calculating 
 *  quantities of steel structures
 *
 *  Copyright © 2024 Huang YongXing.                 
 *
 *  This library is free software, licensed under the terms of the GNU 
 *  General Public License as published by the Free Software Foundation, 
 *  either version 3 of the License, or (at your option) any later version. 
 *  You should have received a copy of the GNU General Public License 
 *  along with this program. If not, see <http://www.gnu.org/licenses/>. 
 *==============================================================================
 *  MainWindow.xaml.cs: code behind for user interface
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using SectionSteel;

namespace SectionSteelCalculationTool {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        private class CategoryInfo {
            public Type Type;
            public ToggleButton LabelTogBtn;
            public List<ToggleButton> ClassifierTogBtns;
        }
        private readonly List<CategoryInfo> categoryTogBtns = new List<CategoryInfo>();

        public MainWindow() {
            InitializeComponent();
            this.Title = Application.ResourceAssembly.GetName().Name +
                " - Ver" + Application.ResourceAssembly.GetName().Version.ToString();
            LoadCategoryControl();
        }

        private void LoadCategoryControl() {
            foreach (var categoryInfo in SectionSteel.SectionSteel.CategoryInfoCollection) {
                //组别
                var border = new Border() {
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(0, 0, 0, 1)
                };
                categoryInfoPanel.Children.Add(border);
                var wrapPanel = new WrapPanel();
                border.Child = wrapPanel;

                var labelTogBtn = new ToggleButton {
                    Content = categoryInfo.Label,
                    Padding = new Thickness(2, 1, 2, 1),
                    Margin = new Thickness(3, 3, 9, 3)
                };
                labelTogBtn.Click += CategoryInfoToggleButton_IsCheckedChanged;
                wrapPanel.Children.Add(labelTogBtn);

                //分组内标识符
                var classifiersTogBtns = new List<ToggleButton>();
                foreach (var classifier in categoryInfo.Classifiers) {
                    var classifierTogBtn = new ToggleButton {
                        Content = classifier,
                        Padding = new Thickness(2, 1, 2, 1),
                        Margin = new Thickness(3)
                    };
                    wrapPanel.Children.Add(classifierTogBtn);
                    classifiersTogBtns.Add(classifierTogBtn);
                }

                //CheckBox集合
                categoryTogBtns.Add(new CategoryInfo {
                    Type = categoryInfo.Type,
                    LabelTogBtn = labelTogBtn,
                    ClassifierTogBtns = classifiersTogBtns,
                });
            }
        }

        private void CategoryInfoToggleButton_IsCheckedChanged(object sender, RoutedEventArgs e) {
            var labelTogBtn = sender as ToggleButton;
            var query = categoryTogBtns.Find(item => item.LabelTogBtn == labelTogBtn).ClassifierTogBtns;
            foreach (var togBtn in query) {
                togBtn.IsChecked = labelTogBtn.IsChecked;
            }
        }

        private void ClearCategoryFilter(object sender, RoutedEventArgs e) {
            foreach (var info in categoryTogBtns) {
                info.LabelTogBtn.IsChecked = false;
                foreach (var tBtn in info.ClassifierTogBtns) {
                    tBtn.IsChecked = false;
                }
            }
        }

        private void GotoSpecified(object sender, RoutedEventArgs e) {
            var control = sender as Button;
            var text = control.Content;
            control.Content = "请稍候...";
            control.IsEnabled = false;

            var filter = new List<(int, int)>();
            for (int i = 0; i < categoryTogBtns.Count; i++) {
                var categoryInfo = categoryTogBtns[i];
                for (int j = 0; j < categoryInfo.ClassifierTogBtns.Count; j++) {
                    var cBox = categoryInfo.ClassifierTogBtns[j];
                    if (cBox.IsChecked == true) filter.Add((i, j));
                }
            }
            Interaction.Goto(filter);

            control.IsEnabled = true;
            control.Content = text;
        }

        private GenerationOption GetGenerationOption(object sender) {
            GenerationOption option = new GenerationOption();
            switch (((Control) sender).Name) {
            case "awGenerate":
                if (rBtnArea.IsChecked == true) {
                    option.GenerationType = GenerationTypeEnum.UnitArea;
                    if (cBoxExcludeTopSurface.IsChecked == true)
                        option.ExcludeTopSurface = true;
                    else
                        option.ExcludeTopSurface = false;
                } else if (rBtnWeight.IsChecked == true) {
                    option.GenerationType = GenerationTypeEnum.UnitWeight;
                }

                if (rBtnRoughly.IsChecked == true)
                    option.Accuracy = FormulaAccuracyEnum.ROUGHLY;
                else if (rBtnPrecisely.IsChecked == true)
                    option.Accuracy = FormulaAccuracyEnum.PRECISELY;
                else if (rBtnGBData.IsChecked == true)
                    option.Accuracy = FormulaAccuracyEnum.GBDATA;

                if (rBtnPIFunc.IsChecked == true)
                    option.PIStyle = PIStyleEnum.FUNC;
                else if (rBtnPINum.IsChecked == true)
                    option.PIStyle = PIStyleEnum.NUM;

                option.TargetOffset.RowOffset = (int) awRowOffset.Value;
                option.TargetOffset.ColumnOffset = (int) awColOffset.Value;

                option.OverwriteExistingData = cBoxAWOverwrite.IsChecked == true;
                break;
            case "stifGenerate":
                option.GenerationType = GenerationTypeEnum.Stiffener;

                option.TruncatedRounding = rBtnRoughly.IsChecked == true;

                option.TargetOffset.RowOffset = (int) stifRowOffset.Value;
                option.TargetOffset.ColumnOffset = (int) stifColOffset.Value;

                option.OverwriteExistingData = cBoxStifOverwrite.IsChecked == true;
                break;
            default:
                break;
            }

            return option;
        }

        private void AreaWeightGenerate(object sender, RoutedEventArgs e) {
            var control = sender as Button;
            var text = control.Content;
            control.Content = "请稍候...";
            control.IsEnabled = false;
            Interaction.Generate(GetGenerationOption(sender));
            control.IsEnabled = true;
            control.Content = text;
        }

        private void StifGenerate(object sender, RoutedEventArgs e) {
            var control = sender as Button;
            var text = control.Content;
            control.Content = "请稍候...";
            control.IsEnabled = false;
            Interaction.Generate(GetGenerationOption(sender));
            control.IsEnabled = true;
            control.Content = text;
        }
    }
}
