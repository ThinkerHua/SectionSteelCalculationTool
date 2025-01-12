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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using SectionSteel;
using SectionSteelCalculationTool.ViewModels;

namespace SectionSteelCalculationTool {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        private class ClassificationControls {
            public ToggleButton CategoryTogBtn;
            public List<ToggleButton> ClassifierTogBtns;
        }
        private readonly List<ClassificationControls> classificationTogBtns = new List<ClassificationControls>();

        public MainWindow() {
            InitializeComponent();
            this.Title = Application.ResourceAssembly.GetName().Name +
                " - Ver" + Application.ResourceAssembly.GetName().Version.ToString();
            LoadClassificationControls();
        }

        private void LoadClassificationControls() {
            var dataContext = DataContext as MainWindowViewModel;
            foreach (var category in dataContext.CategoryCollection) {
                //组别
                var border = new Border() {
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(0, 0, 0, 1)
                };
                classificationPanel.Children.Add(border);
                var wrapPanel = new WrapPanel();
                border.Child = wrapPanel;

                var categoryTogBtn = new ToggleButton {
                    Content = category.Category,
                    Padding = new Thickness(2, 1, 2, 1),
                    Margin = new Thickness(3, 3, 9, 3)
                };
                categoryTogBtn.Click += CategoryToggleButton_Click;
                var binding = new Binding {
                    Source = category,
                    Path = new PropertyPath("IsSelected"),
                };
                BindingOperations.SetBinding(categoryTogBtn, ToggleButton.IsCheckedProperty, binding);
                wrapPanel.Children.Add(categoryTogBtn);

                //分组内标识符
                var classificationViewModels = 
                    from item in dataContext.ClassificationCollection
                    where item.Classification.Category == category.Category
                    select item;
                var classifierTogBtns = new List<ToggleButton>();
                foreach (var classificationViewModel in classificationViewModels) {
                    var classifierTogBtn = new ToggleButton {
                        Content = classificationViewModel.Classification.Classifier,
                        Padding = new Thickness(2, 1, 2, 1),
                        Margin = new Thickness(3)
                    };
                    classifierTogBtn.Click += ClassifierToggleButton_Click;
                    binding = new Binding {
                        Source = classificationViewModel,
                        Path = new PropertyPath("IsSelected"),
                    };
                    BindingOperations.SetBinding(classifierTogBtn, ToggleButton.IsCheckedProperty, binding);
                    wrapPanel.Children.Add(classifierTogBtn);
                    classifierTogBtns.Add(classifierTogBtn);
                }

                //toggleButton集合
                classificationTogBtns.Add(new ClassificationControls {
                    CategoryTogBtn = categoryTogBtn,
                    ClassifierTogBtns = classifierTogBtns,
                });
            }
        }

        private void CategoryToggleButton_Click(object sender, RoutedEventArgs e) {
            var categoryTogBtn = sender as ToggleButton;
            var query = classificationTogBtns.Find(item => item.CategoryTogBtn == categoryTogBtn).ClassifierTogBtns;
            foreach (var togBtn in query) {
                togBtn.IsChecked = categoryTogBtn.IsChecked;
            }
        }

        private void ClassifierToggleButton_Click(object sender, RoutedEventArgs e) {
            var classifierTogBtn = sender as ToggleButton;
            var query = classificationTogBtns.Find(item => item.ClassifierTogBtns.Contains(classifierTogBtn));
            foreach (var togBtn in query.ClassifierTogBtns) {
                if (togBtn.IsChecked == false) {
                    query.CategoryTogBtn.IsChecked = false;
                    return;
                }
            }

            query.CategoryTogBtn.IsChecked = true;
        }
    }
}
