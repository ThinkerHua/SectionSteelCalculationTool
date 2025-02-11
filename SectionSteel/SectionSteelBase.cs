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
 *  SectionSteelBase.cs: 型钢基类
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System;

namespace SectionSteel {
    /// <summary>
    /// 型钢基类。
    /// </summary>
    public abstract class SectionSteelBase {
        private string? _profileText;
        private ProfileTextChangingEventHandler? _profileTextChangingEventHandler;
        /// <summary>
        /// 钢材密度。
        /// </summary>
        public static readonly int DENSITY = 7850;
        /// <summary>
        /// 型钢截面文本。
        /// </summary>
        public string? ProfileText {
            get {
                return _profileText;
            }
            set {
                try {
                    var e = new ProfileTextChangingEventArgs(_profileText, value);
                    OnProfileTextChanging(e);
                    _profileText = value;
                } catch (MismatchedProfileTextException) {
                    //引发MismatchedProfileTextException时，不改变ProfileText
                    throw;
                }
            }
        }
        /// <summary>
        /// 计算式中π的书写样式。
        /// </summary>
        public virtual PIStyleEnum PIStyle { get; set; }
        /// <summary>
        /// 国标数据集合。
        /// </summary>
        public abstract GBData[]? GBDataSet { get; }
        /// <summary>
        /// 截面文本变更事件。
        /// </summary>
        public event ProfileTextChangingEventHandler ProfileTextChanging {
            add {
                //不允许重复注册 SetFieldsValue 方法
                if (value == SetFieldsValue)
                    _profileTextChangingEventHandler -= value;
                _profileTextChangingEventHandler += value;
            }
            remove {
                //不允许移除 SetFieldsValue 方法
                if (value != SetFieldsValue)
                    _profileTextChangingEventHandler -= value;
            }
        }
        /// <summary>
        /// 为 <see cref="ProfileTextChanging"/> 事件注册 <see cref="SetFieldsValue(SectionSteelBase, ProfileTextChangingEventArgs)"/> 方法。
        /// </summary>
        protected SectionSteelBase() {
            ProfileTextChanging += SetFieldsValue;
        }
        protected void OnProfileTextChanging(ProfileTextChangingEventArgs e) {
            _profileTextChangingEventHandler?.Invoke(this, e);
        }
        /// <summary>
        /// 获取型钢单位长度表面积计算式。
        /// <para><b>注意：</b>对于<b>变截面型钢</b>，实际上应当结合型钢长度综合考虑才能计算出准确值。
        /// 此处为简化计算，不考虑型钢长度。非变截面型钢不受影响。</para>
        /// </summary>
        /// <param name="accuracy">
        ///     <list type="bullet">
        ///         <item>ROUGHLY: 粗略的</item>
        ///         <item>PRECISELY: 稍精确的</item>
        ///         <item>GBDATA: 国标截面特性表中的理论值</item>
        ///     </list>
        /// </param>
        /// <param name="exclude_topSurface">
        ///     <list type="bullet">
        ///         <item>True: 扣除上表面积(宽度)</item>
        ///         <item>False: 完整单位面积</item>
        ///     </list>
        /// </param>
        /// <returns>单位为m^2/m。</returns>
        public abstract string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface);
        /// <summary>
        /// 获取型钢单位长度重量计算式。
        /// <para><b>注意：</b>对于<b>变截面型钢</b>，实际上应当结合型钢长度综合考虑才能计算出准确值。
        /// 此处为简化计算，不考虑型钢长度。非变截面型钢不受影响。</para>
        /// </summary>
        /// <param name="accuracy">
        ///     <para>ROUGHLY: 粗略的</para>
        ///     <para>PRECISELY: 稍精确的</para>
        ///     <para>GBDATA: 国标截面特性表中的理论值</para>
        /// </param>
        /// <returns>单位为kg/m。</returns>
        public abstract string GetWeightFormula(FormulaAccuracyEnum accuracy);
        /// <summary>
        /// 获取型钢加劲肋截面文本。
        /// </summary>
        /// <param name="truncatedRounding">截面参数是否截尾取整。</param>
        /// <returns>加劲肋截面文本。</returns>
        public abstract string GetSiffenerProfileStr(bool truncatedRounding);
        /// <summary>
        /// 查找国标数据集合中符合要求的数据。
        /// </summary>
        /// <param name="dataSet">国标数据集合</param>
        /// <param name="byName">按 <see cref="GBData.Name"/> 查找</param>
        /// <returns>找到的符合要求的第一条数据。</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"><paramref name="byName"/> 为 null 或 <see cref="string.Empty"/> 时引发。</exception>
        protected static GBData? FindGBData(GBData[] dataSet, string byName) {
            ArgumentNullException.ThrowIfNull(dataSet);

            if (string.IsNullOrEmpty(byName)) {
                throw new ArgumentException($"“{nameof(byName)}”不能为 null 或空。", nameof(byName));
            }

            return Array.Find(dataSet, d => d.Name == byName);
        }
        /// <summary>
        /// 查找国标数据集合中符合要求的数据。
        /// </summary>
        /// <param name="dataSet">国标数据集合</param>
        /// <param name="byParams">按 <see cref="GBData.Parameters"/> 查找</param>
        /// <returns>找到的符合要求的第一条数据。</returns>
        /// <exception cref="ArgumentNullException"></exception>
        protected static GBData? FindGBData(GBData[] dataSet, params double[] byParams) {
            ArgumentNullException.ThrowIfNull(dataSet);

            ArgumentNullException.ThrowIfNull(byParams);

            static bool Match(double[] arr1, params double[] arr2) {
                ArgumentNullException.ThrowIfNull(arr1);

                ArgumentNullException.ThrowIfNull(arr2);

                var length = Math.Min(arr1.Length, arr2.Length);
                if (length == 0) return false;

                for (int i = 0; i < length; i++) {
                    if (arr1[i] != arr2[i])
                        return false;
                }

                return true;
            }

            return Array.Find(dataSet, d => Match(d.Parameters, byParams));
        }
        /// <summary>
        /// 发生 <see cref="ProfileTextChanging"/> 事件时，为字段赋值。
        /// </summary>
        /// <remarks>具体实现中，应确保如果发生 <see cref="MismatchedProfileTextException"/> 异常，
        /// 原有字段值保持不变。</remarks>
        protected abstract void SetFieldsValue(SectionSteelBase sender, ProfileTextChangingEventArgs e);
    }
}
