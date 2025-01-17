# 书写形式

融合了多种书写形式：国标规定的规格表示方法、Tekla Structures截面库表示方法以及各类设计图纸中常见的规格表示方法，方括号中内容可以省略。

## H型钢
1. 前置标识符为 H 或 HP 或 HW 或 HM 或 HN 或 HT 或 WH，后续参数形式为 h1\[~h2\]\*b1\[/b2\]\*s\*t1\[/t2\]。例如：
   
    - HM244\*175\*7\*11，表示国标热轧中翼缘H型钢，高244，宽175，腹板厚7，翼缘厚11

    - H500~800\*400/300\*16\*22/20，表示焊接楔形H型钢，小头高500、大头高800，上翼缘宽400，下翼缘宽300，腹板厚16，上翼缘厚22，下翼缘厚20。省略第2个高度值表示等高H型钢，省略下翼缘宽度或厚度表示与上翼缘相同。
    
2. 前置标识符为 HI 或 PHI 或 WI，后续参数形式为 h1\[-h2\]-s-t1\*b1\[-t2\*b2\]。例如：
    
    - PHI500-800-16-22\*400-20\*300，表示焊接楔形H型钢，小头高500、大头高800，腹板厚16，上翼缘厚22，上翼缘宽400，下翼缘厚20，下翼缘宽300。省略第2个高度值表示等高H型钢，省略下翼缘厚度和宽度表示与上翼缘相同。
    
3. 前置标识符为 H 或 HW 或 HM 或 HN 或 HT，后续参数形式为 H*B，仅支持整数。

    此模式下在国标截面表格中查找，同一型号名下有多项时按以下规则进行匹配：
    
    - 匹配非星标型号，例如：HW200\*200型号名下，有H200\*200\*8\*12和H200\*204\*12\*12两种型号，其中第1种不带星标，第2种带星标。此时匹配第1种。
    
    - 如均为星标型号，则匹配参数与型号名一致的型号，例如：HM550\*300型号名下，有H544\*300\*11\*15和H550\*300\*11\*18两种型号，均带星标，但第2种参数与型号名一致。此时匹配第2种。
    
    - 如均不符合，则匹配第1项，例如：HW500\*500型号名下，有H492\*465\*15\*20、H502\*465\*15\*25、H502\*470\*20\*25三种型号，均带星标，且三种参数均与型号名不一致。此时匹配第1种。
        
4. 前置标识符为 B_WLD_K，后续参数形式为 h1\*h2\*b1\*s\*t1。例如：
    
    - B_WLD_K1000\*750\*450\*15\*30，表示焊接等宽楔形H型钢，大头高1000、小头高750，宽450，腹板厚15，翼缘厚30。

5. 前置标识符为 B_WLD_A 或 BH，后续参数形式为 h1\*b1\*s\*t1。例如：
    
    - B_WLD_A600\*250\*10\*25，表示焊接等截面H型钢，高600，宽250，腹板厚10，翼缘厚25。

6. 前置标识符为 B_WLD_H，后续参数形式为 h1\*b1\*b2\*s\*t1\*t2。例如：
    
    - B_WLD_H600\*250\*200\*10\*25\*20，表示焊接H型钢，高600，上翼缘宽250、下翼缘宽200，腹板厚10，上翼缘厚25、下翼缘厚20。

7. 前置标识符为 I_VAR_A，后续参数形式为 h1-h2\*b1-b2\*s\*t1。例如：
    
    - I_VAR_A400-600\*400-450\*12\*19，表示对称变截面H型钢，小头高400、大头高600，小头宽400、大头宽450，腹板厚12，翼缘厚19。

## 十字拼接H型钢
1. 前置标识符为 HH，后续参数形式为 h1\*b1\*s1\*t1\[+h2\*b2\*s2\*t2\]。例如：
    
    - HH400\*400\*13\*21+350\*350\*12\*19，表示由H400\*400\*13\*21和H350\*350\*12\*19两个规格的H型钢十字拼接而成。省略第2个H型钢的规格，表示两个H型钢的规格相同。
    
2. 前置标识符为 B_WLD_O，后续参数形式为 b1\*t1\*s1\*b2\*t2\*s2\*h1\*h2。例如：
    
    - B_WLD_O120\*12\*8\*100\*10.8\*5.7\*250\*200，表示由H250\*120\*8\*12和H200\*100\*5.7\*10.8两个规格的H型钢十字拼接而成。

## 剖分T型钢
1. 前置标识符为 T 或 TW 或 TM 或 TN，后续参数形式为 h1\[~h2\]\*b\*s\*t。例如：
    
    - T300~200\*400\*13\*21，表示变截面剖分T型钢，大头高300、小头高200，宽400，腹板厚13，翼缘厚21。省略第2个高度表示等高。
    
2. 前置标识符为 T，后续参数形式为 h1-s-t-b。例如：
    
    - T200-10-15-100，表示剖分T型钢，高200，腹板厚10，翼缘厚15，翼缘宽100。
    
3. 前置标识符为 T 或 TW 或 TM 或 TN，后续参数形式为 H\*B，仅支持整数。此模式下在国标截面表格中查找，同一型号名下有多项时按最接近项匹配：
    
    - TN250\*150型号名下，有TN246\*150\*7\*12、TN250\*152\*9\*16、TN252\*153\*10\*18三种型号，其中第2种型号TN250\*152\*9\*16与TN250\*150最接近。此时匹配第2种。
    
4. 前置标识符为 B_WLD_E，后续参数形式为 h1\*b\*s\*t。例如：
    
    - B_WLD_E400\*250\*10\*25，表示剖分T型钢，高400，宽250，腹板厚10，翼缘厚25。

## 工字钢
1. 前置标识符为 I，后续参数形式为 h\*b\*s。例如：
    
    - I450\*150\*11.5，表示高450，腿宽150，腰厚11.5。
    
2. 前置标识符为 I，后续参数形式为 h\[a|b|c\]。例如：
    
    - I45a，是上例I450\*15\*11.5的简记形式。

## 槽钢
1. 前置标识符为 \[ 或 C，后续参数形式为 h\*b\*s。例如：
    
    - \[200\*75\*9，表示高200，腿宽75，腰厚9。
    
2. 前置标识符为 \[ 或 C，后续参数形式为 h\[a|b|c\]。例如：
    
    - \[20b，是上例\[200\*75\*9的简记形式。

##  双拼槽钢 - 口对口形式
1. 前置标识符为 \[\]，后续参数形式为 h\*b\*s。

    除标识符不同外，与[槽钢](#槽钢)一致。

2. 前置标识符为 \[\]，后续参数形式为 h\[a|b|c\]。

    除标识符不同外，与[槽钢](#槽钢)一致。

##  双拼槽钢 - 背对背形式
1. 前置标识符为 2\[ 或 2C 或\]\[，后续参数形式为 h\*b\*s。

    除标识符不同外，与[槽钢](#槽钢)一致。

2. 前置标识符为 2\[ 或 2C 或\]\[，后续参数形式为 h\[a|b|c\]。

    除标识符不同外，与[槽钢](#槽钢)一致。

## 角钢
1. 前置标识符为 ∠ 或 L，后续参数形式为 h\[\*b\]\*t。例如：

    - L160\*100\*16，表示不等宽角钢，长边宽160、短边宽100，边厚16。省略第2个边宽表示等宽。

2. 前置标识符为 ∠ 或 L，后续参数形式为 h\[/b\]，以cm为单位。
   
    此模式下在国标截面表格中查找，同一型号名下匹配第一项。省略第2个边宽表示等宽。例如：

    - L10/6.3型号名下，有L100\*6.3\*6、L100\*6.3\*7、L100\*6.3\*8、L100\*6.3\*10四种规格。此时匹配第1种。

##  双拼角钢 - 背对背
1. 前置标识符为 2∠ 或 2L，后续参数形式为 h\[\*b\]\*t。
   
    除标识符不同外，与[角钢](#角钢)一致。

2. 前置标识符为 2∠ 或 2L，后续参数形式为 h\[/b\]，以cm为单位。

    除标识符不同外，与[角钢](#角钢)一致。

## 冷弯空心型钢 - 方形型钢和矩形型钢
1. 前置标识符为 F 或 J 或 P 或 TUB 或 RHS 或 SHS 或 CFRHS，后续参数形式为 h1\*t。例如：

    - F100\*6，表示冷弯方形空心型钢，边长100，壁厚6。

2. 前置标识符为 F 或 J 或 P 或 RHS 或 SHS 或 CFRHS，后续参数形式为 h1\*b1\*t。例如：

    - J150\*100\*6，表示冷弯矩形空心型钢，长边150，短边100，壁厚6。

3. 前置标识符为 P 或 RHS 或 SHS 或 CFRHS，后续参数形式为 h1\*b1-h2\*b2\*t。例如：

    - SHS200\*100-300\*150\*5，表示冷弯矩形空心型钢，小头长边200、短边100，大头长边300、短边150，壁厚5。

## 冷弯空心型钢 - 圆管
1. 前置标识符为 Y 或 Φ 或 φ，后续参数形式为 d\*t。例如：

    - Φ273\*5，表示冷弯圆形空心型钢，外径273，壁厚5。

## 箱型管（焊接矩形管）
1. 前置标识符为 B_WLD_F，后续参数形式为 h1\*b1\*s。例如：

    - B_WLD_F1000\*450\*15\*30，表示焊接矩管，高1000，宽450，腹板厚15，翼缘厚30。省略翼缘厚度表示与腹板等厚。

2. 前置标识符为 RECT 或 B_WLD_F 或 B_BUILT，后续参数形式为 h1\*b1\*s\*t。例如：

    - B_BUILT400\*200\*10\*15，表示焊接矩管，高400，宽200，腹板厚10，翼缘厚15。

3. 前置标识符为 R，后续参数形式为 h1~h2\*b1\*s\*t。例如：

    - R400~600\*200\*10\*15，表示楔形焊接矩管，小头高400、大头高600，宽200，腹板厚10，翼缘厚15。

4. 前置标识符为 B_WLD_J，后续参数形式为 h1\*h2\*b1\*s\*t。例如：

    - B_WLD_J400\*600\*200\*10\*15，表示楔形焊接矩管，小头高400、大头高600，宽200，腹板厚10，翼缘厚15。

5. 前置标识符为 B_VAR_A 或 B_VAR_B 或 B_VAR_C，后续参数形式为 h1-h2\*s\[*b1\[-b2\]\]。该类型是Tekla Structures专用截面，b1, b2 值均不起作用，可直接忽略。例如：

    - B_VAR_A400-300\*10\*0-0，表示变截面焊接方管，大头边长400，小头边长300，壁厚10。

6. 前置标识符为 RHSC，后续参数形式为 H1\*h1\*H2\*h2\*s\*b1。例如：

    - RHSC835\*764\*570\*499\*20\*350，表示变截面焊接箱型管，大头一边高835、一边高764，小头一边高570、一边高499，壁厚20，宽350。

## 圆形型钢
1. 前置标识符为 D 或 ELD 或 ROD，后续参数形式为 d1\[\*r1\*d2\*r2\]。例如：

    - ELD200\*150\*180\*120，表示变截面实心圆钢，大头椭圆主轴200、副轴150，小头椭圆主轴180、副轴120。省略第2~4个参数，表示等截面。

2. 前置标识符为 D 或 PIP 或 CFCHS 或 CHS 或 EPD 或 O 或 PD 或 TUBE，后续参数形式为 d1\[\*d2\]\*t。例如：

    - CFCHS200\*150\*10，表示变截面空心圆钢，大头正圆直径200，小头正圆直径150，壁厚10。省略第2个参数表示等截面。

3. 前置标识符为 D 或 PIP 或 CFCHS 或 CHS 或 EPD 或 O 或 PD 或 TUBE，后续参数形式为 d1\*r1\*d2\*r2\*t。例如：

    - CFCHS300\*250\*200\*150\*10，表示变截面空心圆钢，大头椭圆主轴300、副轴250，小头椭圆主轴200、副轴150，壁厚10。

## 冷弯开口型钢 - 内卷边槽钢
1. 前置标识符为 C，后续参数形式为 h\*b1\*c1\*t。例如：

    - C160\*60\*20\*2.0，表示冷弯内卷边槽钢，高160，宽60，内卷边高20，壁厚2。

2. 前置标识符为 CC，后续参数形式为 h-t-c1-b1\[-c2-b2\]。例如：

    - CC200-5-30-100-40-150，表示变截面冷弯内卷边槽钢，高200，壁厚5，小头内卷边高30、宽100，大头内卷边高40、宽150。省略后2个参数表示等截面。

## 双拼冷弯内卷边槽钢 - 口对口
1. 前置标识符为 2CM，后续参数形式为 h\*b1\*c1\*t。

    除标识符不同外，与[内卷边槽钢](#冷弯开口型钢---内卷边槽钢)一致。

2. 前置标识符为 2CCM，后续参数形式为 h-t-c1-b1\[-c2-b2\]。

    除标识符不同外，与[内卷边槽钢](#冷弯开口型钢---内卷边槽钢)一致。

## 双拼冷弯内卷边槽钢 - 背对背
1. 前置标识符为 2C，后续参数形式为 h\*b1\*c1\*t。

    除标识符不同外，与[内卷边槽钢](#冷弯开口型钢---内卷边槽钢)一致。

2. 前置标识符为 2CC，后续参数形式为 h-t-c1-b1\[-c2-b2\]。

    除标识符不同外，与[内卷边槽钢](#冷弯开口型钢---内卷边槽钢)一致。

## 冷弯开口型钢 - 卷边Z形钢
1. 前置标识符为 Z 或 XZ，后续参数形式为 h\*b1\*c1\*t。例如：

    - Z160\*60\*20\*2.5，表示冷弯卷边Z形钢，高160，宽60，卷边高20，壁厚2.5。

2. 前置标识符为 ZZ，后续参数形式为 h-t-c1-b1\[-c2-b2\]。例如：

    - ZZ200-5-30-100-40-150，表示变截面冷弯卷边Z形钢，高200，壁厚5，小头卷边高30、宽100，大头卷边高40、宽150。省略后2个参数表示等截面。

## 板材 - 矩形、直角三角形、梯形、圆形
1. 前置标识符为 PL，后续参数形式为 t\*b1\[~b2\]\[\*l1\[~l2\]\]。参数顺序不做硬性规定，自动以最小值为厚度，最大值为长度（长度省略时较大值为宽度）。例如：

    - PL100\*1000\*10，表示矩形板，厚10，宽100，长1000。可省略长度。

    - PL0~100\*10\*1000，表示直角三角形板，厚10，短边100，长边1000。
    
    - PL10\*100~300\*1000，表示梯形板，厚10，上底100、下底300，高1000。

2. 前置标识符为 PLT，后续参数形式为 t\*b\*l。参数顺序不做硬性规定，自动以最小值为厚度，最大值为长度。例如：

    - PLT300\*500\*10，表示直角三角形板，厚10，短边300，长边500。

3. 前置标识符为 PLD 或 PLO，后续参数形式为 t\*d。参数顺序不做硬性规定，自动以较小值为厚度，较大值为直径。例如：

    - PLD10\*500，表示圆形板，厚10，直径500。

4. nPLt\*b1\[~b2\]\*l1\[~l2\], nPLTt\*b\*l, nPLDt\*d, nPLOt\*d 的任意组合形式。n表示数量。各项之间用 + 或 - 连接，分别表示扩展和剔除。

    应保证厚度值一致。其中"PL"标识符指示的板材，注意其与序号1中单独书写形式的区别，长度值不应省略。例如：

    - PL14\*400\*500-2PLT14\*100.5\*115+1.5PLO14\*250，表示由1块矩形板和1.5块圆形板拼接，并切割掉2块直角三角形板而成的板材。

## 球体
1. 前置标识符为SPHERE，后续参数为d。例如：

    - SPHERE500，表示直径500的球体。

# 单位长度面积和重量计算规则
## 国标数据
在国标截面特性表格中查找。

仅支持热轧H型钢、热轧剖分型钢、热轧工字钢、热轧槽钢、热轧角钢、冷弯内卷边槽钢、冷弯卷边Z形钢、冷弯空心方管、冷弯空心矩管、冷弯空心圆管。

其中冷弯内卷边槽钢、冷弯卷边Z形钢、冷弯空心方管、冷弯空心矩管、冷弯空心圆管不支持单位长度面积（国标表格中未提供数据）。

## 稍精确的
计算时，考虑型材厚度的影响。

## 粗略的
对于单位长度面积，不考虑型材厚度的影响。对于单位长度重量，与“稍精确的”一致。

**应当说明的是，对于变截面的型材，如楔形H型钢，理论上来说计算时必须取得长度数据才能准确计算，但本程序为简化计算，是以变截面两端的平均值计算的。**

# 加劲肋计算规则
厚度值是以型材腹板厚度暂定的，应根据实际需要手动修改输出结果。