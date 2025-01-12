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
 *  SectionSteelClassification.cs: 型钢分类信息
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System;

namespace SectionSteel {
    /// <summary>
    /// 型钢分类信息。
    /// </summary>
    public struct SectionSteelClassification {
        /// <summary>
        /// 类型。
        /// </summary>
        public Type Type;
        /// <summary>
        /// 类别。
        /// </summary>
        public string Category;
        /// <summary>
        /// 标识符。
        /// </summary>
        public string Classifier;
        public SectionSteelClassification(Type type, string category, string classifier) {
            Type = type;
            Category = category;
            Classifier = classifier;
        }
    }
}
