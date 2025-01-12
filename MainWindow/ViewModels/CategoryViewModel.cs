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
 *  CategoryViewModel.cs: 分组信息的视图模型
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/

using CommunityToolkit.Mvvm.ComponentModel;

namespace SectionSteelCalculationTool.ViewModels {
    public partial class CategoryViewModel : ObservableObject {
        [ObservableProperty]
        private bool _isSelected;

        public string Category { get; set; }
    }
}
