﻿/*==============================================================================
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
 *  PIStyleEnum.cs: 圆周率π的书写形式的枚举类型
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
namespace SectionSteel {
    /// <summary>
    /// 圆周率π的书写形式
    /// </summary>
    public enum PIStyleEnum {
        /// <summary>
        /// 函数形式 - PI()
        /// </summary>
        FUNC,
        /// <summary>
        /// 数字形式 - 3.14
        /// </summary>
        NUM,
    }
}
