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
 *  GenerationOption.cs: 包含生成操作所需要的选项信息
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using SectionSteel;

namespace SectionSteelCalculationTool {
    public class GenerationOption {
        public GenerationTypeEnum GenerationType { get; set; }
        public FormulaAccuracyEnum Accuracy { get; set; }
        public PIStyleEnum PIStyle { get; set; }
        public bool ExcludeTopSurface { get; set; }
        public bool TruncatedRounding { get; set; }
        public Offset<int> TargetOffset { get; set; } = new Offset<int>();
        public bool OverwriteExistingData { get; set; }
    }
}
