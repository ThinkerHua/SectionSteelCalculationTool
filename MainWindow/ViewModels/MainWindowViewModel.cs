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
 *  MainWindowViewModel.cs: 主视图模型
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Office.Interop.Excel;
using SectionSteel;
using SectionSteelCalculationTool.DotNetRuntime;
using SectionSteelCalculationTool.Models;
using Excel = Microsoft.Office.Interop.Excel;

namespace SectionSteelCalculationTool.ViewModels {
    public partial class MainWindowViewModel : ObservableObject {
        #region Constructor
        public MainWindowViewModel() {
            Accuracy = FormulaAccuracyEnum.GBDATA;
            TruncatedRounding = true;
            TargetOffset = new Offset(0, 1);

            var query = SectionSteel.SectionSteel.ClassificationCollection.Select(item => item.Category).Distinct();
            CategoryCollection = new List<CategoryViewModel>();
            foreach (var item in query) {
                CategoryCollection.Add(new CategoryViewModel { Category = item, IsSelected = false });
            }

            ClassificationCollection = new List<ClassificationViewModel>();
            foreach (var item in SectionSteel.SectionSteel.ClassificationCollection) {
                ClassificationCollection.Add(new ClassificationViewModel {
                    Classification = item,
                    IsSelected = false
                });
            }
        }

        #endregion Constructor

        #region Properties
        [ObservableProperty]
        private GenerationTypeEnum _generationType;

        [ObservableProperty]
        private FormulaAccuracyEnum _accuracy;

        [ObservableProperty]
        private PIStyleEnum _piStyle;

        [ObservableProperty]
        private bool _excludeTopSurface;

        [ObservableProperty]
        private bool _truncatedRounding;

        [ObservableProperty]
        private Offset _targetOffset;

        [ObservableProperty]
        private bool _overwrite;

        [ObservableProperty]
        private List<CategoryViewModel> _categoryCollection;

        [ObservableProperty]
        private List<ClassificationViewModel> _classificationCollection;

        #endregion Properties

        #region Methods
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private static Excel.Application GetExcelApplication() {
            try {
                return (Excel.Application) MockMarshal.GetActiveObject("Excel.Application");
            } catch (COMException) {
                MessageBox.Show(
                    "Please open an Excel application first!",
                    "Warning!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning,
                    MessageBoxResult.OK,
                    MessageBoxOptions.DefaultDesktopOnly);
                return null;
            }
        }

        private static Range GetUsefulRange(Excel.Application xlApp) {
            var xlWorkbook = xlApp.ActiveWorkbook;
            if (xlWorkbook == null) {
                MessageBox.Show(
                    "Please open an Workbook first!",
                    "Warning",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning,
                    MessageBoxResult.OK,
                    MessageBoxOptions.DefaultDesktopOnly);
                return null;
            }

            if (!(xlApp.Selection is Range)) return null;

            Range xlRange_Filtered_1 = null;
            Range xlRange_Filtered_2 = null;
            Range xlRange_new = null;
            Range xlRange = xlApp.Selection as Range;
            if (xlRange.Count == 1) {
                if (string.IsNullOrEmpty(xlRange.Value as string))
                    xlRange_new = null;
                else
                    xlRange_new = xlRange;
            } else {
                //只处理可见单元格且具有文本的单元格，文本为公式或常量
                try {
                    xlRange_Filtered_1 =
                        xlRange.Cells
                        .SpecialCells(XlCellType.xlCellTypeVisible)
                        .SpecialCells(XlCellType.xlCellTypeConstants, XlSpecialCellsValue.xlTextValues);
                    xlRange_Filtered_2 =
                        xlRange.Cells
                        .SpecialCells(XlCellType.xlCellTypeVisible)
                        .SpecialCells(XlCellType.xlCellTypeFormulas, XlSpecialCellsValue.xlTextValues);
                } catch { } finally {
                    if (xlRange_Filtered_1 != null && xlRange_Filtered_2 != null)
                        xlRange_new = xlApp.Union(xlRange_Filtered_1, xlRange_Filtered_2);
                    else if (xlRange_Filtered_1 != null)
                        xlRange_new = xlRange_Filtered_1;
                    else if (xlRange_Filtered_2 != null)
                        xlRange_new = xlRange_Filtered_2;
                    else
                        xlRange_new = null;
                }
            }

            return xlRange_new;
        }

        [RelayCommand]
        public async Task GenerateAsync() {
            //  异步方法，避免执行过程中参数被修改
            var generationType = GenerationType;
            var accuracy = Accuracy;
            var piStyle = PiStyle;
            var excludeTopSurface = ExcludeTopSurface;
            var truncatedRounding = TruncatedRounding;
            var targetOffset = new Offset(TargetOffset);
            var overwrite = Overwrite;

            var xlApp = GetExcelApplication();
            if (xlApp == null) return;

            xlApp.ScreenUpdating = false;
            await Task.Run(() => {

                var range = GetUsefulRange(xlApp);
                if (range == null) goto Finish;

                string result = string.Empty;
                var sectionSteel = new SectionSteel.SectionSteel {
                    PIStyle = piStyle
                };

                foreach (Range xlCell in range) {
                    var targetCell = xlCell.Offset[targetOffset.RowOffset, targetOffset.ColumnOffset];
                    if (!overwrite && targetCell.Value != null)
                        continue;

                    try {
                        sectionSteel.ProfileText = xlCell.Value as string;
                    } catch (MismatchedProfileTextException) {
                        continue;
                    }

                    switch (generationType) {
                    case GenerationTypeEnum.UnitArea:
                        result = sectionSteel.GetAreaFormula(accuracy, excludeTopSurface);
                        if (result != string.Empty)
                            result = "=" + result;
                        break;
                    case GenerationTypeEnum.UnitWeight:
                        result = sectionSteel.GetWeightFormula(accuracy);
                        if (result != string.Empty)
                            result = "=" + result;
                        break;
                    case GenerationTypeEnum.Stiffener:
                        result = sectionSteel.GetSiffenerProfileStr(truncatedRounding);
                        break;
                    default:
                        break;
                    }

                    targetCell.Formula = result;
                }

            Finish:
                xlApp.ScreenUpdating = true;
            });
        }

        [RelayCommand]
        public async Task Goto() {
            //  异步方法，避免执行过程中参数被修改
            var isSelectedCollection = ClassificationCollection.Select(item => item.IsSelected).ToArray();

            await Task.Run(() => {
                var xlApp = GetExcelApplication();
                if (xlApp == null) return;

                var range = GetUsefulRange(xlApp);
                if (range == null) goto NoValidCells;

                Range newRange = null;
                foreach (Range cell in range) {
                    var index = SectionSteel.SectionSteel.GetClassificationIndex((string) cell.Value);
                    if (index == -1) continue;

                    var isSelected = isSelectedCollection[index];
                    if (!isSelected) continue;

                    newRange ??= cell;
                    newRange = xlApp.Union(newRange, cell);
                }

                if (newRange != null) {
                    xlApp.Goto(newRange);
                    SetForegroundWindow(new IntPtr(xlApp.Hwnd));
                    return;
                }

            NoValidCells:
                MessageBox.Show(
                    "No valid cells found!",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK,
                    MessageBoxOptions.DefaultDesktopOnly);
            });
        }

        [RelayCommand]
        public void ClearFilter(object parameter) {
            foreach (var item in CategoryCollection) {
                item.IsSelected = false;
            }

            foreach (var item in ClassificationCollection) {
                item.IsSelected = false;
            }
        }
        #endregion Methods
    }
}
