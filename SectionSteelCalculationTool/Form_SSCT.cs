/*==============================================================================
 *  SectionSteelCalculationTool - A tool that assists Excel in calculating 
 *  quantities of steel structures
 *
 *  Copyright © 2022 Huang YongXing.                 
 *
 *  This library is free software, licensed under the terms of the GNU 
 *  General Public License as published by the Free Software Foundation, 
 *  either version 3 of the License, or (at your option) any later version. 
 *  You should have received a copy of the GNU General Public License 
 *  along with this program. If not, see <http://www.gnu.org/licenses/>. 
 *==============================================================================
 *  Form_SSCT.cs: user interface
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;

using SectionSteel;

namespace SectionSteelCalculationTool {
    public partial class Form_SSCT : Form {
        public Form_SSCT() {
            InitializeComponent();
        }

        private void Generate(object sender, EventArgs e) {
            Excel.Application xlApp;
            Excel.Workbook xlWorkbook;
            //Excel.Worksheet xlSheet;
            Excel.Range xlRange;
            Excel.Range xlRange_Filtered_1 = null;
            Excel.Range xlRange_Filtered_2 = null;
            Excel.Range xlRange_new = null;
            //Excel.Range xlCell;
            Excel.Range targetCell;
            GenerationOption option;
            string resault = string.Empty;
            SectionSteel.SectionSteel sectionSteel;

            try {
                //xlApp = (Excel.Application)Interaction.GetObject(null, "Excel.Application");
                xlApp = (Excel.Application)Marshal.GetActiveObject("Excel.Application");
            } catch (COMException) {
                MessageBox.Show("Please open an Excel application first!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            xlWorkbook = xlApp.ActiveWorkbook;
            if (xlWorkbook == null) {
                MessageBox.Show("Please open an Workbook first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            xlRange = xlApp.Selection;
            if (xlRange.Count > 1) {
                //只处理可见单元格且具有文本的单元格，文本为公式或常量
                try {
                    xlRange_Filtered_1 = xlRange.Cells.SpecialCells(XlCellType.xlCellTypeVisible).SpecialCells(XlCellType.xlCellTypeConstants, XlSpecialCellsValue.xlTextValues);
                    xlRange_Filtered_2 = xlRange.Cells.SpecialCells(XlCellType.xlCellTypeVisible).SpecialCells(XlCellType.xlCellTypeFormulas, XlSpecialCellsValue.xlTextValues);
                } catch {

                } finally {
                    if (xlRange_Filtered_1 != null && xlRange_Filtered_2 != null)
                        xlRange_new = xlApp.Union(xlRange_Filtered_1, xlRange_Filtered_2);
                    else if (xlRange_Filtered_1 != null)
                        xlRange_new = xlRange_Filtered_1;
                    else if (xlRange_Filtered_2 != null)
                        xlRange_new = xlRange_Filtered_2;
                }
            } else {
                xlRange_new = xlRange;
            }
            if (xlRange_new == null) return;

            option = GatheringInformation(sender);
            sectionSteel = new SectionSteel.SectionSteel {
                PIStyle = option.PIStyle
            };

            xlApp.ScreenUpdating = false;
            foreach (Range xlCell in xlRange_new) {
                targetCell = xlCell.Offset[option.TargetOffset.RowOffset, option.TargetOffset.ColumnOffset];
                if (!option.OverwriteExistingData && targetCell.Value != null)
                    continue;

                sectionSteel.ProfileText = xlCell.Value;
                switch (option.GenerationType) {
                case GenerationTypeEnum.UnitArea:
                    resault = sectionSteel.GetAreaFormula(option.Accuracy, option.ExcludeTopSurface);
                    if (resault != string.Empty)
                        resault = "=" + resault;
                    break;
                case GenerationTypeEnum.UnitWeight:
                    resault = sectionSteel.GetWeightFormula(option.Accuracy);
                    if (resault != string.Empty)
                        resault = "=" + resault;
                    break;
                case GenerationTypeEnum.Stiffener:
                    resault = sectionSteel.GetSiffenerProfileStr(option.TruncatedRounding);
                    break;
                default:
                    break;
                }

                //即使resault == string.Empty，也应对目标单元格赋值
                targetCell.Value = resault;
            }

            xlApp.ScreenUpdating = true;
        }
        private GenerationOption GatheringInformation(object sender) {
            GenerationOption option = new GenerationOption();
            switch (((Control)sender).Name) {
            case "button_AW_Generate":
                if (rButton_Unit_Area.Checked) {
                    option.GenerationType = GenerationTypeEnum.UnitArea;
                    if (cBox_ExcludeTopSurface.Checked)
                        option.ExcludeTopSurface = true;
                    else
                        option.ExcludeTopSurface = false;
                } else if (rButton_Unit_Weight.Checked) {
                    option.GenerationType = GenerationTypeEnum.UnitWeight;
                }

                if (rButton_Roughly.Checked)
                    option.Accuracy = FormulaAccuracyEnum.ROUGHLY;
                else if (rButton_Precisely.Checked)
                    option.Accuracy = FormulaAccuracyEnum.PRECISELY;
                else if (rButton_GB_Data.Checked)
                    option.Accuracy = FormulaAccuracyEnum.GBDATA;

                if (rButton_PI_FUNC.Checked)
                    option.PIStyle = PIStyleEnum.FUNC;
                else if (rButton_PI_NUM.Checked)
                    option.PIStyle = PIStyleEnum.NUM;

                option.TargetOffset.RowOffset = (int)numUD_AW_Rows.Value;
                option.TargetOffset.ColumnOffset = (int)numUD_AW_Columns.Value;

                option.OverwriteExistingData = cBox_AW_Overwrite.Checked;
                break;
            case "button_STIF_Generate":
                option.GenerationType = GenerationTypeEnum.Stiffener;

                option.TruncatedRounding = cBox_Trunc.Checked;

                option.TargetOffset.RowOffset = (int)numUD_STIF_Rows.Value;
                option.TargetOffset.ColumnOffset = (int)numUD_STIF_Columns.Value;

                option.OverwriteExistingData = cBox_STIF_Overwrite.Checked;
                break;
            default:
                break;
            }

            return option;
        }

        private void CBox_AlwaysOnTop_CheckedChanged(object sender, EventArgs e) {
            switch (cBox_AlwaysOnTop.Checked) {
            case true:
                this.TopMost = true;
                break;
            case false:
                this.TopMost = false;
                break;
            default:
                break;
            }
        }

        private void Form_SSCT_Load(object sender, EventArgs e) {
            this.Text = System.Windows.Forms.Application.ProductName + " - Ver" + System.Windows.Forms.Application.ProductVersion;
        }

        private void Button_AW_Generate_Click(object sender, EventArgs e) {
            this.button_AW_Generate.Enabled = false;
            Generate(sender, e);
            this.button_AW_Generate.Enabled = true;
        }

        private void Button_STIF_Generate_Click(object sender, EventArgs e) {
            this.button_STIF_Generate.Enabled = false;
            Generate(sender, e);
            this.button_STIF_Generate.Enabled = true;
        }
    }
    public class Offset<T> {
        public T RowOffset { get; set; }
        public T ColumnOffset { get; set; }
    }
    public enum GenerationTypeEnum {
        UnitArea,
        UnitWeight,
        Stiffener,
    }
    public class GenerationOption {
        public GenerationTypeEnum GenerationType { get; set; }
        public FormulaAccuracyEnum Accuracy { get; set; }
        public PIStyleEnum PIStyle { get; set; }
        public bool ExcludeTopSurface { get; set; }
        public bool TruncatedRounding { get; set; }
        public Offset<int> TargetOffset { get; set; }
        public bool OverwriteExistingData { get; set; }
        public GenerationOption() {
            TargetOffset = new Offset<int>();
        }
    }
}
