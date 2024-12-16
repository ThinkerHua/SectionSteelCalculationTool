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
 *  FormulaAccuracyEnum.cs: 计算式精确度枚举类型
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SectionSteel {
    /// <summary>
    /// 计算式精确度。
    /// </summary>
    public enum FormulaAccuracyEnum {
        /// <summary>
        /// 粗略的
        /// </summary>
        ROUGHLY,
        /// <summary>
        /// 稍准确的
        /// </summary>
        PRECISELY,
        /// <summary>
        /// 国标理论数据
        /// </summary>
        GBDATA,
    }
}
