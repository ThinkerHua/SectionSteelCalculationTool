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
 *  Form_SSCT.cs: user interface
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SectionSteel;

namespace SectionSteelCalculationTool {
    public partial class Form_SSCT : Form {
        private class CategoryInfo {
            public Type Type;
            public CheckBox LabelCBox;
            public List<CheckBox> ClassifierCBoxes;
        }
        private readonly List<CategoryInfo> categoryCBoxes = new List<CategoryInfo>();

        public Form_SSCT() {
            InitializeComponent();
            LoadCategoryControl();
        }

        private GenerationOption GetGenerationOption(object sender) {
            GenerationOption option = new GenerationOption();
            switch (((Control) sender).Name) {
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

                option.TargetOffset.RowOffset = (int) numUD_AW_Rows.Value;
                option.TargetOffset.ColumnOffset = (int) numUD_AW_Columns.Value;

                option.OverwriteExistingData = cBox_AW_Overwrite.Checked;
                break;
            case "button_STIF_Generate":
                option.GenerationType = GenerationTypeEnum.Stiffener;

                option.TruncatedRounding = cBox_Trunc.Checked;

                option.TargetOffset.RowOffset = (int) numUD_STIF_Rows.Value;
                option.TargetOffset.ColumnOffset = (int) numUD_STIF_Columns.Value;

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
            this.Text = Application.ProductName + " - Ver" + Application.ProductVersion;
        }

        private void Button_AW_Generate_Click(object sender, EventArgs e) {
            var control = sender as Control;
            var text = control.Text;
            control.Text = "请稍候...";
            control.Enabled = false;
            Interaction.Generate(GetGenerationOption(sender));
            control.Enabled = true;
            control.Text = text;
        }

        private void Button_STIF_Generate_Click(object sender, EventArgs e) {
            var control = sender as Control;
            var text = control.Text;
            control.Text = "请稍候...";
            control.Enabled = false;
            Interaction.Generate(GetGenerationOption(sender));
            control.Enabled = true;
            control.Text = text;
        }

        private void UnitArea_CheckedChanged(object sender, EventArgs e) {
            if (rButton_Unit_Area.Checked)
                cBox_ExcludeTopSurface.Enabled = true;
            else
                cBox_ExcludeTopSurface.Enabled = false;
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e) {
            switch (tabControl1.SelectedIndex) {
            case 2:
                this.Size = new System.Drawing.Size(330, 600);
                break;
            default:
                this.Size = new System.Drawing.Size(330, 283);
                break;
            }
        }

        private void CategoryInfoCheckBox_CheckedChanged(object sender, EventArgs e) {
            var cBox = sender as CheckBox;
            var query = categoryCBoxes.Find(item => item.LabelCBox == cBox).ClassifierCBoxes;
            foreach (var item in query) {
                item.Checked = cBox.Checked;
            }
        }

        private void LoadCategoryControl() {
            foreach (var categoryInfo in SectionSteel.SectionSteel.CategoryInfoCollection) {
                //组别
                var labelCBox = new CheckBox {
                    Appearance = Appearance.Button,
                    AutoSize = true,
                    Text = categoryInfo.Label,
                };
                labelCBox.CheckedChanged += CategoryInfoCheckBox_CheckedChanged;
                flowLayoutPanel1.Controls.Add(labelCBox);

                //分组内标识符
                var classifiersCBoxes = new List<CheckBox>();
                foreach (var classifier in categoryInfo.Classifiers) {
                    var classifierCBox = new CheckBox {
                        Appearance = Appearance.Button,
                        AutoSize = true,
                        Text = classifier,
                    };
                    flowLayoutPanel1.Controls.Add(classifierCBox);
                    classifiersCBoxes.Add(classifierCBox);
                }
                flowLayoutPanel1.SetFlowBreak(classifiersCBoxes.Last(), true);

                //分隔线
                var split = new Label {
                    Text = string.Empty,
                    AutoSize = false,
                    BorderStyle = BorderStyle.Fixed3D,
                    Size = new System.Drawing.Size(flowLayoutPanel1.Width - 25, 1),
                };
                flowLayoutPanel1.Controls.Add(split);
                flowLayoutPanel1.SetFlowBreak(split, true);

                //CheckBox集合
                categoryCBoxes.Add(new CategoryInfo {
                    Type = categoryInfo.Type,
                    LabelCBox = labelCBox,
                    ClassifierCBoxes = classifiersCBoxes,
                });
            }
        }

        private void Button_Goto_Click(object sender, EventArgs e) {
            var control = sender as Control;
            var text = control.Text;
            control.Text = "请稍候...";
            control.Enabled = false;

            var filter = new List<(int, int)>();
            for (int i = 0; i < categoryCBoxes.Count; i++) {
                var categoryInfo = categoryCBoxes[i];
                for (int j = 0; j < categoryInfo.ClassifierCBoxes.Count; j++) {
                    var cBox = categoryInfo.ClassifierCBoxes[j];
                    if (cBox.Checked) filter.Add((i, j));
                }
            }
            Interaction.Goto(filter);

            control.Enabled = true;
            control.Text = text;
        }
    }
}
