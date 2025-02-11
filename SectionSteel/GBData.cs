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
 *  GBData.cs: data of chinese national standard section steel 
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System;
using System.Linq;

namespace SectionSteel {
    /// <summary>
    /// 国标型钢理论数据。
    /// </summary>
    public class GBData {
        public string Name { get; }
        public double[] Parameters { get; }
        public double Weight { get; }
        public double Area { get; }
        public GBData(string name, double[] parameters, double weight, double area) {
            Name = name;
            Parameters = parameters;
            Weight = weight;
            Area = area;
        }

        /// <summary>
        /// 比较当前实例与给定实例是否相等。
        /// </summary>
        /// <remarks>
        /// 仅比较两个实例的 
        /// <see cref="GBData.Parameters"/>、
        /// <see cref="GBData.Area"/>、
        /// <see cref="GBData.Weight"/> 属性，
        /// <see cref="GBData.Name"/> 属性不参与判断。
        /// </remarks>
        /// <param name="obj">给定实例</param>
        /// <returns>相等返回 true，不等返回 false。</returns>
        public override bool Equals(object? obj) {
            if (obj is not GBData data || data.GetHashCode() != this.GetHashCode())
                return false;
            return Parameters.SequenceEqual(data.Parameters) && Area == data.Area && Weight == data.Weight;
        }

        /// <summary>
        /// 获取当前实例的哈希码。
        /// </summary>
        /// <returns>当前实例的哈希码。</returns>
        public override int GetHashCode() {
            return HashCode.Combine(Parameters.GetHashCode(), Area.GetHashCode(), Weight.GetHashCode());
        }
    }
}
