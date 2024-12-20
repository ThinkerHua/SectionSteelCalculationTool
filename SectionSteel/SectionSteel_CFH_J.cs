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
 *  SectionSteel_CFH_J.cs: 冷弯空心型钢（Cold forming hollow section steel）方管和矩管
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System;
using System.Text.RegularExpressions;

namespace SectionSteel {
    /// <summary>
    /// <para>冷弯空心型钢（Cold forming hollow section steel）方管和矩管。在以下模式中尝试匹配：</para>
    /// <see cref="Pattern_Collection.CFH_J_1"/>: <inheritdoc cref="Pattern_Collection.CFH_J_1"/><br/>
    /// <see cref="Pattern_Collection.CFH_J_2"/>: <inheritdoc cref="Pattern_Collection.CFH_J_2"/><br/>
    /// <see cref="Pattern_Collection.CFH_J_3"/>: <inheritdoc cref="Pattern_Collection.CFH_J_3"/><br/>
    /// </summary>
    public class SectionSteel_CFH_J : SectionSteelBase {
        private double h1, b1, h2, b2, t;
        GBData data;
        private static readonly GBData[] _gbDataSet = new GBData[] {
            new GBData("", new double[] { 20, 20, 1.2 }, 0.679, 0),
            new GBData("", new double[] { 20, 20, 1.5 }, 0.826, 0),
            new GBData("", new double[] { 20, 20, 1.75 }, 0.941, 0),
            new GBData("", new double[] { 20, 20, 2 }, 1.05, 0),
            new GBData("", new double[] { 25, 25, 1.2 }, 0.867, 0),
            new GBData("", new double[] { 25, 25, 1.5 }, 1.061, 0),
            new GBData("", new double[] { 25, 25, 1.75 }, 1.215, 0),
            new GBData("", new double[] { 25, 25, 2 }, 1.363, 0),
            new GBData("", new double[] { 30, 30, 1.5 }, 1.296, 0),
            new GBData("", new double[] { 30, 30, 1.75 }, 1.49, 0),
            new GBData("", new double[] { 30, 30, 2 }, 1.677, 0),
            new GBData("", new double[] { 30, 30, 2.5 }, 2.032, 0),
            new GBData("", new double[] { 30, 30, 3 }, 2.361, 0),
            new GBData("", new double[] { 40, 40, 1.5 }, 1.767, 0),
            new GBData("", new double[] { 40, 40, 1.75 }, 2.039, 0),
            new GBData("", new double[] { 40, 40, 2 }, 2.305, 0),
            new GBData("", new double[] { 40, 40, 2.5 }, 2.817, 0),
            new GBData("", new double[] { 40, 40, 3 }, 3.303, 0),
            new GBData("", new double[] { 40, 40, 4 }, 4.198, 0),
            new GBData("", new double[] { 50, 50, 1.5 }, 2.238, 0),
            new GBData("", new double[] { 50, 50, 1.75 }, 2.589, 0),
            new GBData("", new double[] { 50, 50, 2 }, 2.933, 0),
            new GBData("", new double[] { 50, 50, 2.5 }, 3.602, 0),
            new GBData("", new double[] { 50, 50, 3 }, 4.245, 0),
            new GBData("", new double[] { 50, 50, 4 }, 5.454, 0),
            new GBData("", new double[] { 60, 60, 2 }, 3.56, 0),
            new GBData("", new double[] { 60, 60, 2.5 }, 4.387, 0),
            new GBData("", new double[] { 60, 60, 3 }, 5.187, 0),
            new GBData("", new double[] { 60, 60, 4 }, 6.71, 0),
            new GBData("", new double[] { 60, 60, 5 }, 8.129, 0),
            new GBData("", new double[] { 70, 70, 2.5 }, 5.17, 0),
            new GBData("", new double[] { 70, 70, 3 }, 6.129, 0),
            new GBData("", new double[] { 70, 70, 4 }, 7.966, 0),
            new GBData("", new double[] { 70, 70, 5 }, 9.699, 0),
            new GBData("", new double[] { 80, 80, 2.5 }, 5.957, 0),
            new GBData("", new double[] { 80, 80, 3 }, 7.071, 0),
            new GBData("", new double[] { 80, 80, 4 }, 9.222, 0),
            new GBData("", new double[] { 80, 80, 5 }, 11.269, 0),
            new GBData("", new double[] { 90, 90, 3 }, 8.013, 0),
            new GBData("", new double[] { 90, 90, 4 }, 10.478, 0),
            new GBData("", new double[] { 90, 90, 5 }, 12.839, 0),
            new GBData("", new double[] { 90, 90, 6 }, 15.097, 0),
            new GBData("", new double[] { 100, 100, 4 }, 11.734, 0),
            new GBData("", new double[] { 100, 100, 5 }, 14.409, 0),
            new GBData("", new double[] { 100, 100, 6 }, 16.981, 0),
            new GBData("", new double[] { 110, 110, 4 }, 12.99, 0),
            new GBData("", new double[] { 110, 110, 5 }, 15.98, 0),
            new GBData("", new double[] { 110, 110, 6 }, 18.866, 0),
            new GBData("", new double[] { 120, 120, 4 }, 14.246, 0),
            new GBData("", new double[] { 120, 120, 5 }, 17.549, 0),
            new GBData("", new double[] { 120, 120, 6 }, 20.749, 0),
            new GBData("", new double[] { 120, 120, 8 }, 26.84, 0),
            new GBData("", new double[] { 130, 130, 4 }, 15.502, 0),
            new GBData("", new double[] { 130, 130, 5 }, 19.12, 0),
            new GBData("", new double[] { 130, 130, 6 }, 22.634, 0),
            new GBData("", new double[] { 130, 130, 8 }, 28.921, 0),
            new GBData("", new double[] { 140, 140, 4 }, 16.758, 0),
            new GBData("", new double[] { 140, 140, 5 }, 20.689, 0),
            new GBData("", new double[] { 140, 140, 6 }, 24.517, 0),
            new GBData("", new double[] { 140, 140, 8 }, 31.864, 0),
            new GBData("", new double[] { 150, 150, 4 }, 18.014, 0),
            new GBData("", new double[] { 150, 150, 5 }, 22.26, 0),
            new GBData("", new double[] { 150, 150, 6 }, 26.402, 0),
            new GBData("", new double[] { 150, 150, 8 }, 33.945, 0),
            new GBData("", new double[] { 160, 160, 4 }, 19.27, 0),
            new GBData("", new double[] { 160, 160, 5 }, 23.829, 0),
            new GBData("", new double[] { 160, 160, 6 }, 28.285, 0),
            new GBData("", new double[] { 160, 160, 8 }, 36.888, 0),
            new GBData("", new double[] { 170, 170, 4 }, 20.526, 0),
            new GBData("", new double[] { 170, 170, 5 }, 25.4, 0),
            new GBData("", new double[] { 170, 170, 6 }, 30.17, 0),
            new GBData("", new double[] { 170, 170, 8 }, 38.969, 0),
            new GBData("", new double[] { 180, 180, 4 }, 21.8, 0),
            new GBData("", new double[] { 180, 180, 5 }, 27, 0),
            new GBData("", new double[] { 180, 180, 6 }, 32.1, 0),
            new GBData("", new double[] { 180, 180, 8 }, 41.5, 0),
            new GBData("", new double[] { 190, 190, 4 }, 23, 0),
            new GBData("", new double[] { 190, 190, 5 }, 28.5, 0),
            new GBData("", new double[] { 190, 190, 6 }, 33.9, 0),
            new GBData("", new double[] { 190, 190, 8 }, 44, 0),
            new GBData("", new double[] { 200, 200, 4 }, 24.3, 0),
            new GBData("", new double[] { 200, 200, 5 }, 30.1, 0),
            new GBData("", new double[] { 200, 200, 6 }, 35.8, 0),
            new GBData("", new double[] { 200, 200, 8 }, 46.5, 0),
            new GBData("", new double[] { 200, 200, 10 }, 57, 0),
            new GBData("", new double[] { 220, 220, 5 }, 33.2, 0),
            new GBData("", new double[] { 220, 220, 6 }, 39.6, 0),
            new GBData("", new double[] { 220, 220, 8 }, 51.5, 0),
            new GBData("", new double[] { 220, 220, 10 }, 63.2, 0),
            new GBData("", new double[] { 220, 220, 12 }, 73.5, 0),
            new GBData("", new double[] { 250, 250, 5 }, 38, 0),
            new GBData("", new double[] { 250, 250, 6 }, 45.2, 0),
            new GBData("", new double[] { 250, 250, 8 }, 59.1, 0),
            new GBData("", new double[] { 250, 250, 10 }, 72.7, 0),
            new GBData("", new double[] { 250, 250, 12 }, 84.8, 0),
            new GBData("", new double[] { 280, 280, 5 }, 42.7, 0),
            new GBData("", new double[] { 280, 280, 6 }, 50.9, 0),
            new GBData("", new double[] { 280, 280, 8 }, 66.6, 0),
            new GBData("", new double[] { 280, 280, 10 }, 82.1, 0),
            new GBData("", new double[] { 280, 280, 12 }, 96.1, 0),
            new GBData("", new double[] { 300, 300, 6 }, 54.7, 0),
            new GBData("", new double[] { 300, 300, 8 }, 71.6, 0),
            new GBData("", new double[] { 300, 300, 10 }, 88.4, 0),
            new GBData("", new double[] { 300, 300, 12 }, 104, 0),
            new GBData("", new double[] { 350, 350, 6 }, 64.1, 0),
            new GBData("", new double[] { 350, 350, 8 }, 84.2, 0),
            new GBData("", new double[] { 350, 350, 10 }, 104, 0),
            new GBData("", new double[] { 350, 350, 12 }, 123, 0),
            new GBData("", new double[] { 400, 400, 8 }, 96.7, 0),
            new GBData("", new double[] { 400, 400, 10 }, 120, 0),
            new GBData("", new double[] { 400, 400, 12 }, 141, 0),
            new GBData("", new double[] { 400, 400, 14 }, 163, 0),
            new GBData("", new double[] { 450, 450, 8 }, 109, 0),
            new GBData("", new double[] { 450, 450, 10 }, 135, 0),
            new GBData("", new double[] { 450, 450, 12 }, 160, 0),
            new GBData("", new double[] { 450, 450, 14 }, 185, 0),
            new GBData("", new double[] { 500, 500, 8 }, 122, 0),
            new GBData("", new double[] { 500, 500, 10 }, 151, 0),
            new GBData("", new double[] { 500, 500, 12 }, 179, 0),
            new GBData("", new double[] { 500, 500, 14 }, 207, 0),
            new GBData("", new double[] { 500, 500, 16 }, 235, 0),
            new GBData("", new double[] { 30, 20, 1.5 }, 1.06, 0),
            new GBData("", new double[] { 30, 20, 1.75 }, 1.22, 0),
            new GBData("", new double[] { 30, 20, 2 }, 1.36, 0),
            new GBData("", new double[] { 30, 20, 2.5 }, 1.64, 0),
            new GBData("", new double[] { 40, 20, 1.5 }, 1.3, 0),
            new GBData("", new double[] { 40, 20, 1.75 }, 1.49, 0),
            new GBData("", new double[] { 40, 20, 2 }, 1.68, 0),
            new GBData("", new double[] { 40, 20, 2.5 }, 2.03, 0),
            new GBData("", new double[] { 40, 20, 3 }, 2.36, 0),
            new GBData("", new double[] { 40, 25, 1.5 }, 1.41, 0),
            new GBData("", new double[] { 40, 25, 1.75 }, 1.63, 0),
            new GBData("", new double[] { 40, 25, 2 }, 1.83, 0),
            new GBData("", new double[] { 40, 25, 2.5 }, 2.23, 0),
            new GBData("", new double[] { 40, 25, 3 }, 2.6, 0),
            new GBData("", new double[] { 40, 30, 1.5 }, 1.53, 0),
            new GBData("", new double[] { 40, 30, 1.75 }, 1.77, 0),
            new GBData("", new double[] { 40, 30, 2 }, 1.99, 0),
            new GBData("", new double[] { 40, 30, 2.5 }, 2.42, 0),
            new GBData("", new double[] { 40, 30, 3 }, 2.83, 0),
            new GBData("", new double[] { 50, 25, 1.5 }, 1.65, 0),
            new GBData("", new double[] { 50, 25, 1.75 }, 1.9, 0),
            new GBData("", new double[] { 50, 25, 2 }, 2.15, 0),
            new GBData("", new double[] { 50, 25, 2.5 }, 2.62, 0),
            new GBData("", new double[] { 50, 25, 3 }, 3.07, 0),
            new GBData("", new double[] { 50, 30, 1.5 }, 1.767, 0),
            new GBData("", new double[] { 50, 30, 1.75 }, 2.039, 0),
            new GBData("", new double[] { 50, 30, 2 }, 2.305, 0),
            new GBData("", new double[] { 50, 30, 2.5 }, 2.817, 0),
            new GBData("", new double[] { 50, 30, 3 }, 3.303, 0),
            new GBData("", new double[] { 50, 30, 4 }, 4.198, 0),
            new GBData("", new double[] { 50, 40, 1.5 }, 2.003, 0),
            new GBData("", new double[] { 50, 40, 1.75 }, 2.314, 0),
            new GBData("", new double[] { 50, 40, 2 }, 2.619, 0),
            new GBData("", new double[] { 50, 40, 2.5 }, 3.21, 0),
            new GBData("", new double[] { 50, 40, 3 }, 3.775, 0),
            new GBData("", new double[] { 50, 40, 4 }, 4.826, 0),
            new GBData("", new double[] { 55, 25, 1.5 }, 1.767, 0),
            new GBData("", new double[] { 55, 25, 1.75 }, 2.039, 0),
            new GBData("", new double[] { 55, 25, 2 }, 2.305, 0),
            new GBData("", new double[] { 55, 40, 1.5 }, 2.121, 0),
            new GBData("", new double[] { 55, 40, 1.75 }, 2.452, 0),
            new GBData("", new double[] { 55, 40, 2 }, 2.776, 0),
            new GBData("", new double[] { 55, 50, 1.75 }, 2.726, 0),
            new GBData("", new double[] { 55, 50, 2 }, 3.09, 0),
            new GBData("", new double[] { 60, 30, 2 }, 2.62, 0),
            new GBData("", new double[] { 60, 30, 2.5 }, 3.209, 0),
            new GBData("", new double[] { 60, 30, 3 }, 3.774, 0),
            new GBData("", new double[] { 60, 30, 4 }, 4.826, 0),
            new GBData("", new double[] { 60, 40, 2 }, 2.934, 0),
            new GBData("", new double[] { 60, 40, 2.5 }, 3.602, 0),
            new GBData("", new double[] { 60, 40, 3 }, 4.245, 0),
            new GBData("", new double[] { 60, 40, 4 }, 5.451, 0),
            new GBData("", new double[] { 70, 50, 2 }, 3.562, 0),
            new GBData("", new double[] { 70, 50, 3 }, 5.187, 0),
            new GBData("", new double[] { 70, 50, 4 }, 6.71, 0),
            new GBData("", new double[] { 70, 50, 5 }, 8.129, 0),
            new GBData("", new double[] { 80, 40, 2 }, 3.561, 0),
            new GBData("", new double[] { 80, 40, 2.5 }, 4.387, 0),
            new GBData("", new double[] { 80, 40, 3 }, 5.187, 0),
            new GBData("", new double[] { 80, 40, 4 }, 6.71, 0),
            new GBData("", new double[] { 80, 40, 5 }, 8.129, 0),
            new GBData("", new double[] { 80, 60, 3 }, 6.129, 0),
            new GBData("", new double[] { 80, 60, 4 }, 7.966, 0),
            new GBData("", new double[] { 80, 60, 5 }, 9.699, 0),
            new GBData("", new double[] { 90, 40, 3 }, 5.658, 0),
            new GBData("", new double[] { 90, 40, 4 }, 7.338, 0),
            new GBData("", new double[] { 90, 40, 5 }, 8.914, 0),
            new GBData("", new double[] { 90, 50, 2 }, 4.19, 0),
            new GBData("", new double[] { 90, 50, 2.5 }, 5.172, 0),
            new GBData("", new double[] { 90, 50, 3 }, 6.129, 0),
            new GBData("", new double[] { 90, 50, 4 }, 7.966, 0),
            new GBData("", new double[] { 90, 50, 5 }, 9.699, 0),
            new GBData("", new double[] { 90, 55, 2 }, 4.346, 0),
            new GBData("", new double[] { 90, 55, 2.5 }, 5.368, 0),
            new GBData("", new double[] { 90, 60, 3 }, 6.6, 0),
            new GBData("", new double[] { 90, 60, 4 }, 8.594, 0),
            new GBData("", new double[] { 90, 60, 5 }, 10.484, 0),
            new GBData("", new double[] { 95, 50, 2 }, 4.347, 0),
            new GBData("", new double[] { 95, 50, 2.5 }, 5.369, 0),
            new GBData("", new double[] { 100, 50, 3 }, 6.69, 0),
            new GBData("", new double[] { 100, 50, 4 }, 8.594, 0),
            new GBData("", new double[] { 100, 50, 5 }, 10.484, 0),
            new GBData("", new double[] { 120, 50, 2.5 }, 6.35, 0),
            new GBData("", new double[] { 120, 50, 3 }, 7.543, 0),
            new GBData("", new double[] { 120, 60, 3 }, 8.013, 0),
            new GBData("", new double[] { 120, 60, 4 }, 10.478, 0),
            new GBData("", new double[] { 120, 60, 5 }, 12.839, 0),
            new GBData("", new double[] { 120, 60, 6 }, 15.097, 0),
            new GBData("", new double[] { 120, 80, 3 }, 8.955, 0),
            new GBData("", new double[] { 120, 80, 4 }, 11.734, 0),
            new GBData("", new double[] { 120, 80, 5 }, 14.409, 0),
            new GBData("", new double[] { 120, 80, 6 }, 16.981, 0),
            new GBData("", new double[] { 140, 80, 4 }, 12.99, 0),
            new GBData("", new double[] { 140, 80, 5 }, 15.979, 0),
            new GBData("", new double[] { 140, 80, 6 }, 18.865, 0),
            new GBData("", new double[] { 150, 100, 4 }, 14.874, 0),
            new GBData("", new double[] { 150, 100, 5 }, 18.334, 0),
            new GBData("", new double[] { 150, 100, 6 }, 21.691, 0),
            new GBData("", new double[] { 150, 100, 8 }, 28.096, 0),
            new GBData("", new double[] { 160, 60, 3 }, 9.898, 0),
            new GBData("", new double[] { 160, 60, 4.5 }, 14.498, 0),
            new GBData("", new double[] { 160, 80, 4 }, 14.216, 0),
            new GBData("", new double[] { 160, 80, 5 }, 17.519, 0),
            new GBData("", new double[] { 160, 80, 6 }, 20.749, 0),
            new GBData("", new double[] { 160, 80, 8 }, 26.81, 0),
            new GBData("", new double[] { 180, 65, 3 }, 11.075, 0),
            new GBData("", new double[] { 180, 65, 4.5 }, 16.264, 0),
            new GBData("", new double[] { 180, 100, 4 }, 16.758, 0),
            new GBData("", new double[] { 180, 100, 5 }, 20.689, 0),
            new GBData("", new double[] { 180, 100, 6 }, 24.517, 0),
            new GBData("", new double[] { 180, 100, 8 }, 31.861, 0),
            new GBData("", new double[] { 200, 100, 4 }, 18.014, 0),
            new GBData("", new double[] { 200, 100, 5 }, 22.259, 0),
            new GBData("", new double[] { 200, 100, 6 }, 26.101, 0),
            new GBData("", new double[] { 200, 100, 8 }, 34.376, 0),
            new GBData("", new double[] { 200, 120, 4 }, 19.3, 0),
            new GBData("", new double[] { 200, 120, 5 }, 23.8, 0),
            new GBData("", new double[] { 200, 120, 6 }, 28.3, 0),
            new GBData("", new double[] { 200, 120, 8 }, 36.5, 0),
            new GBData("", new double[] { 200, 150, 4 }, 21.2, 0),
            new GBData("", new double[] { 200, 150, 5 }, 26.2, 0),
            new GBData("", new double[] { 200, 150, 6 }, 31.1, 0),
            new GBData("", new double[] { 200, 150, 8 }, 40.2, 0),
            new GBData("", new double[] { 220, 140, 4 }, 21.8, 0),
            new GBData("", new double[] { 220, 140, 5 }, 27, 0),
            new GBData("", new double[] { 220, 140, 6 }, 32.1, 0),
            new GBData("", new double[] { 220, 140, 8 }, 41.5, 0),
            new GBData("", new double[] { 250, 150, 4 }, 24.3, 0),
            new GBData("", new double[] { 250, 150, 5 }, 30.1, 0),
            new GBData("", new double[] { 250, 150, 6 }, 35.8, 0),
            new GBData("", new double[] { 250, 150, 8 }, 46.5, 0),
            new GBData("", new double[] { 260, 180, 5 }, 33.2, 0),
            new GBData("", new double[] { 260, 180, 6 }, 39.6, 0),
            new GBData("", new double[] { 260, 180, 8 }, 51.5, 0),
            new GBData("", new double[] { 260, 180, 10 }, 63.2, 0),
            new GBData("", new double[] { 300, 200, 5 }, 38, 0),
            new GBData("", new double[] { 300, 200, 6 }, 45.2, 0),
            new GBData("", new double[] { 300, 200, 8 }, 59.1, 0),
            new GBData("", new double[] { 300, 200, 10 }, 72.7, 0),
            new GBData("", new double[] { 350, 250, 5 }, 45.8, 0),
            new GBData("", new double[] { 350, 250, 6 }, 54.7, 0),
            new GBData("", new double[] { 350, 250, 8 }, 71.6, 0),
            new GBData("", new double[] { 350, 250, 10 }, 88.4, 0),
            new GBData("", new double[] { 400, 200, 5 }, 45.8, 0),
            new GBData("", new double[] { 400, 200, 6 }, 54.7, 0),
            new GBData("", new double[] { 400, 200, 8 }, 71.6, 0),
            new GBData("", new double[] { 400, 200, 10 }, 88.4, 0),
            new GBData("", new double[] { 400, 200, 12 }, 104, 0),
            new GBData("", new double[] { 400, 250, 5 }, 49.7, 0),
            new GBData("", new double[] { 400, 250, 6 }, 59.4, 0),
            new GBData("", new double[] { 400, 250, 8 }, 77.9, 0),
            new GBData("", new double[] { 400, 250, 10 }, 96.2, 0),
            new GBData("", new double[] { 400, 250, 12 }, 113, 0),
            new GBData("", new double[] { 450, 250, 6 }, 64.1, 0),
            new GBData("", new double[] { 450, 250, 8 }, 84.2, 0),
            new GBData("", new double[] { 450, 250, 10 }, 104, 0),
            new GBData("", new double[] { 450, 250, 12 }, 123, 0),
            new GBData("", new double[] { 500, 300, 6 }, 73.5, 0),
            new GBData("", new double[] { 500, 300, 8 }, 96.7, 0),
            new GBData("", new double[] { 500, 300, 10 }, 120, 0),
            new GBData("", new double[] { 500, 300, 12 }, 141, 0),
            new GBData("", new double[] { 550, 350, 8 }, 109, 0),
            new GBData("", new double[] { 550, 350, 10 }, 135, 0),
            new GBData("", new double[] { 550, 350, 12 }, 160, 0),
            new GBData("", new double[] { 550, 350, 14 }, 185, 0),
            new GBData("", new double[] { 600, 400, 8 }, 122, 0),
            new GBData("", new double[] { 600, 400, 10 }, 151, 0),
            new GBData("", new double[] { 600, 400, 12 }, 179, 0),
            new GBData("", new double[] { 600, 400, 14 }, 207, 0),
            new GBData("", new double[] { 600, 400, 16 }, 235, 0),
        };

        public override GBData[] GBDataSet => _gbDataSet;

        public SectionSteel_CFH_J() { }
        public SectionSteel_CFH_J(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue(SectionSteelBase sender, ProfileTextChangingEventArgs e) {
            try {
                if (string.IsNullOrEmpty(e.NewText))
                    throw new MismatchedProfileTextException(e.NewText);

                Match match = Regex.Match(e.NewText, Pattern_Collection.CFH_J_1);
                if (!match.Success)
                    match = Regex.Match(e.NewText, Pattern_Collection.CFH_J_2);
                if (!match.Success)
                    match = Regex.Match(e.NewText, Pattern_Collection.CFH_J_3);
                if (!match.Success)
                    throw new MismatchedProfileTextException(e.NewText);

                double.TryParse(match.Groups["h1"].Value, out h1);
                double.TryParse(match.Groups["b1"].Value, out b1);
                double.TryParse(match.Groups["h2"].Value, out h2);
                double.TryParse(match.Groups["b2"].Value, out b2);
                double.TryParse(match.Groups["t"].Value, out t);

                if (b1 == 0) b1 = h1;
                if (h2 == 0) h2 = h1;
                if (b2 == 0) b2 = b1;

                if (h2 == h1 && b2 == b1)
                    data = FindGBData(_gbDataSet, h1, b1, t);
                else
                    data = null;

                h1 *= 0.001; b1 *= 0.001; h2 *= 0.001; b2 *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {

                throw;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc path="/param[1]"/>
        /// <para><b>在本类中：PRECISELY 等效于 ROUGHLY，不实现GBDATA（国标截面特性表格中无表面积数据）</b></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public override string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (h1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (b1 == h1 && b2 == h2 && h2 == h1) {
                    if (exclude_topSurface)
                        formula = $"{h1}*3";
                    else
                        formula = $"{h1}*4";

                    break;
                }

                if (h2 != h1)
                    formula = $"{h1}+{h2}";
                else
                    formula = $"{h1}*2";

                if (b2 != b1) {
                    if (exclude_topSurface)
                        formula += $"+{(b1 + b2) * 0.5}";
                    else
                        formula += $"+{b1}+{b2}";
                } else {
                    if (exclude_topSurface)
                        formula += $"+{b1}";
                    else
                        formula += $"+{b1}*2";
                }
                break;
            case FormulaAccuracyEnum.GBDATA:
                break;
            default:
                break;
            }

            return formula;
        }

        public override string GetSiffenerProfileStr(bool truncatedRounding) {
            string stifProfileText = string.Empty;
            if (h1 == 0) return stifProfileText;

            double t, b, l;
            t = this.t;
            b = (b1 + b2) * 0.5 - this.t * 2;
            l = (h1 + h2) * 0.5 - this.t * 2;
            t *= 1000; b *= 1000; l *= 1000;
            if (truncatedRounding) {
                t = Math.Truncate(t);
                b = Math.Truncate(b);
                l = Math.Truncate(l);
            }
            stifProfileText = $"PL{t}*{b}*{l}";

            return stifProfileText;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public override string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (h1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (h2 == h1 && b2 == b1 && b1 == h1)
                    formula = $"({h1}*4";
                else {
                    if (h2 != h1)
                        formula = $"({h1}+{h2}";
                    else
                        formula = $"({h1}*2";

                    if (b2 != b1)
                        formula += $"+{b1}+{b2}";
                    else
                        formula += $"+{b1}*2";
                }

                formula += $"-{t}*4)*{t}*{DENSITY}";
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data != null)
                    formula = $"{data.Weight}";
                break;
            default:
                break;
            }

            return formula;
        }
    }
}
