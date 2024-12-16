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
 *  ISectionSteel.cs: 型钢接口，定义型钢应具有的属性、方法
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SectionSteel {
    /// <summary>
    /// 型钢接口。
    /// </summary>
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
}
