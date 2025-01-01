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
 *  MismatchedProfileTextException.cs: 不匹配的截面规格文本引发的异常
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System;

namespace SectionSteel {
    /// <summary>
    /// 不匹配的截面规格文本。
    /// </summary>
    public class MismatchedProfileTextException : Exception {
        public MismatchedProfileTextException() { }
        public MismatchedProfileTextException(string profileText) : base(profileText) { }
        public override string ToString() {
            return $"不匹配的截面规格文本：{Message}";
        }
    }
}
