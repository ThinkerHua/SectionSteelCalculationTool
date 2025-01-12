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
 *  Offset.cs: 目标偏移的视图模型
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/

using CommunityToolkit.Mvvm.ComponentModel;

namespace SectionSteelCalculationTool.ViewModels {
    public partial class Offset : ObservableObject {
        [ObservableProperty]
        private int _rowOffset;

        [ObservableProperty]
        private int _columnOffset;

        public Offset(int rowOffset, int columnOffset) {
            _rowOffset = rowOffset;
            _columnOffset = columnOffset;
        }

        public Offset(Offset offset) {
            _rowOffset = offset.RowOffset;
            _columnOffset = offset.ColumnOffset;
        }
    }
}
