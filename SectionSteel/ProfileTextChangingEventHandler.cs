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
 *  ProfileTextChangingEventHandler.cs: 定义处理截面文本变更事件的方法和事件参数
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System;

namespace SectionSteel {
    /// <summary>
    /// 截面文本变更事件参数。
    /// </summary>
    public class ProfileTextChangingEventArgs : EventArgs {
        /// <summary>
        /// 当前截面文本。
        /// </summary>
        public string CurrentText { get; }
        /// <summary>
        /// 新截面文本。
        /// </summary>
        public string NewText { get; }
        /// <summary>
        /// 使用当前和新截面文本创建事件参数实例。
        /// </summary>
        /// <param name="currentText">当前截面文本</param>
        /// <param name="newText">新截面文本</param>
        public ProfileTextChangingEventArgs(string? currentText, string? newText) {
            CurrentText = currentText ?? string.Empty;
            NewText = newText ?? string.Empty;
        }
    }
    /// <summary>
    /// 表示将用于处理截面文本变更事件的方法。
    /// </summary>
    /// <param name="sender">事件源</param>
    /// <param name="e">事件参数</param>
    public delegate void ProfileTextChangingEventHandler(SectionSteelBase sender, ProfileTextChangingEventArgs e);
}
