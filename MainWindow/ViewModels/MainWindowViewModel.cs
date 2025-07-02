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
using System.Runtime.InteropServices;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Office.Interop.Excel;
using SectionSteel;
using SectionSteelCalculationTool.DotNetRuntime;
using SectionSteelCalculationTool.Models;
using Serilog.Core;
using Excel = Microsoft.Office.Interop.Excel;

namespace SectionSteelCalculationTool.ViewModels {
    public partial class MainWindowViewModel : ObservableObject {

        private readonly Logger logger;

        #region Constructor
        public MainWindowViewModel(Logger logger) {
            this.logger = logger;

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

        private static Excel.Application? GetExcelApplication() {
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

        private static Excel.Range? GetUsefulRange(Excel.Application xlApp) {
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

            if (xlApp.Selection is not Excel.Range xlRange) return null;

            Excel.Range? xlRange_Filtered_1 = null;
            Excel.Range? xlRange_Filtered_2 = null;
            Excel.Range? xlRange_new = null;

            int cnt;
            try {
                cnt = xlRange.Count;
            } catch (COMException) {
                cnt = int.MaxValue;
            }
            if (cnt == 1) {
                if (string.IsNullOrEmpty(xlRange.Value as string))
                    xlRange_new = null;
                else
                    xlRange_new = xlRange;
            } else {
                //只处理可见且具有文本的单元格，文本为公式或常量
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

            var range = GetUsefulRange(xlApp);

            await Task.Run(() => {

                if (range == null) goto Finish;

                string result = string.Empty;
                var sectionSteel = new SectionSteel.SectionSteel {
                    PIStyle = piStyle
                };

                foreach (Excel.Range xlCell in range) {
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

            var xlApp = GetExcelApplication();
            if (xlApp == null) return;

            var range = GetUsefulRange(xlApp);
            var noValidCells = true;

            await Task.Run(() => {
                if (range == null) return;

                Excel.Range? newRange = null;
                foreach (Excel.Range cell in range) {
                    //  如果存在合并单元格，上面 GetUsefulRange 返回的单元格区域可能包含空单元格
                    //  此处跳过以避免程序崩溃
                    if (cell.Value == null) continue;
                    var index = SectionSteel.SectionSteel.GetClassificationIndex((string) cell.Value);
                    if (index == -1) continue;

                    var isSelected = isSelectedCollection[index];
                    if (!isSelected) continue;

                    newRange ??= cell;
                    newRange = xlApp.Union(newRange, cell);
                }

                if (newRange == null) return;
                noValidCells = false;
#if DEBUG
                logger.Information("Range.Address: {0}", newRange.Address);
                logger.Information("Range.Areas.Count: {0}", newRange.Areas.Count);
                var cnt = -1;
                foreach (Excel.Range area in newRange.Areas) {
                    cnt++;
                    logger.Information("Area[{0}].Address: {1}", cnt, area.Address);
                }
#endif
                //当非连续单元格区域过多时，此方法定位不完整
                //xlApp.Goto(newRange);
                newRange.Select();
                SetForegroundWindow(new IntPtr(xlApp.Hwnd));
            });

            if (noValidCells) MessageBox.Show(
                "No valid cells found!",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error,
                MessageBoxResult.OK,
                MessageBoxOptions.DefaultDesktopOnly);
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
