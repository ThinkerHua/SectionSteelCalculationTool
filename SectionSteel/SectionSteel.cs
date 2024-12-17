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
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SectionSteel {
    /// <summary>
    /// 型钢总类，实际应用中使用此类即可。根据属性 <b>ProfileText</b> 自动识别具体应用哪一项子类，因此本类依赖于各子类：
    /// <para><see cref="SectionSteel_H"/></para>
    /// <para><see cref="SectionSteel_HH"/></para>
    /// <para><see cref="SectionSteel_T"/></para>
    /// <para><see cref="SectionSteel_I"/></para>
    /// <para><see cref="SectionSteel_CHAN"/></para>
    /// <para><see cref="SectionSteel_CHAN_MtM"/></para>
    /// <para><see cref="SectionSteel_CHAN_BtB"/></para>
    /// <para><see cref="SectionSteel_L"/></para>
    /// <para><see cref="SectionSteel_L_BtB"/></para>
    /// <para><see cref="SectionSteel_CFH_J"/></para>
    /// <para><see cref="SectionSteel_RECT"/></para>
    /// <para><see cref="SectionSteel_CFH_Y"/></para>
    /// <para><see cref="SectionSteel_CIRC"/></para>
    /// <para><see cref="SectionSteel_CFO_CN"/></para>
    /// <para><see cref="SectionSteel_CFO_CN_MtM"/></para>
    /// <para><see cref="SectionSteel_CFO_CN_BtB"/></para>
    /// <para><see cref="SectionSteel_CFO_ZJ"/></para>
    /// <para><see cref="SectionSteel_PL"/></para>
    /// <para><see cref="SectionSteel_PL_Triangle"/></para>
    /// <para><see cref="SectionSteel_PL_Circular"/></para>
    /// <para><see cref="SectionSteel_PL_Composite"/></para>
    /// </summary>
    public class SectionSteel : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private PIStyleEnum _PIStyle;
        private ISectionSteel realSectionSteel;
        protected static Dictionary<Type, string[]> classifierTable = new Dictionary<Type, string[]> {
            {typeof(SectionSteel_H), new string[] {"B_WLD_A", "BH","B_WLD_H","B_WLD_K","H","HI","HM","HN","HP","HT","HW","PHI","WH","WI","I_VAR_A"}},
            {typeof(SectionSteel_HH), new string[] {"B_WLD_O","HH"}},
            {typeof(SectionSteel_T), new string[] {"T","TW","TM","TN","B_WLD_E"}},
            {typeof(SectionSteel_I), new string[] {"I"}},
            {typeof(SectionSteel_CHAN), new string[] {"[","C"}},
            {typeof(SectionSteel_CHAN_MtM), new string[] {"[]"}},
            {typeof(SectionSteel_CHAN_BtB), new string[] {"][","2[","2C"}},
            {typeof(SectionSteel_L), new string[] {"∠","L"}},
            {typeof(SectionSteel_L_BtB), new string[] {"2∠","2L"}},
            {typeof(SectionSteel_CFH_J), new string[] {"CFRHS","F","J","P","RHS","SHS","TUB"}},
            {typeof(SectionSteel_RECT), new string[] {"B_BUILT","B_VAR_A","B_VAR_B","B_VAR_C","B_WLD_F","B_WLD_J","R","RECT","RHSC"}},
            {typeof(SectionSteel_CFH_Y), new string[] {"Y","φ"}},
            {typeof(SectionSteel_CIRC), new string[] {"CFCHS","CHS","D","ELD","EPD","O","PD","PIP","ROD","TUBE"}},
            {typeof(SectionSteel_CFO_CN), new string[] {"C","CC"}},
            {typeof(SectionSteel_CFO_CN_MtM), new string[] {"2CCM","2CM"}},
            {typeof(SectionSteel_CFO_CN_BtB), new string[] {"2C","2CC"}},
            {typeof(SectionSteel_CFO_ZJ), new string[] {"XZ","Z","ZZ"}},
            {typeof(SectionSteel_PL), new string[] {"PL","PLD","PLO","PLT"}},
            {typeof(SectionSteel_SPHERE), new string[]{"SPHERE"} },
        };
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle {
            get => _PIStyle;
            set {
                _PIStyle = value;
                if (realSectionSteel != null) realSectionSteel.PIStyle = value;
            }
        }
        public SectionSteel() {

        }
        public SectionSteel(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            realSectionSteel = null;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.Classifier);
                if (!match.Success)
                    throw new MismatchedProfileTextException();

                var classifier = match.Groups["classifier"].Value;
                //槽钢和冷弯内卷边槽钢及其双拼形式，有重复标识符，需特殊处理
                //PL_Composite不能处理PLt*b形式，需特殊处理
                switch (classifier) {
                case "C":
                    match = Regex.Match(ProfileText, Pattern_Collection.CFO_CN_1);
                    if (match.Success) {
                        realSectionSteel = new SectionSteel_CFO_CN(ProfileText);
                        break;
                    }
                    realSectionSteel = new SectionSteel_CHAN(ProfileText);
                    break;
                case "2C":
                    match = Regex.Match(ProfileText, Pattern_Collection.CFO_CN_BtB_1);
                    if (match.Success) {
                        realSectionSteel = new SectionSteel_CFO_CN_BtB(ProfileText);
                        break;
                    }
                    realSectionSteel = new SectionSteel_CHAN_BtB(ProfileText);
                    break;
                case "PL":
                    var pattern = @"^PL\d+\.?\d*\*\d+\.?\d*$";
                    match = Regex.Match(ProfileText, pattern);
                    if (match.Success) {
                        realSectionSteel = new SectionSteel_PL(ProfileText);
                        break;
                    }
                    realSectionSteel = new SectionSteel_PL_Composite(ProfileText);
                    break;
                default:
                    Type type = null;
                    foreach (var item in classifierTable) {
                        foreach (var value in item.Value) {
                            if (classifier.Equals(value)) {
                                type = item.Key;
                                goto Got_it;
                            }
                        }
                    }
                    throw new MismatchedProfileTextException();

                Got_it:
                    realSectionSteel = Activator.CreateInstance(type) as ISectionSteel;
                    realSectionSteel.ProfileText = ProfileText;

                    break;
                }

                realSectionSteel.PIStyle = this.PIStyle;
            } catch (MismatchedProfileTextException) {
                realSectionSteel = null;
            }
        }
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            if (realSectionSteel == null)
                return string.Empty;

            return realSectionSteel.GetAreaFormula(accuracy, exclude_topSurface);
        }

        public string GetSiffenerProfileStr(bool truncatedRounding) {
            if (realSectionSteel == null)
                return string.Empty;

            return realSectionSteel.GetSiffenerProfileStr(truncatedRounding);
        }

        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            if (realSectionSteel == null)
                return string.Empty;

            return realSectionSteel.GetWeightFormula(accuracy);
        }
    }
}
