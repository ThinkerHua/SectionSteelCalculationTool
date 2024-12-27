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
 *  Pattern_Collection.cs: 型钢截面文本匹配模式的集合
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
namespace SectionSteel {
    /// <summary>
    /// 型钢截面文本匹配模式的集合
    /// </summary>
    static class Pattern_Collection {
        /// <summary>
        /// 前置标识符为 H 或 HP 或 HW 或 HM 或 HN 或 HT 或 WH，后续参数形式为 h1[~h2]*b1[/b2]*s*t1[/t2]。
        /// </summary>
        public static string H_1 => @"^((H[PWMNT]?)|WH)(?<h1>\d+\.?\d*)(~(?<h2>\d+\.?\d*))?"
                                    + @"\*(?<b1>\d+\.?\d*)(/(?<b2>\d+\.?\d*))?"
                                    + @"\*(?<s>\d+\.?\d*)\*(?<t1>\d+\.?\d*)(/(?<t2>\d+\.?\d*))?$";
        /// <summary>
        /// 前置标识符为 HI 或 PHI 或 WI，后续参数形式为 h1[-h2]-s-t1*b1[-t2*b2]。
        /// </summary>
        public static string H_2 => @"^((P?H)|W)I(?<h1>\d+\.?\d*)(-(?<h2>\d+\.?\d*))?-(?<s>\d+\.?\d*)"
                                    + @"-(?<t1>\d+\.?\d*)\*(?<b1>\d+\.?\d*)(-(?<t2>\d+\.?\d*)\*(?<b2>\d+\.?\d*))?$";
        /// <summary>
        /// 前置标识符为 H 或 HW 或 HM 或 HN 或 HT，用"TYPE"分组接收，后续参数形式为 H*B，仅支持整数。
        /// </summary>
        public static string H_3 => @"^(?<TYPE>H([WMN]?|T))(?<H>\d+)\*(?<B>\d+)$";
        /// <summary>
        /// 前置标识符为 B_WLD_K，后续参数形式为 h1*h2*b1*s*t1。
        /// </summary>
        public static string H_4 => @"^B_WLD_K(?<h1>\d+\.?\d*)\*(?<h2>\d+\.?\d*)\*(?<b1>\d+\.?\d*)\*(?<s>\d+\.?\d*)\*(?<t1>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 B_WLD_A 或 BH，后续参数形式为 h1*b1*s*t1。
        /// </summary>
        public static string H_5 => @"^((B_WLD_A)|(BH))(?<h1>\d+\.?\d*)\*(?<b1>\d+\.?\d*)\*(?<s>\d+\.?\d*)\*(?<t1>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 B_WLD_H，后续参数形式为 h1*b1*b2*s*t1*t2。
        /// </summary>
        public static string H_6 => @"^B_WLD_H(?<h1>\d+\.?\d*)\*(?<b1>\d+\.?\d*)\*(?<b2>\d+\.?\d*)\*(?<s>\d+\.?\d*)\*(?<t1>\d+\.?\d*)\*(?<t2>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 I_VAR_A，后续参数形式为 h1-h2*b1-b2*s*t1。
        /// </summary>
        public static string H_7 => @"^I_VAR_A(?<h1>\d+\.?\d*)-(?<h2>\d+\.?\d*)\*(?<b1>\d+\.?\d*)-(?<b2>\d+\.?\d*)\*(?<s>\d+\.?\d*)\*(?<t1>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 HH，后续参数形式为 h1*b1*s1*t1[+h2*b2*s2*t2]。
        /// </summary>
        public static string HH_1 => @"^HH(?<h1>\d+\.?\d*)\*(?<b1>\d+\.?\d*)\*(?<s1>\d+\.?\d*)\*(?<t1>\d+\.?\d*)"
                                    + @"(\+(?<h2>\d+\.?\d*)\*(?<b2>\d+\.?\d*)\*(?<s2>\d+\.?\d*)\*(?<t2>\d+\.?\d*))?$";
        /// <summary>
        /// 前置标识符为 B_WLD_O，后续参数形式为 b1*t1*s1*b2*t2*s2*h1*h2。
        /// </summary>
        public static string HH_2 => @"^B_WLD_O(?<b1>\d+\.?\d*)\*(?<t1>\d+\.?\d*)\*(?<s1>\d+\.?\d*)"
                                    + @"\*(?<b2>\d+\.?\d*)\*(?<t2>\d+\.?\d*)\*(?<s2>\d+\.?\d*)"
                                    + @"\*(?<h1>\d+\.?\d*)\*(?<h2>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 T 或 TW 或 TM 或 TN，后续参数形式为 h1[~h2]*b*s*t。
        /// </summary>
        public static string T_1 => @"^T[WMN]?(?<h1>\d+\.?\d*)(~(?<h2>\d+\.?\d*))?\*(?<b>\d+\.?\d*)\*(?<s>\d+\.?\d*)\*(?<t>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 T，后续参数形式为 h1-s-t-b。
        /// </summary>
        public static string T_2 => @"^T(?<h1>\d+\.?\d*)-(?<s>\d+\.?\d*)-(?<t>\d+\.?\d*)-(?<b>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 T 或 TW 或 TM 或 TN，用"TYPE"分组接收，后续参数形式为 H*B，仅支持整数。
        /// </summary>
        public static string T_3 => @"^(?<TYPE>T[WMN]?)(?<H>\d+)\*(?<B>\d+)$";
        /// <summary>
        /// 前置标识符为 B_WLD_E，后续参数形式为 h1*b*s*t。
        /// </summary>
        public static string T_4 => @"^B_WLD_E(?<h1>\d+\.?\d*)\*(?<b>\d+\.?\d*)\*(?<s>\d+\.?\d*)\*(?<t>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 I，后续参数形式为 h*b*s。
        /// </summary>
        public static string I_1 => @"^I(?<h>\d+\.?\d*)\*(?<b>\d+\.?\d*)\*(?<s>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 I，后续参数整体用"NAME"分组接收，形式为 CODE[SUFFIX]，CODE为整数或小数，SUFFIX为 "a" 或 "b" 或 "c"。
        /// <br/>例如：对于I20a，NAME = "20a"，CODE = "20"，SUFFIX = "a"
        /// </summary>
        public static string I_2 => @"^I(?<NAME>(?<CODE>\d+\.?\d*)(?<SUFFIX>[abc]?))$";
        /// <summary>
        /// 前置标识符为 [ 或 C，后续参数形式为 h*b*s。
        /// </summary>
        public static string CHAN_1 => @"^[\[C](?<h>\d+\.?\d*)\*(?<b>\d+\.?\d*)\*(?<s>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 [ 或 C，后续参数整体用"NAME"分组接收，形式为 CODE[SUFFIX]，CODE为整数或小数，SUFFIX 为 "a" 或 "b" 或 "c"。
        /// <br/>例如：对于[20a，NAME = "20a"，CODE = "20"，SUFFIX = "a"
        /// </summary>
        public static string CHAN_2 => @"^[\[C](?<NAME>(?<CODE>\d+\.?\d*)(?<SUFFIX>[abc]?))$";
        /// <summary>
        /// 前置标识符为 []，后续参数形式为 h*b*s。
        /// </summary>
        public static string CHAN_MtM_1 => @"^\[\](?<h>\d+\.?\d*)\*(?<b>\d+\.?\d*)\*(?<s>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 []，后续参数整体用"NAME"分组接收，形式为 CODE[SUFFIX]，CODE为整数或小数，SUFFIX 为 "a" 或 "b" 或 "c"。
        /// <br/>例如：对于[]20a，NAME = "20a"，CODE = "20"，SUFFIX = "a"
        /// </summary>
        public static string CHAN_MtM_2 => @"^\[\](?<NAME>(?<CODE>\d+\.?\d*)(?<SUFFIX>[abc]?))$";
        /// <summary>
        /// 前置标识符为 2[ 或 2C 或 ][，后续参数形式为 h*b*s。
        /// </summary>
        public static string CHAN_BtB_1 => @"^((2\[)|(2C)|(\]\[))(?<h>\d+\.?\d*)\*(?<b>\d+\.?\d*)\*(?<s>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 2[ 或 2C 或 ][，后续参数整体用"NAME"分组接收，形式为 CODE[SUFFIX]，CODE为整数或小数，SUFFIX 为 "a" 或 "b" 或 "c"。
        /// <br/>例如：对于][20a，NAME = "20a"，CODE = "20"，SUFFIX = "a"
        /// </summary>
        public static string CHAN_BtB_2 => @"^((2\[)|(2C)|(\]\[))(?<NAME>(?<CODE>\d+\.?\d*)(?<SUFFIX>[abc]?))$";
        /// <summary>
        /// 前置标识符为 ∠ 或 L，后续参数形式为 h[*b]*t。
        /// </summary>
        public static string L_1 => @"^[∠L](?<h>\d+\.?\d*)(\*(?<b>\d+\.?\d*))?\*(?<t>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 ∠ 或 L，后续参数形式为 h[/b]，以cm为单位。
        /// </summary>
        public static string L_2 => @"^[∠L](?<h>\d+\.?\d*)(/(?<b>\d+\.?\d*))?$";
        /// <summary>
        /// 前置标识符为 2∠ 或 2L，后续参数形式为 h[*b]*t。
        /// </summary>
        public static string L_BtB_1 => @"^2[∠L](?<h>\d+\.?\d*)(\*(?<b>\d+\.?\d*))?\*(?<t>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 2∠ 或 2L，后续参数形式为 h[/b]，以cm为单位。
        /// </summary>
        public static string L_BtB_2 => @"^2[∠L](?<h>\d+\.?\d*)(/(?<b>\d+\.?\d*))?$";
        /// <summary>
        /// 前置标识符为 F 或 J 或 P 或 TUB 或 RHS 或 SHS 或 CFRHS，后续参数形式为 h1*t。
        /// </summary>
        public static string CFH_J_1 => @"^(F|J|P|(TUB)|(RHS)|(SHS)|(CFRHS))(?<h1>\d+\.?\d*)\*(?<t>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 F 或 J 或 P 或 RHS 或 SHS 或 CFRHS，后续参数形式为 h1*b1*t。
        /// </summary>
        public static string CFH_J_2 => @"^(F|J|P|(RHS)|(SHS)|(CFRHS))(?<h1>\d+\.?\d*)\*(?<b1>\d+\.?\d*)\*(?<t>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 P 或 RHS 或 SHS 或 CFRHS，后续参数形式为 h1*b1-h2*b2*t。
        /// </summary>
        public static string CFH_J_3 => @"^(P|(RHS)|(SHS)|(CFRHS))(?<h1>\d+\.?\d*)\*(?<b1>\d+\.?\d*)-(?<h2>\d+\.?\d*)\*(?<b2>\d+\.?\d*)\*(?<t>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 B_WLD_F，后续参数形式为 h1*b1*s。
        /// </summary>
        public static string RECT_1 => @"^B_WLD_F(?<h1>\d+\.?\d*)\*(?<b1>\d+\.?\d*)\*(?<s>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 RECT 或 B_WLD_F 或 B_BUILT，后续参数形式为 h1*b1*s*t。
        /// </summary>
        public static string RECT_2 => @"^((RECT)|(B_WLD_F)|(B_BUILT))(?<h1>\d+\.?\d*)\*(?<b1>\d+\.?\d*)\*(?<s>\d+\.?\d*)\*(?<t>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 R，后续参数形式为 h1~h2*b1*s*t。
        /// </summary>
        public static string RECT_3 => @"^R(?<h1>\d+\.?\d*)~(?<h2>\d+\.?\d*)\*(?<b1>\d+\.?\d*)\*(?<s>\d+\.?\d*)\*(?<t>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 B_WLD_J，后续参数形式为 h1*h2*b1*s*t。
        /// </summary>
        public static string RECT_4 => @"^B_WLD_J(?<h1>\d+\.?\d*)\*(?<h2>\d+\.?\d*)\*(?<b1>\d+\.?\d*)\*(?<s>\d+\.?\d*)\*(?<t>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 B_VAR_A 或 B_VAR_B 或 B_VAR_C，后续参数形式为 h1-h2*s[*b1[-b2]]。b1, b2 值均忽略。
        /// </summary>
        public static string RECT_5 => @"^B_VAR_[ABC](?<h1>\d+\.?\d*)-(?<h2>\d+\.?\d*)\*(?<s>\d+\.?\d*)\*(\d+\.?\d*)-(\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 RHSC，后续参数形式为 H1*h1*H2*h2*s*b1。
        /// </summary>
        public static string RECT_6 => @"^RHSC(?<H1>\d+\.?\d*)\*(?<h1>\d+\.?\d*)\*(?<H2>\d+\.?\d*)\*(?<h2>\d+\.?\d*)\*(?<s>\d+\.?\d*)\*(?<b1>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 Y 或 Φ 或 φ，后续参数形式为 d*t。
        /// </summary>
        public static string CFH_Y_1 => @"^[YΦφ](?<d>\d+\.?\d*)\*(?<t>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 D 或 ELD 或 ROD，后续参数形式为 d1[*r1*d2*r2]。
        /// </summary>
        public static string CIRC_1 => @"^((EL)|(RO))?D(?<d1>\d+\.?\d*)(\*(?<r1>\d+\.?\d*)\*(?<d2>\d+\.?\d*)\*(?<r2>\d+\.?\d*))?$";
        /// <summary>
        /// 前置标识符为 D 或 PIP 或 CFCHS 或 CHS 或 EPD 或 O 或 PD 或 TUBE，后续参数形式为 d1[*d2]*t。
        /// </summary>
        public static string CIRC_2 => @"^(D|(PIP)|(CFCHS)|(CHS)|(EPD)|O|(PD)|(TUBE))(?<d1>\d+\.?\d*)(\*(?<d2>\d+\.?\d*))?\*(?<t>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 D 或 PIP 或 CFCHS 或 CHS 或 EPD 或 O 或 PD 或 TUBE，后续参数形式为 d1*r1*d2*r2*t。
        /// </summary>
        public static string CIRC_3 => @"^(D|(PIP)|(CFCHS)|(CHS)|(EPD)|O|(PD)|(TUBE))(?<d1>\d+\.?\d*)\*(?<r1>\d+\.?\d*)"
                                        + @"\*(?<d2>\d+\.?\d*)\*(?<r2>\d+\.?\d*)\*(?<t>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 C，后续参数形式为 h*b1*c1*t。
        /// </summary>
        public static string CFO_CN_1 => @"^C(?<h>\d+\.?\d*)\*(?<b1>\d+\.?\d*)\*(?<c1>\d+\.?\d*)\*(?<t>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 CC，后续参数形式为 h-t-c1-b1[-c2-b2]。
        /// </summary>
        public static string CFO_CN_2 => @"^CC(?<h>\d+\.?\d*)-(?<t>\d+\.?\d*)-(?<c1>\d+\.?\d*)-(?<b1>\d+\.?\d*)(-(?<c2>\d+\.?\d*)-(?<b2>\d+\.?\d*))?$";
        /// <summary>
        /// <b>* 暂定：</b>前置标识符为 2CM，后续参数形式为 h*b1*c1*t。
        /// </summary>
        public static string CFO_CN_MtM_1 => @"^2CM(?<h>\d+\.?\d*)\*(?<b1>\d+\.?\d*)\*(?<c1>\d+\.?\d*)\*(?<t>\d+\.?\d*)$";
        /// <summary>
        /// <b>* 暂定：</b>前置标识符为 2CCM，后续参数形式为 h-t-c1-b1[-c2-b2]。
        /// </summary>
        public static string CFO_CN_MtM_2 => @"^2CCM(?<h>\d+\.?\d*)-(?<t>\d+\.?\d*)-(?<c1>\d+\.?\d*)-(?<b1>\d+\.?\d*)(-(?<c2>\d+\.?\d*)-(?<b2>\d+\.?\d*))?$";
        /// <summary>
        /// 前置标识符为 2C，后续参数形式为 h*b1*c1*t。
        /// </summary>
        public static string CFO_CN_BtB_1 => @"^2C(?<h>\d+\.?\d*)\*(?<b1>\d+\.?\d*)\*(?<c1>\d+\.?\d*)\*(?<t>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 2CC，后续参数形式为 h-t-c1-b1[-c2-b2]。
        /// </summary>
        public static string CFO_CN_BtB_2 => @"^2CC(?<h>\d+\.?\d*)-(?<t>\d+\.?\d*)-(?<c1>\d+\.?\d*)-(?<b1>\d+\.?\d*)(-(?<c2>\d+\.?\d*)-(?<b2>\d+\.?\d*))?$";
        /// <summary>
        /// 前置标识符为 Z 或 XZ，后续参数形式为 h*b1*c1*t。
        /// </summary>
        public static string CFO_ZJ_1 => @"^X?Z(?<h>\d+\.?\d*)\*(?<b1>\d+\.?\d*)\*(?<c1>\d+\.?\d*)\*(?<t>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 ZZ，后续参数形式为 h-t-c1-b1[-c2-b2]。
        /// </summary>
        public static string CFO_ZJ_2 => @"^ZZ(?<h>\d+\.?\d*)-(?<t>\d+\.?\d*)-(?<c1>\d+\.?\d*)-(?<b1>\d+\.?\d*)(-(?<c2>\d+\.?\d*)-(?<b2>\d+\.?\d*))?$";
        /// <summary>
        /// 前置标识符为 PL，后续参数形式为 t*b[*l]。
        /// <br/>具体实现中 t、b、l 顺序无关，以其中最小值为 t，最大值为 l。
        /// <br/>可以使用 '~' 符号表示尺寸渐变（只可以使用在 b, l 其中之一上）。
        /// 虽然此匹配模式对三个参数均可匹配到 '~' 符号，但在具体实现中将对不符合规则的行为进行屏蔽。
        /// </summary>
        public static string PL_1 => @"^PL(?<t>\d+\.?\d*(~\d+\.?\d*)?)\*(?<b>\d+\.?\d*(~\d+\.?\d*)?)(\*(?<l>\d+\.?\d*(~\d+\.?\d*)?))?$";
        /// <summary>
        /// 前置标识符为 PLT，后续参数形式为 t*b*l。
        /// <br/>具体实现中 t、b、l 顺序无关，以其中最小值为 t，最大值为 l。
        /// <br/>可以使用 '~' 符号表示尺寸渐变（只可以使用在 b, l 其中之一上）。
        /// 虽然此匹配模式对三个参数均可匹配到 '~' 符号，但在具体实现中将对不符合规则的行为进行屏蔽。
        /// </summary>
        public static string PL_T_1 => @"^PLT(?<t>\d+\.?\d*(~\d+\.?\d*)?)\*(?<b>\d+\.?\d*(~\d+\.?\d*)?)\*(?<l>\d+\.?\d*(~\d+\.?\d*)?)$";
        /// <summary>
        /// 前置标识符为 PLD 或 PLO，后续参数形式为 t*d。
        /// <br/>具体实现中，t、d 顺序无关，以其中较小值为 t，较大值为 d。
        /// </summary>
        public static string PL_O_1 => @"^PL[DO](?<t>\d+\.?\d*)\*(?<d>\d+\.?\d*)";
        /// <summary>
        /// nPLt*b*l, nPLTt*b*l, nPLDt*d, nPLOt*d 的任意组合形式。n表示数量。
        /// 各项之间用 + 或 - 连接，分别表示扩展和剔除。
        /// <br/><b> PL 标识符后参数应为完整形式。</b>
        /// <br/>实际使用中，应保持各项参数中的 t 一致。
        /// <br/>例如：2PL14*400*500-1.5PLT14*100.5*115+3PLO14*250。
        /// </summary>
        public static string PL_CMP_1 => @"^-?(\d+\.?\d*)?((PLT?(\d+\.?\d*(~\d+\.?\d*)?)\*(\d+\.?\d*(~\d+\.?\d*)?)\*(\d+\.?\d*(~\d+\.?\d*)?))"
                                        + @"|(PL[DO](\d+\.?\d*)\*(\d+\.?\d*)))"
                                        + @"([+-](\d+\.?\d*)?((PLT?(\d+\.?\d*(~\d+\.?\d*)?)\*(\d+\.?\d*(~\d+\.?\d*)?)\*(\d+\.?\d*(~\d+\.?\d*)?))"
                                        + @"|(PL[DO](\d+\.?\d*)\*(\d+\.?\d*))))*$";
        /// <summary>
        /// <see cref="Pattern_Collection.PL_CMP_1"/>的子项。由两个分组合成：
        /// <list type="bullet">
        ///     <item><b>num: </b>[-][n]。n为数量。</item>
        ///     <item><b>main: </b>PLt*b*l。参见<see cref="Pattern_Collection.PL_1"/>。</item>
        /// </list>
        /// </summary>
        public static string PL_CMP_SUB_PL => @"^(?<num>-?(\d+\.?\d*)?)(?<main>PL\d+\.?\d*(~\d+\.?\d*)?\*\d+\.?\d*(~\d+\.?\d*)?\*\d+\.?\d*(~\d+\.?\d*)?)$";
        /// <summary>
        /// <see cref="Pattern_Collection.PL_CMP_1"/>的子项。由两个分组合成：
        /// <list type="bullet">
        ///     <item><b>num: </b>[-][n]。n为数量。</item>
        ///     <item><b>main: </b>PLTt*b*l。参见<see cref="Pattern_Collection.PL_T_1"/>。</item>
        /// </list>
        /// </summary>
        public static string PL_CMP_SUB_PLT => @"^(?<num>-?(\d+\.?\d*)?)(?<main>PLT\d+\.?\d*(~\d+\.?\d*)?\*\d+\.?\d*(~\d+\.?\d*)?\*\d+\.?\d*(~\d+\.?\d*)?)$";
        /// <summary>
        /// <see cref="Pattern_Collection.PL_CMP_1"/>的子项。由两个分组合成：
        /// <list type="bullet">
        ///     <item><b>num: </b>[-][n]。n为数量。</item>
        ///     <item><b>main: </b>(PLD | PLO)t*d。参见<see cref="Pattern_Collection.PL_O_1"/>。</item>
        /// </list>
        /// </summary>
        public static string PL_CMP_SUB_PLO => @"^(?<num>-?(\d+\.?\d*)?)(?<main>PL[DO]\d+\.?\d*\*\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为SPHERE，后续参数为d。
        /// </summary>
        public static string SPHERE_1 => @"^SPHERE(?<d>\d+\.?\d*)$";
        /// <summary>
        /// 匹配任意标识符。
        /// </summary>
        public static string Classifier => @"^(?<classifier>"
            + @"(B_WLD_A)|(BH)|(B_WLD_H)|(B_WLD_K)|(H)|(HI)|(HM)|(HN)|(HP)|(HT)|(HW)|(PHI)|(WH)|(WI)|(I_VAR_A)|"
            + @"(B_WLD_O)|(HH)|(T)|(TW)|(TM)|(TN)|(B_WLD_E)|(I)|(\[)|(C)|(\[\])|(\]\[)|(2\[)|(2C)|(∠)|(L)|(2∠)|(2L)|"
            + @"(CFRHS)|(F)|(J)|(P)|(RHS)|(SHS)|(TUB)|(B_BUILT)|(B_VAR_A)|(B_VAR_B)|(B_VAR_C)|(B_WLD_F)|(B_WLD_J)|(R)|(RECT)|(RHSC)|"
            + @"(Y)|(Φ)|(φ)|(CFCHS)|(CHS)|(D)|(ELD)|(EPD)|(O)|(PD)|(PIP)|(ROD)|(TUBE)|(CC)|(2CCM)|(2CM)|(2CC)|(XZ)|(Z)|(ZZ)|"
            + @"(-?(\d+\.?\d*)?PL[DOT]?)|(SPHERE))\d";
        /// <summary>
        /// 变截面，形式为 v1~v2，表示尺寸从v1变化到v2。
        /// </summary>
        public static string VariableCrossSection => @"^(?<v1>-?\d+\.?\d*)~(?<v2>-?\d+\.?\d*)$";
    }
}
