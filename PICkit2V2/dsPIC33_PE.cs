using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Pk2 = PICkit2V2.PICkitFunctions;
using KONST = PICkit2V2.Constants;

namespace PICkit2V2
{
    public class dsPIC33_PE
    {
        public static DelegateStatusWin UpdateStatusWinText;
        public static DelegateResetStatusBar ResetStatusBar;
        public static DelegateStepStatusBar StepStatusBar;    
        
        private static byte ICSPSpeedRestore = 0;                                                
                                                        
        // Program Executive version 0x0026
        private const int dsPIC33_PE_Version = 0x0026;
        private const int dsPIC33_PE_ID = 0x00CB;
        private static uint[] dsPIC33_PE_Code = new uint[1024]{
            0x00040080, 0x00000080, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 
            0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 
            0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 
            0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 
            0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 
            0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 
            0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 
            0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 
            0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 
            0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 
            0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 
            0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 
            0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 
            0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 
            0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 
            0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 0x00800120, 
            0x00FA0000, 0x0020ADCF, 0x00227F80, 0x00880100, 0x00000000, 0x00070002, 0x00FA8000, 0x00060000, 
            0x00FA0000, 0x00EB0000, 0x00883A20, 0x00070260, 0x00070226, 0x000701AE, 0x0007000B, 0x0037FFFD, 
            0x00F80034, 0x00781F88, 0x00B3C008, 0x008801A8, 0x0078044F, 0x00FA0000, 0x00FE0000, 0x00FA8000, 
            0x00F90034, 0x00064000, 0x00FA0004, 0x00805060, 0x00DE004C, 0x00FB8000, 0x00B90161, 0x00BE8F02, 
            0x002000E0, 0x00200001, 0x00BE011E, 0x00510F80, 0x00598F81, 0x003E002F, 0x00BE001E, 0x00016000, 
            0x0037000E, 0x0037000D, 0x0037000C, 0x00370029, 0x00370010, 0x00370013, 0x00370026, 0x00370013, 
            0x00370024, 0x00370013, 0x00370014, 0x00370017, 0x00370018, 0x00370009, 0x00370012, 0x00A88AD3, 
            0x00A9AAD3, 0x00A9CAD3, 0x00A9EAD3, 0x00070020, 0x0037001D, 0x000700AE, 0x0037001B, 0x000700CE, 
            0x00370019, 0x00070107, 0x00370017, 0x00070062, 0x00370015, 0x00070077, 0x00370013, 0x00070144, 
            0x00370011, 0x00070145, 0x0037000F, 0x00070167, 0x0037000D, 0x000701F3, 0x008856C0, 0x00A88AD3, 
            0x00A9AAD3, 0x00A9CAD3, 0x00A9EAD3, 0x00070008, 0x00370005, 0x00A88AD3, 0x00A8AAD3, 0x00A9CAD3, 
            0x00A9EAD3, 0x00070002, 0x00FA8000, 0x00060000, 0x00FA0000, 0x00805691, 0x002F0000, 0x00608080, 
            0x00210000, 0x00508F80, 0x00320006, 0x00805691, 0x002F0000, 0x00608080, 0x00230000, 0x00508F80, 
            0x003A0003, 0x00EB4000, 0x00B7EAD2, 0x00370002, 0x00B3C010, 0x00B7EAD2, 0x00805060, 0x00DE004C, 
            0x00FB8000, 0x0060006F, 0x00DD0148, 0x00805691, 0x002F0FF0, 0x00608000, 0x00700002, 0x00885690, 
            0x00805691, 0x0020F000, 0x00608080, 0x00201000, 0x00508F80, 0x003A0005, 0x00BFCA0F, 0x00FB8000, 
            0x00E88000, 0x008856A0, 0x00370020, 0x00805691, 0x0020F000, 0x00608080, 0x00202000, 0x00508F80, 
            0x003A000D, 0x00805070, 0x00D10000, 0x00B90063, 0x00E88000, 0x008856A0, 0x00805070, 0x00600061, 
            0x00E00400, 0x00320011, 0x008056A0, 0x00E88000, 0x008856A0, 0x0037000D, 0x00805691, 0x0020F000, 
            0x00608080, 0x0020C000, 0x00508F80, 0x003A0005, 0x00200030, 0x008856A0, 0x008056C0, 0x008856B0, 
            0x00370002, 0x00200020, 0x008856A0, 0x0007012A, 0x00FA8000, 0x00060000, 0x00FA0000, 0x002404F0, 
            0x00883B00, 0x00A8C761, 0x00200557, 0x00883B37, 0x00200AA7, 0x00883B37, 0x00A8E761, 0x00000000, 
            0x00000000, 0x00BFC761, 0x00B3C801, 0x00604001, 0x00E00400, 0x003AFFFB, 0x00A88AD3, 0x00A9AAD3, 
            0x00A9CAD3, 0x00A9EAD3, 0x0007FFA1, 0x00FA8000, 0x00060000, 0x00FA0006, 0x00240420, 0x00883B00, 
            0x00BFCA0E, 0x00FB8000, 0x00200001, 0x00DD01C0, 0x00200002, 0x00805080, 0x00200001, 0x00411F00, 
            0x00499701, 0x00EB0000, 0x00980720, 0x00370014, 0x00A8C761, 0x00200557, 0x00883B37, 0x00200AA7, 
            0x00883B37, 0x00A8E761, 0x00000000, 0x00000000, 0x00BFC761, 0x00B3C801, 0x00604001, 0x00E00400, 
            0x003AFFFB, 0x00200400, 0x00200001, 0x00400F1E, 0x0048975E, 0x0090002E, 0x00E80000, 0x00980720, 
            0x00BFCA0F, 0x00FB8080, 0x0090002E, 0x00508F80, 0x003EFFE7, 0x00A88AD3, 0x00A9AAD3, 0x00A9CAD3, 
            0x00A9EAD3, 0x0007FF72, 0x00FA8000, 0x00060000, 0x00FA0004, 0x00BFCA0E, 0x00FB8000, 0x00880190, 
            0x00240000, 0x00883B00, 0x00805090, 0x00980710, 0x00805080, 0x00780F00, 0x0090009E, 0x0078001E, 
            0x00BB0801, 0x00A8C761, 0x00200550, 0x00883B30, 0x00200AA0, 0x00883B30, 0x00A8E761, 0x00000000, 
            0x00000000, 0x00980710, 0x00BFC761, 0x00B3C801, 0x00604001, 0x00E00400, 0x003AFFFB, 0x00A88AD3, 
            0x00A9AAD3, 0x00A9CAD3, 0x00A9EAD3, 0x0007FF50, 0x00FA8000, 0x00060000, 0x00FA0004, 0x00BFCA0E, 
            0x00FB8000, 0x00880190, 0x00240030, 0x00883B00, 0x00805090, 0x00980710, 0x00805080, 0x00780F00, 
            0x0090009E, 0x0078001E, 0x00BB0801, 0x00BFCA0F, 0x00FB8000, 0x00980710, 0x0090009E, 0x0078001E, 
            0x00BBC801, 0x00A8C761, 0x00200550, 0x00883B30, 0x00200AA0, 0x00883B30, 0x00A8E761, 0x00000000, 
            0x00000000, 0x00980710, 0x00BFC761, 0x00B3C801, 0x00604001, 0x00E00400, 0x003AFFFB, 0x0078001E, 
            0x00BA0010, 0x00980710, 0x00A98AD3, 0x00A8AAD3, 0x00A9CAD3, 0x00A9EAD3, 0x00805091, 0x0090001E, 
            0x00508F80, 0x003A000C, 0x0078001E, 0x00BAC010, 0x00980710, 0x0090001E, 0x00784080, 0x00BFCA0F, 
            0x0050CF80, 0x003A0004, 0x00A88AD3, 0x00A9AAD3, 0x00A9CAD3, 0x00A9EAD3, 0x0007FF15, 0x00FA8000, 
            0x00060000, 0x00240010, 0x00883B00, 0x0020A0C5, 0x00EB0000, 0x00904425, 0x009004A5, 0x00428366, 
            0x00880198, 0x00780389, 0x00200100, 0x00BB0BB6, 0x00BBDBB6, 0x00BBEBB6, 0x00BB1BB6, 0x00BB0BB6, 
            0x00BBDBB6, 0x00BBEBB6, 0x00BB1BB6, 0x00E90000, 0x003AFFF6, 0x00090005, 0x00780B46, 0x00200080, 
            0x00538380, 0x00BB0BB6, 0x00A8C761, 0x00200557, 0x00883B37, 0x00200AA7, 0x00883B37, 0x00A8E761, 
            0x00000000, 0x00000000, 0x00803B00, 0x00A3F000, 0x0031FFFD, 0x00070005, 0x00DD004C, 0x0020AD21, 
            0x00780880, 0x0007FEEA, 0x00060000, 0x0020A0C0, 0x00EB0080, 0x00904420, 0x009004A0, 0x002000C7, 
            0x004001E6, 0x00880198, 0x00780289, 0x002001F4, 0x00BA0315, 0x00E13033, 0x003A000B, 0x00BADBB5, 
            0x00BAD3D5, 0x00E13033, 0x003A0007, 0x00BA0335, 0x00E13033, 0x003A0004, 0x00E90204, 0x003BFFF4, 
            0x00200010, 0x00370001, 0x00200020, 0x00060000, 0x00EF2032, 0x002FFFE7, 0x00370005, 0x0020A0C0, 
            0x009001B0, 0x00880193, 0x009001C0, 0x005183E2, 0x00EB8200, 0x002FFFE6, 0x0020A0C0, 0x00900190, 
            0x00900120, 0x00B10012, 0x00B18003, 0x0032000E, 0x00BA0057, 0x00E78004, 0x00370009, 0x00BAC017, 
            0x00E78404, 0x00370006, 0x00E13007, 0x003AFFF5, 0x00800190, 0x00E80000, 0x00880190, 0x0037FFF1, 
            0x002000F1, 0x00370001, 0x00200F01, 0x0021A000, 0x00700001, 0x0020AD23, 0x00781980, 0x00200020, 
            0x00780980, 0x0007001C, 0x00060000, 0x00200261, 0x0021B000, 0x00700001, 0x0020AD21, 0x00781880, 
            0x00200020, 0x00780880, 0x00070013, 0x00060000, 0x00070067, 0x0020A0C7, 0x00781B80, 0x0020FFF3, 
            0x00600183, 0x00E90183, 0x00320004, 0x00070060, 0x00781B80, 0x00E90183, 0x003AFFFC, 0x00A9E241, 
            0x00205000, 0x00881210, 0x00EFA248, 0x00A8E241, 0x00EFA248, 0x00060000, 0x00EF2240, 0x0009001D, 
            0x00000000, 0x00204000, 0x00881210, 0x00A94085, 0x00A8E241, 0x0020AD27, 0x00780037, 0x00881240, 
            0x00780037, 0x0007004F, 0x0020AD20, 0x00900290, 0x00E98285, 0x00320038, 0x00904010, 0x00B240F0, 
            0x00B3C011, 0x00E10401, 0x003A000A, 0x0020A0C0, 0x009040A0, 0x00FB8081, 0x00880191, 0x009003A0, 
            0x00BA0037, 0x0007003F, 0x00E90285, 0x003AFFFC, 0x00370029, 0x00B3C021, 0x00E10401, 0x003A002E, 
            0x0020A0C0, 0x00900290, 0x00D10305, 0x00B00040, 0x00784090, 0x00780401, 0x007800D0, 0x00780381, 
            0x00E90005, 0x00320015, 0x00200001, 0x00880198, 0x00000000, 0x00BA0897, 0x0007002A, 0x00BAD897, 
            0x00B00027, 0x00B08008, 0x00880198, 0x00000000, 0x00BAD097, 0x00070023, 0x00BA0017, 0x00070021, 
            0x00B00027, 0x00B08008, 0x00880198, 0x00E90306, 0x003AFFEE, 0x00A60005, 0x00370007, 0x00880198, 
            0x00000000, 0x00BA0017, 0x00070016, 0x00BAC017, 0x00FB8000, 0x00070013, 0x00AE4085, 0x0037FFFE, 
            0x00A94085, 0x00A9E241, 0x00A86243, 0x00A8E241, 0x00801241, 0x00060000, 0x00B3C0C1, 0x00E10401, 
            0x003AFFF5, 0x00780037, 0x00070006, 0x0037FFF2, 0x00AE4085, 0x0037FFFE, 0x00A94085, 0x00801240, 
            0x00060000, 0x00781F81, 0x00AE4085, 0x0037FFFE, 0x00AE4085, 0x0037FFFC, 0x00A94085, 0x00801241, 
            0x00881240, 0x007800CF, 0x00060000, 0x00800210, 0x00B30A00, 0x00880210, 0x0020C000, 0x00881210, 
            0x00A94085, 0x00EF20A8, 0x00A800A9, 0x00A84095, 0x00280000, 0x00881200, 0x00EF2762, 0x00EF2764, 
            0x00060000, 0x00FA000C, 0x00B80060, 0x00980720, 0x00980731, 0x00B80060, 0x00980740, 0x00980751, 
            0x00BFCA0F, 0x00784F00, 0x00470064, 0x00E88080, 0x00BFCA0E, 0x00784880, 0x00805080, 0x00B80161, 
            0x0090002E, 0x009000BE, 0x00710000, 0x00718081, 0x00980720, 0x00980731, 0x00470068, 0x00E88080, 
            0x00805090, 0x00780880, 0x008050A0, 0x00B80161, 0x0090004E, 0x009000DE, 0x00710000, 0x00718081, 
            0x00980740, 0x00980751, 0x0090014E, 0x009001DE, 0x0090002E, 0x009000BE, 0x0078421E, 0x0007002D, 
            0x00980710, 0x0090001E, 0x00FA8000, 0x00060000, 0x00FA0006, 0x00EB0000, 0x00980710, 0x0037001F, 
            0x0090001E, 0x00DD0048, 0x00980720, 0x00B3C080, 0x00784F00, 0x0037000E, 0x0090002E, 0x00E00000, 
            0x003D0007, 0x0090002E, 0x00400000, 0x00780080, 0x00210210, 0x00688000, 0x00980720, 0x00370003, 
            0x0090002E, 0x00400000, 0x00980720, 0x00E94F1E, 0x00E0041E, 0x003AFFF0, 0x0090001E, 0x00400080, 
            0x002080C0, 0x00408000, 0x009000AE, 0x00780801, 0x0090001E, 0x00E80000, 0x00980710, 0x0090009E, 
            0x00200FF0, 0x00508F80, 0x0034FFDD, 0x00FA8000, 0x00060000, 0x00FA001A, 0x00980760, 0x00980771, 
            0x00980F02, 0x00980F13, 0x00985744, 0x00EB8000, 0x00980740, 0x0090016E, 0x009001FE, 0x00200000, 
            0x00200011, 0x00400002, 0x00488083, 0x00200002, 0x002FFFF3, 0x00780200, 0x00780281, 0x00780002, 
            0x00780083, 0x00620100, 0x00628001, 0x00B80261, 0x00980F34, 0x00980F45, 0x0090083E, 0x009008CE, 
            0x00DD00C0, 0x00200000, 0x00980F30, 0x00980F41, 0x0090083E, 0x009008CE, 0x00B81261, 0x00980F34, 
            0x00980F45, 0x0090093E, 0x009009CE, 0x00700002, 0x00708083, 0x00980710, 0x00980721, 0x0037003C, 
            0x0090080E, 0x0090089E, 0x00400100, 0x00488181, 0x0090006E, 0x009000FE, 0x00400102, 0x00488183, 
            0x0090001E, 0x009000AE, 0x00510F80, 0x00598F81, 0x0039001A, 0x0090011E, 0x009001AE, 0x0090006E, 
            0x009000FE, 0x00510000, 0x00598081, 0x00D10081, 0x00D38000, 0x00980730, 0x0090011E, 0x009001AE, 
            0x00200000, 0x00200011, 0x00400002, 0x00488083, 0x00980710, 0x00980721, 0x0090003E, 0x00B80161, 
            0x0090080E, 0x0090089E, 0x00500002, 0x00588083, 0x00980F00, 0x00980F11, 0x00370005, 0x0090098E, 
            0x00980733, 0x00B80060, 0x00980F00, 0x00980F11, 0x009001CE, 0x0090013E, 0x0090006E, 0x009000FE, 
            0x00070032, 0x00980740, 0x0090003E, 0x00200001, 0x00400100, 0x00488181, 0x0090006E, 0x009000FE, 
            0x00410000, 0x00498081, 0x00980760, 0x00980771, 0x0090080E, 0x0090089E, 0x00500FE0, 0x00588FE0, 
            0x003AFFBF, 0x0090004E, 0x00200001, 0x00FA8000, 0x00060000, 0x00200326, 0x00780B01, 0x00780380, 
            0x002080A6, 0x00780B02, 0x00D5280A, 0x002080C8, 0x00208006, 0x00BA1B17, 0x00BADB37, 0x00BADB57, 
            0x00BA1B37, 0x00208004, 0x00200065, 0x00FD8003, 0x00784003, 0x007840B4, 0x0068C000, 0x00EF6001, 
            0x00D20000, 0x00400008, 0x00780010, 0x00EB4180, 0x00698180, 0x00ED200A, 0x003AFFF4, 0x00ED280A, 
            0x003AFFEB, 0x00780003, 0x00060000, 0x00200326, 0x00780B01, 0x00780380, 0x002080A6, 0x00780B02, 
            0x00D5280A, 0x002080C8, 0x00200062, 0x00208006, 0x00BA1B17, 0x00BADB37, 0x00BADB57, 0x00BA1B37, 
            0x00208004, 0x00200035, 0x00EB0080, 0x00FD8003, 0x00784003, 0x006848B4, 0x00EF6001, 0x00D20000, 
            0x00400008, 0x00EB4180, 0x00698910, 0x00FD8003, 0x00784003, 0x006848B4, 0x00EF6001, 0x00D20000, 
            0x00400008, 0x00EB4180, 0x00698910, 0x00ED200A, 0x003AFFEE, 0x00ED280A, 0x003AFFE4, 0x00780003, 
            0x00060000, 0x00000ADA, 0x00000002, 0x00000000, 0x00000A0C, 0x000000CE, 0x00000000, 0x0000080C, 
            0x00000200, 0x00000000, 0x00000800, 0x0000000C, 0x00000000, 0x00000000, 0x00FFFFFF, 0x00FFFFFF, 
            0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 
            0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 
            0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 
            0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 
            0x000000CB, 0x00000026, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF};
            
        private static byte[] BitReverseTable = new byte[256]
        {
          0x00, 0x80, 0x40, 0xC0, 0x20, 0xA0, 0x60, 0xE0, 0x10, 0x90, 0x50, 0xD0, 0x30, 0xB0, 0x70, 0xF0, 
          0x08, 0x88, 0x48, 0xC8, 0x28, 0xA8, 0x68, 0xE8, 0x18, 0x98, 0x58, 0xD8, 0x38, 0xB8, 0x78, 0xF8, 
          0x04, 0x84, 0x44, 0xC4, 0x24, 0xA4, 0x64, 0xE4, 0x14, 0x94, 0x54, 0xD4, 0x34, 0xB4, 0x74, 0xF4, 
          0x0C, 0x8C, 0x4C, 0xCC, 0x2C, 0xAC, 0x6C, 0xEC, 0x1C, 0x9C, 0x5C, 0xDC, 0x3C, 0xBC, 0x7C, 0xFC, 
          0x02, 0x82, 0x42, 0xC2, 0x22, 0xA2, 0x62, 0xE2, 0x12, 0x92, 0x52, 0xD2, 0x32, 0xB2, 0x72, 0xF2, 
          0x0A, 0x8A, 0x4A, 0xCA, 0x2A, 0xAA, 0x6A, 0xEA, 0x1A, 0x9A, 0x5A, 0xDA, 0x3A, 0xBA, 0x7A, 0xFA,
          0x06, 0x86, 0x46, 0xC6, 0x26, 0xA6, 0x66, 0xE6, 0x16, 0x96, 0x56, 0xD6, 0x36, 0xB6, 0x76, 0xF6, 
          0x0E, 0x8E, 0x4E, 0xCE, 0x2E, 0xAE, 0x6E, 0xEE, 0x1E, 0x9E, 0x5E, 0xDE, 0x3E, 0xBE, 0x7E, 0xFE,
          0x01, 0x81, 0x41, 0xC1, 0x21, 0xA1, 0x61, 0xE1, 0x11, 0x91, 0x51, 0xD1, 0x31, 0xB1, 0x71, 0xF1,
          0x09, 0x89, 0x49, 0xC9, 0x29, 0xA9, 0x69, 0xE9, 0x19, 0x99, 0x59, 0xD9, 0x39, 0xB9, 0x79, 0xF9, 
          0x05, 0x85, 0x45, 0xC5, 0x25, 0xA5, 0x65, 0xE5, 0x15, 0x95, 0x55, 0xD5, 0x35, 0xB5, 0x75, 0xF5,
          0x0D, 0x8D, 0x4D, 0xCD, 0x2D, 0xAD, 0x6D, 0xED, 0x1D, 0x9D, 0x5D, 0xDD, 0x3D, 0xBD, 0x7D, 0xFD,
          0x03, 0x83, 0x43, 0xC3, 0x23, 0xA3, 0x63, 0xE3, 0x13, 0x93, 0x53, 0xD3, 0x33, 0xB3, 0x73, 0xF3, 
          0x0B, 0x8B, 0x4B, 0xCB, 0x2B, 0xAB, 0x6B, 0xEB, 0x1B, 0x9B, 0x5B, 0xDB, 0x3B, 0xBB, 0x7B, 0xFB,
          0x07, 0x87, 0x47, 0xC7, 0x27, 0xA7, 0x67, 0xE7, 0x17, 0x97, 0x57, 0xD7, 0x37, 0xB7, 0x77, 0xF7, 
          0x0F, 0x8F, 0x4F, 0xCF, 0x2F, 0xAF, 0x6F, 0xEF, 0x1F, 0x9F, 0x5F, 0xDF, 0x3F, 0xBF, 0x7F, 0xFF
        };

        private static ushort[] CRC_LUT_Array = new ushort[256]
                                           {0x0000,
                                            0x1021,
                                            0x2042,
                                            0x3063,
                                            0x4084,
                                            0x50a5,
                                            0x60c6,
                                            0x70e7,
                                            0x8108,
                                            0x9129,
                                            0xa14a,
                                            0xb16b,
                                            0xc18c,
                                            0xd1ad,
                                            0xe1ce,
                                            0xf1ef,
                                            0x1231,
                                            0x0210,
                                            0x3273,
                                            0x2252,
                                            0x52b5,
                                            0x4294,
                                            0x72f7,
                                            0x62d6,
                                            0x9339,
                                            0x8318,
                                            0xb37b,
                                            0xa35a,
                                            0xd3bd,
                                            0xc39c,
                                            0xf3ff,
                                            0xe3de,
                                            0x2462,
                                            0x3443,
                                            0x0420,
                                            0x1401,
                                            0x64e6,
                                            0x74c7,
                                            0x44a4,
                                            0x5485,
                                            0xa56a,
                                            0xb54b,
                                            0x8528,
                                            0x9509,
                                            0xe5ee,
                                            0xf5cf,
                                            0xc5ac,
                                            0xd58d,
                                            0x3653,
                                            0x2672,
                                            0x1611,
                                            0x0630,
                                            0x76d7,
                                            0x66f6,
                                            0x5695,
                                            0x46b4,
                                            0xb75b,
                                            0xa77a,
                                            0x9719,
                                            0x8738,
                                            0xf7df,
                                            0xe7fe,
                                            0xd79d,
                                            0xc7bc,
                                            0x48c4,
                                            0x58e5,
                                            0x6886,
                                            0x78a7,
                                            0x0840,
                                            0x1861,
                                            0x2802,
                                            0x3823,
                                            0xc9cc,
                                            0xd9ed,
                                            0xe98e,
                                            0xf9af,
                                            0x8948,
                                            0x9969,
                                            0xa90a,
                                            0xb92b,
                                            0x5af5,
                                            0x4ad4,
                                            0x7ab7,
                                            0x6a96,
                                            0x1a71,
                                            0x0a50,
                                            0x3a33,
                                            0x2a12,
                                            0xdbfd,
                                            0xcbdc,
                                            0xfbbf,
                                            0xeb9e,
                                            0x9b79,
                                            0x8b58,
                                            0xbb3b,
                                            0xab1a,
                                            0x6ca6,
                                            0x7c87,
                                            0x4ce4,
                                            0x5cc5,
                                            0x2c22,
                                            0x3c03,
                                            0x0c60,
                                            0x1c41,
                                            0xedae,
                                            0xfd8f,
                                            0xcdec,
                                            0xddcd,
                                            0xad2a,
                                            0xbd0b,
                                            0x8d68,
                                            0x9d49,
                                            0x7e97,
                                            0x6eb6,
                                            0x5ed5,
                                            0x4ef4,
                                            0x3e13,
                                            0x2e32,
                                            0x1e51,
                                            0x0e70,
                                            0xff9f,
                                            0xefbe,
                                            0xdfdd,
                                            0xcffc,
                                            0xbf1b,
                                            0xaf3a,
                                            0x9f59,
                                            0x8f78,
                                            0x9188,
                                            0x81a9,
                                            0xb1ca,
                                            0xa1eb,
                                            0xd10c,
                                            0xc12d,
                                            0xf14e,
                                            0xe16f,
                                            0x1080,
                                            0x00a1,
                                            0x30c2,
                                            0x20e3,
                                            0x5004,
                                            0x4025,
                                            0x7046,
                                            0x6067,
                                            0x83b9,
                                            0x9398,
                                            0xa3fb,
                                            0xb3da,
                                            0xc33d,
                                            0xd31c,
                                            0xe37f,
                                            0xf35e,
                                            0x02b1,
                                            0x1290,
                                            0x22f3,
                                            0x32d2,
                                            0x4235,
                                            0x5214,
                                            0x6277,
                                            0x7256,
                                            0xb5ea,
                                            0xa5cb,
                                            0x95a8,
                                            0x8589,
                                            0xf56e,
                                            0xe54f,
                                            0xd52c,
                                            0xc50d,
                                            0x34e2,
                                            0x24c3,
                                            0x14a0,
                                            0x0481,
                                            0x7466,
                                            0x6447,
                                            0x5424,
                                            0x4405,
                                            0xa7db,
                                            0xb7fa,
                                            0x8799,
                                            0x97b8,
                                            0xe75f,
                                            0xf77e,
                                            0xc71d,
                                            0xd73c,
                                            0x26d3,
                                            0x36f2,
                                            0x0691,
                                            0x16b0,
                                            0x6657,
                                            0x7676,
                                            0x4615,
                                            0x5634,
                                            0xd94c,
                                            0xc96d,
                                            0xf90e,
                                            0xe92f,
                                            0x99c8,
                                            0x89e9,
                                            0xb98a,
                                            0xa9ab,
                                            0x5844,
                                            0x4865,
                                            0x7806,
                                            0x6827,
                                            0x18c0,
                                            0x08e1,
                                            0x3882,
                                            0x28a3,
                                            0xcb7d,
                                            0xdb5c,
                                            0xeb3f,
                                            0xfb1e,
                                            0x8bf9,
                                            0x9bd8,
                                            0xabbb,
                                            0xbb9a,
                                            0x4a75,
                                            0x5a54,
                                            0x6a37,
                                            0x7a16,
                                            0x0af1,
                                            0x1ad0,
                                            0x2ab3,
                                            0x3a92,
                                            0xfd2e,
                                            0xed0f,
                                            0xdd6c,
                                            0xcd4d,
                                            0xbdaa,
                                            0xad8b,
                                            0x9de8,
                                            0x8dc9,
                                            0x7c26,
                                            0x6c07,
                                            0x5c64,
                                            0x4c45,
                                            0x3ca2,
                                            0x2c83,
                                            0x1ce0,
                                            0x0cc1,
                                            0xef1f,
                                            0xff3e,
                                            0xcf5d,
                                            0xdf7c,
                                            0xaf9b,
                                            0xbfba,
                                            0x8fd9,
                                            0x9ff8,
                                            0x6e17,
                                            0x7e36,
                                            0x4e55,
                                            0x5e74,
                                            0x2e93,
                                            0x3eb2,
                                            0x0ed1,
                                            0x1ef0 };

        public static bool DownloadPE()
        {
        
            Pk2.RunScript(KONST.PROG_ENTRY, 1);
            // Erase Executive Memory
            Pk2.ExecuteScript(Pk2.DevFile.PartsList[Pk2.ActivePart].DebugWriteVectorScript);
            
            // Set address
            if (Pk2.DevFile.PartsList[Pk2.ActivePart].ProgMemWrPrepScript != 0)
            { // if prog mem address set script exists for this part
                Pk2.DownloadAddress3(0x800000); // start of exec memory
                Pk2.RunScript(KONST.PROGMEM_WR_PREP, 1);
            }
            
            int instruction = 0;
            uint workaround = 0;
            int commOffSet = 0;
            byte[] commandArrayp = new byte[64];
            
            // Program the exec in 16 rows
            for (int row = 0; row < 16; row++)
            {
                // Download a 64-instruction row 
                for (int section = 0; section < 4; section++)
                {
                    commOffSet = 0;
                    if (section == 0)
                    {
                        commandArrayp[commOffSet++] = KONST.CLR_DOWNLOAD_BUFFER;
                    }
                    commandArrayp[commOffSet++] = KONST.DOWNLOAD_DATA;
                    commandArrayp[commOffSet++] = 48; // 16 instructions.
                    for (int word = 0; word < 16; word++)
                    {
                        commandArrayp[commOffSet++] = (byte)(dsPIC33_PE_Code[instruction] & 0xFF);
                        commandArrayp[commOffSet++] = (byte)((dsPIC33_PE_Code[instruction] >> 8) & 0xFF);
                        commandArrayp[commOffSet++] = (byte)((dsPIC33_PE_Code[instruction] >> 16) & 0xFF); 
                        instruction++;
                    }
                    workaround = dsPIC33_PE_Code[instruction - 4];
                    for (; commOffSet < 64; commOffSet++)
                    {
                        commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
                    }
                    Pk2.writeUSB(commandArrayp);
                }
                // Program the row
                commOffSet = 0;
                commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
                commandArrayp[commOffSet++] = 0; // fill in later
                commandArrayp[commOffSet++] = KONST._WRITE_BUFWORD_W;
                commandArrayp[commOffSet++] = 0;
                commandArrayp[commOffSet++] = KONST._WRITE_BUFBYTE_W;
                commandArrayp[commOffSet++] = 1;
                commandArrayp[commOffSet++] = KONST._WRITE_BUFWORD_W;
                commandArrayp[commOffSet++] = 3;
                commandArrayp[commOffSet++] = KONST._WRITE_BUFBYTE_W;
                commandArrayp[commOffSet++] = 2;
                commandArrayp[commOffSet++] = KONST._COREINST24; // TBLWTL W0, [W7]
                commandArrayp[commOffSet++] = 0x80;
                commandArrayp[commOffSet++] = 0x0B;
                commandArrayp[commOffSet++] = 0xBB;
                commandArrayp[commOffSet++] = KONST._NOP24;
                commandArrayp[commOffSet++] = KONST._NOP24;
                commandArrayp[commOffSet++] = KONST._COREINST24; // TBLWTH W1, [W7++]
                commandArrayp[commOffSet++] = 0x81;
                commandArrayp[commOffSet++] = 0x9B;
                commandArrayp[commOffSet++] = 0xBB;
                commandArrayp[commOffSet++] = KONST._NOP24;
                commandArrayp[commOffSet++] = KONST._NOP24;
                commandArrayp[commOffSet++] = KONST._COREINST24; // TBLWTH W2, [W7]
                commandArrayp[commOffSet++] = 0x82;
                commandArrayp[commOffSet++] = 0x8B;
                commandArrayp[commOffSet++] = 0xBB;
                commandArrayp[commOffSet++] = KONST._NOP24;
                commandArrayp[commOffSet++] = KONST._NOP24;
                commandArrayp[commOffSet++] = KONST._COREINST24; // TBLWTL W3, [W7++]
                commandArrayp[commOffSet++] = 0x83;
                commandArrayp[commOffSet++] = 0x1B;
                commandArrayp[commOffSet++] = 0xBB;
                commandArrayp[commOffSet++] = KONST._NOP24;
                commandArrayp[commOffSet++] = KONST._NOP24;
                commandArrayp[commOffSet++] = KONST._LOOP;
                commandArrayp[commOffSet++] = 32;
                commandArrayp[commOffSet++] = 31;
                // Workaround for Device ID errata
                commandArrayp[commOffSet++] = KONST._COREINST24; // MOV.W #8, W8
                commandArrayp[commOffSet++] = 0x88;
                commandArrayp[commOffSet++] = 0x00;
                commandArrayp[commOffSet++] = 0x20;
                commandArrayp[commOffSet++] = KONST._COREINST24; // SUBR.W W8, W7, W8
                commandArrayp[commOffSet++] = 0x07;
                commandArrayp[commOffSet++] = 0x04;
                commandArrayp[commOffSet++] = 0x14;
                commandArrayp[1] = (byte)(commOffSet-2);  // script length
                for (; commOffSet < 64; commOffSet++)
                {
                    commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
                }
                Pk2.writeUSB(commandArrayp);
                commOffSet = 0;
                commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
                commandArrayp[commOffSet++] = 0; // fill in later
                commandArrayp[commOffSet++] = KONST._COREINST24; // MOV.W #(workaround & 0xFFFF), W0
                commandArrayp[commOffSet++] = (byte)(workaround << 4);
                commandArrayp[commOffSet++] = (byte)(workaround >> 4);
                commandArrayp[commOffSet++] = (byte)(0x20 | (0x0F & (workaround >> 12)));
                commandArrayp[commOffSet++] = KONST._COREINST24; // MOV.W #(workaround >> 16), W1
                workaround >>= 16;
                commandArrayp[commOffSet++] = (byte)(0x01 | (workaround << 4));
                commandArrayp[commOffSet++] = (byte)(workaround >> 4);
                commandArrayp[commOffSet++] = 0x20;
                commandArrayp[commOffSet++] = KONST._COREINST24; // TBLWTL.W W0, [W8]
                commandArrayp[commOffSet++] = 0x00;
                commandArrayp[commOffSet++] = 0x0C;
                commandArrayp[commOffSet++] = 0xBB;
                commandArrayp[commOffSet++] = KONST._NOP24;
                commandArrayp[commOffSet++] = KONST._NOP24;
                commandArrayp[commOffSet++] = KONST._COREINST24; // TBLWTH.B W1, [W8]
                commandArrayp[commOffSet++] = 0x01;
                commandArrayp[commOffSet++] = 0x8C;
                commandArrayp[commOffSet++] = 0xBB;
                commandArrayp[commOffSet++] = KONST._NOP24;
                commandArrayp[commOffSet++] = KONST._NOP24;
                // END Device ID errata workaround
                commandArrayp[commOffSet++] = KONST._COREINST24; // BSET.B 0x0761, #7
                commandArrayp[commOffSet++] = 0x61;
                commandArrayp[commOffSet++] = 0xE7;
                commandArrayp[commOffSet++] = 0xA8;
                commandArrayp[commOffSet++] = KONST._NOP24;
                commandArrayp[commOffSet++] = KONST._COREINST24; // GOTO 0x200
                commandArrayp[commOffSet++] = 0x00;
                commandArrayp[commOffSet++] = 0x02;
                commandArrayp[commOffSet++] = 0x04;
                commandArrayp[commOffSet++] = KONST._NOP24;
                commandArrayp[commOffSet++] = KONST._LOOP;
                commandArrayp[commOffSet++] = 1;
                commandArrayp[commOffSet++] = 3;
                commandArrayp[commOffSet++] = KONST._DELAY_SHORT;
                commandArrayp[commOffSet++] = 72;
                commandArrayp[1] = (byte)(commOffSet - 2);  // script length
                for (; commOffSet < 64; commOffSet++)
                {
                    commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
                }
                Pk2.writeUSB(commandArrayp);
            }
            
            // VERIFY PE
            // Set address
            if (Pk2.DevFile.PartsList[Pk2.ActivePart].ProgMemWrPrepScript != 0)
            { // if prog mem address set script exists for this part
                Pk2.DownloadAddress3(0x800000); // start of exec memory
                Pk2.RunScript(KONST.PROGMEM_ADDRSET, 1);
            }
            
            // verify the exec in 32 sections
            byte[] upload_buffer = new byte[KONST.UploadBufferSize];
            instruction = 0;
            for (int section = 0; section < 32; section++)
            {
                //Pk2.RunScriptUploadNoLen2(KONST.PROGMEM_RD, 1);
                Pk2.RunScriptUploadNoLen(KONST.PROGMEM_RD, 1);
                Array.Copy(Pk2.Usb_read_array, 1, upload_buffer, 0, KONST.USB_REPORTLENGTH);
                //Pk2.GetUpload();
                Pk2.UploadDataNoLen();
                Array.Copy(Pk2.Usb_read_array, 1, upload_buffer, KONST.USB_REPORTLENGTH, KONST.USB_REPORTLENGTH);
                int uploadIndex = 0;
                for (int word = 0; word < 32; word++)
                {
                    uint memWord = (uint)upload_buffer[uploadIndex++];
                    memWord |= (uint)upload_buffer[uploadIndex++] << 8;
                    memWord |= (uint)upload_buffer[uploadIndex++] << 16;
                    if (memWord != dsPIC33_PE_Code[instruction++])
                    {
                        Pk2.RunScript(KONST.PROG_EXIT, 1);
                        return false;
                    }
                }
            }
            
            Pk2.RunScript(KONST.PROG_EXIT, 1);
            
            return true;
        }
        
        public static bool PE_Connect()
        {
            Pk2.RunScript(KONST.PROG_ENTRY, 1);

            if (Pk2.DevFile.PartsList[Pk2.ActivePart].ProgMemWrPrepScript != 0)
            { // if prog mem address set script exists for this part
                Pk2.DownloadAddress3(0x8007C0); // last 32 words of exec memory
                Pk2.RunScript(KONST.PROGMEM_ADDRSET, 1);
            }
            byte[] upload_buffer = new byte[KONST.UploadBufferSize];
            //Pk2.RunScriptUploadNoLen2(KONST.PROGMEM_RD, 1);
            Pk2.RunScriptUploadNoLen(KONST.PROGMEM_RD, 1);
            Array.Copy(Pk2.Usb_read_array, 1, upload_buffer, 0, KONST.USB_REPORTLENGTH);
            //Pk2.GetUpload();
            Pk2.UploadDataNoLen();
            Array.Copy(Pk2.Usb_read_array, 1, upload_buffer, KONST.USB_REPORTLENGTH, KONST.USB_REPORTLENGTH);
            // check ID word
            int memValue = (int)upload_buffer[72]; // addresss 0x8007F0
            memValue |= (int)(upload_buffer[73] << 8);
            if (memValue != dsPIC33_PE_ID)
            {
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                return false;
            }
            // check PE version
            memValue = (int)upload_buffer[75]; // addresss 0x8007F2
            memValue |= (int)(upload_buffer[76] << 8);
            if (memValue != dsPIC33_PE_Version)
            {
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                return false;
            }

            Pk2.RunScript(KONST.PROG_EXIT, 1);
            
            // It looks like there is a PE there.  Try talking to the PE directly:
            int commOffSet = 0;
            byte[] commandArrayp = new byte[64];
            // entering programming mode with PE (4D434850)
            commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayp[commOffSet++] = 0; // fill in later
            commandArrayp[commOffSet++] = KONST._VPP_OFF;
            commandArrayp[commOffSet++] = KONST._MCLR_GND_ON;
            commandArrayp[commOffSet++] = KONST._VPP_PWM_ON;
            commandArrayp[commOffSet++] = KONST._BUSY_LED_ON;
            commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
            commandArrayp[commOffSet++] = 0x00;
            commandArrayp[commOffSet++] = KONST._DELAY_LONG;
            commandArrayp[commOffSet++] = 20;
            commandArrayp[commOffSet++] = KONST._MCLR_GND_OFF;
            commandArrayp[commOffSet++] = KONST._VPP_ON;
            commandArrayp[commOffSet++] = KONST._DELAY_SHORT;
            commandArrayp[commOffSet++] = 23;
            commandArrayp[commOffSet++] = KONST._VPP_OFF;
            commandArrayp[commOffSet++] = KONST._MCLR_GND_ON;
            commandArrayp[commOffSet++] = KONST._DELAY_SHORT;
            commandArrayp[commOffSet++] = 47;
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
            commandArrayp[commOffSet++] = 0xB2;
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
            commandArrayp[commOffSet++] = 0xC2;
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
            commandArrayp[commOffSet++] = 0x12;
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
            commandArrayp[commOffSet++] = 0x0A;
            commandArrayp[commOffSet++] = KONST._MCLR_GND_OFF;
            commandArrayp[commOffSet++] = KONST._VPP_ON;
            commandArrayp[commOffSet++] = KONST._DELAY_LONG;
            commandArrayp[commOffSet++] = 6;    //32ms
            commandArrayp[1] = (byte)(commOffSet - 2);  // script length
            for (; commOffSet < 64; commOffSet++)
            {
                commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
            }
            Pk2.writeUSB(commandArrayp);
            // Try sanity Check
            commOffSet = 0;
            commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayp[commOffSet++] = 12;
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // sanity check = 0x00 01
            commandArrayp[commOffSet++] = 0x00;
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
            commandArrayp[commOffSet++] = 0x80;                     // PE talks MSB first, script routines are LSB first.
            commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
            commandArrayp[commOffSet++] = 0x02; // PGD is input
            commandArrayp[commOffSet++] = KONST._DELAY_SHORT;
            commandArrayp[commOffSet++] = 5;                        //100+ us
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST.UPLOAD_DATA;
            for (; commOffSet < 64; commOffSet++)
            {
                commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
            }
            Pk2.writeUSB(commandArrayp);
            if (!Pk2.readUSB())
            {
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                return false;
            }
            if (Pk2.Usb_read_array[1] != 4) // expect 4 bytes back : 0x10 00 00 02
            {
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                return false;
            }
            if ((Pk2.Usb_read_array[2] != 0x08) || (Pk2.Usb_read_array[3] != 0x00)
                || (Pk2.Usb_read_array[4] != 0x00) || (Pk2.Usb_read_array[5] != 0x40))
            {
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                return false;
            }
            
            // Passed sanity check; verify version.
            commOffSet = 0;
            commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayp[commOffSet++] = 14;
            commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
            commandArrayp[commOffSet++] = 0x00; // PGD is output
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // QVER = 0xB0 01
            commandArrayp[commOffSet++] = 0x0D;
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
            commandArrayp[commOffSet++] = 0x80;                     // PE talks MSB first, script routines are LSB first.
            commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
            commandArrayp[commOffSet++] = 0x02; // PGD is input
            commandArrayp[commOffSet++] = KONST._DELAY_SHORT;
            commandArrayp[commOffSet++] = 5;                        //100+ us
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST.UPLOAD_DATA;
            for (; commOffSet < 64; commOffSet++)
            {
                commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
            }
            Pk2.writeUSB(commandArrayp);
            if (!Pk2.readUSB())
            {
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                return false;
            }
            if (Pk2.Usb_read_array[1] != 4) // expect 4 bytes back : 0x1B <Ver> 00 02
            {
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                return false;
            }
            if ((Pk2.Usb_read_array[2] != 0xD8) || (BitReverseTable[Pk2.Usb_read_array[3]] != (byte)dsPIC33_PE_Version)
                || (Pk2.Usb_read_array[4] != 0x00) || (Pk2.Usb_read_array[5] != 0x40))
            {
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                return false;
            }
            
            // Do not exit programming mode if we successfully find a PE
            return true;
        }

        public static bool PE_DownloadAndConnect()
        {
            // VDD must already be on!
            // reduce PE comm speed to 500kHz max
            ICSPSpeedRestore = Pk2.LastICSPSpeed;
            if (Pk2.LastICSPSpeed < 2)
                Pk2.SetProgrammingSpeed(2);
            
            // See if the PE already exists
            if (!PE_Connect())
            { // it doesn't, download it    
                UpdateStatusWinText("Downloading Programming Executive...");
                if (!DownloadPE())
                { // download failed
                    UpdateStatusWinText("Downloading Programming Executive...FAILED!");
                    restoreICSPSpeed();
                    return false;
                }
                if (!PE_Connect())
                { // try connecting
                    UpdateStatusWinText("Downloading Programming Executive...FAILED!");
                    restoreICSPSpeed();
                    return false;
                }
            }

            return true;
        }
        
        private static void restoreICSPSpeed()
        {
            if (ICSPSpeedRestore != Pk2.LastICSPSpeed)
                Pk2.SetProgrammingSpeed(ICSPSpeedRestore);
        }
        
        public static bool PEBlankCheck(uint lengthWords)
        {
            // Use QBLANK (0xA)
            int commOffSet = 0;
            byte[] commandArrayp = new byte[64];
            
            lengthWords++; // command arg is length + 1

            commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayp[commOffSet++] = 0;
            commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
            commandArrayp[commOffSet++] = 0x00; // PGD is output
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; //0xA0 03
            commandArrayp[commOffSet++] = 0x05;
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
            commandArrayp[commOffSet++] = 0xC0;
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
            commandArrayp[commOffSet++] = 0x00;
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // lengthWords
            commandArrayp[commOffSet++] = BitReverseTable[((lengthWords >> 16) & 0xFF)];
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // lengthWords
            commandArrayp[commOffSet++] = BitReverseTable[((lengthWords >> 8) & 0xFF)];
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
            commandArrayp[commOffSet++] = BitReverseTable[(lengthWords & 0xFF)];
            commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
            commandArrayp[commOffSet++] = 0x02; // PGD is input
            commandArrayp[1] = (byte)(commOffSet - 2);  // script length
            for (; commOffSet < 64; commOffSet++)
            {
                commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
            }
            Pk2.writeUSB(commandArrayp);

            // wait 2 seconds for the results.
            Thread.Sleep(2000);
                
            // get results
            commOffSet = 0;
            commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayp[commOffSet++] = 4;
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST.UPLOAD_DATA;
            for (; commOffSet < 64; commOffSet++)
            {
                commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
            }
            Pk2.writeUSB(commandArrayp);
            if (!Pk2.readUSB())
            {
                return false;
            }
            if ((Pk2.Usb_read_array[1] != 4)
              || (Pk2.Usb_read_array[2] != BitReverseTable[0x1A]) || (Pk2.Usb_read_array[3] != BitReverseTable[0xF0])
              || (Pk2.Usb_read_array[4] != 0x00) || (Pk2.Usb_read_array[5] != BitReverseTable[0x02]))
                return false; // device not blank or error
            
            return true;
        }

        public static bool PE33BlankCheck(string saveText)
        {
            if (!PE_DownloadAndConnect())
            {
                return false;
            }
            
            UpdateStatusWinText(saveText);

            // Check Program Memory ====================================================================================
            if (!PEBlankCheck((uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem))
            {
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                restoreICSPSpeed();
                return false;
            }

            Pk2.RunScript(KONST.PROG_EXIT, 1);
            restoreICSPSpeed();
            return true;
        }
        
        public static bool PE33Read(string saveText)
        {
            if (!PE_DownloadAndConnect())
            {
                return false;
            }

            UpdateStatusWinText(saveText);

            // Read Program Memory ====================================================================================
            byte[] upload_buffer = new byte[KONST.UploadBufferSize];
            int bytesPerWord = Pk2.DevFile.Families[Pk2.GetActiveFamily()].BytesPerLocation;
            int wordsPerLoop = 32;
            int wordsRead = 0;
            int uploadIndex = 0;
            int commOffSet = 0;
            byte[] commandArrayp = new byte[64];
            ResetStatusBar((int)(Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem / wordsPerLoop));

            do
            {
                commOffSet = 0;
                commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
                commandArrayp[commOffSet++] = 0;
                commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
                commandArrayp[commOffSet++] = 0x00; // PGD is output
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; //0x20 04
                commandArrayp[commOffSet++] = 0x04;
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
                commandArrayp[commOffSet++] = 0x20;
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // Length N
                commandArrayp[commOffSet++] = 0x00;
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
                commandArrayp[commOffSet++] = BitReverseTable[wordsPerLoop];
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // address MSW
                commandArrayp[commOffSet++] = 0x00;
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
                commandArrayp[commOffSet++] = BitReverseTable[((wordsRead >> 15) & 0xFF)];
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // address LSW
                commandArrayp[commOffSet++] = BitReverseTable[((wordsRead >> 7) & 0xFF)];
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
                commandArrayp[commOffSet++] = BitReverseTable[((wordsRead << 1) & 0xFF)];
                commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
                commandArrayp[commOffSet++] = 0x02; // PGD is input
                commandArrayp[commOffSet++] = KONST._DELAY_SHORT;
                commandArrayp[commOffSet++] = 5;
                commandArrayp[commOffSet++] = KONST._READ_BYTE;         // Read & toss 2 response words
                commandArrayp[commOffSet++] = KONST._READ_BYTE;
                commandArrayp[commOffSet++] = KONST._READ_BYTE;
                commandArrayp[commOffSet++] = KONST._READ_BYTE;
                commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;  // read 32 3-byte words
                commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
                commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
                commandArrayp[commOffSet++] = KONST._LOOP;
                commandArrayp[commOffSet++] = 3;
                commandArrayp[commOffSet++] = 31;
                commandArrayp[commOffSet++] = KONST.UPLOAD_DATA_NOLEN;
                commandArrayp[commOffSet++] = KONST.UPLOAD_DATA_NOLEN;
                commandArrayp[1] = (byte)(commOffSet - 4);  // script length
                for (; commOffSet < 64; commOffSet++)
                {
                    commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
                }
                Pk2.writeUSB(commandArrayp);

                Pk2.GetUpload();
                Array.Copy(Pk2.Usb_read_array, 1, upload_buffer, 0, KONST.USB_REPORTLENGTH);
                Pk2.GetUpload();
                Array.Copy(Pk2.Usb_read_array, 1, upload_buffer, KONST.USB_REPORTLENGTH, KONST.USB_REPORTLENGTH);
                uploadIndex = 0;
                for (int word = 0; word < wordsPerLoop; word+= 2)
                {
                    // two word2 of packed instructions
                    uint memWord1 = (uint)BitReverseTable[upload_buffer[uploadIndex++]] << 8;
                    memWord1 |= (uint)BitReverseTable[upload_buffer[uploadIndex++]];
                    uint memWord2 = (uint)BitReverseTable[upload_buffer[uploadIndex++]] << 16;
                    memWord1 |= (uint)BitReverseTable[upload_buffer[uploadIndex++]] << 16;
                    memWord2 |= (uint)BitReverseTable[upload_buffer[uploadIndex++]] << 8;
                    memWord2 |= (uint)BitReverseTable[upload_buffer[uploadIndex++]];
                    Pk2.DeviceBuffers.ProgramMemory[wordsRead++] = memWord1;
                    Pk2.DeviceBuffers.ProgramMemory[wordsRead++] = memWord2;
                    if (wordsRead >= Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem)
                    {
                        break; // for cases where ProgramMemSize%WordsPerLoop != 0
                    }
                }
                StepStatusBar();
            } while (wordsRead < Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem);

            Pk2.RunScript(KONST.PROG_EXIT, 1);
            restoreICSPSpeed();
            return true;
        }
        
        public static bool PE33Write(int endOfBuffer, string saveText)
        {
            if (!PE_DownloadAndConnect())
            {
                return false;
            }

            UpdateStatusWinText(saveText);
        
            byte[] downloadBuffer = new byte[KONST.DownLoadBufferSize];
            int wordsPerLoop = 64;
            int wordsWritten = 0;

            ResetStatusBar((int)(endOfBuffer / wordsPerLoop));

            do
            {
                int downloadIndex = 0;
                for (int word = 0; word < wordsPerLoop; word+=2)
                {
                    // Put in packed format for PE  
                    uint memWord = Pk2.DeviceBuffers.ProgramMemory[wordsWritten++];
                    downloadBuffer[downloadIndex + 1] = BitReverseTable[(memWord & 0xFF)];
                    //checksumPk2Go += (byte) (memWord & 0xFF);
                    memWord >>= 8;
                    downloadBuffer[downloadIndex] = BitReverseTable[(memWord & 0xFF)];
                    //checksumPk2Go += (byte)(memWord & 0xFF);
                    memWord >>= 8;
                    downloadBuffer[downloadIndex + 3] = BitReverseTable[(memWord & 0xFF)];
                    //checksumPk2Go += (byte)(memWord & 0xFF);

                    memWord = Pk2.DeviceBuffers.ProgramMemory[wordsWritten++];
                    downloadBuffer[downloadIndex + 5] = BitReverseTable[(memWord & 0xFF)];
                    //checksumPk2Go += (byte) (memWord & 0xFF);
                    memWord >>= 8;
                    downloadBuffer[downloadIndex + 4] = BitReverseTable[(memWord & 0xFF)];
                    //checksumPk2Go += (byte)(memWord & 0xFF);
                    memWord >>= 8;
                    downloadBuffer[downloadIndex + 2] = BitReverseTable[(memWord & 0xFF)];
                    //checksumPk2Go += (byte)(memWord & 0xFF);
                    
                    downloadIndex += 6;
                            
                }
                // download data
                int dataIndex = Pk2.DataClrAndDownload(downloadBuffer, 0);
                while (dataIndex < downloadIndex)
                {
                    dataIndex = Pk2.DataDownload(downloadBuffer, dataIndex, downloadIndex);
                }

                int commOffSet = 0;
                byte[] commandArrayp = new byte[64];
                commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
                commandArrayp[commOffSet++] = 0; // fill in later
                commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
                commandArrayp[commOffSet++] = 0x00; // PGD is output
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // PROGP = 0x50 63
                commandArrayp[commOffSet++] = 0x0A;
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
                commandArrayp[commOffSet++] = 0xC6;                      // PE talks MSB first, script routines are LSB first.
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // address MSW
                commandArrayp[commOffSet++] = 0x00;
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
                commandArrayp[commOffSet++] = BitReverseTable[(((wordsWritten - 64) >> 15) & 0xFF)];
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // address LSW
                commandArrayp[commOffSet++] = BitReverseTable[(((wordsWritten - 64) >> 7) & 0xFF)];
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
                commandArrayp[commOffSet++] = BitReverseTable[(((wordsWritten - 64) << 1) & 0xFF)];
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_BUFFER;  // write 64 3-byte words
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_BUFFER;
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_BUFFER;
                commandArrayp[commOffSet++] = KONST._LOOP;
                commandArrayp[commOffSet++] = 3;
                commandArrayp[commOffSet++] = 63;
                commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
                commandArrayp[commOffSet++] = 0x02; // PGD is input
                commandArrayp[commOffSet++] = KONST._DELAY_SHORT;
                commandArrayp[commOffSet++] = 118;                        //2.5ms
                commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;  // read response
                commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
                commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
                commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
                commandArrayp[commOffSet++] = KONST.UPLOAD_DATA;
                commandArrayp[1] = (byte)(commOffSet - 3);
                for (; commOffSet < 64; commOffSet++)
                {
                    commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
                }
                Pk2.writeUSB(commandArrayp);
                if (!Pk2.readUSB())
                {
                    UpdateStatusWinText("Programming Executive Error during Write.");
                    Pk2.RunScript(KONST.PROG_EXIT, 1);
                    restoreICSPSpeed();
                    return false;
                }
                if (Pk2.Usb_read_array[1] != 4) // expect 4 bytes back : 0x15 00 00 02
                {
                    UpdateStatusWinText("Programming Executive Error during Write.");
                    Pk2.RunScript(KONST.PROG_EXIT, 1);
                    restoreICSPSpeed();
                    return false;
                }
                if ((BitReverseTable[Pk2.Usb_read_array[2]] != 0x15) || (Pk2.Usb_read_array[3] != 0x00)
                    || (Pk2.Usb_read_array[4] != 0x00) || (BitReverseTable[Pk2.Usb_read_array[5]] != 0x02))
                {
                    UpdateStatusWinText("Programming Executive Error during Write.");
                    Pk2.RunScript(KONST.PROG_EXIT, 1);
                    restoreICSPSpeed();
                    return false;
                }

                StepStatusBar();
            } while (wordsWritten < endOfBuffer);

            Pk2.RunScript(KONST.PROG_EXIT, 1);
            restoreICSPSpeed();
            return true;
        }
        
        public static bool PE33VerifyCRC(string saveText)
        {
            if (!PE_DownloadAndConnect())
            {
                return false;
            }

            UpdateStatusWinText(saveText);
        
            // Use CRCP (0xC)
            int commOffSet = 0;
            byte[] commandArrayp = new byte[64];

            ushort bufferCRC = CalcCRCProgMem();
            uint lengthWords = Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem;

            commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayp[commOffSet++] = 0;
            commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
            commandArrayp[commOffSet++] = 0x00; // PGD is output
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; //0xC0 05
            commandArrayp[commOffSet++] = 0x03;
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
            commandArrayp[commOffSet++] = 0xA0;
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // word 2
            commandArrayp[commOffSet++] = 0x80;                      // Program Memory
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // start address MSB
            commandArrayp[commOffSet++] = 0x00;
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // word 3
            commandArrayp[commOffSet++] = 0x00;
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
            commandArrayp[commOffSet++] = 0x00;
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // word 4
            commandArrayp[commOffSet++] = 0x00;
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // lengthWords MSB
            commandArrayp[commOffSet++] = BitReverseTable[((lengthWords >> 16) & 0xFF)];
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // word5 lengthWords LSW
            commandArrayp[commOffSet++] = BitReverseTable[((lengthWords >> 8) & 0xFF)];
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
            commandArrayp[commOffSet++] = BitReverseTable[(lengthWords & 0xFF)];
            commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
            commandArrayp[commOffSet++] = 0x02; // PGD is input
            commandArrayp[1] = (byte)(commOffSet - 2);  // script length
            for (; commOffSet < 64; commOffSet++)
            {
                commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
            }
            Pk2.writeUSB(commandArrayp);
            // wait (memsize) * (1.5 seconds / 128k) for the results.
            float sleepTime = (float)Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem * (1500F / 44032F);
            Thread.Sleep((int)sleepTime);

            // get results
            commOffSet = 0;
            commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayp[commOffSet++] = 6;
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST.UPLOAD_DATA;
            for (; commOffSet < 64; commOffSet++)
            {
                commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
            }
            Pk2.writeUSB(commandArrayp);
            if (!Pk2.readUSB())
            {
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                restoreICSPSpeed();
                return false;
            }
            if (Pk2.Usb_read_array[1] != 6)
            {
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                restoreICSPSpeed();
                return false; // error
            }

            Pk2.RunScript(KONST.PROG_EXIT, 1);
            restoreICSPSpeed();

            ushort deviceCRC = (ushort)BitReverseTable[Pk2.Usb_read_array[7]];
            deviceCRC += (ushort)(BitReverseTable[Pk2.Usb_read_array[6]] << 8);

            if (deviceCRC == bufferCRC)
                return true;
            
            return false;
        }
        
        private static ushort CalcCRCProgMem()
        {
            uint CRC_Value = 0xFFFF; // seed
            
            for (int word = 0; word < Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem; word+=2)
            {
                uint memWord = Pk2.DeviceBuffers.ProgramMemory[word];
                CRC_Calculate((byte)(memWord & 0xFF), ref CRC_Value);
                CRC_Calculate((byte)((memWord >> 8) & 0xFF), ref CRC_Value);
                CRC_Calculate((byte)((memWord >> 16) & 0xFF), ref CRC_Value);
                memWord = Pk2.DeviceBuffers.ProgramMemory[word+1];
                CRC_Calculate((byte)((memWord >> 16) & 0xFF), ref CRC_Value);
                CRC_Calculate((byte)(memWord & 0xFF), ref CRC_Value);
                CRC_Calculate((byte)((memWord >> 8) & 0xFF), ref CRC_Value);
            }
            
            return (ushort)(CRC_Value & 0xFFFF);
        }
        
        private static void CRC_Calculate(byte ByteValue , ref uint CRC_Value )
        {
            byte  value;

            value = (byte)((CRC_Value >> (8)) ^ ByteValue);
            CRC_Value = CRC_LUT_Array[value] ^ (CRC_Value << 8);

        }




    }
}
