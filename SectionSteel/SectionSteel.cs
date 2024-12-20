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
    /// </summary>
    public class SectionSteel : SectionSteelBase {
        private PIStyleEnum _PIStyle;
        private SectionSteelBase realSectionSteel;
        public static SectionSteelCategoryInfo[] CategoryInfoCollection = new SectionSteelCategoryInfo[] {
            new SectionSteelCategoryInfo (
                typeof(SectionSteel_H),
                "H型钢",
                new string[] {"B_WLD_A", "BH","B_WLD_H","B_WLD_K","H","HI","HM","HN","HP","HT","HW","PHI","WH","WI","I_VAR_A"}),
            new SectionSteelCategoryInfo (
                typeof(SectionSteel_HH),
                "十字拼接H型钢",
                new string[] {"B_WLD_O","HH"}),
            new SectionSteelCategoryInfo (
                typeof(SectionSteel_T),
                "T型钢",
                new string[] {"T","TW","TM","TN","B_WLD_E"}),
            new SectionSteelCategoryInfo (
                typeof(SectionSteel_I),
                "工字钢",
                new string[] {"I"}),
            new SectionSteelCategoryInfo (
                typeof(SectionSteel_CHAN),
                "槽钢",
                new string[] {"[","C"}),
            new SectionSteelCategoryInfo (
                typeof(SectionSteel_CHAN_MtM),
                "双拼槽钢(口对口)",
                new string[] {"[]"}),
            new SectionSteelCategoryInfo (
                typeof(SectionSteel_CHAN_BtB),
                "双拼槽钢(背对背)",
                new string[] {"][","2[","2C"}),
            new SectionSteelCategoryInfo (
                typeof(SectionSteel_L),
                "角钢",
                new string[] {"∠","L"}),
            new SectionSteelCategoryInfo (
                typeof(SectionSteel_L_BtB),
                "双拼角钢",
                new string[] {"2∠","2L"}),
            new SectionSteelCategoryInfo (
                typeof(SectionSteel_CFH_J),
                "冷弯矩管",
                new string[] {"CFRHS","F","J","P","RHS","SHS","TUB"}),
            new SectionSteelCategoryInfo (
                typeof(SectionSteel_RECT),
                "焊接矩管",
                new string[] {"B_BUILT","B_VAR_A","B_VAR_B","B_VAR_C","B_WLD_F","B_WLD_J","R","RECT","RHSC"}),
            new SectionSteelCategoryInfo(
                typeof(SectionSteel_CFH_Y),
                "冷弯圆管",
                new string[] {"Y","φ"}),
            new SectionSteelCategoryInfo(
                typeof(SectionSteel_CIRC),
                "圆钢或圆管",
                new string[] {"CFCHS","CHS","D","ELD","EPD","O","PD","PIP","ROD","TUBE"}),
            new SectionSteelCategoryInfo(
                typeof(SectionSteel_CFO_CN),
                "C型钢",
                new string[] { "C", "CC" }),
            new SectionSteelCategoryInfo(
                typeof(SectionSteel_CFO_CN_MtM),
                "双拼C型钢(口对口)",
                new string[] { "2CCM", "2CM" }),
            new SectionSteelCategoryInfo(
                typeof(SectionSteel_CFO_CN_BtB),
                "双拼C型钢(背对背)",
                new string[] { "2C", "2CC" }),
            new SectionSteelCategoryInfo(
                typeof(SectionSteel_CFO_ZJ),
                "Z型钢",
                new string[] { "XZ", "Z", "ZZ" }),
            new SectionSteelCategoryInfo(
                typeof(SectionSteel_PL),
                "矩形板",
                new string[] { "PL" }),
            new SectionSteelCategoryInfo(
                typeof(SectionSteel_PL_Triangle),
                "三角板",
                new string[] { "PLT" }),
            new SectionSteelCategoryInfo(
                typeof(SectionSteel_PL_Circular),
                "圆板",
                new string[] { "PLD", "PLO" }),
            new SectionSteelCategoryInfo(
                typeof(SectionSteel_PL_Composite),
                "复合板件",
                new string[] {"nPL", "nPLT", "nPLD","nPLO" }),
            new SectionSteelCategoryInfo(
                typeof(SectionSteel_SPHERE),
                "球体",
                new string[] { "SPHERE" }),
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

                var type = GetCategoryInfo(e.NewText).Type ?? throw new MismatchedProfileTextException(e.NewText);

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
        /// 根据截面规格文本，获取型钢分类和标识符。
        /// </summary>
        /// <param name="profileText">截面规格文本</param>
        /// <returns>型钢分类和标识符。
        /// 其中组合板件的标识符约定为 "nPL", "nPLT", "nPLD", "nPLO"。
        /// </returns>
        /// <exception cref="ArgumentException"></exception>
        public static (Type Type, string Classifier) GetCategoryInfo(string profileText) {
            if (string.IsNullOrEmpty(profileText)) {
                throw new ArgumentException($"“{nameof(profileText)}”不能为 null 或空。", nameof(profileText));
            }

            var match = Regex.Match(profileText, Pattern_Collection.Classifier);
            if (!match.Success) return default;

            var classifier = match.Groups["classifier"].Value;
            //槽钢和冷弯内卷边槽钢及其双拼形式，有重复标识符，需特殊处理
            //PL和PL_Composite需特殊处理
            switch (classifier) {
            case "C":
                match = Regex.Match(profileText, Pattern_Collection.CFO_CN_1);
                if (match.Success)
                    return (Array.Find(CategoryInfoCollection, item => item.Type == typeof(SectionSteel_CFO_CN)).Type, classifier);

                match = Regex.Match(profileText, Pattern_Collection.CHAN_1);
                if (!match.Success) match = Regex.Match(profileText, Pattern_Collection.CHAN_2);
                if (match.Success)
                    return (Array.Find(CategoryInfoCollection, item => item.Type == typeof(SectionSteel_CHAN)).Type, classifier);

                return default;
            case "2C":
                match = Regex.Match(profileText, Pattern_Collection.CFO_CN_BtB_1);
                if (match.Success)
                    return (Array.Find(CategoryInfoCollection, item => item.Type == typeof(SectionSteel_CFO_CN_BtB)).Type, classifier);

                match = Regex.Match(profileText, Pattern_Collection.CHAN_BtB_1);
                if (!match.Success) match = Regex.Match(profileText, Pattern_Collection.CHAN_BtB_2);
                if (match.Success)
                    return (Array.Find(CategoryInfoCollection, item => item.Type == typeof(SectionSteel_CHAN_BtB)).Type, classifier);

                return default;
            case "PL":
                var pattern = @"^PL\d+\.?\d*\*\d+\.?\d*$";
                match = Regex.Match(profileText, pattern);
                if (match.Success)
                    return (Array.Find(CategoryInfoCollection, item => item.Type == typeof(SectionSteel_PL)).Type, classifier);

                match = Regex.Match(profileText, Pattern_Collection.PL_CMP_1);
                if (match.Success)
                    return (Array.Find(CategoryInfoCollection, item => item.Type == typeof(SectionSteel_PL_Composite)).Type, "nPL");

                return default;
            default:
                var info = Array.Find(CategoryInfoCollection, item => item.Classifiers.Contains(classifier));
                if (!info.Equals(default(SectionSteelCategoryInfo))) {
                    return (info.Type, classifier);
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

                        return (Array.Find(CategoryInfoCollection, item => item.Type == typeof(SectionSteel_PL_Composite)).Type, classifier);
                    }

                    return default;
                }
            }
        }
    }
}
