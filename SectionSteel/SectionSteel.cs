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
 *  SectionSteel.cs: 型钢总类，实际应用中使用此类即可
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SectionSteel {
    /// <summary>
    /// 型钢总类，实际应用中使用此类即可。根据属性 <b>ProfileText</b> 自动识别具体应用哪一项子类，因此本类依赖于各子类：<br/>
    /// <see cref="SectionSteel_H"/><br/>
    /// <see cref="SectionSteel_HH"/><br/>
    /// <see cref="SectionSteel_T"/><br/>
    /// <see cref="SectionSteel_I"/><br/>
    /// <see cref="SectionSteel_CHAN"/><br/>
    /// <see cref="SectionSteel_CHAN_MtM"/><br/>
    /// <see cref="SectionSteel_CHAN_BtB"/><br/>
    /// <see cref="SectionSteel_L"/><br/>
    /// <see cref="SectionSteel_L_BtB"/><br/>
    /// <see cref="SectionSteel_CFH_J"/><br/>
    /// <see cref="SectionSteel_RECT"/><br/>
    /// <see cref="SectionSteel_CFH_Y"/><br/>
    /// <see cref="SectionSteel_CIRC"/><br/>
    /// <see cref="SectionSteel_CFO_CN"/><br/>
    /// <see cref="SectionSteel_CFO_CN_MtM"/><br/>
    /// <see cref="SectionSteel_CFO_CN_BtB"/><br/>
    /// <see cref="SectionSteel_CFO_ZJ"/><br/>
    /// <see cref="SectionSteel_PL"/><br/>
    /// <see cref="SectionSteel_PL_Triangle"/><br/>
    /// <see cref="SectionSteel_PL_Circular"/><br/>
    /// <see cref="SectionSteel_PL_Composite"/><br/>
    /// <see cref="SectionSteel_SPHERE"/><br/>
    /// </summary>
    public class SectionSteel : SectionSteelBase {
        private PIStyleEnum _PIStyle;
        private SectionSteelBase realSectionSteel;

        /// <summary>
        /// 型钢分类信息集合。
        /// </summary>
        public static readonly SectionSteelClassification[] ClassificationCollection = new SectionSteelClassification[] {
            new SectionSteelClassification (typeof(SectionSteel_H), "H型钢", "H"),
            new SectionSteelClassification (typeof(SectionSteel_H), "H型钢", "HW"),
            new SectionSteelClassification (typeof(SectionSteel_H), "H型钢", "HM"),
            new SectionSteelClassification (typeof(SectionSteel_H), "H型钢", "HN"),
            new SectionSteelClassification (typeof(SectionSteel_H), "H型钢", "HT"),
            new SectionSteelClassification (typeof(SectionSteel_H), "H型钢", "BH"),
            new SectionSteelClassification (typeof(SectionSteel_H), "H型钢", "HI"),
            new SectionSteelClassification (typeof(SectionSteel_H), "H型钢", "HP"),
            new SectionSteelClassification (typeof(SectionSteel_H), "H型钢", "PHI"),
            new SectionSteelClassification (typeof(SectionSteel_H), "H型钢", "WH"),
            new SectionSteelClassification (typeof(SectionSteel_H), "H型钢", "WI"),
            new SectionSteelClassification (typeof(SectionSteel_H), "H型钢", "B_WLD_A"),
            new SectionSteelClassification (typeof(SectionSteel_H), "H型钢", "B_WLD_H"),
            new SectionSteelClassification (typeof(SectionSteel_H), "H型钢", "B_WLD_K"),
            new SectionSteelClassification (typeof(SectionSteel_H), "H型钢", "I_VAR_A"),
            new SectionSteelClassification (typeof(SectionSteel_HH), "十字拼接H型钢", "HH"),
            new SectionSteelClassification (typeof(SectionSteel_HH), "十字拼接H型钢", "B_WLD_O"),
            new SectionSteelClassification (typeof(SectionSteel_T), "T型钢", "T"),
            new SectionSteelClassification (typeof(SectionSteel_T), "T型钢", "TW"),
            new SectionSteelClassification (typeof(SectionSteel_T), "T型钢", "TM"),
            new SectionSteelClassification (typeof(SectionSteel_T), "T型钢", "TN"),
            new SectionSteelClassification (typeof(SectionSteel_T), "T型钢", "B_WLD_E"),
            new SectionSteelClassification (typeof(SectionSteel_I), "工字钢", "I"),
            new SectionSteelClassification (typeof(SectionSteel_CHAN), "槽钢", "["),
            new SectionSteelClassification (typeof(SectionSteel_CHAN), "槽钢", "C"),
            new SectionSteelClassification (typeof(SectionSteel_CHAN_MtM), "双拼槽钢(口对口)", "[]"),
            new SectionSteelClassification (typeof(SectionSteel_CHAN_BtB), "双拼槽钢(背对背)", "]["),
            new SectionSteelClassification (typeof(SectionSteel_CHAN_BtB), "双拼槽钢(背对背)", "2["),
            new SectionSteelClassification (typeof(SectionSteel_CHAN_BtB), "双拼槽钢(背对背)", "2C"),
            new SectionSteelClassification (typeof(SectionSteel_L), "角钢", "∠"),
            new SectionSteelClassification (typeof(SectionSteel_L), "角钢", "L"),
            new SectionSteelClassification (typeof(SectionSteel_L_BtB), "双拼角钢", "2∠"),
            new SectionSteelClassification (typeof(SectionSteel_L_BtB), "双拼角钢", "2L"),
            new SectionSteelClassification (typeof(SectionSteel_CFH_J), "冷弯矩管", "J"),
            new SectionSteelClassification (typeof(SectionSteel_CFH_J), "冷弯矩管", "F"),
            new SectionSteelClassification (typeof(SectionSteel_CFH_J), "冷弯矩管", "P"),
            new SectionSteelClassification (typeof(SectionSteel_CFH_J), "冷弯矩管", "TUB"),
            new SectionSteelClassification (typeof(SectionSteel_CFH_J), "冷弯矩管", "CFRHS"),
            new SectionSteelClassification (typeof(SectionSteel_CFH_J), "冷弯矩管", "RHS"),
            new SectionSteelClassification (typeof(SectionSteel_CFH_J), "冷弯矩管", "SHS"),
            new SectionSteelClassification (typeof(SectionSteel_RECT), "焊接矩管", "RECT"),
            new SectionSteelClassification (typeof(SectionSteel_RECT), "焊接矩管", "R"),
            new SectionSteelClassification (typeof(SectionSteel_RECT), "焊接矩管", "RHSC"),
            new SectionSteelClassification (typeof(SectionSteel_RECT), "焊接矩管", "B_BUILT"),
            new SectionSteelClassification (typeof(SectionSteel_RECT), "焊接矩管", "B_VAR_A"),
            new SectionSteelClassification (typeof(SectionSteel_RECT), "焊接矩管", "B_VAR_B"),
            new SectionSteelClassification (typeof(SectionSteel_RECT), "焊接矩管", "B_VAR_C"),
            new SectionSteelClassification (typeof(SectionSteel_RECT), "焊接矩管", "B_WLD_F"),
            new SectionSteelClassification (typeof(SectionSteel_RECT), "焊接矩管", "B_WLD_J"),
            new SectionSteelClassification (typeof(SectionSteel_CFH_Y), "冷弯圆管", "Y"),
            new SectionSteelClassification (typeof(SectionSteel_CFH_Y), "冷弯圆管", "Φ"),
            new SectionSteelClassification (typeof(SectionSteel_CFH_Y), "冷弯圆管", "φ"),
            new SectionSteelClassification (typeof(SectionSteel_CIRC), "圆钢或圆管", "D"),
            new SectionSteelClassification (typeof(SectionSteel_CIRC), "圆钢或圆管", "O"),
            new SectionSteelClassification (typeof(SectionSteel_CIRC), "圆钢或圆管", "CFCHS"),
            new SectionSteelClassification (typeof(SectionSteel_CIRC), "圆钢或圆管", "CHS"),
            new SectionSteelClassification (typeof(SectionSteel_CIRC), "圆钢或圆管", "ELD"),
            new SectionSteelClassification (typeof(SectionSteel_CIRC), "圆钢或圆管", "EPD"),
            new SectionSteelClassification (typeof(SectionSteel_CIRC), "圆钢或圆管", "PD"),
            new SectionSteelClassification (typeof(SectionSteel_CIRC), "圆钢或圆管", "PIP"),
            new SectionSteelClassification (typeof(SectionSteel_CIRC), "圆钢或圆管", "ROD"),
            new SectionSteelClassification (typeof(SectionSteel_CIRC), "圆钢或圆管", "TUBE"),
            new SectionSteelClassification (typeof(SectionSteel_CFO_CN), "C型钢", "C"),
            new SectionSteelClassification (typeof(SectionSteel_CFO_CN), "C型钢", "CC"),
            new SectionSteelClassification (typeof(SectionSteel_CFO_CN_MtM), "双拼C型钢(口对口)", "2CCM"),
            new SectionSteelClassification (typeof(SectionSteel_CFO_CN_MtM), "双拼C型钢(口对口)", "2CM"),
            new SectionSteelClassification (typeof(SectionSteel_CFO_CN_BtB), "双拼C型钢(背对背)", "2C"),
            new SectionSteelClassification (typeof(SectionSteel_CFO_CN_BtB), "双拼C型钢(背对背)", "2CC"),
            new SectionSteelClassification (typeof(SectionSteel_CFO_ZJ), "Z型钢", "Z"),
            new SectionSteelClassification (typeof(SectionSteel_CFO_ZJ), "Z型钢", "ZZ"),
            new SectionSteelClassification (typeof(SectionSteel_CFO_ZJ), "Z型钢", "XZ"),
            new SectionSteelClassification (typeof(SectionSteel_PL), "矩形板", "PL"),
            new SectionSteelClassification (typeof(SectionSteel_PL_Triangle), "三角板", "PLT"),
            new SectionSteelClassification (typeof(SectionSteel_PL_Circular), "圆板", "PLD"),
            new SectionSteelClassification (typeof(SectionSteel_PL_Circular), "圆板", "PLO"),
            new SectionSteelClassification (typeof(SectionSteel_PL_Composite), "复合板件", "nPL"),
            new SectionSteelClassification (typeof(SectionSteel_PL_Composite), "复合板件", "nPLT"),
            new SectionSteelClassification (typeof(SectionSteel_PL_Composite), "复合板件", "nPLD"),
            new SectionSteelClassification (typeof(SectionSteel_PL_Composite), "复合板件", "nPLO"),
            new SectionSteelClassification (typeof(SectionSteel_SPHERE), "球体", "SPHERE"),
        };

        public override PIStyleEnum PIStyle {
            get => _PIStyle;
            set {
                _PIStyle = value;
                if (realSectionSteel != null) realSectionSteel.PIStyle = value;
            }
        }

        public override GBData[] GBDataSet {
            get {
                if (realSectionSteel != null)
                    return realSectionSteel.GBDataSet;
                else
                    return null;
            }
        }

        /// <summary>
        /// 仅当实际类型为 <see cref="SectionSteel_HH"/> 时有效，
        /// 返回 <see cref="SectionSteel_HH.GBDataSet2"/>。
        /// </summary>
        public GBData[] GBDataSet2 {
            get {
                if (realSectionSteel != null && realSectionSteel is SectionSteel_HH sshh)
                    return sshh.GBDataSet2;
                else
                    return null;
            }
        }

        public SectionSteel() { }

        public SectionSteel(string profileText) {
            this.ProfileText = profileText;
        }

        protected override void SetFieldsValue(SectionSteelBase sender, ProfileTextChangingEventArgs e) {
            var tmp = realSectionSteel;
            try {
                if (string.IsNullOrEmpty(e.NewText))
                    throw new MismatchedProfileTextException(e.NewText);

                var index = GetClassificationIndex(e.NewText);
                if (index == -1)
                    throw new MismatchedProfileTextException(e.NewText);

                var type = ClassificationCollection[index].Type;
                realSectionSteel = Activator.CreateInstance(type) as SectionSteelBase;
                realSectionSteel.ProfileText = e.NewText;
                realSectionSteel.PIStyle = this.PIStyle;
            } catch (MismatchedProfileTextException) {
                realSectionSteel = tmp;
                throw;
            }
        }

        public override string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            if (realSectionSteel == null)
                return string.Empty;

            return realSectionSteel.GetAreaFormula(accuracy, exclude_topSurface);
        }

        public override string GetSiffenerProfileStr(bool truncatedRounding) {
            if (realSectionSteel == null)
                return string.Empty;

            return realSectionSteel.GetSiffenerProfileStr(truncatedRounding);
        }

        public override string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            if (realSectionSteel == null)
                return string.Empty;

            return realSectionSteel.GetWeightFormula(accuracy);
        }

        /// <summary>
        /// 根据截面规格文本，获取其对应的型钢分类信息在 <see cref="SectionSteel.ClassificationCollection"/> 中的序号。
        /// </summary>
        /// <param name="profileText">截面规格文本</param>
        /// <returns>型钢分类信息在 <see cref="SectionSteel.ClassificationCollection"/> 中的序号。
        /// 获取失败则返回 -1。
        /// </returns>
        /// <exception cref="ArgumentException"></exception>
        public static int GetClassificationIndex(string profileText) {
            if (string.IsNullOrEmpty(profileText)) {
                throw new ArgumentException($"“{nameof(profileText)}”不能为 null 或空。", nameof(profileText));
            }

            var match = Regex.Match(profileText, Pattern_Collection.Classifier);
            if (!match.Success) return -1;

            var classifier = match.Groups["classifier"].Value;
            //槽钢和冷弯内卷边槽钢及其双拼形式，有重复标识符，需特殊处理
            //PL、PLT、PLD、PLO需特殊处理（可能是组件板件）
            switch (classifier) {
            case "C":
                match = Regex.Match(profileText, Pattern_Collection.CFO_CN_1);
                if (match.Success)
                    return Array.FindIndex(
                        ClassificationCollection,
                        item => item.Type == typeof(SectionSteel_CFO_CN) && item.Classifier == classifier);

                match = Regex.Match(profileText, Pattern_Collection.CHAN_1);
                if (!match.Success) match = Regex.Match(profileText, Pattern_Collection.CHAN_2);
                if (match.Success)
                    return Array.FindIndex(
                        ClassificationCollection,
                        item => item.Type == typeof(SectionSteel_CHAN) && item.Classifier == classifier);

                break;
            case "2C":
                match = Regex.Match(profileText, Pattern_Collection.CFO_CN_BtB_1);
                if (match.Success)
                    return Array.FindIndex(
                        ClassificationCollection,
                        item => item.Type == typeof(SectionSteel_CFO_CN_BtB) && item.Classifier == classifier);

                match = Regex.Match(profileText, Pattern_Collection.CHAN_BtB_1);
                if (!match.Success) match = Regex.Match(profileText, Pattern_Collection.CHAN_BtB_2);
                if (match.Success)
                    return Array.FindIndex(
                        ClassificationCollection,
                        item => item.Type == typeof(SectionSteel_CHAN_BtB) && item.Classifier == classifier);

                break;
            case "PL":
                match = Regex.Match(profileText, Pattern_Collection.PL_1);
                if (match.Success)
                    return Array.FindIndex(
                        ClassificationCollection,
                        item => item.Type == typeof(SectionSteel_PL) && item.Classifier == classifier);

                return Array.FindIndex(
                    ClassificationCollection,
                    item => item.Type == typeof(SectionSteel_PL_Composite) && item.Classifier == "nPL");
            case "PLT":
                match = Regex.Match(profileText, Pattern_Collection.PL_T_1);
                if (match.Success)
                    return Array.FindIndex(
                        ClassificationCollection,
                        item => item.Type == typeof(SectionSteel_PL_Triangle) && item.Classifier == classifier);

                return Array.FindIndex(
                    ClassificationCollection,
                    item => item.Type == typeof(SectionSteel_PL_Composite) && item.Classifier == "nPLT");
            case "PLD":
                match = Regex.Match(profileText, Pattern_Collection.PL_O_1);
                if (match.Success)
                    return Array.FindIndex(
                        ClassificationCollection,
                        item => item.Type == typeof(SectionSteel_PL_Circular) && item.Classifier == classifier);

                return Array.FindIndex(
                    ClassificationCollection,
                    item => item.Type == typeof(SectionSteel_PL_Composite) && item.Classifier == "nPLD");
            case "PLO":
                match = Regex.Match(profileText, Pattern_Collection.PL_O_1);
                if (match.Success)
                    return Array.FindIndex(
                        ClassificationCollection,
                        item => item.Type == typeof(SectionSteel_PL_Circular) && item.Classifier == classifier);

                return Array.FindIndex(
                    ClassificationCollection,
                    item => item.Type == typeof(SectionSteel_PL_Composite) && item.Classifier == "nPLO");
            default:
                var index = Array.FindIndex(
                    ClassificationCollection, 
                    item => item.Classifier == classifier);

                if (index != -1) {
                    return index;
                } else {
                    //由于表中组合板件标识符用"nPL", "nPLT", "nPLD", "nPLO"表示
                    //可能查找不到，所以要进入此分支
                    match = Regex.Match(profileText, Pattern_Collection.PL_CMP_1);
                    if (match.Success) {
                        if (classifier.Contains("PLT"))
                            classifier = "nPLT";
                        else if (classifier.Contains("PLD"))
                            classifier = "nPLD";
                        else if (classifier.Contains("PLO"))
                            classifier = "nPLO";
                        else
                            classifier = "nPL";

                        return Array.FindIndex(
                            ClassificationCollection,
                            item => item.Type == typeof(SectionSteel_PL_Composite) && item.Classifier == classifier);
                    }
                }

                break;
            }

            return -1;
        }
    }
}
