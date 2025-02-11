using SectionSteel;
using Xunit.Abstractions;

namespace xUnitTest {
    public class UnitTest1 {
        private readonly ITestOutputHelper _output;
        public UnitTest1(ITestOutputHelper output) {
            _output = output;
        }
        private void ShowInfo(SectionSteelBase sectionSteel) {
            _output.WriteLine($"{"Profile = ",20}{sectionSteel.ProfileText}");
            _output.WriteLine($"{"Area Precisely = ",20}{sectionSteel.GetAreaFormula(FormulaAccuracyEnum.PRECISELY, true)}");
            _output.WriteLine($"{"Area GBData = ",20}{sectionSteel.GetAreaFormula(FormulaAccuracyEnum.GBDATA, true)}");
            _output.WriteLine($"{"Weight Precisely = ",20}{sectionSteel.GetWeightFormula(FormulaAccuracyEnum.PRECISELY)}");
            _output.WriteLine($"{"Weight GBData = ",20}{sectionSteel.GetWeightFormula(FormulaAccuracyEnum.GBDATA)}");
            _output.WriteLine($"{"Stiffener Profile = ",20}{sectionSteel.GetSiffenerProfileStr(true)}");
            _output.WriteLine("============================================================");
        }
        [Fact]
        public void Test_H() {
            var sectionSteel = new SectionSteel_H();
            ShowInfo(sectionSteel);
            try {
                sectionSteel.ProfileText = "HM244*175*7*11";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "WI300-700-10-12*250-14*300";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "H300~700*250/300*10*12/14";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "HW175*175*7.5*11";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "HW200*200";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "HT200*200";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "H200*200";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "HW200*250";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "J244*175*7*11";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "HM244*175";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "I_VAR_A600-300*300-300*12*14";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
        }
        [Fact]
        public void Test_HH() {
            var sectionSteel = new SectionSteel_HH();
            ShowInfo(sectionSteel);
            try {
                sectionSteel.ProfileText = "HH500*300*10*12";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "B_WLD_O300*12*10*350*14*12*500*550";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "HH244*175*7*11";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
        }
        [Fact]
        public void Test_T() {
            var sectionSteel = new SectionSteel_T();
            ShowInfo(sectionSteel);
            try {
                sectionSteel.ProfileText = "T150*200";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "T200~400*100*8*12";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "T300-8-12-100";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "TW300*300";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "TM300*300";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
        }
        [Fact]
        public void Test_I() {
            var sectionSteel = new SectionSteel_I();
            ShowInfo(sectionSteel);
            try {
                sectionSteel.ProfileText = "I200*100*8.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "I200*100*7.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
        }
        [Fact]
        public void Test_CHAN() {
            var sectionSteel = new SectionSteel_CHAN();
            ShowInfo(sectionSteel);
            try {
                sectionSteel.ProfileText = "[200*73*8.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "[200*73*7.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "C200*73*7.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
        }
        [Fact]
        public void Test_CHAN_MtM() {
            var sectionSteel = new SectionSteel_CHAN_MtM();
            ShowInfo(sectionSteel);
            try {
                sectionSteel.ProfileText = "[]200*73*8.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "[]200*73*7.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
        }
        [Fact]
        public void Test_CHAN_BtB() {
            var sectionSteel = new SectionSteel_CHAN_BtB();
            ShowInfo(sectionSteel);
            try {
                sectionSteel.ProfileText = "2[200*73*8.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "2[200*73*7.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "][20b";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
        }
        [Fact]
        public void Test_L() {
            var sectionSteel = new SectionSteel_L();
            ShowInfo(sectionSteel);
            try {
                sectionSteel.ProfileText = "L100*120*4";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "L4.5/2.8";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
        }
        [Fact]
        public void Test_L_BtB() {
            var sectionSteel = new SectionSteel_L_BtB();
            ShowInfo(sectionSteel);
            try {
                sectionSteel.ProfileText = "2L100*120*4";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "2L4.5/2.8";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
        }
        [Fact]
        public void Test_CFH_J() {
            var sectionSteel = new SectionSteel_CFH_J();
            ShowInfo(sectionSteel);
            try {
                sectionSteel.ProfileText = "J200*10";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "F200*10";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "F80*40*5";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "J80*40*5";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
        }
        [Fact]
        public void Test_RECT() {
            var sectionSteel = new SectionSteel_RECT();
            ShowInfo(sectionSteel);
            try {
                sectionSteel.ProfileText = "B_WLD_F300*200*10*14";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "R400~300*320*12*14";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "B_WLD_J400*300*320*12*14";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "B_VAR_C400-300*12*200-200";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "RHSC400*300*450*320*14*350";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "RECT400*500*16*14";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "RECT400*400*14*14";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
        }
        [Fact]
        public void Test_CFH_Y() {
            var sectionSteel = new SectionSteel_CFH_Y();
            ShowInfo(sectionSteel);
            try {
                sectionSteel.ProfileText = "Y200*10";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "Y219.1*8";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
        }
        [Fact]
        public void Test_CIRC() {
            var sectionSteel = new SectionSteel_CIRC();
            ShowInfo(sectionSteel);
            try {
                sectionSteel.ProfileText = "D200*10";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "CFCHS300*250*200*150*10";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "CFCHS300*250*300*250*10";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "CFCHS300*300*250*250*10";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
        }
        [Fact]
        public void Test_CFO_CN() {
            var sectionSteel = new SectionSteel_CFO_CN();
            ShowInfo(sectionSteel);
            try {
                sectionSteel.ProfileText = "CC200-5-30-100-40-150";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "C180*70*20*3.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
        }
        [Fact]
        public void Test_CFO_CN_MtM() {
            var sectionSteel = new SectionSteel_CFO_CN_MtM();
            ShowInfo(sectionSteel);
            try {
                sectionSteel.ProfileText = "2CCM200-5-30-100-40-150";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "2CM180*70*20*3.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
        }
        [Fact]
        public void Test_CFO_CN_BtB() {
            var sectionSteel = new SectionSteel_CFO_CN_BtB();
            ShowInfo(sectionSteel);
            try {
                sectionSteel.ProfileText = "2CC200-5-30-100-40-150";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "2C180*70*20*3.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
        }
        [Fact]
        public void Test_CFO_ZJ() {
            var sectionSteel = new SectionSteel_CFO_ZJ();
            ShowInfo(sectionSteel);
            try {
                sectionSteel.ProfileText = "ZZ200-5-30-100-40-150";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "Z180*70*20*3.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "XZ200*100*30*5.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
        }
        [Fact]
        public void Test_PL() {
            var sectionSteel = new SectionSteel_PL();
            ShowInfo(sectionSteel);
            try {
                sectionSteel.ProfileText = "PL377*16";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "PL16*319";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
        }
        [Fact]
        public void Test_PL_Composite() {
            var sectionSteel = new SectionSteel_PL_Composite();
            ShowInfo(sectionSteel);
            try {
                sectionSteel.ProfileText = "2PL14*400*500-1.5PLT14*100.5*115+3PLO14*250";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "PL14*400*500";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "PL18*575*560-PLT18*50*280-PL18*50~382*280";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "PL377*16";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "PL16*319";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
        }
        [Fact]
        public void Test_SectionSteel() {
            var sectionSteel = new SectionSteel.SectionSteel();
            ShowInfo(sectionSteel);
            try {
                sectionSteel.ProfileText = "C180*70*20*3.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "C200*73*7.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "C20a";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "2C20a";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "2C180*70*20*3.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "PL300*10";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "PL300*200*14";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "PLT300*200*14";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "2PL300*200*14-1.5PLT100*200*10";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "HM244*175*7*11";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "WI300-700-10-12*250-14*300";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "H300~700*250/300*10*12/14";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "HW175*175*7.5*11";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "HW200*200";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "HT200*200";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "H200*200";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "HW200*250";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "J244*175*7*11";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "HH500*300*10*12";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "B_WLD_O300*12*10*350*14*12*500*550";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "HH244*175*7*11";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "T150*200";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "T200~400*100*8*12";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "T300-8-12-100";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "TW300*300";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "I200*100*8.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "I200*100*7.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "[200*73*8.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "[200*73*7.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "C200*73*7.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "[]200*73*8.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "[]200*73*7.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "2[200*73*8.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "2[200*73*7.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "][20b";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "L100*120*4";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "L4.5/2.8";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "2L100*120*4";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "2L4.5/2.8";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "J200*10";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "F200*10";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "F80*40*5";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "J80*40*5";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "B_WLD_F300*200*10*14";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "R400~300*320*12*14";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "B_WLD_J400*300*320*12*14";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "B_VAR_C400-300*12*200-200";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "RHSC400*300*450*320*14*350";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "RECT400*500*16*14";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "RECT400*400*14*14";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "Y200*10";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "Y219.1*8";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "D200*10";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "CFCHS300*250*200*150*10";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "CFCHS300*250*300*250*10";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "CFCHS300*300*250*250*10";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "CC200-5-30-100-40-150";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "C180*70*20*3.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "2CCM200-5-30-100-40-150";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "2CM180*70*20*3.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "2CC200-5-30-100-40-150";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "2C180*70*20*3.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "ZZ200-5-30-100-40-150";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "Z180*70*20*3.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "XZ200*100*30*5.0";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "2PL14*400*500-1.5PLT14*100.5*115+3PLO14*250";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "PL14*400*500";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "PL377*16";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "PL16*319";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "PL18*575*560-PLT18*50*280-PL18*50~382*280";
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
            try {
                sectionSteel.ProfileText = "SPHERE1000";
                sectionSteel.PIStyle = PIStyleEnum.NUM;
                ShowInfo(sectionSteel);
            } catch (MismatchedProfileTextException e) {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
