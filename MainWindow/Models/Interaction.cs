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
 *  Interaction.cs: 与Excel交互
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.Office.Interop.Excel;
using SectionSteel;
using Excel = Microsoft.Office.Interop.Excel;

namespace SectionSteelCalculationTool {
    public class Interaction {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        private static Excel.Application GetExcelApplication() {
            try {
                //xlApp = (Excel.Application)Interaction.GetObject(null, "Excel.Application");
                return (Excel.Application) Marshal.GetActiveObject("Excel.Application");
            } catch (COMException) {
                MessageBox.Show("Please open an Excel application first!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
        }
        private static Range GetUsefulRange(Excel.Application xlApp) {
            var xlWorkbook = xlApp.ActiveWorkbook;
            if (xlWorkbook == null) {
                MessageBox.Show("Please open an Workbook first!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            if (!(xlApp.Selection is Range)) return null;

            Range xlRange_Filtered_1 = null;
            Range xlRange_Filtered_2 = null;
            Range xlRange_new = null;
            Range xlRange = xlApp.Selection;
            if (xlRange.Count == 1) {
                if (xlRange.Value == null || xlRange.Value == string.Empty)
                    xlRange_new = null;
                else
                    xlRange_new = xlRange;
            } else {
                //只处理可见单元格且具有文本的单元格，文本为公式或常量
                try {
                    xlRange_Filtered_1 = xlRange.Cells.SpecialCells(XlCellType.xlCellTypeVisible).SpecialCells(XlCellType.xlCellTypeConstants, XlSpecialCellsValue.xlTextValues);
                    xlRange_Filtered_2 = xlRange.Cells.SpecialCells(XlCellType.xlCellTypeVisible).SpecialCells(XlCellType.xlCellTypeFormulas, XlSpecialCellsValue.xlTextValues);
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
        public static void Generate(GenerationOption option) {
            if (option is null) {
                throw new ArgumentNullException(nameof(option));
            }

            var xlApp = GetExcelApplication();
            if (xlApp == null) return;

            xlApp.ScreenUpdating = false;
            var range = GetUsefulRange(xlApp);
            if (range == null) return;

            string result = string.Empty;
            var sectionSteel = new SectionSteel.SectionSteel {
                PIStyle = option.PIStyle
            };

            foreach (Range xlCell in range) {
                var targetCell = xlCell.Offset[option.TargetOffset.RowOffset, option.TargetOffset.ColumnOffset];
                if (!option.OverwriteExistingData && targetCell.Value != null)
                    continue;

                try {
                    sectionSteel.ProfileText = xlCell.Value;
                } catch (MismatchedProfileTextException) {
                    continue;
                }
                switch (option.GenerationType) {
                case GenerationTypeEnum.UnitArea:
                    result = sectionSteel.GetAreaFormula(option.Accuracy, option.ExcludeTopSurface);
                    if (result != string.Empty)
                        result = "=" + result;
                    break;
                case GenerationTypeEnum.UnitWeight:
                    result = sectionSteel.GetWeightFormula(option.Accuracy);
                    if (result != string.Empty)
                        result = "=" + result;
                    break;
                case GenerationTypeEnum.Stiffener:
                    result = sectionSteel.GetSiffenerProfileStr(option.TruncatedRounding);
                    break;
                default:
                    break;
                }

                targetCell.Value = result;
            }

            xlApp.ScreenUpdating = true;
        }
        public static void Goto(List<(int CategoryIndex, int ClassifierIndex)> filter) {
            if (filter is null) {
                throw new ArgumentNullException(nameof(filter));
            }

            if (filter.Count == 0) return;

            var xlApp = GetExcelApplication();
            if (xlApp == null) return;

            var range = GetUsefulRange(xlApp);
            if (range == null) return;

            Range newRange = null;
            foreach (Range cell in range) {
                (Type Type, string Classifier) categoryInfo = SectionSteel.SectionSteel.GetCategoryInfo(cell.Value);
                if (categoryInfo.Equals(default)) continue;

                var categoryIndex = Array.FindIndex(
                    SectionSteel.SectionSteel.CategoryInfoCollection, info => info.Type == categoryInfo.Type);
                if (categoryIndex == -1) continue;

                var classifierIndex = Array.FindIndex(
                    SectionSteel.SectionSteel.CategoryInfoCollection[categoryIndex].Classifiers, classifier => classifier == categoryInfo.Classifier);

                var index = filter.FindIndex(item => item.CategoryIndex == categoryIndex && item.ClassifierIndex == classifierIndex);
                if (index == -1) continue;

                if (newRange == null) newRange = cell;
                newRange = xlApp.Union(newRange, cell);
            }

            if (newRange == null) {
                MessageBox.Show("No valid cells found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            } else{
                xlApp.Goto(newRange);
                SetForegroundWindow(new IntPtr(xlApp.Hwnd));
            }

        }
    }
}
