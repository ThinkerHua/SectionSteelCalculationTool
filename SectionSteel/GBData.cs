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
        /// 比较当前实例与给定实例是否相等
        /// </summary>
        /// <remarks>通过比较两个实例的 <see cref="GBData.Parameters"/> 属性进行判断，
        /// 其他的属性不参与。</remarks>
        /// <param name="data">给定实例</param>
        /// <returns>相等返回 true，不等返回 false。</returns>
        public bool Equals(GBData data) {
            if (data.GetHashCode() == this.GetHashCode())
                return true;
            return Parameters.SequenceEqual(data.Parameters);
        }
    }
}
