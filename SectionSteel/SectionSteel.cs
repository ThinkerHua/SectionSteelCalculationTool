using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;

namespace SectionSteel {
    public class MismatchedProfileTextException : Exception {

    }
    public enum FormulaAccuracyEnum {
        ROUGHLY,
        PRECISELY,
        GBDATA,
    }
    /// <summary>
    /// <para><b>FUNC: </b>"PI()"</para>
    /// <para><b>NUM: </b>"3.14"</para>
    /// </summary>
    public enum PIStyleEnum {
        FUNC,
        NUM,
    }
    public interface ISectionSteel {
        /// <summary>
        /// 型钢截面文本。
        /// </summary>
        string ProfileText { get; set; }
        /// <summary>
        /// 计算式中π的书写样式。
        /// </summary>
        PIStyleEnum PIStyle { get; set; }
        /// <summary>
        /// 获取型钢单位长度表面积计算式。
        /// <para><b>注意：</b>对于<b>变截面型钢</b>，实际上应当结合型钢长度综合考虑才能计算出准确值。
        /// 此处为简化计算，不考虑型钢长度。非变截面型钢不受影响。</para>
        /// </summary>
        /// <param name="accuracy">
        ///     <list type="bullet">
        ///         <item>ROUGHLY: 粗略的</item>
        ///         <item>PRECISELY: 稍精确的</item>
        ///         <item>GBDATA: 国标截面特性表中的理论值</item>
        ///     </list>
        /// </param>
        /// <param name="exclude_topSurface">
        ///     <list type="bullet">
        ///         <item>True: 扣除上表面积(宽度)</item>
        ///         <item>False: 完整单位面积</item>
        ///     </list>
        /// </param>
        /// <returns>单位为m^2/m。</returns>
        string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface);
        /// <summary>
        /// 获取型钢单位长度重量计算式。
        /// <para><b>注意：</b>对于<b>变截面型钢</b>，实际上应当结合型钢长度综合考虑才能计算出准确值。
        /// 此处为简化计算，不考虑型钢长度。非变截面型钢不受影响。</para>
        /// </summary>
        /// <param name="accuracy">
        ///     <para>ROUGHLY: 粗略的</para>
        ///     <para>PRECISELY: 稍精确的</para>
        ///     <para>GBDATA: 国标截面特性表中的理论值</para>
        /// </param>
        /// <returns>单位为kg/m。</returns>
        string GetWeightFormula(FormulaAccuracyEnum accuracy);
        /// <summary>
        /// 获取型钢加劲肋截面文本。
        /// </summary>
        /// <param name="truncatedRounding">截面参数是否截尾取整。</param>
        /// <returns>加劲肋截面文本。</returns>
        string GetSiffenerProfileStr(bool truncatedRounding);
    }
    public class SectionSteelBase {
        /// <summary>
        /// 为字段赋值。
        /// </summary>
        protected virtual void SetFieldsValue() { }
    }
    /// <summary>
    /// 型材截面文本匹配模式的集合
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
        /// 前置标识符为 B_WLD_A，后续参数形式为 h1*b1*s*t1。
        /// </summary>
        public static string H_5 => @"^B_WLD_A(?<h1>\d+\.?\d*)\*(?<b1>\d+\.?\d*)\*(?<s>\d+\.?\d*)\*(?<t1>\d+\.?\d*)$";
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
        /// 前置标识符为 I，后续参数整体用"NAME"分组接收，形式为 CODE[SUFFIX]，CODE为整数或小数，SUFFIX为 "a" || "b" || "c"。
        /// <para>例如：对于I20a，NAME = "20a"，CODE = "20"，SUFFIX = "a"</para>
        /// </summary>
        public static string I_2 => @"^I(?<NAME>(?<CODE>\d+\.?\d*)(?<SUFFIX>[abc]?))$";
        /// <summary>
        /// 前置标识符为 [ 或 C，后续参数形式为 h*b*s。
        /// </summary>
        public static string CHAN_1 => @"^[\[C](?<h>\d+\.?\d*)\*(?<b>\d+\.?\d*)\*(?<s>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 [ 或 C，后续参数整体用"NAME"分组接收，形式为 CODE[SUFFIX]，CODE为整数或小数，SUFFIX 为 "a" || "b" || "c"。
        /// <para>例如：对于[20a，NAME = "20a"，CODE = "20"，SUFFIX = "a"</para>
        /// </summary>
        public static string CHAN_2 => @"^[\[C](?<NAME>(?<CODE>\d+\.?\d*)(?<SUFFIX>[abc]?))$";
        /// <summary>
        /// 前置标识符为 []，后续参数形式为 h*b*s。
        /// </summary>
        public static string CHAN_MtM_1 => @"^\[\](?<h>\d+\.?\d*)\*(?<b>\d+\.?\d*)\*(?<s>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 []，后续参数整体用"NAME"分组接收，形式为 CODE[SUFFIX]，CODE为整数或小数，SUFFIX 为 "a" || "b" || "c"。
        /// <para>例如：对于[]20a，NAME = "20a"，CODE = "20"，SUFFIX = "a"</para>
        /// </summary>
        public static string CHAN_MtM_2 => @"^\[\](?<NAME>(?<CODE>\d+\.?\d*)(?<SUFFIX>[abc]?))$";
        /// <summary>
        /// 前置标识符为 2[ 或 2C 或 ][，后续参数形式为 h*b*s。
        /// </summary>
        public static string CHAN_BtB_1 => @"^((2\[)|(2C)|(\]\[))(?<h>\d+\.?\d*)\*(?<b>\d+\.?\d*)\*(?<s>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 2[ 或 2C 或 ][，后续参数整体用"NAME"分组接收，形式为 CODE[SUFFIX]，CODE为整数或小数，SUFFIX 为 "a" || "b" || "c"。
        /// <para>例如：对于][20a，NAME = "20a"，CODE = "20"，SUFFIX = "a"</para>
        /// </summary>
        public static string CHAN_BtB_2 => @"^((2\[)|(2C)|(\]\[))(?<NAME>(?<CODE>\d+\.?\d*)(?<SUFFIX>[abc]?))$";
        /// <summary>
        /// 前置标识符为 ∠ 或 L，后续参数形式为 h[*b]*t。
        /// </summary>
        public static string L_1 => @"^[∠L](?<h>\d+\.?\d*)(\*(?<b>\d+\.?\d*))?\*(?<t>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 ∠ 或 L，后续参数形式为 h/b，以cm为单位。
        /// </summary>
        public static string L_2 => @"^[∠L](?<h>\d+\.?\d*)/(?<b>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 2∠ 或 2L，后续参数形式为 h[*b]*t。
        /// </summary>
        public static string L_BtB_1 => @"^2[∠L](?<h>\d+\.?\d*)(\*(?<b>\d+\.?\d*))?\*(?<t>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 2∠ 或 2L，后续参数形式为 h/b，以cm为单位。
        /// </summary>
        public static string L_BtB_2 => @"^2[∠L](?<h>\d+\.?\d*)/(?<b>\d+\.?\d*)$";
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
        /// 前置标识符为 B_WLD_F 或 B_BUILT，后续参数形式为 h1*b1*s*t。
        /// </summary>
        public static string RECT_2 => @"^((B_WLD_F)|(B_BUILT))(?<h1>\d+\.?\d*)\*(?<b1>\d+\.?\d*)\*(?<s>\d+\.?\d*)\*(?<t>\d+\.?\d*)$";
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
        /// 前置标识符为 Y 或 φ，后续参数形式为 d*t。
        /// </summary>
        public static string CFH_Y_1 => @"^[Yφ](?<d>\d+\.?\d*)\*(?<t>\d+\.?\d*)$";
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
        /// 前置标识符为 2CM，后续参数形式为 h*b1*c1*t。
        /// <para><b>暂定</b></para>
        /// </summary>
        public static string CFO_CN_MtM_1 => @"^2CM(?<h>\d+\.?\d*)\*(?<b1>\d+\.?\d*)\*(?<c1>\d+\.?\d*)\*(?<t>\d+\.?\d*)$";
        /// <summary>
        /// 前置标识符为 2CCM，后续参数形式为 h-t-c1-b1[-c2-b2]。
        /// <para><b>暂定</b></para>
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
        /// 前置标识符为 PL，后续参数形式为 t*b[*l]，可以使用"~"符号表示尺寸渐变（不应使用在厚度值上）。
        /// <para>实际使用中，t、b、l 顺序无关，以其中最小值为 t，最大值为 l。</para>
        /// </summary>
        public static string PL_1 => @"^PL(?<t>\d+\.?\d*(~\d+\.?\d*)?)\*(?<b>\d+\.?\d*(~\d+\.?\d*)?)(\*(?<l>\d+\.?\d*(~\d+\.?\d*)?))?$";
        /// <summary>
        /// 前置标识符为 PLT，后续参数形式为 t*b*l，可以使用"~"符号表示尺寸渐变（不应使用在厚度值上）。
        /// <para>实际使用中，t、b、l 顺序无关，以其中最小值为 t，最大值为 l。</para>
        /// </summary>
        public static string PL_T_1 => @"^PLT(?<t>\d+\.?\d*(~\d+\.?\d*)?)\*(?<b>\d+\.?\d*(~\d+\.?\d*)?)\*(?<l>\d+\.?\d*(~\d+\.?\d*)?)$";
        /// <summary>
        /// 前置标识符为 PLD 或 PLO，后续参数形式为 t*d。
        /// <para>实际使用中，t、d 顺序无关，以其中较小值为 t，较大值为 d。</para>
        /// </summary>
        public static string PL_O_1 => @"^PL[DO](?<t>\d+\.?\d*)\*(?<d>\d+\.?\d*)";
        /// <summary>
        /// nPLt*b*l, nPLTt*b*l, nPLDt*d, nPLOt*d 的任意组合形式。n表示数量。
        /// 各项之间用 + 或 - 连接，分别表示扩展和剔除。
        /// <para><b>PL标识符后参数应为完整形式。</b></para>
        /// <para>实际使用中，应保持各项参数中的 t 一致。</para>
        /// <para>例如：2PL14*400*500-1.5PLT14*100.5*115+3PLO14*250。</para>
        /// </summary>
        public static string PL_CMP_1 => @"^(\d+\.?\d*)?((PLT?(\d+\.?\d*(~\d+\.?\d*)?)\*(\d+\.?\d*(~\d+\.?\d*)?)\*(\d+\.?\d*(~\d+\.?\d*)?))"
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
            + @"(B_WLD_A)|(B_WLD_H)|(B_WLD_K)|(H)|(HI)|(HM)|(HN)|(HP)|(HT)|(HW)|(PHI)|(WH)|(WI)|(I_VAR_A)|"
            + @"(B_WLD_O)|(HH)|(T)|(TW)|(TM)|(TN)|(B_WLD_E)|(I)|(\[)|(C)|(\[\])|(\]\[)|(2\[)|(2C)|(∠)|(L)|(2∠)|(2L)|"
            + @"(CFRHS)|(F)|(J)|(P)|(RHS)|(SHS)|(TUB)|(B_BUILT)|(B_VAR_A)|(B_VAR_B)|(B_VAR_C)|(B_WLD_F)|(B_WLD_J)|(R)|(RHSC)|"
            + @"(Y)|(φ)|(CFCHS)|(CHS)|(D)|(ELD)|(EPD)|(O)|(PD)|(PIP)|(ROD)|(TUBE)|(CC)|(2CCM)|(2CM)|(2CC)|(XZ)|(Z)|(ZZ)|"
            + @"(-?(\d+\.?\d*)?PL)|(-?(\d+\.?\d*)?PLD)|(-?(\d+\.?\d*)?PLO)|(-?(\d+\.?\d*)?PLT)|(SPHERE))\d";
        /// <summary>
        /// 变截面，形式为 v1~v2，表示尺寸从v1变化到v2。
        /// </summary>
        public static string VariableCrossSection => @"^(?<v1>-?\d+\.?\d*)~(?<v2>-?\d+\.?\d*)$";
    }
    /// <summary>
    /// <para>H型钢。在以下模式中尝试匹配：</para>
    /// <para><see cref="Pattern_Collection.H_1"/></para>
    /// <para><see cref="Pattern_Collection.H_2"/></para>
    /// <para><see cref="Pattern_Collection.H_3"/></para>
    /// <para><see cref="Pattern_Collection.H_4"/></para>
    /// <para><see cref="Pattern_Collection.H_5"/></para>
    /// <para><see cref="Pattern_Collection.H_6"/></para>
    /// <para><see cref="Pattern_Collection.H_7"/></para>
    /// <para>当匹配到H3模式时，在国标截面特性表格中查找，同一型号名下有多项时按以下规则进行匹配：</para>
    /// <list type="number">
    ///     <item>
    ///         <para>匹配<b>非星标型号</b>，例如：</para>
    ///         <para>HW200*200型号名下，有H200*200*8*12和H200*204*12*12两种型号，其中第1种不带星标，第2种带星标。此时匹配第1种。</para>
    ///     </item>
    ///     <item>
    ///         <para>如均为星标型号，则匹配<b>参数与型号名一致的型号</b>，例如：</para>
    ///         <para>HM550*300型号名下，有H544*300*11*15和H550*300*11*18两种型号，均带星标，但第2种参数与型号名一致。此时匹配第2种。</para>
    ///     </item>
    ///     <item>
    ///         <para>如<b>均不符合</b>，则匹配<b>第1项</b>，例如：</para>
    ///         <para>HW500*500型号名下，有H492*465*15*20、H502*465*15*25、H502*470*20*25三种型号，均带星标，且三种参数均与型号名不致。此时匹配第1种。</para>
    ///     </item>
    /// </list>
    /// </summary>
    public class SectionSteel_H : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private string type;
        private double h1, h2, b1, b2, s, t1, t2;
        private GBDataBase data;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_H() {
        }
        public SectionSteel_H(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            type = string.Empty;
            h1 = h2 = b1 = b2 = s = t1 = t2 = 0;
            data = null;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.H_1);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.H_2);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.H_4);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.H_5);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.H_6);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.H_7);
                if (match.Success) {
                    double.TryParse(match.Groups["h1"].Value, out h1);
                    double.TryParse(match.Groups["h2"].Value, out h2);
                    double.TryParse(match.Groups["b1"].Value, out b1);
                    double.TryParse(match.Groups["b2"].Value, out b2);
                    double.TryParse(match.Groups["s"].Value, out s);
                    double.TryParse(match.Groups["t1"].Value, out t1);
                    double.TryParse(match.Groups["t2"].Value, out t2);

                    if (h2 != 0 && h2 != h1
                        || b2 != 0 && b2 != b1
                        || t2 != 0 && t2 != t1)
                        ;
                    else
                        data = GetGBData(new double[] { h1, b1, s, t1 });
                } else {
                    match = Regex.Match(ProfileText, Pattern_Collection.H_3);
                    if (!match.Success)
                        throw new MismatchedProfileTextException();

                    type = match.Groups["TYPE"].Value;
                    string name = match.Groups["H"] + "*" + match.Groups["B"];
                    data = GetGBData(name);
                    if (data == null) {
                        double.TryParse(match.Groups["H"].Value, out double H);
                        double.TryParse(match.Groups["B"].Value, out double B);
                        data = GetGBData(new double[] { H, B });
                    }
                    if (data == null)
                        throw new MismatchedProfileTextException();

                    h1 = data.Parameters[0];
                    b1 = data.Parameters[1];
                    s = data.Parameters[2];
                    t1 = data.Parameters[3];
                }

                if (h2 == 0) h2 = h1;
                if (b2 == 0) b2 = b1;
                if (t2 == 0) t2 = t1;

                h1 *= 0.001; h2 *= 0.001; b1 *= 0.001; b2 *= 0.001; s *= 0.001; t1 *= 0.001; t2 *= 0.001;
            } catch (MismatchedProfileTextException) {
                type = null;
                h1 = h2 = b1 = b2 = s = t1 = t2 = 0;
                data = null;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public virtual string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (h1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
                //byte key = 0b0000;
                //if (h2 != 0 && h2 != h1)
                //    key |= 0b0001;
                //if (b2 != 0 && b2 != b1)
                //    key |= 0b0010;
                //if (exclude_topSurface)
                //    key |= 0b0100;

                //switch (key) {
                //case 0b0000:
                //    formula = $"{h1}*2+{b1}*4";
                //    break;
                //case 0b0100:
                //    formula = $"{h1}*2+{b1}*3";
                //    break;
                //case 0b0010:
                //    formula = $"{h1}*2+{b1}*2+{b2}*2";
                //    break;
                //case 0b0110:
                //    formula = $"{h1}*2+{b1}+{b2}*2";
                //    break;
                //case 0b0001:
                //    formula = $"{h1}+{h2}+{b1}*4";
                //    break;
                //case 0b0101:
                //    formula = $"{h1}+{h2}+{b1}*3";
                //    break;
                //case 0b0011:
                //    formula = $"{h1}+{h2}+{b1}*2+{b2}*2";
                //    break;
                //case 0b0111:
                //    formula = $"{h1}+{h2}+{b1}+{b2}*2";
                //    break;
                //default:
                //    break;
                //}
                if (h2 != h1) {
                    formula = h1 + "+" + h2;
                } else {
                    formula = h1 + "*2";
                }
                if (b2 != b1) {
                    if (exclude_topSurface) {
                        formula += "+" + b1 + "+" + b2 + "*2";
                    } else {
                        formula += "+" + b1 + "*2+" + b2 + "*2";
                    }
                } else {
                    if (exclude_topSurface) {
                        formula += "+" + b1 + "*3";
                    } else {
                        formula += "+" + b1 + "*4";
                    }
                }
                break;
            case FormulaAccuracyEnum.PRECISELY:
                formula = this.GetAreaFormula(FormulaAccuracyEnum.ROUGHLY, exclude_topSurface);
                formula += $"-{s}*2";
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data == null)
                    break;
                formula = $"{data.Area}";
                if (exclude_topSurface)
                    formula += $"-{b1}";
                break;
            default:
                break;
            }

            return formula;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetWeightFormula(FormulaAccuracyEnum)" path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public virtual string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (h1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                //byte key = 0b0000;
                //if(h2 != 0 && h2 != h1)
                //    key |= 0b0001;
                //if(b2 != 0 && b2 != b1)
                //    key |= 0b0010;
                //if(t2 != 0 && t2 != t1)
                //    key |= 0b0100;

                //switch(key) {
                //case 0b0000:
                //    formula = $"(({h1}-{t1}*2)*{s}+{b1}*{t1}*2)*{GBData.DENSITY}";
                //    break;
                //case 0b0001:
                //    formula = $"(({(h1 + h2) * 0.5}-{t1}*2)*{s}+{b1}*{t1}*2)*{GBData.DENSITY}";
                //    break;
                //case 0b0010:
                //    formula = $"(({h1}-{t1}*2)*{s}+({b1}+{b2})*{t1})*{GBData.DENSITY}";
                //    break;
                //case 0b0011:
                //    formula = $"(({(h1 + h2) * 0.5}-{t1}*2)*{s}+({b1}+{b2})*{t1})*{GBData.DENSITY}";
                //    break;
                //case 0b0100:
                //    formula = $"(({h1}-{t1}-{t2})*{s}+{b1}*({t1}+{t2}))*{GBData.DENSITY}";
                //    break;
                //case 0b0101:
                //    formula = $"(({(h1 + h2) * 0.5}-{t1}-{t2})*{s}+{b1}*({t1}+{t2}))*{GBData.DENSITY}";
                //    break;
                //case 0b0110:
                //    formula = $"(({h1}-{t1}-{t2})*{s}+{b1}*{t1}+{b2}*{t2})*{GBData.DENSITY}";
                //    break;
                //case 0b0111:
                //    formula = $"(({(h1 + h2) * 0.5}-{t1}-{t2})*{s}+{b1}*{t1}+{b2}*{t2})*{GBData.DENSITY}";
                //    break;
                //default:
                //    break;
                //}
                if (h2 != h1) {
                    formula = "((" + (h1 + h2) / 2;
                } else {
                    formula = "((" + h1;
                }
                if (t2 != t1) {
                    formula += "-" + t1 + "-" + t2;
                } else {
                    formula += "-" + t1 + "*2";
                }
                formula += ")*" + s;
                if (b2 != b1) {
                    if (t2 != t1) {
                        formula += "+" + b1 + "*" + t1 + "+" + b2 + "*" + t2 + ")";
                    } else {
                        formula += "+(" + b1 + "+" + b2 + ")*" + t1 + ")";
                    }
                } else {
                    if (t2 != t1) {
                        formula += "+" + b1 + "*(" + t1 + "+" + t2 + "))";
                    } else {
                        formula += "+" + b1 + "*" + t1 + "*2)";
                    }
                }
                formula += "*" + GBData.DENSITY;
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data != null)
                    formula = $"{data.Weight}";
                break;
            default:
                break;
            }

            return formula;
        }

        public virtual string GetSiffenerProfileStr(bool truncatedRounding) {
            string stifProfileText = string.Empty;
            if (h1 == 0) return stifProfileText;

            double t, b, l;
            t = s;
            b = ((b1 + b2) * 0.5 - s) * 0.5;
            l = (h1 + h2) * 0.5 - t1 - t2;
            t *= 1000; b *= 1000; l *= 1000;
            if (truncatedRounding) {
                t = Math.Truncate(t);
                b = Math.Truncate(b);
                l = Math.Truncate(l);
            }
            stifProfileText = $"PL{t}*{b}*{l}";

            return stifProfileText;
        }
        private GBDataBase GetGBData(string byName) {
            GBDataBase data;
            switch (type) {
            case "HW":
                data = GBData.SearchGBData(GBData.HW, byName);
                break;
            case "HM":
                data = GBData.SearchGBData(GBData.HM, byName);
                break;
            case "HN":
                data = GBData.SearchGBData(GBData.HN, byName);
                break;
            case "HT":
                data = GBData.SearchGBData(GBData.HT, byName);
                break;
            case "H":
            default:
                data = GBData.SearchGBData(GBData.HW, byName);
                if (data == null)
                    data = GBData.SearchGBData(GBData.HM, byName);
                if (data == null)
                    data = GBData.SearchGBData(GBData.HN, byName);
                if (data == null)
                    data = GBData.SearchGBData(GBData.HT, byName);
                break;
            }
            return data;
        }
        private GBDataBase GetGBData(double[] byParameters) {
            GBDataBase data;
            switch (type) {
            case "HW":
                data = GBData.SearchGBData(GBData.HW, byParameters);
                break;
            case "HM":
                data = GBData.SearchGBData(GBData.HM, byParameters);
                break;
            case "HN":
                data = GBData.SearchGBData(GBData.HN, byParameters);
                break;
            case "HT":
                data = GBData.SearchGBData(GBData.HT, byParameters);
                break;
            case "H":
            default:
                data = GBData.SearchGBData(GBData.HW, byParameters);
                if (data == null)
                    data = GBData.SearchGBData(GBData.HM, byParameters);
                if (data == null)
                    data = GBData.SearchGBData(GBData.HN, byParameters);
                if (data == null)
                    data = GBData.SearchGBData(GBData.HT, byParameters);
                break;
            }
            return data;
        }
    }
    /// <summary>
    /// <para>十字交叉焊接H型钢。在以下模式中尝试匹配：</para>
    /// <para><see cref="Pattern_Collection.HH_1"/></para>
    /// <para><see cref="Pattern_Collection.HH_2"/></para>
    /// </summary>
    public class SectionSteel_HH : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private double h1, h2, b1, b2, s1, s2, t1, t2;
        private GBDataBase data1, data2;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_HH() {

        }
        public SectionSteel_HH(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            h1 = h2 = b1 = b2 = s1 = s2 = t1 = t2 = 0;
            data1 = data2 = null;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.HH_1);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.HH_2);
                if (!match.Success)
                    throw new MismatchedProfileTextException();
                double.TryParse(match.Groups["h1"].Value, out h1);
                double.TryParse(match.Groups["b1"].Value, out b1);
                double.TryParse(match.Groups["s1"].Value, out s1);
                double.TryParse(match.Groups["t1"].Value, out t1);
                double.TryParse(match.Groups["h2"].Value, out h2);
                double.TryParse(match.Groups["b2"].Value, out b2);
                double.TryParse(match.Groups["s2"].Value, out s2);
                double.TryParse(match.Groups["t2"].Value, out t2);

                double[] parameters1 = { h1, b1, s1, t1 };
                data1 = GetGBData(parameters1);
                if (h2 == 0 && b2 == 0 && s2 == 0 && t2 == 0) {
                    h2 = h1; b2 = b1; s2 = s1; t2 = t1;
                }
                if (h2 == h1 && b2 == b1 && s2 == s1 && t2 == t1)
                    data2 = data1;
                else {
                    double[] parameters2 = { h2, b2, s2, t2 };
                    data2 = GetGBData(parameters2);
                }

                h1 *= 0.001; b1 *= 0.001; s1 *= 0.001; t1 *= 0.001;
                h2 *= 0.001; b2 *= 0.001; s2 *= 0.001; t2 *= 0.001;
            } catch (MismatchedProfileTextException) {
                h1 = h2 = b1 = b2 = s1 = s2 = t1 = t2 = 0;
                //data1 = data2 = null;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (h1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
                if (h2 != h1) {
                    formula = h1 + "*2+" + h2 + "*2+";
                } else {
                    formula = h1 + "*4+";
                }
                if (b2 != b1) {
                    if (exclude_topSurface) {
                        formula += b1 + "*3+" + b2 + "*4";
                    } else {
                        formula += "(" + b1 + b2 + ")*4";
                    }
                } else {
                    if (exclude_topSurface) {
                        formula += b1 + "*7";
                    } else {
                        formula += b1 + "*8";
                    }
                }
                break;
            case FormulaAccuracyEnum.PRECISELY:
                formula = this.GetAreaFormula(FormulaAccuracyEnum.ROUGHLY, exclude_topSurface);
                if (s2 != s1) {
                    formula += "-" + s1 + "*4-" + s2 + "*4";
                } else {
                    formula += "-" + s1 + "*8";
                }
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data2 == null || data1 == null)
                    break;
                if (data2.Equals(data1)) {
                    formula = data1.Area + "*2";
                } else {
                    formula = data1.Area + "+" + data2.Area;
                }
                if (s2 != s1) {
                    formula += "-" + s1 + "*2-" + s2 + "*2";
                } else {
                    formula += "-" + s1 + "*4";
                }
                if (exclude_topSurface)
                    formula += "-" + b1;
                break;
            default:
                break;
            }

            return formula;
        }

        public string GetSiffenerProfileStr(bool truncatedRounding) {
            string stifProfileText = string.Empty;
            if (h1 == 0) return stifProfileText;

            double t, b, l;
            t = Math.Max(s1, s2);
            b = (h2 - t2 * 2 - s1) * 0.5;
            l = (h1 - t1 * 2 - s2) * 0.5;
            t *= 1000; b *= 1000; l *= 1000;
            if (truncatedRounding) {
                t = Math.Truncate(t);
                b = Math.Truncate(b);
                l = Math.Truncate(l);
            }
            stifProfileText = $"PL{t}*{b}*{l}";

            return stifProfileText;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetWeightFormula(FormulaAccuracyEnum)" path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (h1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (h2 != h1) {
                    if (s2 != s1) {
                        formula = "((" + h1 + "-" + t1 + "*2)*" + s1 + "+(" + h2 + "-" + t2 + "*2)*" + s2;
                    } else {
                        if (t2 != t1) {
                            formula = "((" + h1 + "-" + t1 + "*2+" + h2 + "-" + t2 + "*2)*" + s1;
                        } else {
                            formula = "((" + h1 + "+" + h2 + "-" + t1 + "*4)*" + s1;
                        }
                    }
                } else {
                    formula = "((" + h1;
                    if (t2 != t1) {
                        formula += "-" + t1 + "-" + t2 + ")*";
                        if (s2 != s1) {
                            formula += "(" + s1 + "+" + s2 + ")";
                        } else {
                            formula += s1 + "*2";
                        }
                    } else {
                        formula += "-" + t1 + "*2)*";
                        if (s2 != s1) {
                            formula += "(" + s1 + "+" + s2 + ")";
                        } else {
                            formula += s1 + "*2";
                        }
                    }
                }
                if (s2 != s1) {
                    formula += "-" + s1 + "*" + s2;
                } else {
                    formula += "-" + s1 + "*" + s1;
                }
                if (b2 != b1) {
                    if (t2 != t1) {
                        formula += "+(" + b1 + "*" + t1 + "+" + b2 + "*" + t2 + ")*2)";
                    } else {
                        formula += "+(" + b1 + "+" + b2 + ")*" + t1 + "*2)";
                    }
                } else {
                    if (t2 != t1) {
                        formula += "+" + b1 + "*(" + t1 + "+" + t2 + ")*2)";
                    } else {
                        formula += "+" + b1 + "*" + t1 + "*4)";
                    }
                }
                formula += "*" + GBData.DENSITY;
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data2 == null || data1 == null)
                    break;
                if (data2.Equals(data1)) {
                    formula = data1.Weight + "*2";
                } else {
                    formula = data1.Weight + "+" + data2.Weight;
                }
                if (s2 != s1) {
                    formula += "-" + s1 + "*" + s2 + "*" + GBData.DENSITY;
                } else {
                    formula += "-" + s1 + "*" + s1 + "*" + GBData.DENSITY;
                }
                break;
            default:
                break;
            }
            return formula;
        }
        private GBDataBase GetGBData(double[] byParameters) {
            GBDataBase data;
            data = GBData.SearchGBData(GBData.HW, byParameters);
            if (data == null)
                data = GBData.SearchGBData(GBData.HM, byParameters);
            if (data == null)
                data = GBData.SearchGBData(GBData.HN, byParameters);
            if (data == null)
                data = GBData.SearchGBData(GBData.HT, byParameters);
            return data;
        }
    }
    /// <summary>
    /// <para>T型钢。在以下模式中尝试匹配：</para>
    /// <para><see cref="Pattern_Collection.T_1"/></para>
    /// <para><see cref="Pattern_Collection.T_2"/></para>
    /// <para><see cref="Pattern_Collection.T_3"/></para>
    /// <para>当匹配到T3模式时，在国标截面特性表格中查找，同一型号名下有多项时按最接近项匹配。</para>
    /// </summary>
    public class SectionSteel_T : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private string type;
        private double h1, h2, b, s, t;
        private GBDataBase data;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_T() {

        }
        public SectionSteel_T(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            type = string.Empty;
            h1 = h2 = b = s = t = 0;
            data = null;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.T_1);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.T_2);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.T_4);
                if (match.Success) {
                    double.TryParse(match.Groups["h1"].Value, out h1);
                    double.TryParse(match.Groups["h2"].Value, out h2);
                    double.TryParse(match.Groups["b"].Value, out b);
                    double.TryParse(match.Groups["s"].Value, out s);
                    double.TryParse(match.Groups["t"].Value, out t);

                    if (h2 == 0 || h2 == h1) {
                        double[] parameters = { h1, b, s, t };
                        data = GetGBData(parameters);
                    }
                } else {
                    match = Regex.Match(ProfileText, Pattern_Collection.T_3);
                    if (!match.Success)
                        throw new MismatchedProfileTextException();

                    type = match.Groups["TYPE"].Value;
                    string name = match.Groups["H"].Value + "*" + match.Groups["B"].Value;
                    data = GetGBData(name);
                    if (data == null) {
                        double.TryParse(match.Groups["H"].Value, out double H);
                        double.TryParse(match.Groups["B"].Value, out double B);
                        data = GetGBData(new double[] { H, B });
                    }
                    if (data == null)
                        throw new MismatchedProfileTextException();

                    h1 = data.Parameters[0];
                    b = data.Parameters[1];
                    s = data.Parameters[2];
                    t = data.Parameters[3];
                }

                if (h2 == 0) h2 = h1;
                h1 *= 0.001; h2 *= 0.001; b *= 0.001; s *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {
                type = null;
                h1 = h2 = b = s = t = 0;
                data = null;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// <para><b>在本类中：PRECISELY 等效于 ROUGHLY</b></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (h1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (h2 != h1) {
                    formula = $"{(h1 + h2) * 0.5}";
                } else {
                    formula = $"{h1}";
                }
                formula += $"*2+{b}";
                if (!exclude_topSurface)
                    formula += $"*2";
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data == null)
                    break;
                formula = $"{data.Area}";
                if (exclude_topSurface)
                    formula += $"-{b}";
                break;
            default:
                break;
            }

            return formula;
        }

        public string GetSiffenerProfileStr(bool truncatedRounding) {
            string stifProfileText = string.Empty;
            if (h1 == 0) return stifProfileText;

            double t, b, l;
            t = s;
            b = (this.b - s) * 0.5;
            l = (h1 + h2) * 0.5 - this.t;
            t *= 1000; b *= 1000; l *= 1000;
            if (truncatedRounding) {
                t = Math.Truncate(t);
                b = Math.Truncate(b);
                l = Math.Truncate(l);
            }
            stifProfileText = $"PL{t}*{b}*{l}";

            return stifProfileText;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">ROUGHLY 等效于 PRECISELY</param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (h1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (h2 != h1) {
                    formula = $"(({(h1 + h2) * 0.5}-{t})*{s}+{b}*{t})*{GBData.DENSITY}";
                } else {
                    formula = $"(({h1}-{t})*{s}+{b}*{t})*{GBData.DENSITY}";
                }
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data != null)
                    formula = $"{data.Weight}";
                break;
            default:
                break;
            }

            return formula;
        }
        private GBDataBase GetGBData(string byName) {
            GBDataBase data;
            switch (type) {
            case "TW":
                data = GBData.SearchGBData(GBData.TW, byName);
                break;
            case "TM":
                data = GBData.SearchGBData(GBData.TM, byName);
                break;
            case "TN":
                data = GBData.SearchGBData(GBData.TN, byName);
                break;
            case "T":
            default:
                data = GBData.SearchGBData(GBData.TW, byName);
                if (data == null)
                    data = GBData.SearchGBData(GBData.TM, byName);
                if (data == null)
                    data = GBData.SearchGBData(GBData.TN, byName);
                break;
            }
            return data;
        }
        private GBDataBase GetGBData(double[] byParameters) {
            GBDataBase data;
            switch (type) {
            case "TW":
                data = GBData.SearchGBData(GBData.TW, byParameters);
                break;
            case "TM":
                data = GBData.SearchGBData(GBData.TM, byParameters);
                break;
            case "TN":
                data = GBData.SearchGBData(GBData.TN, byParameters);
                break;
            case "T":
            default:
                data = GBData.SearchGBData(GBData.TW, byParameters);
                if (data == null)
                    data = GBData.SearchGBData(GBData.TM, byParameters);
                if (data == null)
                    data = GBData.SearchGBData(GBData.TN, byParameters);
                break;
            }
            return data;
        }
    }
    /// <summary>
    /// <para>工字钢。在以下模式中尝试匹配：</para>
    /// <para><see cref="Pattern_Collection.I_1"/></para>
    /// <para><see cref="Pattern_Collection.I_2"/></para>
    /// <para>匹配到两种模式时，均在国标截面特性表格中查找。</para>
    /// <para>I2模式下，当型号大于等于20号且无后缀时，按后缀为"a"处理。</para>
    /// </summary>
    public class SectionSteel_I : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private double h, b, s, t;
        private GBDataBase data;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_I() {

        }
        public SectionSteel_I(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            h = b = s = t = 0;
            data = null;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.I_1);
                if (match.Success) {
                    double.TryParse(match.Groups["h"].Value, out h);
                    double.TryParse(match.Groups["b"].Value, out b);
                    double.TryParse(match.Groups["s"].Value, out s);
                    double[] parameters = { h, b, s };
                    data = GBData.SearchGBData(GBData.I, parameters);
                    if (data == null)
                        throw new MismatchedProfileTextException();

                    t = data.Parameters[3];
                } else {
                    match = Regex.Match(ProfileText, Pattern_Collection.I_2);
                    if (!match.Success)
                        throw new MismatchedProfileTextException();

                    string name, suffix;
                    double.TryParse(match.Groups["CODE"].Value, out double code);
                    suffix = match.Groups["SUFFIX"].Value;
                    name = match.Groups["NAME"].Value;
                    if (code >= 20 && string.IsNullOrEmpty(suffix))
                        name += "a";
                    data = GBData.SearchGBData(GBData.I, name);
                    if (data == null)
                        throw new MismatchedProfileTextException();

                    h = data.Parameters[0];
                    b = data.Parameters[1];
                    s = data.Parameters[2];
                    t = data.Parameters[3];
                }

                h *= 0.001; b *= 0.001; s *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {
                h = s = b = t = 0;
                data = null;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (h == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
                formula = $"{h}*2+{b}";
                if (exclude_topSurface)
                    formula += $"*3";
                else
                    formula += $"*4";
                break;
            case FormulaAccuracyEnum.PRECISELY:
                formula = this.GetAreaFormula(FormulaAccuracyEnum.ROUGHLY, exclude_topSurface);
                formula += $"-{s}*2";
                break;
            case FormulaAccuracyEnum.GBDATA:
                //构造成功必定给data字段分配值
                formula = $"{data.Area}";
                if (exclude_topSurface)
                    formula += $"-{b}";
                break;
            default:
                break;
            }

            return formula;
        }

        public string GetSiffenerProfileStr(bool truncatedRounding) {
            string stifProfileText = string.Empty;
            if (h == 0) return stifProfileText;

            double t, b, l;
            t = s;
            b = (this.b - s) * 0.5;
            l = h - this.t * 2;
            t *= 1000; b *= 1000; l *= 1000;
            if (truncatedRounding) {
                t = Math.Truncate(t);
                b = Math.Truncate(b);
                l = Math.Truncate(l);
            }
            stifProfileText = $"PL{t}*{b}*{l}";

            return stifProfileText;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetWeightFormula(FormulaAccuracyEnum)" path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY, PRECISELY 均等效于 GBDATA</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;

            if (data != null)
                formula = $"{data.Weight}";

            return formula;
        }
    }
    /// <summary>
    /// <para>槽钢。在以下模式中尝试匹配：</para>
    /// <para><see cref="Pattern_Collection.CHAN_1"/></para>
    /// <para><see cref="Pattern_Collection.CHAN_2"/></para>
    /// <para>匹配到两种模式时，均在国标截面特性表格中查找。</para>
    /// <para>CHAN_2模式下，当型号大于等于14号且无后缀时，按后缀为"a"处理。</para>
    /// </summary>
    public class SectionSteel_CHAN : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private double h, b, s, t;
        private GBDataBase data;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_CHAN() {

        }
        public SectionSteel_CHAN(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            h = b = s = t = 0;
            data = null;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.CHAN_1);
                if (match.Success) {
                    double.TryParse(match.Groups["h"].Value, out h);
                    double.TryParse(match.Groups["b"].Value, out b);
                    double.TryParse(match.Groups["s"].Value, out s);
                    double[] parameters = { h, b, s };
                    data = GBData.SearchGBData(GBData.CHAN, parameters);
                    if (data == null)
                        throw new MismatchedProfileTextException();

                    t = data.Parameters[3];
                } else {
                    match = Regex.Match(ProfileText, Pattern_Collection.CHAN_2);
                    if (!match.Success)
                        throw new MismatchedProfileTextException();

                    string name, suffix;
                    double.TryParse(match.Groups["CODE"].Value, out double code);
                    suffix = match.Groups["SUFFIX"].Value;
                    name = match.Groups["NAME"].Value;
                    if (code >= 14 && string.IsNullOrEmpty(suffix))
                        name += "a";

                    data = GBData.SearchGBData(GBData.CHAN, name);
                    if (data == null)
                        throw new MismatchedProfileTextException();

                    h = data.Parameters[0];
                    b = data.Parameters[1];
                    s = data.Parameters[2];
                    t = data.Parameters[3];
                }

                h *= 0.001; b *= 0.001; s *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {
                h = b = s = t = 0;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (h == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
                formula = $"{h}*2+{b}";
                if (exclude_topSurface)
                    formula += "*3";
                else
                    formula += "*4";
                break;
            case FormulaAccuracyEnum.PRECISELY:
                formula = this.GetAreaFormula(FormulaAccuracyEnum.ROUGHLY, exclude_topSurface);
                formula += $"-{s}*2";
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data == null)
                    return formula;

                formula = $"{data.Area}";
                if (exclude_topSurface)
                    formula += $"-{b}";
                break;
            default:
                break;
            }

            return formula;
        }

        public string GetSiffenerProfileStr(bool truncatedRounding) {
            string stifProfileText = string.Empty;
            if (h == 0) return stifProfileText;

            double t, b, l;
            t = s;
            b = this.b - s;
            l = h - this.t * 2;
            t *= 1000; b *= 1000; l *= 1000;
            if (truncatedRounding) {
                t = Math.Truncate(t);
                b = Math.Truncate(b);
                l = Math.Truncate(l);
            }
            stifProfileText = $"PL{t}*{b}*{l}";

            return stifProfileText;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetWeightFormula(FormulaAccuracyEnum)" path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY, PRECISELY 均等效于 GBDATA</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;

            if (data != null)
                formula = $"{data.Weight}";

            return formula;
        }
    }
    /// <summary>
    /// <para>槽钢，口对口双拼。在以下模式中尝试匹配：</para>
    /// <para><see cref="Pattern_Collection.CHAN_MtM_1"/></para>
    /// <para><see cref="Pattern_Collection.CHAN_MtM_2"/></para>
    /// <para>匹配到两种模式时，均在国标截面特性表格中查找。</para>
    /// <para>CHAN_MtM_2模式下，当型号大于等于14号且无后缀时，按后缀为"a"处理。</para>
    /// </summary>
    public class SectionSteel_CHAN_MtM : SectionSteelBase, ISectionSteel {
        private string _profileText;
        double h, b, s, t;
        GBDataBase data;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_CHAN_MtM() {

        }
        public SectionSteel_CHAN_MtM(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            h = b = s = t = 0;
            data = null;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.CHAN_MtM_1);
                if (match.Success) {
                    double.TryParse(match.Groups["h"].Value, out h);
                    double.TryParse(match.Groups["b"].Value, out b);
                    double.TryParse(match.Groups["s"].Value, out s);

                    data = GBData.SearchGBData(GBData.CHAN, new double[] { h, b, s });
                    if (data == null)
                        throw new MismatchedProfileTextException();

                    t = data.Parameters[3];
                } else {
                    match = Regex.Match(ProfileText, Pattern_Collection.CHAN_MtM_2);
                    if (!match.Success)
                        throw new MismatchedProfileTextException();

                    string name, suffix;
                    double.TryParse(match.Groups["CODE"].Value, out double code);
                    suffix = match.Groups["SUFFIX"].Value;
                    name = match.Groups["NAME"].Value;

                    if (code >= 14 && string.IsNullOrEmpty(suffix))
                        name += "a";

                    data = GBData.SearchGBData(GBData.CHAN, name);
                    if (data == null)
                        throw new MismatchedProfileTextException();

                    h = data.Parameters[0];
                    b = data.Parameters[1];
                    s = data.Parameters[2];
                    t = data.Parameters[3];
                }

                h *= 0.001; b *= 0.001; s *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {
                h = b = t = 0;
                data = null;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// <para><b>在本类中：PRECISELY, GBDATA 均等效于 ROUGHLY</b></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (h == 0) return formula;

            formula = $"{h}*2+{b}";
            if (exclude_topSurface)
                formula += "*2";
            else
                formula += "*4";

            return formula;
        }

        public string GetSiffenerProfileStr(bool truncatedRounding) {
            string stifProfileText = string.Empty;
            if (h == 0) return stifProfileText;

            double t, b, l;
            t = s;
            b = (this.b - s) * 2;
            l = h - this.t * 2;
            t *= 1000; b *= 1000; l *= 1000;
            if (truncatedRounding) {
                t = Math.Truncate(t);
                b = Math.Truncate(b);
                l = Math.Truncate(l);
            }
            stifProfileText = $"PL{t}*{b}*{l}";

            return stifProfileText;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetWeightFormula(FormulaAccuracyEnum)" path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY, PRECISELY 均等效于 GBDATA</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;

            if (data != null)
                formula = $"{data.Weight}*2";

            return formula;
        }
    }
    /// <summary>
    /// <para>槽钢，背对背双拼。在以下模式中尝试匹配：</para>
    /// <para><see cref="Pattern_Collection.CHAN_BtB_1"/></para>
    /// <para><see cref="Pattern_Collection.CHAN_BtB_2"/></para>
    /// <para>匹配到两种模式时，均在国标截面特性表格中查找。</para>
    /// <para>CHAN_BtB_2模式下，当型号大于等于14号且无后缀时，按后缀为"a"处理。</para>
    /// </summary>
    public class SectionSteel_CHAN_BtB : SectionSteelBase, ISectionSteel {
        private string _profileText;
        double h, b, s, t;
        GBDataBase data;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_CHAN_BtB() {

        }
        public SectionSteel_CHAN_BtB(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            h = b = s = t = 0;
            data = null;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.CHAN_BtB_1);
                if (match.Success) {
                    double.TryParse(match.Groups["h"].Value, out h);
                    double.TryParse(match.Groups["b"].Value, out b);
                    double.TryParse(match.Groups["s"].Value, out s);

                    data = GBData.SearchGBData(GBData.CHAN, new double[] { h, b, s });
                    if (data == null)
                        throw new MismatchedProfileTextException();

                    t = data.Parameters[3];
                } else {
                    match = Regex.Match(ProfileText, Pattern_Collection.CHAN_BtB_2);
                    if (!match.Success)
                        throw new MismatchedProfileTextException();

                    string name, suffix;
                    double.TryParse(match.Groups["CODE"].Value, out double code);
                    name = match.Groups["NAME"].Value;
                    suffix = match.Groups["SUFFIX"].Value;

                    if (code >= 14 && string.IsNullOrEmpty(suffix))
                        name += "a";

                    data = GBData.SearchGBData(GBData.CHAN, name);
                    if (data == null)
                        throw new MismatchedProfileTextException();

                    h = data.Parameters[0];
                    b = data.Parameters[1];
                    s = data.Parameters[2];
                    t = data.Parameters[3];
                }

                h *= 0.001; b *= 0.001; s *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {
                h = b = s = t = 0;
                data = null;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (h == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
                formula = $"{h}*2+{b}";
                if (exclude_topSurface)
                    formula += "*6";
                else
                    formula += "*8";
                break;
            case FormulaAccuracyEnum.PRECISELY:
                formula = this.GetAreaFormula(FormulaAccuracyEnum.ROUGHLY, exclude_topSurface);
                formula += $"-{s}*4";
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data == null)
                    break;
                formula = $"{data.Area}*2-{h}*2";
                if (exclude_topSurface)
                    formula += $"-{b}*2";
                break;
            default:
                break;
            }

            return formula;
        }

        public string GetSiffenerProfileStr(bool truncatedRounding) {
            string stifProfileText = string.Empty;
            if (h == 0) return stifProfileText;

            double t, b, l;
            t = s;
            b = this.b - s;
            l = h - this.t * 2;
            t *= 1000; b *= 1000; l *= 1000;
            if (truncatedRounding) {
                t = Math.Truncate(t);
                b = Math.Truncate(b);
                l = Math.Truncate(l);
            }
            stifProfileText = $"PL{t}*{b}*{l}";

            return stifProfileText;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetWeightFormula(FormulaAccuracyEnum)" path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY, PRECISELY 均等效于 GBDATA</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;

            if (data != null)
                formula = $"{data.Weight}*2";

            return formula;
        }
    }
    /// <summary>
    /// <para>角钢。在以下模式中尝试匹配：</para>
    /// <para><see cref="Pattern_Collection.L_1"/></para>
    /// <para><see cref="Pattern_Collection.L_2"/></para>
    /// <para>当匹配到L2模式时，在国标截面特性表格中查找，同一型号名下按第一个进行匹配。</para>
    /// </summary>
    public class SectionSteel_L : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private double h, b, t;
        private GBDataBase data;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_L() {

        }
        public SectionSteel_L(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            h = b = t = 0;
            data = null;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.L_1);
                if (match.Success) {
                    double.TryParse(match.Groups["h"].Value, out h);
                    double.TryParse(match.Groups["b"].Value, out b);
                    double.TryParse(match.Groups["t"].Value, out t);

                    if (b == 0)
                        b = h;
                    data = GBData.SearchGBData(GBData.L, new double[] { h, b, t });
                } else {
                    match = Regex.Match(ProfileText, Pattern_Collection.L_2);
                    if (!match.Success)
                        throw new MismatchedProfileTextException();

                    double.TryParse(match.Groups["h"].Value, out h);
                    double.TryParse(match.Groups["b"].Value, out b);
                    h *= 10; b *= 10;
                    data = GBData.SearchGBData(GBData.L, new double[] { h, b });
                    if (data == null)
                        throw new MismatchedProfileTextException();

                    h = data.Parameters[0];
                    b = data.Parameters[1];
                    t = data.Parameters[2];
                }

                h *= 0.001; b *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {
                h = b = t = 0;
                data = null;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// <para><b>在本类中：PRECISELY 等效于 ROUGHLY</b></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (h == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (b != h) {
                    formula = $"{h}*2+{b}";
                    if (!exclude_topSurface)
                        formula += $"*2";
                } else {
                    formula = $"{h}";
                    if (exclude_topSurface)
                        formula += $"*3";
                    else
                        formula += $"*4";
                }
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data == null)
                    return formula;

                formula = $"{data.Area}";
                if (exclude_topSurface)
                    formula += $"-{b}";
                break;
            default:
                break;
            }

            return formula;
        }

        public string GetSiffenerProfileStr(bool truncatedRounding) {
            string stifProfileText = string.Empty;
            if (h == 0) return stifProfileText;

            double t, b, l;
            t = this.t;
            b = this.b - this.t;
            l = h - this.t;
            t *= 1000; b *= 1000; l *= 1000;
            if (truncatedRounding) {
                t = Math.Truncate(t);
                b = Math.Truncate(b);
                l = Math.Truncate(l);
            }
            stifProfileText = $"PL{t}*{b}*{l}";

            return stifProfileText;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetWeightFormula(FormulaAccuracyEnum)" path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY, PRECISELY 均等效于 GBDATA</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;

            if (data != null)
                formula = $"{data.Weight}";

            return formula;
        }
    }
    /// <summary>
    /// <para>角钢，背对背双拼。在以下模式中尝试匹配：</para>
    /// <para><see cref="Pattern_Collection.L_BtB_1"/></para>
    /// <para><see cref="Pattern_Collection.L_BtB_2"/></para>
    /// <para>当匹配到L_BtB_2模式时，在国标截面特性表格中查找，同一型号名下按第一个进行匹配。</para>
    /// </summary>
    public class SectionSteel_L_BtB : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private double h, b, t;
        private GBDataBase data;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_L_BtB() {

        }
        public SectionSteel_L_BtB(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            h = b = t = 0;
            data = null;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.L_BtB_1);
                if (match.Success) {
                    double.TryParse(match.Groups["h"].Value, out h);
                    double.TryParse(match.Groups["b"].Value, out b);
                    double.TryParse(match.Groups["t"].Value, out t);

                    if (b == 0)
                        b = h;
                    data = GBData.SearchGBData(GBData.L, new double[] { h, b, t });
                } else {
                    match = Regex.Match(ProfileText, Pattern_Collection.L_BtB_2);
                    if (!match.Success)
                        throw new MismatchedProfileTextException();

                    double.TryParse(match.Groups["h"].Value, out h);
                    double.TryParse(match.Groups["b"].Value, out b);
                    h *= 10; b *= 10;
                    data = GBData.SearchGBData(GBData.L, new double[] { h, b });
                    if (data == null)
                        throw new MismatchedProfileTextException();

                    h = data.Parameters[0];
                    b = data.Parameters[1];
                    t = data.Parameters[2];
                }

                h *= 0.001; b *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {
                h = b = t = 0;
                data = null;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// <para><b>在本类中：PRECISELY 等效于 ROUGHLY</b></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (h == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                formula = $"{h}*2";
                if (exclude_topSurface)
                    formula += $"+{b}*2";
                else
                    formula += $"+{b}*4";
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data == null)
                    return formula;

                formula = $"{data.Area}*2";
                if (exclude_topSurface)
                    formula += $"-{b}*2";
                break;
            default:
                break;
            }

            return formula;
        }

        public string GetSiffenerProfileStr(bool truncatedRounding) {
            string stifProfileText = string.Empty;
            if (h == 0) return stifProfileText;

            double t, b, l;
            t = this.t;
            b = this.b - this.t;
            l = h - this.t;
            t *= 1000; b *= 1000; l *= 1000;
            if (truncatedRounding) {
                t = Math.Truncate(t);
                b = Math.Truncate(b);
                l = Math.Truncate(l);
            }
            stifProfileText = $"PL{t}*{b}*{l}";

            return stifProfileText;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetWeightFormula(FormulaAccuracyEnum)" path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY, PRECISELY 均等效于 GBDATA</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;

            if (data != null)
                formula = $"{data.Weight}*2";

            return formula;
        }
    }
    /// <summary>
    /// <para>冷弯空心型钢（Cold forming hollow section steel）方管和矩管。在以下模式中尝试匹配：</para>
    /// <para><see cref="Pattern_Collection.CFH_J_1"/></para>
    /// <para><see cref="Pattern_Collection.CFH_J_2"/></para>
    /// <para><see cref="Pattern_Collection.CFH_J_3"/></para>
    /// </summary>
    public class SectionSteel_CFH_J : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private double h1, b1, h2, b2, t;
        GBDataBase data;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_CFH_J() {

        }
        public SectionSteel_CFH_J(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            h1 = b1 = h2 = b2 = t = 0;
            data = null;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.CFH_J_1);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.CFH_J_2);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.CFH_J_3);
                if (!match.Success)
                    throw new MismatchedProfileTextException();

                double.TryParse(match.Groups["h1"].Value, out h1);
                double.TryParse(match.Groups["b1"].Value, out b1);
                double.TryParse(match.Groups["h2"].Value, out h2);
                double.TryParse(match.Groups["b2"].Value, out b2);
                double.TryParse(match.Groups["t"].Value, out t);

                if (b1 == 0) b1 = h1;
                if (h2 == 0) h2 = h1;
                if (b2 == 0) b2 = b1;

                if (h2 == h1 && b2 == b1)
                    data = GBData.SearchGBData(GBData.CFH_J, new double[] { h1, b1, t });

                h1 *= 0.001; b1 *= 0.001; h2 *= 0.001; b2 *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {
                h1 = b1 = h2 = b2 = t = 0;
                data = null;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// <para><b>在本类中：PRECISELY 等效于 ROUGHLY，不实现GBDATA（国标截面特性表格中无表面积数据）</b></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (h1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (h2 != h1)
                    formula = $"{h1}+{h2}";
                else
                    formula = $"{h1}*2";

                if (b2 != b1) {
                    if (exclude_topSurface)
                        formula += $"+{(b1 + b2) * 0.5}";
                    else
                        formula += $"+{b1}+{b2}";
                } else {
                    if (exclude_topSurface)
                        formula += $"+{b1}";
                    else
                        formula += $"+{b1}*2";
                }
                break;
            case FormulaAccuracyEnum.GBDATA:
                break;
            default:
                break;
            }

            return formula;
        }

        public string GetSiffenerProfileStr(bool truncatedRounding) {
            string stifProfileText = string.Empty;
            if (h1 == 0) return stifProfileText;

            double t, b, l;
            t = this.t;
            b = (b1 + b2) * 0.5 - this.t * 2;
            l = (h1 + h2) * 0.5 - this.t * 2;
            t *= 1000; b *= 1000; l *= 1000;
            if (truncatedRounding) {
                t = Math.Truncate(t);
                b = Math.Truncate(b);
                l = Math.Truncate(l);
            }
            stifProfileText = $"PL{t}*{b}*{l}";

            return stifProfileText;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetWeightFormula(FormulaAccuracyEnum)" path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (h1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (h2 == h1 && b2 == b1 && b1 == h1)
                    formula = $"({h1}*4";
                else {
                    if (h2 != h1)
                        formula = $"({h1}+{h2}";
                    else
                        formula = $"({h1}*2";

                    if (b2 != b1)
                        formula += $"+{b1}+{b2}";
                    else
                        formula += $"+{b1}*2";
                }

                formula += $"-{t}*4)*{t}*{GBData.DENSITY}";
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data != null)
                    formula = $"{data.Weight}";
                break;
            default:
                break;
            }

            return formula;
        }
    }
    /// <summary>
    /// <para>焊接矩形管。在以下模式中尝试匹配：</para>
    /// <para><see cref="Pattern_Collection.RECT_1"/></para>
    /// <para><see cref="Pattern_Collection.RECT_2"/></para>
    /// <para><see cref="Pattern_Collection.RECT_3"/></para>
    /// <para><see cref="Pattern_Collection.RECT_4"/></para>
    /// <para><see cref="Pattern_Collection.RECT_5"/></para>
    /// <para><see cref="Pattern_Collection.RECT_6"/></para>
    /// </summary>
    public class SectionSteel_RECT : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private double h1, h2, b1, b2, s, t;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_RECT() {

        }
        public SectionSteel_RECT(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            h1 = h2 = b1 = b2 = s = t = 0;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.RECT_1);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.RECT_2);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.RECT_3);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.RECT_4);
                if (match.Success) {
                    double.TryParse(match.Groups["h1"].Value, out h1);
                    double.TryParse(match.Groups["h2"].Value, out h2);
                    double.TryParse(match.Groups["b1"].Value, out b1);
                    double.TryParse(match.Groups["b2"].Value, out b2);
                    double.TryParse(match.Groups["s"].Value, out s);
                    double.TryParse(match.Groups["t"].Value, out t);
                } else {
                    match = Regex.Match(ProfileText, Pattern_Collection.RECT_5);
                    if (match.Success) {
                        double.TryParse(match.Groups["h1"].Value, out h1);
                        double.TryParse(match.Groups["h2"].Value, out h2);
                        double.TryParse(match.Groups["s"].Value, out s);

                        b1 = h1; b2 = h2;
                    } else {
                        match = Regex.Match(ProfileText, Pattern_Collection.RECT_6);
                        if (!match.Success)
                            throw new MismatchedProfileTextException();

                        double.TryParse(match.Groups["H1"].Value, out double H1);
                        double.TryParse(match.Groups["H2"].Value, out double H2);
                        double.TryParse(match.Groups["h1"].Value, out h1);
                        double.TryParse(match.Groups["h2"].Value, out h2);
                        double.TryParse(match.Groups["b1"].Value, out b1);
                        double.TryParse(match.Groups["s"].Value, out s);

                        h1 = (H1 + h1) * 0.5;
                        h2 = (H2 + h2) * 0.5;
                    }
                }

                if (b1 == 0) b1 = h1;
                if (h2 == 0) h2 = h1;
                if (b2 == 0) b2 = b1;
                if (t == 0) t = s;

                h1 *= 0.001; h2 *= 0.001; b1 *= 0.001; b2 *= 0.001; s *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {
                h1 = h2 = b1 = b2 = s = t = 0;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// <para><b>在本类中：PRECISELY 等效于 ROUGHLY，不实现 GBDATA</b></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (h1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (h2 != h1)
                    formula = $"{h1}+{h2}";
                else
                    formula = $"{h1}*2";

                if (b2 != b1) {
                    if (exclude_topSurface)
                        formula += $"+({b1}+{b2})*0.5";
                    else
                        formula += $"+{b1}+{b2}";
                } else {
                    if (exclude_topSurface)
                        formula += $"+{b1}";
                    else
                        formula += $"+{b1}*2";
                }
                break;
            case FormulaAccuracyEnum.GBDATA:
                break;
            default:
                break;
            }

            return formula;
        }

        public string GetSiffenerProfileStr(bool truncatedRounding) {
            string stifProfileText = string.Empty;
            if (h1 == 0) return stifProfileText;

            double t, b, l;
            t = s;
            b = (b1 + b2) * 0.5 - s * 2;
            l = (h1 + h2) * 0.5 - this.t * 2;
            t *= 1000; b *= 1000; l *= 1000;
            if (truncatedRounding) {
                t = Math.Truncate(t);
                b = Math.Truncate(b);
                l = Math.Truncate(l);
            }
            stifProfileText = $"PL{t}*{b}*{l}";

            return stifProfileText;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetWeightFormula(FormulaAccuracyEnum)" path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY，不实现GBDATA</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;

            if (h1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (h2 != h1)
                    formula = $"(({(h1 + h2) * 0.5}-{t}*2)*{s}";
                else
                    formula = $"(({h1}-{t}*2)*{s}";

                if (b2 != b1)
                    formula += $"+{(b2 + b1) * 0.5}*{t})*2";
                else
                    formula += $"+{b1}*{t})*2";

                formula += $"*{GBData.DENSITY}";
                break;
            case FormulaAccuracyEnum.GBDATA:
                break;
            default:
                break;
            }

            return formula;
        }
    }
    /// <summary>
    /// <para>冷弯空心型钢（Cold forming hollow section steel）圆管。在以下模式中尝试匹配：</para>
    /// <para><see cref="Pattern_Collection.CFH_Y_1"/></para>
    /// </summary>
    public class SectionSteel_CFH_Y : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private double d, t;
        private GBDataBase data;
        public PIStyleEnum PIStyle { get; set; }
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public SectionSteel_CFH_Y() {

        }
        public SectionSteel_CFH_Y(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            d = t = 0;
            data = null;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.CFH_Y_1);
                if (!match.Success)
                    throw new MismatchedProfileTextException();

                double.TryParse(match.Groups["d"].Value, out d);
                double.TryParse(match.Groups["t"].Value, out t);

                data = GBData.SearchGBData(GBData.CFH_Y, new double[] { d, t });

                d *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {
                d = t = 0;
                data = null;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// <para><b>在本类中：PRECISELY 等效于 ROUGHLY，不实现GBDATA（国标截面特性表格中无表面积数据）</b></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (d == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                formula = $"{(PIStyle == 0 ? "PI()" : "3.14")}*{d}";
                break;
            case FormulaAccuracyEnum.GBDATA:
                break;
            default:
                break;
            }

            return formula;
        }

        public string GetSiffenerProfileStr(bool truncatedRounding) {
            string stifProfileText = string.Empty;
            if (this.d == 0) return stifProfileText;

            double t, d;
            t = this.t;
            d = this.d - this.t * 2;
            t *= 1000; d *= 1000;
            if (truncatedRounding) {
                t = Math.Truncate(t);
                d = Math.Truncate(d);
            }
            stifProfileText = $"PLD{t}*{d}";

            return stifProfileText;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetWeightFormula(FormulaAccuracyEnum)" path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (d == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                formula = $"{(PIStyle == 0 ? "PI()" : "3.14")}*({d * 0.5}^2-{d * 0.5 - t}^2)*{GBData.DENSITY}";
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data != null)
                    formula = $"{data.Weight}";
                break;
            default:
                break;
            }

            return formula;
        }
    }
    /// <summary>
    /// <para>圆钢或圆管。在以下模式中尝试匹配：</para>
    /// <para><see cref="Pattern_Collection.CIRC_1"/></para>
    /// <para><see cref="Pattern_Collection.CIRC_2"/></para>
    /// <para><see cref="Pattern_Collection.CIRC_3"/></para>
    /// </summary>
    public class SectionSteel_CIRC : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private double d1, r1, d2, r2, t;
        public PIStyleEnum PIStyle { get; set; }
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public SectionSteel_CIRC() {

        }
        public SectionSteel_CIRC(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            d1 = r1 = d2 = r2 = t = 0;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.CIRC_1);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.CIRC_2);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.CIRC_3);
                if (!match.Success)
                    throw new MismatchedProfileTextException();

                double.TryParse(match.Groups["d1"].Value, out d1);
                double.TryParse(match.Groups["r1"].Value, out r1);
                double.TryParse(match.Groups["d2"].Value, out d2);
                double.TryParse(match.Groups["r2"].Value, out r2);
                double.TryParse(match.Groups["t"].Value, out t);
            } catch (MismatchedProfileTextException) {
                d1 = r1 = d2 = r2 = t = 0;
            }

            if (r1 == 0) r1 = d1;
            if (d2 == 0) d2 = d1;
            if (r2 == 0) r2 = d2;
            d1 *= 0.001; r1 *= 0.001; d2 *= 0.001; r2 *= 0.001; t *= 0.001;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// <para><b>在本类中：PRECISELY 等效于 ROUGHLY，不实现GBDATA</b></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/>
        /// <para>椭圆周长采用估算公式：h=(a-b)^2/(a+b)^2, p=π(a+b)(1+3h/(10+(4-3h)^0.5))</para>
        /// </returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            string PI, c1, c2;

            if (d1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                PI = PIStyle == 0 ? "PI()" : "3.14";

                if (r1 == d1)
                    c1 = $"{PI}*{d1}";
                else
                    c1 = EllipseCircumference(d1 * 0.5, r1 * 0.5).ToString();

                if (r2 == d2)
                    c2 = $"{PI}*{d2}";
                else
                    c2 = EllipseCircumference(d2 * 0.5, r2 * 0.5).ToString();

                if (c2.Equals(c1))
                    formula = $"{c1}";
                else {
                    if (r1 == d1 && r2 == d2)
                        formula = $"{PI}*({c1.Remove(0, PI.Length + 1)}+{c2.Remove(0, PI.Length + 1)})*0.5";
                    else
                        formula = $"({c1}+{c2})*0.5";
                }
                break;
            case FormulaAccuracyEnum.GBDATA:
                break;
            default:
                break;
            }

            return formula;
        }
        /// <summary>
        /// 根据椭圆的长、短半轴估算周长，估算公式h=(a-b)^2/(a+b)^2, p=π(a+b)(1+3h/(10+(4-3h)^0.5))
        /// </summary>
        /// <param name="a">长半轴</param>
        /// <param name="b">短半轴</param>
        /// <returns>椭圆周长的估算值，保留三位小数。</returns>
        protected double EllipseCircumference(double a, double b) {
            double h, resault = 0;

            if (a + b == 0) return resault;
            h = Math.Pow(a - b, 2) / Math.Pow(a + b, 2);
            resault = Math.PI * (a + b) * (1 + 3 * h / (10 + Math.Sqrt(4 - 3 * h)));

            return Math.Round(resault, 3);
        }

        public string GetSiffenerProfileStr(bool truncatedRounding) {
            string stifProfileText = string.Empty;
            if (d1 == 0) return stifProfileText;

            double t, d;
            t = this.t;
            d = (d1 + r1 + d2 + r2) * 0.25;
            t *= 1000; d *= 1000;
            if (truncatedRounding) {
                t = Math.Truncate(t);
                d = Math.Truncate(d);
            }
            stifProfileText = $"PLD{t}*{d}";

            return stifProfileText;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetWeightFormula(FormulaAccuracyEnum)" path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY，不实现 GBDATA</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            string PI, s1, s2;

            if (d1 == 0) return formula;
            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                PI = PIStyle == 0 ? "PI()" : "3.14";
                if (r1 == d1) {
                    if (t == 0)
                        s1 = $"{d1 * 0.5}^2";
                    else
                        s1 = $"{d1 * 0.5}^2-{d1 * 0.5 - t}^2";
                } else {
                    if (t == 0)
                        s1 = $"{d1 * 0.5}*{r1 * 0.5}";
                    else
                        s1 = $"{d1 * 0.5}*{r1 * 0.5}-{d1 * 0.5 - t}*{r1 * 0.5 - t}";
                }

                if (r2 == d2) {
                    if (t == 0)
                        s2 = $"{d2 * 0.5}^2";
                    else
                        s2 = $"{d2 * 0.5}^2-{d2 * 0.5 - t}^2";
                } else {
                    if (t == 0)
                        s2 = $"{d2 * 0.5}*{r2 * 0.5}";
                    else
                        s2 = $"{d2 * 0.5}*{r2 * 0.5}-{d2 * 0.5 - t}*{r2 * 0.5 - t}";
                }

                if (s2.Equals(s1)) {
                    if (t == 0)
                        formula = $"{PI}*{s1}*{GBData.DENSITY}";
                    else
                        formula = $"{PI}*({s1})*{GBData.DENSITY}";
                } else
                    formula = $"{PI}*({s1}+{s2})*0.5*{GBData.DENSITY}";
                break;
            case FormulaAccuracyEnum.GBDATA:
                break;
            default:
                break;
            }

            return formula;
        }
    }
    /// <summary>
    /// <para>冷弯开口型钢（Cold forming open section steel）内卷边槽钢。在以下模式中尝试匹配：</para>
    /// <para><see cref="Pattern_Collection.CFO_CN_1"/></para>
    /// <para><see cref="Pattern_Collection.CFO_CN_2"/></para>
    /// </summary>
    public class SectionSteel_CFO_CN : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private double h, b1, c1, b2, c2, t;
        GBDataBase data;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_CFO_CN() {

        }
        public SectionSteel_CFO_CN(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            h = b1 = c1 = b2 = c2 = t = 0;
            data = null;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.CFO_CN_1);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.CFO_CN_2);
                if (!match.Success)
                    throw new MismatchedProfileTextException();

                double.TryParse(match.Groups["h"].Value, out h);
                double.TryParse(match.Groups["b1"].Value, out b1);
                double.TryParse(match.Groups["c1"].Value, out c1);
                double.TryParse(match.Groups["b2"].Value, out b2);
                double.TryParse(match.Groups["c2"].Value, out c2);
                double.TryParse(match.Groups["t"].Value, out t);

                if (b2 == 0) b2 = b1;
                if (c2 == 0) c2 = c1;

                if (b2 == b1 && c2 == c1)
                    data = GBData.SearchGBData(GBData.CFO_CN, new double[] { h, b1, c1, t });

                h *= 0.001; b1 *= 0.001; c1 *= 0.001; b2 *= 0.001; c2 *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {
                h = b1 = c1 = b2 = c2 = t = 0;
                data = null;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// <para><b>在本类中：不实现 GBDATA（国标截面特性表格中无表面积数据）</b></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (h == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
                formula = $"{h}*2";
                if (b2 != b1) {
                    formula += $"+{b1}*2+{b2}";
                    if (!exclude_topSurface)
                        formula += $"*2";
                } else {
                    if (exclude_topSurface)
                        formula += $"+{b1}*3";
                    else
                        formula += $"+{b1}*4";
                }
                if (c2 != c1)
                    formula += $"+{c1}*2+{c2}*2";
                else
                    formula += $"+{c1}*4";
                break;
            case FormulaAccuracyEnum.PRECISELY:
                formula = this.GetAreaFormula(FormulaAccuracyEnum.ROUGHLY, exclude_topSurface);
                formula += $"-{t}*8";
                break;
            case FormulaAccuracyEnum.GBDATA:
                break;
            default:
                break;
            }

            return formula;
        }
        /// <summary>
        /// <inheritdoc/>
        /// <para><b>本类不实现此方法。</b></para>
        /// </summary>
        /// <param name="truncatedRounding"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public string GetSiffenerProfileStr(bool truncatedRounding) {
            return string.Empty;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetWeightFormula(FormulaAccuracyEnum)" path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (h == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                formula = $"({h}";
                if (b2 != b1)
                    formula += $"+{b1}+{b2}";
                else
                    formula += $"+{b1}*2";

                if (c2 != c1)
                    formula += $"+{c1}+{c2}";
                else
                    formula += $"+{c1}*2";

                formula += $"-{t}*4)*{t}*{GBData.DENSITY}";
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data != null)
                    formula = $"{data.Weight}";
                break;
            default:
                break;
            }

            return formula;
        }
    }
    /// <summary>
    /// <para>冷弯开口型钢（Cold forming open section steel）内卷边槽钢，口对口双拼。在以下模式中尝试匹配：</para>
    /// <para><see cref="Pattern_Collection.CFO_CN_MtM_1"/></para>
    /// <para><see cref="Pattern_Collection.CFO_CN_MtM_2"/></para>
    /// </summary>
    public class SectionSteel_CFO_CN_MtM : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private double h, b1, c1, b2, c2, t;
        private GBDataBase data;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_CFO_CN_MtM() {

        }
        public SectionSteel_CFO_CN_MtM(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            h = b1 = c1 = b2 = c2 = t = 0;
            data = null;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.CFO_CN_MtM_1);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.CFO_CN_MtM_2);
                if (!match.Success)
                    throw new MismatchedProfileTextException();

                double.TryParse(match.Groups["h"].Value, out h);
                double.TryParse(match.Groups["b1"].Value, out b1);
                double.TryParse(match.Groups["c1"].Value, out c1);
                double.TryParse(match.Groups["b2"].Value, out b2);
                double.TryParse(match.Groups["c2"].Value, out c2);
                double.TryParse(match.Groups["t"].Value, out t);

                if (b2 == 0) b2 = b1;
                if (c2 == 0) c2 = c1;

                if (b2 == b1 && c2 == c1)
                    data = GBData.SearchGBData(GBData.CFO_CN, new double[] { h, b1, c1, t });

                h *= 0.001; b1 *= 0.001; c1 *= 0.001; b2 *= 0.001; c2 *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {
                h = b1 = c1 = b2 = c2 = t = 0;
                data = null;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// <para><b>在本类中：PRECISELY 等效于 ROUGHLY，不实现 GBDATA（国标截面特性表格中无表面积数据）</b></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (h == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (exclude_topSurface)
                    formula += $"{h}*2+{b1}+{b2}";
                else
                    formula += $"{h}*2+{b1}*2+{b2}*2";
                break;
            case FormulaAccuracyEnum.GBDATA:
                break;
            default:
                break;
            }

            return formula;
        }
        /// <summary>
        /// <inheritdoc/>
        /// <para><b>本类不实现此方法。</b></para>
        /// </summary>
        /// <param name="truncatedRounding"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public string GetSiffenerProfileStr(bool truncatedRounding) {
            return string.Empty;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetWeightFormula(FormulaAccuracyEnum)" path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (h == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                formula = $"({h}+{b1}*2+{c1}*2-{t}*4)*{t}*{GBData.DENSITY}*2";
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data != null)
                    formula = $"{data.Weight}*2";
                break;
            default:
                break;
            }

            return formula;
        }
    }
    /// <summary>
    /// <para>冷弯开口型钢（Cold forming open section steel）内卷边槽钢，背对背双拼。在以下模式中尝试匹配：</para>
    /// <para><see cref="Pattern_Collection.CFO_CN_BtB_1"/></para>
    /// <para><see cref="Pattern_Collection.CFO_CN_BtB_2"/></para>
    /// </summary>
    public class SectionSteel_CFO_CN_BtB : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private double h, b1, c1, b2, c2, t;
        private GBDataBase data;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_CFO_CN_BtB() {

        }
        public SectionSteel_CFO_CN_BtB(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            h = b1 = c1 = b2 = c2 = t = 0;
            data = null;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.CFO_CN_BtB_1);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.CFO_CN_BtB_2);
                if (!match.Success)
                    throw new MismatchedProfileTextException();

                double.TryParse(match.Groups["h"].Value, out h);
                double.TryParse(match.Groups["b1"].Value, out b1);
                double.TryParse(match.Groups["c1"].Value, out c1);
                double.TryParse(match.Groups["b2"].Value, out b2);
                double.TryParse(match.Groups["c2"].Value, out c2);
                double.TryParse(match.Groups["t"].Value, out t);

                if (b2 == 0) b2 = b1;
                if (c2 == 0) c2 = c1;

                if (b2 == b1 && c2 == c1)
                    data = GBData.SearchGBData(GBData.CFO_CN, new double[] { h, b1, c1, t });

                h *= 0.001; b1 *= 0.001; c1 *= 0.001; b2 *= 0.001; c2 *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {
                h = b1 = c1 = b2 = c2 = t = 0;
                data = null;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// <para><b>在本类中：不实现 GBDATA（国标截面特性表格中无表面积数据）</b></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (h == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
                if (b2 != b1) {
                    formula = $"{h}*2+{b1}*4+{b2}";
                    if (exclude_topSurface)
                        formula += "*2";
                    else
                        formula += "*4";
                } else {
                    formula = $"{h}*2+{b1}";
                    if (exclude_topSurface)
                        formula += "*6";
                    else
                        formula += "*8";
                }

                if (c2 != c1)
                    formula += $"+{c1}*4+{c2}*4";
                else
                    formula += $"+{c1}*8";
                break;
            case FormulaAccuracyEnum.PRECISELY:
                formula = this.GetAreaFormula(FormulaAccuracyEnum.ROUGHLY, exclude_topSurface);
                formula += $"-{t}*16";
                break;
            case FormulaAccuracyEnum.GBDATA:
                break;
            default:
                break;
            }

            return formula;
        }
        /// <summary>
        /// <inheritdoc/>
        /// <para><b>本类不实现此方法。</b></para>
        /// </summary>
        /// <param name="truncatedRounding"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public string GetSiffenerProfileStr(bool truncatedRounding) {
            return string.Empty;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetWeightFormula(FormulaAccuracyEnum)" path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (h == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (b2 != b1)
                    formula = $"({h}+{b1}+{b2}";
                else
                    formula = $"({h}+{b1}*2";

                if (c2 != c1)
                    formula += $"+{c1}+{c2}-{t}*4)*{t}*{GBData.DENSITY}*2";
                else
                    formula += $"+{c1}*2-{t}*4)*{t}*{GBData.DENSITY}*2";
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data != null)
                    formula = $"{data.Weight}*2";
                break;
            default:
                break;
            }

            return formula;
        }
    }
    /// <summary>
    /// <para>冷弯开口型钢（Cold forming open section steel）卷边Z型钢（含斜卷边Z型钢XZ）。在以下模式中尝试匹配：</para>
    /// <para><see cref="Pattern_Collection.CFO_ZJ_1"/></para>
    /// <para><see cref="Pattern_Collection.CFO_ZJ_2"/></para>
    /// </summary>
    public class SectionSteel_CFO_ZJ : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private double h, b1, c1, b2, c2, t;
        private GBDataBase data;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_CFO_ZJ() {

        }
        public SectionSteel_CFO_ZJ(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            h = b1 = c1 = b2 = c2 = t = 0;
            data = null;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.CFO_ZJ_1);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.CFO_ZJ_2);
                if (!match.Success)
                    throw new MismatchedProfileTextException();

                double.TryParse(match.Groups["h"].Value, out h);
                double.TryParse(match.Groups["b1"].Value, out b1);
                double.TryParse(match.Groups["c1"].Value, out c1);
                double.TryParse(match.Groups["b2"].Value, out b2);
                double.TryParse(match.Groups["c2"].Value, out c2);
                double.TryParse(match.Groups["t"].Value, out t);

                if (b2 == 0) b2 = b1;
                if (c2 == 0) c2 = c1;

                if (b2 == b1 && c2 == c1)
                    data = GBData.SearchGBData(GBData.CFO_ZJ, new double[] { h, b1, c1, t });

                h *= 0.001; b1 *= 0.001; c1 *= 0.001; b2 *= 0.001; c2 *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {
                h = b1 = c1 = b2 = c2 = t = 0;
                data = null;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// <para><b>在本类中：不实现 GBDATA（国标截面特性表格中无表面积数据）</b></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (h == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
                if (b2 != b1) {
                    formula = $"{h}*2+{b1}*2+{b2}";
                    if (!exclude_topSurface)
                        formula += $"*2";
                } else {
                    formula = $"{h}*2+{b1}";
                    if (exclude_topSurface)
                        formula += "*3";
                    else
                        formula += "*4";
                }

                if (c2 != c1)
                    formula += $"+{c1}*2+{c2}*2";
                else
                    formula += $"+{c1}*4";
                break;
            case FormulaAccuracyEnum.PRECISELY:
                formula = this.GetAreaFormula(FormulaAccuracyEnum.ROUGHLY, exclude_topSurface);
                formula += $"-{t}*8";
                break;
            case FormulaAccuracyEnum.GBDATA:
                break;
            default:
                break;
            }

            return formula;
        }
        /// <summary>
        /// <inheritdoc/>
        /// <para><b>本类不实现此方法。</b></para>
        /// </summary>
        /// <param name="truncatedRounding"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public string GetSiffenerProfileStr(bool truncatedRounding) {
            return string.Empty;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetWeightFormula(FormulaAccuracyEnum)" path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (h == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (b2 != b1)
                    formula = $"({h}+{b1}+{b2}";
                else
                    formula = $"({h}+{b1}*2";

                if (c2 != c1)
                    formula += $"+{c1}+{c2}";
                else
                    formula += $"+{c1}*2";

                formula += $"-{t}*4)*{t}*{GBData.DENSITY}";
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data != null)
                    formula = $"{data.Weight}";
                break;
            default:
                break;
            }

            return formula;
        }
    }
    /// <summary>
    /// <para>矩形板件。在以下模式中尝试匹配：</para>
    /// <para><see cref="Pattern_Collection.PL_1"/></para>
    /// </summary>
    public class SectionSteel_PL : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private double t, b, l;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_PL() {

        }
        public SectionSteel_PL(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            t = b = l = 0;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.PL_1);
                if (!match.Success)
                    throw new MismatchedProfileTextException();

                Match subMatch;
                List<double> parameters = new List<double> { Capacity = 3 };
                bool[] isVariable = new bool[3] { false, false, false };
                double tmp1, tmp2;

                subMatch = Regex.Match(match.Groups["t"].Value, Pattern_Collection.VariableCrossSection);
                if (subMatch.Success) {
                    double.TryParse(subMatch.Groups["v1"].Value, out tmp1);
                    double.TryParse(subMatch.Groups["v2"].Value, out tmp2);
                    parameters.Add((tmp1 + tmp2) * 0.5);
                    isVariable[0] = true;
                } else {
                    double.TryParse(match.Groups["t"].Value, out tmp1);
                    parameters.Add(tmp1);
                }

                subMatch = Regex.Match(match.Groups["b"].Value, Pattern_Collection.VariableCrossSection);
                if (subMatch.Success) {
                    double.TryParse(subMatch.Groups["v1"].Value, out tmp1);
                    double.TryParse(subMatch.Groups["v2"].Value, out tmp2);
                    parameters.Add((tmp1 + tmp2) * 0.5);
                    isVariable[1] = true;
                } else {
                    double.TryParse(match.Groups["b"].Value, out tmp1);
                    parameters.Add(tmp1);
                }

                subMatch = Regex.Match(match.Groups["l"].Value, Pattern_Collection.VariableCrossSection);
                if (subMatch.Success) {
                    double.TryParse(subMatch.Groups["v1"].Value, out tmp1);
                    double.TryParse(subMatch.Groups["v2"].Value, out tmp2);
                    parameters.Add((tmp1 + tmp2) * 0.5);
                    isVariable[2] = true;
                } else {
                    double.TryParse(match.Groups["l"].Value, out tmp1);
                    parameters.Add(tmp1);
                }


                //  不应全部使用"~"符号，也不应对厚度值使用"~"符号
                //  附带完成t, b, l的赋值
                if (isVariable[0] == true && isVariable[1] == true && isVariable[2] == true)
                    throw new MismatchedProfileTextException();
                int minParameterIndex;
                if (parameters[2] == 0) {
                    minParameterIndex = parameters.IndexOf(Math.Min(parameters[0], parameters[1]));

                    t = parameters[minParameterIndex];
                    b = minParameterIndex == 0 ? parameters[1] : parameters[0];
                } else {
                    minParameterIndex = parameters.IndexOf(parameters.Min());

                    parameters.Sort();
                    t = parameters[0]; b = parameters[1]; l = parameters[2];
                }
                if (isVariable[minParameterIndex])
                    throw new MismatchedProfileTextException();


                t *= 0.001; b *= 0.001; l *= 0.001;
            } catch (MismatchedProfileTextException) {
                t = b = l = 0;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// <para><b>在本类中：PRECISELY 等效于 ROUGHLY，不实现 GBDATA</b></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (b == 0) return formula;//实现使用中可能 t == 0

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (l == 0)
                    formula = $"{b}";
                else
                    formula = $"{b}*{l}";

                if (!exclude_topSurface)
                    formula += "*2";
                break;
            case FormulaAccuracyEnum.GBDATA:
                break;
            default:
                break;
            }

            return formula;
        }
        /// <summary>
        /// <inheritdoc/>
        /// <para><b>本类不实现此方法。</b></para>
        /// </summary>
        /// <param name="truncatedRounding"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public string GetSiffenerProfileStr(bool truncatedRounding) {
            return string.Empty;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetWeightFormula(FormulaAccuracyEnum)" path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY，不实现 GBDATA</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (b == 0) return formula;//实际使用中可能t == 0

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (t == 0) {
                    formula = "0";
                } else {
                    if (l == 0)
                        formula = $"{b}*{t}*{GBData.DENSITY}";
                    else
                        formula = $"{b}*{l}*{t}*{GBData.DENSITY}";
                }
                break;
            case FormulaAccuracyEnum.GBDATA:
                break;
            default:
                break;
            }

            return formula;
        }
    }
    /// <summary>
    /// <para>（直角）三角板。在以下模式中尝试匹配：</para>
    /// <para><see cref="Pattern_Collection.PL_T_1"/></para>
    /// </summary>
    public class SectionSteel_PL_Triangle : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private double t, b, l;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_PL_Triangle() {

        }
        public SectionSteel_PL_Triangle(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            t = b = l = 0;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.PL_T_1);
                if (!match.Success)
                    throw new MismatchedProfileTextException();

                Match subMatch;
                List<double> parameters = new List<double> { Capacity = 3 };
                bool[] isVariable = new bool[3] { false, false, false };
                double tmp1, tmp2;

                subMatch = Regex.Match(match.Groups["t"].Value, Pattern_Collection.VariableCrossSection);
                if (subMatch.Success) {
                    double.TryParse(subMatch.Groups["v1"].Value, out tmp1);
                    double.TryParse(subMatch.Groups["v2"].Value, out tmp2);
                    parameters.Add((tmp1 + tmp2) * 0.5);
                    isVariable[0] = true;
                } else {
                    double.TryParse(match.Groups["t"].Value, out tmp1);
                    parameters.Add(tmp1);
                }

                subMatch = Regex.Match(match.Groups["b"].Value, Pattern_Collection.VariableCrossSection);
                if (subMatch.Success) {
                    double.TryParse(subMatch.Groups["v1"].Value, out tmp1);
                    double.TryParse(subMatch.Groups["v2"].Value, out tmp2);
                    parameters.Add((tmp1 + tmp2) * 0.5);
                    isVariable[1] = true;
                } else {
                    double.TryParse(match.Groups["b"].Value, out tmp1);
                    parameters.Add(tmp1);
                }

                subMatch = Regex.Match(match.Groups["l"].Value, Pattern_Collection.VariableCrossSection);
                if (subMatch.Success) {
                    double.TryParse(subMatch.Groups["v1"].Value, out tmp1);
                    double.TryParse(subMatch.Groups["v2"].Value, out tmp2);
                    parameters.Add((tmp1 + tmp2) * 0.5);
                    isVariable[2] = true;
                } else {
                    double.TryParse(match.Groups["l"].Value, out tmp1);
                    parameters.Add(tmp1);
                }


                //  不应全部使用"~"符号，也不应对厚度值使用"~"符号
                //  附带完成t, b, l的赋值
                if (isVariable[0] == true && isVariable[1] == true && isVariable[2] == true)
                    throw new MismatchedProfileTextException();
                int minParameterIndex;
                if (parameters[2] == 0) {
                    minParameterIndex = parameters.IndexOf(Math.Min(parameters[0], parameters[1]));

                    t = parameters[minParameterIndex];
                    b = minParameterIndex == 0 ? parameters[1] : parameters[0];
                } else {
                    minParameterIndex = parameters.IndexOf(parameters.Min());

                    parameters.Sort();
                    t = parameters[0]; b = parameters[1]; l = parameters[2];
                }
                if (isVariable[minParameterIndex])
                    throw new MismatchedProfileTextException();

                t *= 0.001; b *= 0.001; l *= 0.001;
            } catch (MismatchedProfileTextException) {
                t = b = l = 0;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// <para><b>在本类中：PRECISELY 等效于 ROUGHLY，不实现 GBDATA</b></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (b == 0) return formula;//实际使用中可能 t == 0

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                formula = $"{b}*{l}";
                if (exclude_topSurface)
                    formula += "*0.5";
                break;
            case FormulaAccuracyEnum.GBDATA:
                break;
            default:
                break;
            }

            return formula;
        }
        /// <summary>
        /// <inheritdoc/>
        /// <para><b>本类不实现此方法。</b></para>
        /// </summary>
        /// <param name="truncatedRounding"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public string GetSiffenerProfileStr(bool truncatedRounding) {
            return string.Empty;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetWeightFormula(FormulaAccuracyEnum)" path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY，不实现 GBDATA</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (b == 0) return formula;//实现使用中可能 t == 0

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (t == 0)
                    formula = "0";
                else
                    formula = $"{b}*{l}*0.5*{t}*{GBData.DENSITY}";
                break;
            case FormulaAccuracyEnum.GBDATA:
                break;
            default:
                break;
            }

            return formula;
        }
    }
    /// <summary>
    /// <para>圆形板。在以下模式中尝试匹配：</para>
    /// <para><see cref="Pattern_Collection.PL_O_1"/></para>
    /// </summary>
    public class SectionSteel_PL_Circular : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private double t, d;
        public PIStyleEnum PIStyle { get; set; }
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public SectionSteel_PL_Circular() {

        }
        public SectionSteel_PL_Circular(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            t = d = 0;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.PL_O_1);
                if (!match.Success)
                    throw new MismatchedProfileTextException();

                double.TryParse(match.Groups["t"].Value, out t);
                double.TryParse(match.Groups["d"].Value, out d);

                if (d < t) {
                    double temp = d;
                    d = t;
                    t = temp;
                }

                t *= 0.001; d *= 0.001;
            } catch (MismatchedProfileTextException) {
                t = d = 0;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// <para><b>在本类中：PRECISELY 等效于 ROUGHLY，不实现 GBDATA</b></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (d == 0) return formula;//实际使用中可能 t == 0

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                var PI = PIStyle == 0 ? "PI()" : "3.14";
                formula = $"{PI}*{d * 0.5}^2";
                if (!exclude_topSurface)
                    formula += "*2";
                break;
            case FormulaAccuracyEnum.GBDATA:
                break;
            default:
                break;
            }

            return formula;
        }
        /// <summary>
        /// <inheritdoc/>
        /// <para><b>本类不实现此方法。</b></para>
        /// </summary>
        /// <param name="truncatedRounding"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public string GetSiffenerProfileStr(bool truncatedRounding) {
            return string.Empty;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetWeightFormula(FormulaAccuracyEnum)" path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY，不实现 GBDATA</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (d == 0) return formula;//实际使用中可能 t == 0

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                var PI = PIStyle == 0 ? "PI()" : "3.14";
                if (t == 0)
                    formula = "0";
                else
                    formula = $"{PI}*{d * 0.5}^2*{t}*{GBData.DENSITY}";
                break;
            case FormulaAccuracyEnum.GBDATA:
                break;
            default:
                break;
            }

            return formula;
        }
    }
    /// <summary>
    /// <para>复合板件。在以下模式中尝试匹配：</para>
    /// <para><see cref="Pattern_Collection.PL_CMP_1"/></para>
    /// </summary>
    public class SectionSteel_PL_Composite : SectionSteelBase, ISectionSteel {
        private string _profileText;
        protected struct SubPlate {
            public double num;//数量
            public ISectionSteel plate;//子板件
        }
        private List<SubPlate> subPlates = new List<SubPlate>();
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_PL_Composite() {

        }
        public SectionSteel_PL_Composite(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            string tempProfileText;
            string[] profileTexts;
            SubPlate subplate;

            subPlates.Clear();
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.PL_CMP_1);
                if (!match.Success)
                    throw new MismatchedProfileTextException();

                tempProfileText = ProfileText;
                tempProfileText = tempProfileText.Replace("-", "+-");
                profileTexts = tempProfileText.Split('+');

                foreach (var profileText in profileTexts) {
                    match = Regex.Match(profileText, Pattern_Collection.PL_CMP_SUB_PL);
                    if (match.Success) {
                        var pl = new SectionSteel_PL(match.Groups["main"].Value);
                        double.TryParse(match.Groups["num"].Value, out subplate.num);
                        if (subplate.num == 0) {
                            subplate.num = match.Groups["num"].Value == "-" ? -1 : 1;
                        }
                        subplate.plate = pl;
                        subPlates.Add(subplate);

                        continue;
                    }
                    match = Regex.Match(profileText, Pattern_Collection.PL_CMP_SUB_PLT);
                    if (match.Success) {
                        var plt = new SectionSteel_PL_Triangle(match.Groups["main"].Value);
                        double.TryParse(match.Groups["num"].Value, out subplate.num);
                        if (subplate.num == 0) {
                            subplate.num = match.Groups["num"].Value == "-" ? -1 : 1;
                        }
                        subplate.plate = plt;
                        subPlates.Add(subplate);

                        continue;
                    }
                    match = Regex.Match(profileText, Pattern_Collection.PL_CMP_SUB_PLO);
                    if (match.Success) {
                        var plo = new SectionSteel_PL_Circular(match.Groups["main"].Value);
                        double.TryParse(match.Groups["num"].Value, out subplate.num);
                        if (subplate.num == 0) {
                            subplate.num = match.Groups["num"].Value == "-" ? -1 : 1;
                        }
                        subplate.plate = plo;
                        subPlates.Add(subplate);

                        continue;
                    }

                    throw new MismatchedProfileTextException();
                }

            } catch (MismatchedProfileTextException) {
                subPlates.Clear();
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// <para>具体说明参见：</para>
        /// <para><see cref="SectionSteel_PL.GetAreaFormula(FormulaAccuracyEnum, bool)"/></para>
        /// <para><see cref="SectionSteel_PL_Triangle.GetAreaFormula(FormulaAccuracyEnum, bool)"/></para>
        /// <para><see cref="SectionSteel_PL_Circular.GetAreaFormula(FormulaAccuracyEnum, bool)"/></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (subPlates == null || accuracy == FormulaAccuracyEnum.GBDATA) return formula;

            foreach(var subplate in subPlates) {
                if(subplate.num < 0)
                    formula += $"{subplate.num}*{subplate.plate.GetAreaFormula(accuracy, exclude_topSurface)}";
                else if (subplate.num == 1)
                    formula += $"+{subplate.plate.GetAreaFormula(accuracy, exclude_topSurface)}";
                else
                    formula += $"+{subplate.num}*{subplate.plate.GetAreaFormula(accuracy, exclude_topSurface)}";
            }
            if (formula[0] == '+') formula = formula.Remove(0, 1);

            return formula;
        }
        /// <summary>
        /// <inheritdoc/>
        /// <para><b>本类不实现此方法。</b></para>
        /// </summary>
        /// <param name="truncatedRounding"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public string GetSiffenerProfileStr(bool truncatedRounding) {
            return string.Empty;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetWeightFormula(FormulaAccuracyEnum)" path="/param[1]"/>
        /// <para>具体说明参见：</para>
        /// <para><see cref="SectionSteel_PL.GetWeightFormula(FormulaAccuracyEnum)"/></para>
        /// <para><see cref="SectionSteel_PL_Triangle.GetWeightFormula(FormulaAccuracyEnum)"/></para>
        /// <para><see cref="SectionSteel_PL_Circular.GetWeightFormula(FormulaAccuracyEnum)"/></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (subPlates == null || accuracy == FormulaAccuracyEnum.GBDATA) return formula;

            //foreach(var subplate in subPlates) {
            //    if(subplate.num < 0)
            //        formula += $"{subplate.num}*{subplate.plate.GetWeightFormula(accuracy)}";
            //    else if(subplate.num == 1)
            //        formula += $"{subplate.plate.GetWeightFormula(accuracy)}";
            //    else
            //        formula += $"+{subplate.num}*{subplate.plate.GetWeightFormula(accuracy)}";
            //}
            //if(formula.IndexOf('+') == 0) formula = formula.Remove(0, 1);

            if (subPlates.Count == 1) goto NoIdenticalItems;

            List<string> weights = new List<string>();
            foreach (var subplate in subPlates) {
                weights.Add(subplate.plate.GetWeightFormula(accuracy));
            }

            string pattern1 = @"\*\d+\.?\d*\*" + GBData.DENSITY + "$";
            string pattern2 = @"\*" + GBData.DENSITY + "$";
            Match match = Regex.Match(weights[0], pattern1);
            string value = match.Value;
            int i;
            for (i = 1; i < weights.Count; i++) {
                if (!weights[i].EndsWith(value))
                    break;
            }
            if (i < weights.Count) {
                match = Regex.Match(weights[0], pattern2);
                value = match.Value;
                for (i = 1; i < weights.Count; i++) {
                    if (!weights[i].EndsWith(value))
                        break;
                }
            }
            //无同类项返回
            if (i < weights.Count) {
                goto NoIdenticalItems;
            }

            //合并同类项返回
            for (i = 0; i < weights.Count; i++) {
                var item = weights[i];
                weights[i] = item.Remove(item.Length - value.Length, value.Length);
            }
            for(i = 0; i < subPlates.Count; i++) {
                if(subPlates[i].num < 0)
                    formula += $"{subPlates[i].num}*{weights[i]}";
                else if (subPlates[i].num == 1)
                    formula += $"+{weights[i]}";
                else
                    formula += $"+{subPlates[i].num}*{weights[i]}";
            }
            if (formula[0] == '+') formula = formula.Remove(0, 1);
            formula = $"({formula}){value}";

            return formula;

        NoIdenticalItems:
            foreach(var subplate in subPlates) {
                if(subplate.num < 0)
                    formula += $"{subplate.num}*{subplate.plate.GetWeightFormula(accuracy)}";
                else if (subplate.num == 1)
                    formula += $"+{subplate.plate.GetWeightFormula(accuracy)}";
                else
                    formula += $"+{subplate.num}*{subplate.plate.GetWeightFormula(accuracy)}";
            }
            if (formula[0] == '+') formula = formula.Remove(0, 1);

            return formula;
        }
    }
    /// <summary>
    /// <para>球体。在以下模式中尝试匹配：</para>
    /// <para><see cref="Pattern_Collection.SPHERE_1"/></para>
    /// </summary>
    public class SectionSteel_SPHERE : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private double d;
        public PIStyleEnum PIStyle { get; set; }
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public SectionSteel_SPHERE() {

        }
        public SectionSteel_SPHERE(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            d = 0;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.SPHERE_1);
                if (!match.Success)
                    throw new MismatchedProfileTextException();

                double.TryParse(match.Groups["d"].Value, out d);

                d *= 0.001;
            } catch (MismatchedProfileTextException) {
                d = 0;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy"><inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// <para><b>在本类中：PRECISELY 等效于 ROUGHLY，不实现 GBDATA</b></para>
        /// </param>
        /// <param name="exclude_topSurface"><inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/></param>
        /// <returns><inheritdoc/></returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (d == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                var PI = PIStyle == 0 ? "PI()" : "3.14";
                formula = $"4*{PI}*{d * 0.5}^2";
                break;
            case FormulaAccuracyEnum.GBDATA:
                break;
            default:
                break;
            }

            return formula;
        }
        /// <summary>
        /// <inheritdoc/>
        /// <para><b>本类不实现此方法。</b></para>
        /// </summary>
        /// <param name="truncatedRounding"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public string GetSiffenerProfileStr(bool truncatedRounding) {
            return string.Empty;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy"><inheritdoc/>
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY，不实现 GBDATA</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (d == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                var PI = PIStyle == 0 ? "PI()" : "3.14";
                formula = $"4/3*{PI}*{d * 0.5}^3*{GBData.DENSITY}";
                break;
            case FormulaAccuracyEnum.GBDATA:
                break;
            default:
                break;
            }

            return formula;
        }
    }
    /// <summary>
    /// 型钢总类。根据属性<b>ProfileText</b>自动识别具体应用哪一项子类，因此本类依赖于各子类：
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
        protected static Dictionary<string, string[]> classifierTable = new Dictionary<string, string[]> {
            {"H", new string[] {"B_WLD_A","B_WLD_H","B_WLD_K","H","HI","HM","HN","HP","HT","HW","PHI","WH","WI","I_VAR_A"}},
            {"HH", new string[] {"B_WLD_O","HH"}},
            {"T", new string[] {"T","TW","TM","TN","B_WLD_E"}},
            {"I", new string[] {"I"}},
            {"CHAN", new string[] {"[","C"}},
            {"CHAN_MtM", new string[] {"[]"}},
            {"CHAN_BtB", new string[] {"][","2[","2C"}},
            {"L", new string[] {"∠","L"}},
            {"L_BtB", new string[] {"2∠","2L"}},
            {"CFH_J", new string[] {"CFRHS","F","J","P","RHS","SHS","TUB"}},
            {"RECT", new string[] {"B_BUILT","B_VAR_A","B_VAR_B","B_VAR_C","B_WLD_F","B_WLD_J","R","RHSC"}},
            {"CFH_Y", new string[] {"Y","φ"}},
            {"CIRC", new string[] {"CFCHS","CHS","D","ELD","EPD","O","PD","PIP","ROD","TUBE"}},
            {"CFO_CN", new string[] {"C","CC"}},
            {"CFO_CN_MtM", new string[] {"2CCM","2CM"}},
            {"CFO_CN_BtB", new string[] {"2C","2CC"}},
            {"CFO_ZJ", new string[] {"XZ","Z","ZZ"}},
            {"PL", new string[] {"PL","PLD","PLO","PLT"}},
            {"SPHERE", new string[]{"SPHERE"} },
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
                    string type = string.Empty;
                    foreach (var item in classifierTable) {
                        foreach (var value in item.Value) {
                            if (classifier.Equals(value)) {
                                type = item.Key;
                                goto Got_it;
                            }
                        }
                    }
                Got_it:
                    switch (type) {
                    case "H":
                        realSectionSteel = new SectionSteel_H(ProfileText);
                        break;
                    case "HH":
                        realSectionSteel = new SectionSteel_HH(ProfileText);
                        break;
                    case "T":
                        realSectionSteel = new SectionSteel_T(ProfileText);
                        break;
                    case "I":
                        realSectionSteel = new SectionSteel_I(ProfileText);
                        break;
                    case "CHAN":
                        realSectionSteel = new SectionSteel_CHAN(ProfileText);
                        break;
                    case "CHAN_MtM":
                        realSectionSteel = new SectionSteel_CHAN_MtM(ProfileText);
                        break;
                    case "CHAN_BtB":
                        realSectionSteel = new SectionSteel_CHAN_BtB(ProfileText);
                        break;
                    case "L":
                        realSectionSteel = new SectionSteel_L(ProfileText);
                        break;
                    case "L_BtB":
                        realSectionSteel = new SectionSteel_L_BtB(ProfileText);
                        break;
                    case "CFH_J":
                        realSectionSteel = new SectionSteel_CFH_J(ProfileText);
                        break;
                    case "RECT":
                        realSectionSteel = new SectionSteel_RECT(ProfileText);
                        break;
                    case "CFH_Y":
                        realSectionSteel = new SectionSteel_CFH_Y(ProfileText);
                        break;
                    case "CIRC":
                        realSectionSteel = new SectionSteel_CIRC(ProfileText);
                        break;
                    case "CFO_CN":
                        realSectionSteel = new SectionSteel_CFO_CN(ProfileText);
                        break;
                    case "CFO_CN_MtM":
                        realSectionSteel = new SectionSteel_CFO_CN_MtM(ProfileText);
                        break;
                    case "CFO_CN_BtB":
                        realSectionSteel = new SectionSteel_CFO_CN_BtB(ProfileText);
                        break;
                    case "CFO_ZJ":
                        realSectionSteel = new SectionSteel_CFO_ZJ(ProfileText);
                        break;
                    case "SPHERE":
                        realSectionSteel = new SectionSteel_SPHERE(ProfileText);
                        break;
                    //PL可能带前缀n，进入default分支处理
                    case "PL":
                    default:
                        realSectionSteel = new SectionSteel_PL_Composite(ProfileText);
                        break;
                    }

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
