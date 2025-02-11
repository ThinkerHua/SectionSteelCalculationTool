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
 *  EnumerableGenericExtension.cs: extension of IEnumerable<T> 
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System;
using System.Collections.Generic;

namespace SectionSteel {
    /// <summary>
    /// 提供 <see cref="IEnumerable{T}"/> 的扩展方法。
    /// </summary>
    public static class EnumerableGenericExtension {
        /// <summary>
        /// 获取当前实例的哈希码。
        /// </summary>
        /// <typeparam name="T">枚举对象的类型</typeparam>
        /// <param name="source">当前实例</param>
        /// <returns>当前实例的哈希码。</returns>
        public static int GetHashCode<T>(this IEnumerable<T> source) {
            ArgumentNullException.ThrowIfNull(source);

            int hash = 0;
            foreach (var item in source) {
                hash = HashCode.Combine(hash, item);
            }
            return hash;
        }
    }
}
