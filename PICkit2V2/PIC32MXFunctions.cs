using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Pk2 = PICkit2V2.PICkitFunctions;
using KONST = PICkit2V2.Constants;

namespace PICkit2V2
{
    public class PIC32MXFunctions
    {
        public static DelegateStatusWin UpdateStatusWinText;
        public static DelegateResetStatusBar ResetStatusBar;
        public static DelegateStepStatusBar StepStatusBar;

        private static uint[] pe_Loader = new uint[42] {0x3C07, 0xDEAD,
                                                        0x3C06, 0xFF20,
                                                        0x3C05, 0xFF20,
                                                        0x8CC4, 0x0000,
                                                        0x8CC3, 0x0000,
                                                        0x1067, 0x000B,
                                                        0x0000, 0x0000,
                                                        0x1060, 0xFFFB,
                                                        0x0000, 0x0000,
                                                        0x8CA2, 0x0000,
                                                        0x2463, 0xFFFF,
                                                        0xAC82, 0x0000,
                                                        0x2484, 0x0004,
                                                        0x1460, 0xFFFB,
                                                        0x0000, 0x0000,
                                                        0x1000, 0xFFF3,
                                                        0x0000, 0x0000,
                                                        0x3C02, 0xA000,
                                                        0x3442, 0x0900,
                                                        0x0040, 0x0008,
                                                        0x0000, 0x0000};

        // Program Executive version 0x0109          
        private const int pic32_PE_Version_RS128 = 0x0109;
        private static uint[] PIC32_PE_RS128 = new uint[1231]{
            0x3C1CA000, 0x279C7FF0, 0x3C1DA000, 0x37BD08FC, 0x3C08A000, 0x2508109C, 0x01000008, 0x00000000, 
            0x3C06BF88, 0x90C86160, 0x3C05BF88, 0x35070040, 0xA0C76160, 0x90A46140, 0x2402FFBF, 0x00821824, 
            0xA0A36140, 0x03E00008, 0x00000000, 0x3C06BF88, 0x90C86160, 0x2405FFBF, 0x01053824, 0xA0C76160, 
            0x3C03BF88, 0x90646140, 0x00851024, 0xA0626140, 0x03E00008, 0x00000000, 0x3C06BF88, 0x90C86161, 
            0x3C05BF88, 0x35070001, 0xA0C76161, 0x90A46141, 0x2402FFFE, 0x00821824, 0xA0A36141, 0x03E00008, 
            0x00000000, 0x3C06BF88, 0x90C86161, 0x2405FFFE, 0x01053824, 0xA0C76161, 0x3C03BF88, 0x90646141, 
            0x00851024, 0xA0626141, 0x03E00008, 0x00000000, 0x00052882, 0x10A0001B, 0x00003821, 0x3C061FC0, 
            0x3C03BFC0, 0x3C027FFF, 0x34C92FFC, 0x34682FFC, 0x344BFFFF, 0x10000006, 0x240AFFFF, 0x8C990000, 
            0x172B000E, 0x00000000, 0x10C0000E, 0x24840004, 0x0089C026, 0x00887826, 0x2F0D0001, 0x2DEE0001, 
            0x24E70001, 0x01AE6025, 0x1580FFF4, 0x00E5302B, 0x8C820000, 0x104AFFF4, 0x00000000, 0x03E00008, 
            0x24020001, 0x03E00008, 0x00001021, 0x3C04FF20, 0x8C830000, 0x2C620002, 0x10400003, 0x24040001, 
            0xAF838014, 0x00002021, 0x03E00008, 0x00801021, 0x10A00007, 0x3C02FF20, 0x3443000C, 0x8C860000, 
            0x24A5FFFF, 0xAC660000, 0x14A0FFFC, 0x24840004, 0x03E00008, 0x00000000, 0x08000500, 0x00000000, 
            0x27BDFFD0, 0xAFB50024, 0xAFB40020, 0xAFB20018, 0xAFBF002C, 0xAFB60028, 0xAFB3001C, 0xAFB10014, 
            0xAFB00010, 0x00A0A021, 0x00809021, 0x10A00024, 0x3C15FF20, 0x3C021FFF, 0x3456FFFF, 0x2A830081, 
            0x24130080, 0x0283980B, 0x0013282A, 0x10A00008, 0x3C06A000, 0x24C40210, 0x02601821, 0x8EA70000, 
            0x2463FFFF, 0xAC870000, 0x1460FFFC, 0x24840004, 0x24040080, 0x5264001D, 0x3C05A000, 0x10A0000D, 
            0x00008821, 0x3C09A000, 0x25300210, 0x8E050000, 0x02402021, 0x0C000500, 0x26310001, 0x26100004, 
            0x0233182A, 0x14400007, 0x26520004, 0x5460FFF8, 0x8E050000, 0x0293A023, 0x1680FFE1, 0x2A830081, 
            0x00001021, 0x8FBF002C, 0x8FB60028, 0x8FB50024, 0x8FB40020, 0x8FB3001C, 0x8FB20018, 0x8FB10014, 
            0x8FB00010, 0x03E00008, 0x27BD0030, 0x24A30210, 0x04600003, 0x02C32824, 0x3C084000, 0x00682821, 
            0x0C000508, 0x02402021, 0x1440FFEE, 0x0293A023, 0x1680FFCA, 0x26520200, 0x1000FFEA, 0x00001021, 
            0x3C02A000, 0x24470210, 0x3C06FF20, 0x00E02821, 0x2403007F, 0x8CC80000, 0x2463FFFF, 0xACA80000, 
            0x0461FFFC, 0x24A50004, 0x3C064000, 0x00E01821, 0x04E10004, 0x00E62821, 0x3C051FFF, 0x34A7FFFF, 
            0x00672824, 0x08000508, 0x00000000, 0x27BDFFE0, 0xAFB20018, 0xAFB10014, 0xAFB00010, 0xAFBF001C, 
            0x00A09021, 0x00808021, 0x14A00007, 0x00008821, 0x8FBF001C, 0x8FB20018, 0x8FB10014, 0x8FB00010, 
            0x03E00008, 0x27BD0020, 0x02002021, 0x0C000510, 0x26310001, 0x26101000, 0x1040FFF5, 0x0232182B, 
            0x1460FFFA, 0x02002021, 0x8FBF001C, 0x8FB20018, 0x8FB10014, 0x8FB00010, 0x03E00008, 0x27BD0020, 
            0x08000516, 0x00000000, 0x8F828014, 0x27BDFFB8, 0xAFB7003C, 0xAFB60038, 0xAFB00020, 0xAFBF0040, 
            0xAFB50034, 0xAFB40030, 0xAFB3002C, 0xAFB20028, 0xAFB10024, 0x00808021, 0x00A0B821, 0x00C0B021, 
            0xA3A00018, 0x10400041, 0xA3A00019, 0x00058A02, 0x3C050001, 0x240A0F00, 0x3C0BBF88, 0x3C09BF88, 
            0x340884CF, 0x3C06BF88, 0x34A41021, 0x3C03BF88, 0xAD6A3034, 0xAD2A3038, 0xACC83040, 0xAC643050, 
            0x1A200016, 0x32E600FF, 0x3C073FFF, 0x34F5FFFF, 0x3C14A000, 0x3C13C000, 0x24120003, 0x02B0682B, 
            0x02146025, 0x02132821, 0x24070002, 0x018D280A, 0x02C02021, 0x24060100, 0x0C000582, 0xAFB20010, 
            0x00403821, 0x2631FFFF, 0x1440003B, 0x26100100, 0x1E20FFF3, 0x02B0682B, 0x32E600FF, 0x10C0000E, 
            0x3C123FFF, 0x3651FFFF, 0x3C0FA000, 0x0230702B, 0x11C00003, 0x020F2825, 0x3C13C000, 0x02132821, 
            0x24070002, 0x02C02021, 0x24100003, 0x0C000582, 0xAFB00010, 0x00403821, 0x10E00033, 0x24020001, 
            0x8FBF0040, 0x8FB7003C, 0x8FB60038, 0x8FB50034, 0x8FB40030, 0x8FB3002C, 0x8FB20028, 0x8FB10024, 
            0x8FB00020, 0x03E00008, 0x27BD0048, 0x0C000554, 0x3404FFFF, 0x3C1F3FFF, 0x37F9FFFF, 0x3C18A000, 
            0x0330A82B, 0x12A00003, 0x02182025, 0x3C02C000, 0x02022021, 0x0C00056B, 0x02E02821, 0x0C000580, 
            0x00000000, 0xAEC20000, 0x8FBF0040, 0x8FB7003C, 0x8FB60038, 0x8FB50034, 0x8FB40030, 0x8FB3002C, 
            0x8FB20028, 0x8FB10024, 0x8FB00020, 0x00001021, 0x03E00008, 0x27BD0048, 0x8FBF0040, 0x8FB7003C, 
            0x8FB60038, 0x8FB50034, 0x8FB40030, 0x8FB3002C, 0x8FB20028, 0x8FB10024, 0x8FB00020, 0x24020001, 
            0x03E00008, 0x27BD0048, 0x02C02021, 0x27A50018, 0x24140003, 0x24060002, 0x24070002, 0x0C000582, 
            0xAFB40010, 0x8FBF0040, 0x8FB7003C, 0x8FB60038, 0x8FB50034, 0x8FB40030, 0x8FB3002C, 0x8FB20028, 
            0x8FB10024, 0x8FB00020, 0x0002102B, 0x03E00008, 0x27BD0048, 0x27BDFFC8, 0xAFB7002C, 0xAFB50024, 
            0xAFB3001C, 0xAFB20018, 0xAFB10014, 0xAFB00010, 0xAFBF0034, 0xAFBE0030, 0xAFB60028, 0xAFB40020, 
            0x00A08821, 0x00809021, 0x0000A821, 0x24170001, 0x3C10FF20, 0x10A00039, 0x00009821, 0x3C02A000, 
            0x3C031FFF, 0x24560210, 0x347EFFFF, 0x02C02021, 0x2403007F, 0x8E050000, 0x2463FFFF, 0xAC850000, 
            0x0461FFFC, 0x24840004, 0x12E00046, 0x00000000, 0x0000B821, 0x2644FE00, 0x0095980B, 0x56600009, 
            0x2631FF80, 0x3C084000, 0x03D63024, 0x01162821, 0x2AC70000, 0x00C7280B, 0x0C000518, 0x02402021, 
            0x2631FF80, 0x2A290080, 0x1520001A, 0x26520200, 0x3C0BA000, 0x256A1C38, 0x8D540000, 0x2403007F, 
            0x02802021, 0x8E0C0000, 0x2463FFFF, 0xAC8C0000, 0x0461FFFC, 0x24840004, 0x0C00052C, 0x00000000, 
            0x264DFE00, 0x01A2980B, 0x16600008, 0x0040A821, 0x3C184000, 0x2A8F0000, 0x03D47024, 0x03142821, 
            0x01CF280B, 0x0C000518, 0x02402021, 0x2631FF80, 0x26520200, 0x1620FFCE, 0x02C02021, 0x0C00052C, 
            0x0013A02B, 0x0002882B, 0x02348025, 0x12000019, 0x3C17FF20, 0x3C16FF20, 0x36C4000C, 0x24150002, 
            0xAC950000, 0x52600022, 0x2653FE00, 0xAC930000, 0x8FBF0034, 0x8FBE0030, 0x8FB7002C, 0x8FB60028, 
            0x8FB50024, 0x8FB40020, 0x8FB3001C, 0x8FB20018, 0x8FB10014, 0x8FB00010, 0x00001021, 0x03E00008, 
            0x27BD0038, 0x0C00052C, 0x00000000, 0x1000FFB9, 0x0040A821, 0x36F2000C, 0xAE400000, 0x8FBF0034, 
            0x8FBE0030, 0x8FB7002C, 0x8FB60028, 0x8FB50024, 0x8FB40020, 0x8FB3001C, 0x8FB20018, 0x8FB10014, 
            0x8FB00010, 0x00001021, 0x03E00008, 0x27BD0038, 0xAC930000, 0x1000FFDF, 0x8FBF0034, 0x27BDFFC0, 
            0x3C02BF88, 0x24040FC3, 0xAFBF0038, 0xAFB10034, 0xAFB00030, 0xAC443030, 0x8C433030, 0x106400B0, 
            0x24050001, 0xAF808014, 0x3C09BF80, 0x3528F220, 0x8D070000, 0x3C06A000, 0x7CF13B00, 0x24D01BF4, 
            0x3C0DFF20, 0x8DAC0000, 0x240B0001, 0x000C3402, 0x3185FFFF, 0x2CCA0010, 0xAF8B8010, 0xAFA60010, 
            0x11400006, 0xAFA50014, 0x0006C080, 0x03107821, 0x8DEE0000, 0x01C00008, 0x00000000, 0x24050003, 
            0x241F000F, 0x30A4FFFF, 0x10DF0021, 0x3C03000F, 0x24030007, 0x10C3007E, 0x2407000A, 0x10C70084, 
            0x240B0002, 0x10CBFFE6, 0x00067C00, 0x35F90002, 0x3C18FF20, 0x0325780B, 0x3706000C, 0xACCF0000, 
            0x8FA30010, 0x2CA40001, 0x386E0001, 0x2DCD0001, 0x01A46024, 0x1580007D, 0x38620008, 0x2C430001, 
            0x0064F824, 0x53E0FFD7, 0x3C0DFF20, 0x8FA40028, 0xACC40000, 0x1000FFD3, 0x3C0DFF20, 0x8F858014, 
            0x241F000F, 0x30A4FFFF, 0x14DFFFE1, 0x3C03000F, 0x3C02FF20, 0x00832025, 0x3446000C, 0xACC40000, 
            0x1000FFC8, 0x3C0DFF20, 0x0C000293, 0x00000000, 0x8FA60010, 0x1000FFD2, 0x00402821, 0x1000FFD0, 
            0x00002821, 0x3C0DFF20, 0x8DA40000, 0xAFA40018, 0x8DAC0000, 0x000C5882, 0x01602821, 0x0C0002A8, 
            0xAFAB0014, 0x8FA60010, 0x1000FFC5, 0x00402821, 0xAFA00028, 0x3C19FF20, 0x8F240000, 0x27A60028, 
            0xAFA40018, 0x8F380000, 0x03002821, 0x0C000322, 0xAFB80014, 0x8FA60010, 0x1000FFB9, 0x00402821, 
            0x1000FFB7, 0x24050109, 0x3C0FFF20, 0x8DE40000, 0xAFA40018, 0x8DEE0000, 0x01C02821, 0x0C000274, 
            0xAFAE0014, 0x8FA60010, 0x1000FFAD, 0x00402821, 0x3C02FF20, 0x8C460000, 0x00C02021, 0x0C000303, 
            0xAFA60018, 0x8FA60010, 0x1000FFA5, 0x00402821, 0x0C000320, 0x00000000, 0x8FA60010, 0x1000FFA0, 
            0x00402821, 0x3C1FFF20, 0x8FE40000, 0xAFA40018, 0x8FF90000, 0x03202821, 0x0C0002A6, 0xAFB9001C, 
            0x8FA60010, 0x1000FF96, 0x00402821, 0x3C0AFF20, 0x8D440000, 0xAFA40018, 0x8D490000, 0x00094082, 
            0x01002821, 0x0C0003AD, 0xAFA80014, 0x8FA60010, 0x1000FF8B, 0x00402821, 0x3C05FF20, 0x8CA70000, 
            0x24050002, 0x0007280B, 0x1000FF85, 0xAFA70018, 0x3C04FF20, 0x8C830000, 0x00602021, 0x0C0002F0, 
            0xAFA30018, 0x8FA60010, 0x1000FF7D, 0x00402821, 0x30A4FFFF, 0x3C030007, 0x3C02FF20, 0x00832025, 
            0x3446000C, 0xACC40000, 0x1000FF66, 0x3C0DFF20, 0x3C0A000A, 0x3C09FF20, 0x022A4025, 0x3525000C, 
            0xACA80000, 0x1000FF5F, 0x3C0DFF20, 0x8FA40018, 0x0C00029C, 0x8FA50014, 0x1000FF5A, 0x3C0DFF20, 
            0x1000FF51, 0xAF858014, 0x03E00008, 0x00001021, 0x34844000, 0x3C05BF81, 0xACA4F400, 0x3C08BF80, 
            0x3508F400, 0x3C09AA99, 0x35296655, 0x3C0A5566, 0x354A99AA, 0x3C0B0000, 0x356B8000, 0xAD090010, 
            0xAD0A0010, 0xAD0B0008, 0x8CA3F400, 0x30628000, 0x1440FFFD, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x24084000, 0x3C07BF81, 0xACE8F404, 0x8CA6F400, 0x03E00008, 0x30C22000, 
            0x00803021, 0x3C02BF81, 0x3C03BF81, 0x24040001, 0xAC46F420, 0xAC65F430, 0x080004E4, 0x00000000, 
            0x00803021, 0x3C02BF81, 0x3C03BF81, 0x24040003, 0xAC46F420, 0xAC65F440, 0x080004E4, 0x00000000, 
            0x00801821, 0x3C02BF81, 0x24040004, 0xAC43F420, 0x080004E4, 0x00000000, 0x080004E4, 0x2404000E, 
            0x3C06BF81, 0xACC4F420, 0x3C03BF81, 0x24044003, 0x3C02BF81, 0xAC65F440, 0xAC44F400, 0x3C08BF80, 
            0x3508F400, 0x3C09AA99, 0x35296655, 0x3C0A5566, 0x354A99AA, 0x3C0B0000, 0x356B8000, 0xAD090010, 
            0xAD0A0010, 0xAD0B0008, 0x03E00008, 0x00001021, 0x3C04BF81, 0x8C83F400, 0x30628000, 0x1440FFFD, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x24074000, 0x3C06BF81, 0xACC7F404, 
            0x8C85F400, 0x03E00008, 0x30A22000, 0x3C02A000, 0x24490210, 0x00004021, 0x00082200, 0x3083FFFF, 
            0x00002821, 0x24060007, 0x00056040, 0x00A35826, 0x39871021, 0x7C0B5620, 0x05400002, 0x30E5FFFF, 
            0x3185FFFF, 0x00036840, 0x24C6FFFF, 0x04C1FFF6, 0x31A3FFFF, 0x25080001, 0x29030100, 0xAD250000, 
            0x1460FFED, 0x25290004, 0x03E00008, 0x00000000, 0x27BDFFE8, 0xAFBF0010, 0xA784801C, 0x0C00053B, 
            0xAF808018, 0x8FBF0010, 0x24020001, 0x27BD0018, 0x03E00008, 0xAF828018, 0x9786801C, 0x308B00FF, 
            0x7CCC3A00, 0x016C5026, 0x3C09A000, 0x000A3880, 0x25280210, 0x00E82021, 0x8C850000, 0x00061200, 
            0x00451826, 0x03E00008, 0xA783801C, 0x27BDFFE0, 0xAFB10014, 0xAFB00010, 0xAFBF001C, 0xAFB20018, 
            0x00808821, 0x10A00008, 0x24B0FFFF, 0x2412FFFF, 0x92240000, 0x2610FFFF, 0x0C00055E, 0x26310001, 
            0x5612FFFC, 0x92240000, 0x8FBF001C, 0x8FB20018, 0x8FB10014, 0x8FB00010, 0x03E00008, 0x27BD0020, 
            0x03E00008, 0x9782801C, 0x240800C0, 0x70E8C802, 0x27BDFFE8, 0xAFBF0010, 0x3C1FA000, 0x8FE91C34, 
            0x3C0CBF88, 0x240A0080, 0xAD8A3034, 0x03294021, 0x91180000, 0x00E05821, 0x7F0F01C0, 0x24090040, 
            0x240E1000, 0x3C0DBF88, 0x34038000, 0x3C02BF88, 0x2DE70001, 0xAD090018, 0xADAE3004, 0xAC433008, 
            0x10E00003, 0x00000000, 0xAD0A0008, 0xAD090018, 0x91070000, 0x7CEA01C0, 0x1540FFFD, 0x8FB80028, 
            0x240F0038, 0x240EFFFF, 0x240D0003, 0xAD180000, 0x3C02BF88, 0xAD0F0014, 0x356300C0, 0xAD0E0024, 
            0xAD8D3034, 0x3C0C4000, 0xAC433038, 0x04A10004, 0x00AC1021, 0x3C191FFF, 0x3729FFFF, 0x00A91024, 
            0x3C054000, 0xAD020030, 0x04810004, 0x00851021, 0x3C0A1FFF, 0x355FFFFF, 0x009F1024, 0xAD020040, 
            0xAD060090, 0x8D070090, 0x01602021, 0xAD070060, 0x8D060060, 0x24050002, 0xAD060050, 0x0C0005C4, 
            0x00003021, 0x8FBF0010, 0x03E00008, 0x27BD0018, 0x240900C0, 0x70893802, 0x3C0AA000, 0x8D481C34, 
            0x24030080, 0x00E82021, 0x240200FF, 0xAC820024, 0x00005021, 0xAC830008, 0x0006782B, 0xAC830018, 
            0x10A00039, 0x00000000, 0x240B0001, 0x10AB0016, 0x24030001, 0x8C8C0050, 0x11800002, 0x24080100, 
            0x8C880050, 0x8C8D0060, 0x11A00002, 0x24070100, 0x8C870060, 0x8C8E0090, 0x11C00004, 0x24030100, 
            0x8C830090, 0x10000002, 0x0107102B, 0x0107102B, 0x0102380A, 0x00E3C821, 0x2738FFFF, 0x0303001B, 
            0x006001F4, 0x00001812, 0x2468FFFF, 0x240E0001, 0x240D0004, 0x240C0080, 0x240BFFFF, 0x8C830020, 
            0x30780003, 0x7C690080, 0x13000006, 0x7C6700C0, 0x7C640000, 0x10800014, 0x240A0002, 0x10000012, 
            0x240A0001, 0x14E00010, 0x00000000, 0x11200008, 0x00000000, 0x10AE000C, 0x00000000, 0x5100000A, 
            0x240A0003, 0x2508FFFF, 0xAC8D0024, 0xAC8C0018, 0x11E0FFEA, 0x00000000, 0x24C6FFFF, 0x14CBFFE7, 
            0x00000000, 0x240A0005, 0x03E00008, 0x01401021, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x0109CDAB, 0xA0001310, 0xA00012F8, 0xA00012CC, 
            0xA00012A4, 0xA0001290, 0xA0001270, 0xA0001248, 0xA0001240, 0xA0001210, 0xA00011E4, 0xA00011DC, 
            0xA00011C8, 0xA000111C, 0xA000111C, 0xA000111C, 0xA000119C, 0xBF883060, 0xA0000010};

        private static int pic32_PE_Version = 0x00;
        private static uint[] PIC32_PE;

        private const int pic32_PE_Version_RS32 = 0x0301;
        private static uint[] PIC32_PE_RS32 = new uint[]{
            0x3C1CA000,0x279C7FF0,0x3C1DA000,0x37BD08FC,
            0x3C08A000,0x25080BE9,0x01000008,0x00000000,
            0x6E0035AA,0xB20B2513,0x2203EA8E,0xEB8EB30A,
            0x9C402B04,0xEAEEB709,0x9C401002,0x2202EA4F,
            0x6A01E820,0xEEA346C1,0x61ED4484,0x6A00E820,
            0x1FC00BFC,0xBFC00BFC,0x7FFFFFFF,0xB3042506,
            0x45AF9C40,0x4484DB40,0xE8A02DFB,0xFF20000C,
            0x6F00B20B,0x6103EA83,0xEA8DB20A,0xB30A1002,
            0x6782E469,0x2506456F,0x436FA440,0xE75DED6F,
            0x2DFA4481,0xE8206A00,0x6500DEE0,0x3FFFFFFF,
            0xA0000000,0xC0000000,0x1C0064C3,0x6500037B,
            0xE8A06443,0xD40564F5,0x2536D504,0x50219004,
            0x68206101,0xEA026A00,0xB41B6008,0xB5196770,
            0x9DC0436F,0x4484DCC0,0x70202BFA,0xB3166111,
            0x60035300,0xED6CB515,0xB4151002,0x1C00E395,
            0x94050373,0x97052A1A,0x47614F7F,0x1010D305,
            0xE9026900,0xB50C600D,0xE6A93628,0x1C009405,
            0x9AA0037B,0x93052A0A,0x43844121,0x17F1D405,
            0xE21F9204,0x2FCAD704,0x64756A00,0x6500E8A0,
            0xFF200000,0xA0000008,0x1FFFFFFF,0x40000000,
            0xB50D64C3,0x67C5B70B,0x9F406B1F,0x5300436F,
            0x46C4DE40,0x550060FA,0x60036745,0xED4CB507,
            0xB3071002,0x1C00E575,0x65000373,0xE8A06443,
            0xFF200000,0xA0000008,0x1FFFFFFF,0x40000000,
            0x64F4F008,0x69006704,0x1C00250A,0x6790036D,
            0x93092206,0xE9634121,0x4800F400,0x647461F6,
            0x6500E8A0,0x1C0064C3,0x6500036B,0xE8A06443,
            0x64F5F00C,0x6704677D,0xC3906C00,0xF7FFC391,
            0x18006C1F,0x672503BE,0xEA03B20A,0xB40A6103,
            0x1002EC0D,0xE0B1B509,0x03D01800,0x180067B1,
            0x650003D7,0xDE40960C,0x64756A00,0x6500E8A0,
            0x3FFFFFFF,0xA0000000,0xC0000000,0x6A0064F6,
            0xD2046B01,0x6704D305,0xD2066725,0xB432214B,
            0xB5306B1F,0x9DC0436F,0xDCC05300,0x60F94484,
            0x2C059405,0x039D1C00,0xD2046500,0x6F001002,
            0x9204D705,0x67702203,0xD3064B80,0x2E0B9606,
            0x5300B325,0xB5256003,0x1002ED6C,0xE395B424,
            0x03831C00,0x49E06790,0x5120487F,0x61D64001,
            0x6B1FB420,0x436FB51B,0x53009DE0,0x4484DCE0,
            0x1C0060F9,0x6500039D,0x2203D204,0x4A806750,
            0x9306D206,0xB3172B0B,0x60035300,0xED6CB513,
            0xB6131002,0x1C00E3D5,0x67900383,0x49E0487F,
            0x17B44001,0x039D1C00,0x2A026500,0x240A9406,
            0x9206B30D,0xDBE06F02,0xDB402202,0x48801006,
            0x1003DB00,0x9206B508,0x6A00DD40,0xE8A06476,
            0xFF200000,0xA0000008,0x1FFFFFFF,0x40000000,
            0xA0000088,0xFF20000C,0xB54B64E7,0x9D80B649,
            0x6F03F7C1,0x32723382,0xDEE06702,0xB547E811,
            0x33429D40,0xEA313362,0xD3045B0D,0x6050D205,
            0x3768B243,0x9E80E759,0x9D80EC80,0x9DA0D406,
            0x026A1800,0x1040D507,0x18009D80,0xD4060290,
            0x1800103B,0x650002A9,0x9D801037,0x18009505,
            0xD40602A0,0xF3001031,0x10336B01,0x5B019D60,
            0xD30667B8,0x102D33A4,0xD4069D80,0x35CA9DC0,
            0x02BF1800,0x1020D505,0xD4069D80,0x35EA9DE0,
            0x026D1800,0x1018D505,0xD4069D80,0x18009DA0,
            0xD5050248,0x9D801011,0xD406060A,0x18009DA0,
            0xD50502AC,0x6C001009,0x9D80D40B,0xD406060B,
            0x18009DA0,0xD505025C,0x10036762,0x10016B00,
            0x92046B03,0x61057207,0x32403240,0xEA6DEB31,
            0x720A1005,0x35406106,0xEA0D32A0,0xDB40B315,
            0x72021796,0x36406094,0x34C0B512,0x6F022302,
            0xDD80ECED,0x72019204,0x2B056106,0x18009406,
            0x95050257,0x72081784,0x2B026103,0x1006920A,
            0xF77F720C,0xF77F611C,0x920B2B1A,0x1777DD40,
            0xBF883030,0xBF80F220,0xFF200000,0xA0000F64,
            0xFF20000C,0x34844000,0x3C05BF81,0xACA4F400,
            0x40034800,0x40074800,0x00E33023,0x2CC200F0,
            0x1440FFFC,0x00000000,0x3C08BF80,0x3508F400,
            0x3C09AA99,0x35296655,0x3C0A5566,0x354A99AA,
            0x3C0B0000,0x356B8000,0xAD090010,0xAD0A0010,
            0xAD0B0008,0x8CA8F400,0x31038000,0x1460FFFD,
            0x00000000,0x00000000,0x00000000,0x00000000,
            0x00000000,0x240B4000,0x3C0ABF81,0xAD4BF404,
            0x8CA9F400,0x03E00008,0x31222000,0x08000349,
            0x2404000E,0x00801821,0x3C02BF81,0x24040004,
            0xAC43F420,0x08000349,0x00000000,0x00801821,
            0x3C06BF81,0x3C02BF81,0x24040003,0xACC3F420,
            0xAC45F440,0x08000349,0x00000000,0x00801821,
            0x3C06BF81,0x3C02BF81,0x24040001,0xACC3F420,
            0xAC45F430,0x08000349,0x00000000,0x3C06BF81,
            0xACC4F420,0x24034003,0x3C04BF81,0x3C02BF81,
            0xAC85F440,0xAC43F400,0x40034800,0x40084800,
            0x01033823,0x2CE500F0,0x14A0FFFC,0x00000000,
            0x3C08BF80,0x3508F400,0x3C09AA99,0x35296655,
            0x3C0A5566,0x354A99AA,0x3C0B0000,0x356B8000,
            0xAD090010,0xAD0A0010,0xAD0B0008,0x03E00008,
            0x00001021,0x3C04BF81,0x8C83F400,0x30628000,
            0x1440FFFD,0x00000000,0x00000000,0x00000000,
            0x00000000,0x00000000,0x24074000,0x3C06BF81,
            0xACC7F404,0x8C85F400,0x03E00008,0x30A22000,
            0xB01164A1,0x32E06F00,0x67626D00,0x67C5EB31,
            0xEA6E6745,0x329E3442,0x67A434A4,0x46C13364,
            0x2204ED31,0x7401F022,0xED3167B8,0xEB315608,
            0x47E161EF,0x5700F100,0x4004D8A0,0x642161E4,
            0x6500E8A0,0xA0000008,0x671C64E3,0xF0106B00,
            0xF010D870,0x1800C894,0x650003AC,0xF0106A01,
            0x6463D850,0x6500E8A0,0xF01067BC,0xEC11AD74,
            0xEE3167C3,0xEC4E32C2,0x3688B705,0x9C40E6F1,
            0xEB4E3360,0xCD74F010,0x6500E8A0,0xA0000008,
            0x672464F4,0x2507450F,0x1800A180,0x400F03C6,
            0x4121EA0F,0x64742AF9,0x6500E8A0,0xF010675C,
            0xE8A0AA54,
            0xA0000C29,0xA0000C4D,0xA0000C59,0xA0000C1B,
            0xA0000C33,0xA0000C3B,0xA0000C79,0xA0000C47,
            0xA0000C87,0xA0000C69,0xA0000CAD,0xA0000CB1,
            0xA0000C97};

        public static void EnterSerialExecution()
        { // assumes already in programming mode
            int commOffSet = 0;

            byte[] commandArrayp = new byte[29];
            commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayp[commOffSet++] = 27;
            commandArrayp[commOffSet++] = KONST._JT2_SENDCMD;
            commandArrayp[commOffSet++] = 0x04;                 // MTAP_SW_MTAP
            commandArrayp[commOffSet++] = KONST._JT2_SENDCMD;
            commandArrayp[commOffSet++] = 0x07;                 // MTAP_COMMAND
            commandArrayp[commOffSet++] = KONST._JT2_XFERDATA8_LIT;
            commandArrayp[commOffSet++] = 0x00;                 // MCHP_STATUS
            commandArrayp[commOffSet++] = KONST._JT2_SENDCMD;
            commandArrayp[commOffSet++] = 0x04;                 // MTAP_SW_MTAP
            commandArrayp[commOffSet++] = KONST._JT2_SENDCMD;
            commandArrayp[commOffSet++] = 0x07;                 // MTAP_COMMAND
            commandArrayp[commOffSet++] = KONST._JT2_XFERDATA8_LIT;
            commandArrayp[commOffSet++] = 0xD1;                 // MCHP_ASSERT_RST
            commandArrayp[commOffSet++] = KONST._JT2_SENDCMD;
            commandArrayp[commOffSet++] = 0x05;                 // MTAP_SW_ETAP
            commandArrayp[commOffSet++] = KONST._JT2_SETMODE;
            commandArrayp[commOffSet++] = 6;
            commandArrayp[commOffSet++] = 0x1F;
            commandArrayp[commOffSet++] = KONST._JT2_SENDCMD;
            commandArrayp[commOffSet++] = 0x0C;                 // ETAP_EJTAGBOOT
            commandArrayp[commOffSet++] = KONST._JT2_SENDCMD;
            commandArrayp[commOffSet++] = 0x04;                 // MTAP_SW_MTAP       
            commandArrayp[commOffSet++] = KONST._JT2_SENDCMD;
            commandArrayp[commOffSet++] = 0x07;                 // MTAP_COMMAND
            commandArrayp[commOffSet++] = KONST._JT2_XFERDATA8_LIT;
            commandArrayp[commOffSet++] = 0xD0;                 // MCHP_DE_ASSERT_RST
            commandArrayp[commOffSet++] = KONST._JT2_XFERDATA8_LIT;
            commandArrayp[commOffSet++] = 0xFE;                 // MCHP_EN_FLASH            

            Pk2.writeUSB(commandArrayp);


            // Try turning on LED 
            /*commOffSet = 0;
            commandArrayp[commOffSet++] = KONST.CLR_DOWNLOAD_BUFFER;
            commandArrayp[commOffSet++] = KONST.DOWNLOAD_DATA;
            commandArrayp[commOffSet++] = 16;
            // STEP 1
            commOffSet = addInstruction(commandArrayp, 0x24020080, commOffSet);
            commOffSet = addInstruction(commandArrayp, 0x3c03bf88, commOffSet);
            commOffSet = addInstruction(commandArrayp, 0xac626028, commOffSet);
            commOffSet = addInstruction(commandArrayp, 0xac626004, commOffSet);
            // execute
            commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayp[commOffSet++] = 4;
            commandArrayp[commOffSet++] = KONST._JT2_XFERINST_BUF;
            commandArrayp[commOffSet++] = KONST._JT2_XFERINST_BUF;
            commandArrayp[commOffSet++] = KONST._JT2_XFERINST_BUF;
            commandArrayp[commOffSet++] = KONST._JT2_XFERINST_BUF;
            for (; commOffSet < 29; commOffSet++)
            {
                commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
            }
            Pk2.writeUSB(commandArrayp); */

        }

        public static bool DownloadPE()
        {
            // Serial execution mode must already be entered
            int commOffSet = 0;
            byte[] commandArrayp = new byte[64];
            commandArrayp[commOffSet++] = KONST.CLR_DOWNLOAD_BUFFER;
            commandArrayp[commOffSet++] = KONST.DOWNLOAD_DATA;
            commandArrayp[commOffSet++] = 28;
            // STEP 1
            commOffSet = addInstruction(commandArrayp, 0x3c04bf88, commOffSet);
            commOffSet = addInstruction(commandArrayp, 0x34842000, commOffSet);
            commOffSet = addInstruction(commandArrayp, 0x3c05001f, commOffSet);
            commOffSet = addInstruction(commandArrayp, 0x34a50040, commOffSet);
            commOffSet = addInstruction(commandArrayp, 0xac850000, commOffSet);
            // STEP 2
            commOffSet = addInstruction(commandArrayp, 0x34050800, commOffSet);
            commOffSet = addInstruction(commandArrayp, 0xac850010, commOffSet);
            // execute
            commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayp[commOffSet++] = 12;
            commandArrayp[commOffSet++] = KONST._JT2_SENDCMD;
            commandArrayp[commOffSet++] = 0x05;                 // MTAP_SW_ETAP
            commandArrayp[commOffSet++] = KONST._JT2_SETMODE;
            commandArrayp[commOffSet++] = 6;
            commandArrayp[commOffSet++] = 0x1F;
            commandArrayp[commOffSet++] = KONST._JT2_XFERINST_BUF;
            commandArrayp[commOffSet++] = KONST._JT2_XFERINST_BUF;
            commandArrayp[commOffSet++] = KONST._JT2_XFERINST_BUF;
            commandArrayp[commOffSet++] = KONST._JT2_XFERINST_BUF;
            commandArrayp[commOffSet++] = KONST._JT2_XFERINST_BUF;
            commandArrayp[commOffSet++] = KONST._JT2_XFERINST_BUF;
            commandArrayp[commOffSet++] = KONST._JT2_XFERINST_BUF;
            for (; commOffSet < 64; commOffSet++)
            {
                commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
            }
            Pk2.writeUSB(commandArrayp);
            if (Pk2.BusErrorCheck())    // Any timeouts?
            {
                return false;           // yes - abort
            }

            commOffSet = 0;
            commandArrayp[commOffSet++] = KONST.CLR_DOWNLOAD_BUFFER;
            commandArrayp[commOffSet++] = KONST.DOWNLOAD_DATA;
            commandArrayp[commOffSet++] = 20;
            // STEP 3
            commOffSet = addInstruction(commandArrayp, 0x34058000, commOffSet);
            commOffSet = addInstruction(commandArrayp, 0xac850020, commOffSet);
            commOffSet = addInstruction(commandArrayp, 0xac850030, commOffSet);
            // STEP 4
            commOffSet = addInstruction(commandArrayp, 0x3c04a000, commOffSet);
            commOffSet = addInstruction(commandArrayp, 0x34840800, commOffSet);
            // execute
            commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayp[commOffSet++] = 5;
            commandArrayp[commOffSet++] = KONST._JT2_XFERINST_BUF;
            commandArrayp[commOffSet++] = KONST._JT2_XFERINST_BUF;
            commandArrayp[commOffSet++] = KONST._JT2_XFERINST_BUF;
            commandArrayp[commOffSet++] = KONST._JT2_XFERINST_BUF;
            commandArrayp[commOffSet++] = KONST._JT2_XFERINST_BUF;
            for (; commOffSet < 64; commOffSet++)
            {
                commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
            }
            Pk2.writeUSB(commandArrayp);
            if (Pk2.BusErrorCheck())    // Any timeouts?
            {
                return false;           // yes - abort
            }

            // Download the PE loader
            for (int i = 0; i < pe_Loader.Length; i += 2)
            {
                commOffSet = 0;
                commandArrayp[commOffSet++] = KONST.CLR_DOWNLOAD_BUFFER;
                commandArrayp[commOffSet++] = KONST.DOWNLOAD_DATA;
                commandArrayp[commOffSet++] = 16;
                // STEP 5
                commOffSet = addInstruction(commandArrayp, (0x3c060000 | pe_Loader[i]), commOffSet);
                commOffSet = addInstruction(commandArrayp, (0x34c60000 | pe_Loader[i + 1]), commOffSet);
                commOffSet = addInstruction(commandArrayp, 0xac860000, commOffSet);
                commOffSet = addInstruction(commandArrayp, 0x24840004, commOffSet);
                // execute
                commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
                commandArrayp[commOffSet++] = 4;
                commandArrayp[commOffSet++] = KONST._JT2_XFERINST_BUF;
                commandArrayp[commOffSet++] = KONST._JT2_XFERINST_BUF;
                commandArrayp[commOffSet++] = KONST._JT2_XFERINST_BUF;
                commandArrayp[commOffSet++] = KONST._JT2_XFERINST_BUF;
                for (; commOffSet < 64; commOffSet++)
                {
                    commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
                }
                Pk2.writeUSB(commandArrayp);
                if (Pk2.BusErrorCheck())    // Any timeouts?
                {
                    return false;           // yes - abort
                }
            }

            // jump to PE loader
            commOffSet = 0;
            commandArrayp[commOffSet++] = KONST.CLR_DOWNLOAD_BUFFER;
            commandArrayp[commOffSet++] = KONST.DOWNLOAD_DATA;
            commandArrayp[commOffSet++] = 16;
            // STEP 6
            commOffSet = addInstruction(commandArrayp, 0x3c19a000, commOffSet);
            commOffSet = addInstruction(commandArrayp, 0x37390800, commOffSet);
            commOffSet = addInstruction(commandArrayp, 0x03200008, commOffSet);
            commOffSet = addInstruction(commandArrayp, 0x00000000, commOffSet);
            // execute
            commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayp[commOffSet++] = 21;
            commandArrayp[commOffSet++] = KONST._JT2_XFERINST_BUF;
            commandArrayp[commOffSet++] = KONST._JT2_XFERINST_BUF;
            commandArrayp[commOffSet++] = KONST._JT2_XFERINST_BUF;
            commandArrayp[commOffSet++] = KONST._JT2_XFERINST_BUF;
            // STEP 7-A
            commandArrayp[commOffSet++] = KONST._JT2_SENDCMD;
            commandArrayp[commOffSet++] = 0x05;                 // MTAP_SW_ETAP
            commandArrayp[commOffSet++] = KONST._JT2_SETMODE;
            commandArrayp[commOffSet++] = 6;
            commandArrayp[commOffSet++] = 0x1F;
            commandArrayp[commOffSet++] = KONST._JT2_SENDCMD;
            commandArrayp[commOffSet++] = 0x0E;                 // ETAP_FASTDATA
            commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_LIT;
            commandArrayp[commOffSet++] = 0x00;                 // PE_ADDRESS = 0xA000_0900
            commandArrayp[commOffSet++] = 0x09;
            commandArrayp[commOffSet++] = 0x00;
            commandArrayp[commOffSet++] = 0xA0;
            commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_LIT;
            commandArrayp[commOffSet++] = (byte)(PIC32_PE.Length & 0xFF);// PE_SIZE = PIC32_PE.Length
            commandArrayp[commOffSet++] = (byte)((PIC32_PE.Length >> 8) & 0xFF);
            commandArrayp[commOffSet++] = 0x00;
            commandArrayp[commOffSet++] = 0x00;

            for (; commOffSet < 64; commOffSet++)
            {
                commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
            }
            Pk2.writeUSB(commandArrayp);
            if (Pk2.BusErrorCheck())    // Any timeouts?
            {
                return false;           // yes - abort
            }

            // Download the PE itself (STEP 7-B)
            int numLoops = PIC32_PE.Length / 10;
            for (int i = 0, j = 0; i < numLoops; i++)
            { // download 10 at a time
                commOffSet = 0;
                commandArrayp[commOffSet++] = KONST.CLR_DOWNLOAD_BUFFER;
                commandArrayp[commOffSet++] = KONST.DOWNLOAD_DATA;
                commandArrayp[commOffSet++] = 40;
                // download the PE instructions
                j = i * 10;
                commOffSet = addInstruction(commandArrayp, PIC32_PE[j], commOffSet);
                commOffSet = addInstruction(commandArrayp, PIC32_PE[j + 1], commOffSet);
                commOffSet = addInstruction(commandArrayp, PIC32_PE[j + 2], commOffSet);
                commOffSet = addInstruction(commandArrayp, PIC32_PE[j + 3], commOffSet);
                commOffSet = addInstruction(commandArrayp, PIC32_PE[j + 4], commOffSet);
                commOffSet = addInstruction(commandArrayp, PIC32_PE[j + 5], commOffSet);
                commOffSet = addInstruction(commandArrayp, PIC32_PE[j + 6], commOffSet);
                commOffSet = addInstruction(commandArrayp, PIC32_PE[j + 7], commOffSet);
                commOffSet = addInstruction(commandArrayp, PIC32_PE[j + 8], commOffSet);
                commOffSet = addInstruction(commandArrayp, PIC32_PE[j + 9], commOffSet);
                // execute
                commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
                commandArrayp[commOffSet++] = 10;
                commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_BUF;
                commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_BUF;
                commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_BUF;
                commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_BUF;
                commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_BUF;
                commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_BUF;
                commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_BUF;
                commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_BUF;
                commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_BUF;
                commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_BUF;
                for (; commOffSet < 64; commOffSet++)
                {
                    commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
                }
                Pk2.writeUSB(commandArrayp);
                if (Pk2.BusErrorCheck())    // Any timeouts?
                {
                    return false;           // yes - abort
                }
            }
            // Download the remaining words
            Thread.Sleep(100);
            int arrayOffset = numLoops * 10;
            numLoops = PIC32_PE.Length % 10;
            if (numLoops > 0)
            {
                commOffSet = 0;
                commandArrayp[commOffSet++] = KONST.CLR_DOWNLOAD_BUFFER;
                commandArrayp[commOffSet++] = KONST.DOWNLOAD_DATA;
                commandArrayp[commOffSet++] = (byte)(4 * numLoops);
                // download the PE instructions
                for (int i = 0; i < numLoops; i++)
                {
                    commOffSet = addInstruction(commandArrayp, PIC32_PE[i + arrayOffset], commOffSet);
                }
                // execute
                commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
                commandArrayp[commOffSet++] = (byte)numLoops;
                for (int i = 0; i < numLoops; i++)
                {
                    commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_BUF;
                }
                for (; commOffSet < 64; commOffSet++)
                {
                    commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
                }
                Pk2.writeUSB(commandArrayp);
                if (Pk2.BusErrorCheck())    // Any timeouts?
                {
                    return false;           // yes - abort
                }
            }

            // STEP 8 - Jump to PE
            commOffSet = 0;
            commandArrayp[commOffSet++] = KONST.CLR_DOWNLOAD_BUFFER;
            commandArrayp[commOffSet++] = KONST.DOWNLOAD_DATA;
            commandArrayp[commOffSet++] = 8;
            // download the PE instructions
            commOffSet = addInstruction(commandArrayp, 0x00000000, commOffSet);
            commOffSet = addInstruction(commandArrayp, 0xDEAD0000, commOffSet);
            // execute
            commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayp[commOffSet++] = 2;
            commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_BUF;
            commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_BUF;
            for (; commOffSet < 64; commOffSet++)
            {
                commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
            }
            Pk2.writeUSB(commandArrayp);
            if (Pk2.BusErrorCheck())    // Any timeouts?
            {
                return false;           // yes - abort
            }
            Thread.Sleep(100);
            return true;
        }

        public static int ReadPEVersion()
        {
            byte[] commandArrayp = new byte[11];
            int commOffSet = 0;
            commandArrayp[commOffSet++] = KONST.CLR_UPLOAD_BUFFER;
            commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayp[commOffSet++] = 8;
            commandArrayp[commOffSet++] = KONST._JT2_SENDCMD;
            commandArrayp[commOffSet++] = 0x0E;                 // ETAP_FASTDATA
            commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_LIT;
            commandArrayp[commOffSet++] = 0x00;     // Length = 0
            commandArrayp[commOffSet++] = 0x00;
            commandArrayp[commOffSet++] = 0x07;     // EXEC_VERSION
            commandArrayp[commOffSet++] = 0x00;
            commandArrayp[commOffSet++] = KONST._JT2_GET_PE_RESP;
            Pk2.writeUSB(commandArrayp);
            if (Pk2.BusErrorCheck())    // Any timeouts?
            {
                return 0;           // yes - abort
            }
            if (!Pk2.UploadData())
            {
                return 0;
            }
            int version = (Pk2.Usb_read_array[4] + (Pk2.Usb_read_array[5] * 0x100));
            if (version != 0x0007) // command echo
            {
                return 0;
            }
            version = (Pk2.Usb_read_array[2] + (Pk2.Usb_read_array[3] * 0x100));
            return version;
        }

        public static bool PEBlankCheck(uint startAddress, uint lengthBytes)
        {
            byte[] commandArrayp = new byte[21];
            int commOffSet = 0;
            commandArrayp[commOffSet++] = KONST.CLR_UPLOAD_BUFFER;
            commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayp[commOffSet++] = 18;
            commandArrayp[commOffSet++] = KONST._JT2_SENDCMD;
            commandArrayp[commOffSet++] = 0x0E;                 // ETAP_FASTDATA
            commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_LIT;
            commandArrayp[commOffSet++] = 0x00;
            commandArrayp[commOffSet++] = 0x00;
            commandArrayp[commOffSet++] = 0x06;     // BLANK_CHECK
            commandArrayp[commOffSet++] = 0x00;
            commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_LIT;
            commandArrayp[commOffSet++] = (byte)(startAddress & 0xFF);
            commandArrayp[commOffSet++] = (byte)((startAddress >> 8) & 0xFF);
            commandArrayp[commOffSet++] = (byte)((startAddress >> 16) & 0xFF);
            commandArrayp[commOffSet++] = (byte)((startAddress >> 24) & 0xFF);
            commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_LIT;
            commandArrayp[commOffSet++] = (byte)(lengthBytes & 0xFF);
            commandArrayp[commOffSet++] = (byte)((lengthBytes >> 8) & 0xFF);
            commandArrayp[commOffSet++] = (byte)((lengthBytes >> 16) & 0xFF);
            commandArrayp[commOffSet++] = (byte)((lengthBytes >> 24) & 0xFF);
            commandArrayp[commOffSet++] = KONST._JT2_GET_PE_RESP;
            Pk2.writeUSB(commandArrayp);
            if (Pk2.BusErrorCheck())    // Any timeouts?
            {
                return false;           // yes - abort
            }
            if (!Pk2.UploadData())
            {
                return false;
            }
            if ((Pk2.Usb_read_array[4] != 6) || (Pk2.Usb_read_array[2] != 0)) // response code 0 = success
            {
                return false;
            }

            return true;
        }

        public static int PEGetCRC(uint startAddress, uint lengthBytes)
        {
            byte[] commandArrayp = new byte[20];
            int commOffSet = 0;
            commandArrayp[commOffSet++] = KONST.CLR_UPLOAD_BUFFER;
            commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayp[commOffSet++] = 17;
            commandArrayp[commOffSet++] = KONST._JT2_SENDCMD;
            commandArrayp[commOffSet++] = 0x0E;                 // ETAP_FASTDATA
            commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_LIT;
            commandArrayp[commOffSet++] = 0x00;
            commandArrayp[commOffSet++] = 0x00;
            commandArrayp[commOffSet++] = 0x08;     // GET_CRC
            commandArrayp[commOffSet++] = 0x00;
            commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_LIT;
            commandArrayp[commOffSet++] = (byte)(startAddress & 0xFF);
            commandArrayp[commOffSet++] = (byte)((startAddress >> 8) & 0xFF);
            commandArrayp[commOffSet++] = (byte)((startAddress >> 16) & 0xFF);
            commandArrayp[commOffSet++] = (byte)((startAddress >> 24) & 0xFF);
            commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_LIT;
            commandArrayp[commOffSet++] = (byte)(lengthBytes & 0xFF);
            commandArrayp[commOffSet++] = (byte)((lengthBytes >> 8) & 0xFF);
            commandArrayp[commOffSet++] = (byte)((lengthBytes >> 16) & 0xFF);
            commandArrayp[commOffSet++] = (byte)((lengthBytes >> 24) & 0xFF);
            Pk2.writeUSB(commandArrayp);

            // Hack to solve timeout with memory sizes > 128 kB
            byte[] commandArrayr = new byte[7];
            commOffSet = 0;
            commandArrayr[commOffSet++] = KONST.CLR_UPLOAD_BUFFER;
            commandArrayr[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayr[commOffSet++] = 4;
            commandArrayr[commOffSet++] = KONST._DELAY_LONG;
            commandArrayr[commOffSet++] = 0x0b; // 5.46ms*11 = 60,1 ms. Tests with PIC32MX170F256B showed that minimum needed 
                                                // delay is 10*long with PICkit2. With PICkit3, no delay is needed at all.
                                                // To be checked if 512 kB chips require longer delay.
                                                // 7.11.2022: Checked with PIC32MX470F512H, it is enough, with both pk2 and pk3.
                                                // But this chip uses different PE, still need to check with 1xx/2xx/5xx device.
                                                // There are 6 such devices in PIC32MX1XX/2XX/5XX 64/100 PIN FAMILY
            //commandArrayr[commOffSet++] = 0xFF; // 5.46ms*255 = 1.392s
            commandArrayr[commOffSet++] = KONST._JT2_GET_PE_RESP;
            commandArrayr[commOffSet++] = KONST._JT2_GET_PE_RESP;
            Pk2.writeUSB(commandArrayr);
            // End of delay hack

            // Orig code below, without delay
            //byte[] commandArrayr = new byte[5];
            //commOffSet = 0;
            //commandArrayr[commOffSet++] = KONST.CLR_UPLOAD_BUFFER;
            //commandArrayr[commOffSet++] = KONST.EXECUTE_SCRIPT;
            //commandArrayr[commOffSet++] = 2;
            //commandArrayr[commOffSet++] = KONST._JT2_GET_PE_RESP;
            //commandArrayr[commOffSet++] = KONST._JT2_GET_PE_RESP;
            //Pk2.writeUSB(commandArrayr);

            if (Pk2.BusErrorCheck())    // Any timeouts?
            {
                return 0;           // yes - abort
            }
            if (!Pk2.UploadData())
            {
                return 0;
            }
            if ((Pk2.Usb_read_array[4] != 8) || (Pk2.Usb_read_array[2] != 0)) // response code 0 = success
            {
                return 0;
            }

            int crc = (int)(Pk2.Usb_read_array[6] + (Pk2.Usb_read_array[7] << 8));

            return crc;
        }

        private static int addInstruction(byte[] commandarray, uint instruction, int offset)
        {
            commandarray[offset++] = (byte)(instruction & 0xFF);
            commandarray[offset++] = (byte)((instruction >> 8) & 0xFF);
            commandarray[offset++] = (byte)((instruction >> 16) & 0xFF);
            commandarray[offset++] = (byte)((instruction >> 24) & 0xFF);
            return offset;
        }

        public static bool PE_DownloadAndConnect()
        {
            // VDD must already be on!
            UpdateStatusWinText("Downloading Programming Executive...");

            //            if (Pk2.DevFile.PartsList[Pk2.ActivePart].PartName.IndexOf("TCHIP-USB-MX2") == 0 ||
            //                Pk2.DevFile.PartsList[Pk2.ActivePart].PartName.IndexOf("PIC32MX2") == 0 ||
            //                Pk2.DevFile.PartsList[Pk2.ActivePart].PartName.IndexOf("PIC32MX1") == 0)
            if ((Pk2.DevFile.PartsList[Pk2.ActivePart].ProgMemPanelBufs & 0xf0) == 0x20 )
            {
                pic32_PE_Version = pic32_PE_Version_RS32;
                PIC32_PE = PIC32_PE_RS32;
            }
            else
            {
                pic32_PE_Version = pic32_PE_Version_RS128;
                PIC32_PE = PIC32_PE_RS128;
            }

            Pk2.RunScript(KONST.PROG_ENTRY, 1);
            Pk2.UploadData();
            if ((Pk2.Usb_read_array[2] & 0x80) == 0)
            {
                UpdateStatusWinText("Device is Code Protected and must be Erased first.");
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                return false;
            }

            EnterSerialExecution();
            DownloadPE();
            int PEVersion = ReadPEVersion();
            if (PEVersion != pic32_PE_Version)
            {
                UpdateStatusWinText("Downloading Programming Executive...FAILED!");
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                return false;
            }
            return true;
        }

        public static bool PIC32Read()
        {
            Pk2.SetMCLRTemp(true);     // assert /MCLR to prevent code execution before programming mode entered.
            Pk2.VddOn();

            if (!PE_DownloadAndConnect())
            {
                return false;
            }

            string statusWinText = "Reading device:\n";
            UpdateStatusWinText(statusWinText);

            byte[] upload_buffer = new byte[KONST.UploadBufferSize];

            int progMemP32 = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem;
            int bootMemP32 = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].BootFlash;
            progMemP32 -= bootMemP32; // boot flash at upper end of prog mem.

            // Read Program Memory =====================================================================================         
            statusWinText += "Program Flash... ";
            UpdateStatusWinText(statusWinText);

            int bytesPerWord = Pk2.DevFile.Families[Pk2.GetActiveFamily()].BytesPerLocation;
            int scriptRunsToFillUpload = KONST.UploadBufferSize /
                (Pk2.DevFile.PartsList[Pk2.ActivePart].ProgMemRdWords * bytesPerWord);
            int wordsPerLoop = scriptRunsToFillUpload * Pk2.DevFile.PartsList[Pk2.ActivePart].ProgMemRdWords;
            int wordsRead = 0;

            ResetStatusBar(progMemP32 / wordsPerLoop);
            int uploadIndex = 0;
            do
            {
                // Download address for up to 15 script runs.
                int runs = (progMemP32 - wordsRead) / wordsPerLoop;
                if (runs > 15)
                    runs = 15;
                uint address = (uint)(wordsRead * bytesPerWord) + KONST.P32_PROGRAM_FLASH_START_ADDR;
                byte[] commandArrayp = new byte[3 + (runs * 4)];
                int commOffSet = 0;
                commandArrayp[commOffSet++] = KONST.CLR_DOWNLOAD_BUFFER;
                commandArrayp[commOffSet++] = KONST.DOWNLOAD_DATA;
                commandArrayp[commOffSet++] = (byte)(runs * 4);
                for (int i = 0; i < runs; i++)
                {
                    commOffSet = addInstruction(commandArrayp, (address + (uint)(i * Pk2.DevFile.PartsList[Pk2.ActivePart].ProgMemRdWords * bytesPerWord)), commOffSet);
                }
                Pk2.writeUSB(commandArrayp);

                for (int j = 0; j < runs; j++)
                {
                    //Pk2.RunScriptUploadNoLen2(KONST.PROGMEM_RD, scriptRunsToFillUpload);
                    Pk2.RunScriptUploadNoLen(KONST.PROGMEM_RD, scriptRunsToFillUpload);
                    Array.Copy(Pk2.Usb_read_array, 1, upload_buffer, 0, KONST.USB_REPORTLENGTH);
                    //Pk2.GetUpload();
                    Pk2.UploadDataNoLen();
                    Array.Copy(Pk2.Usb_read_array, 1, upload_buffer, KONST.USB_REPORTLENGTH, KONST.USB_REPORTLENGTH);
                    uploadIndex = 0;
                    for (int word = 0; word < wordsPerLoop; word++)
                    {
                        int bite = 0;
                        uint memWord = (uint)upload_buffer[uploadIndex + bite++];
                        if (bite < bytesPerWord)
                        {
                            memWord |= (uint)upload_buffer[uploadIndex + bite++] << 8;
                        }
                        if (bite < bytesPerWord)
                        {
                            memWord |= (uint)upload_buffer[uploadIndex + bite++] << 16;
                        }
                        if (bite < bytesPerWord)
                        {
                            memWord |= (uint)upload_buffer[uploadIndex + bite++] << 24;
                        }
                        uploadIndex += bite;
                        Pk2.DeviceBuffers.ProgramMemory[wordsRead++] = memWord;
                        if (wordsRead == progMemP32)
                        {
                            j = runs;
                            break; // for cases where ProgramMemSize%WordsPerLoop != 0
                        }
                    }
                    StepStatusBar();
                }
            } while (wordsRead < progMemP32);

            // Read Boot Memory ========================================================================================
            statusWinText += "Boot... ";
            UpdateStatusWinText(statusWinText);

            wordsRead = 0;

            ResetStatusBar(bootMemP32 / wordsPerLoop);

            do
            {
                // Download address.
                uint address = (uint)(wordsRead * bytesPerWord) + KONST.P32_BOOT_FLASH_START_ADDR;
                byte[] commandArrayp = new byte[3 + (scriptRunsToFillUpload * 4)];
                int commOffSet = 0;
                commandArrayp[commOffSet++] = KONST.CLR_DOWNLOAD_BUFFER;
                commandArrayp[commOffSet++] = KONST.DOWNLOAD_DATA;
                commandArrayp[commOffSet++] = (byte)(scriptRunsToFillUpload * 4);
                for (int i = 0; i < scriptRunsToFillUpload; i++)
                {
                    commOffSet = addInstruction(commandArrayp, address, commOffSet);
                }
                Pk2.writeUSB(commandArrayp);

                //Pk2.RunScriptUploadNoLen2(KONST.PROGMEM_RD, scriptRunsToFillUpload);
                Pk2.RunScriptUploadNoLen(KONST.PROGMEM_RD, scriptRunsToFillUpload);
                Array.Copy(Pk2.Usb_read_array, 1, upload_buffer, 0, KONST.USB_REPORTLENGTH);
                //Pk2.GetUpload();
                Pk2.UploadDataNoLen();
                Array.Copy(Pk2.Usb_read_array, 1, upload_buffer, KONST.USB_REPORTLENGTH, KONST.USB_REPORTLENGTH);
                uploadIndex = 0;
                for (int word = 0; word < wordsPerLoop; word++)
                {
                    int bite = 0;
                    uint memWord = (uint)upload_buffer[uploadIndex + bite++];
                    if (bite < bytesPerWord)
                    {
                        memWord |= (uint)upload_buffer[uploadIndex + bite++] << 8;
                    }
                    if (bite < bytesPerWord)
                    {
                        memWord |= (uint)upload_buffer[uploadIndex + bite++] << 16;
                    }
                    if (bite < bytesPerWord)
                    {
                        memWord |= (uint)upload_buffer[uploadIndex + bite++] << 24;
                    }
                    uploadIndex += bite;
                    Pk2.DeviceBuffers.ProgramMemory[progMemP32 + wordsRead++] = memWord;
                    if (wordsRead == bootMemP32)
                    {
                        break; // for cases where ProgramMemSize%WordsPerLoop != 0
                    }
                }
                StepStatusBar();
            } while (wordsRead < bootMemP32);

            // User ID Memory ========================================================================================
            statusWinText += "UserIDs... ";
            UpdateStatusWinText(statusWinText);
            // User IDs & Configs are in last block of boot mem
            Pk2.DeviceBuffers.UserIDs[0] = (uint)upload_buffer[uploadIndex];
            Pk2.DeviceBuffers.UserIDs[1] = (uint)upload_buffer[uploadIndex + 1];

            //timijk: quick fix for PIC32MX1xx/2xx
            //uploadIndex += bytesPerWord; 
            if (Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigAddr != Pk2.DevFile.PartsList[Pk2.ActivePart].UserIDAddr)
            {
                uploadIndex += bytesPerWord;
            }

            // Config Memory ========================================================================================
            statusWinText += "Config... ";
            UpdateStatusWinText(statusWinText);
            for (int cfg = 0; cfg < Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigWords; cfg++)
            {
                Pk2.DeviceBuffers.ConfigWords[cfg] = (uint)upload_buffer[uploadIndex++];
                Pk2.DeviceBuffers.ConfigWords[cfg] |= (uint)(upload_buffer[uploadIndex++] << 8);
            }

            statusWinText += "Done.";
            UpdateStatusWinText(statusWinText);

            Pk2.RunScript(KONST.PROG_EXIT, 1);

            return true; // success
        }

        public static bool PIC32BlankCheck()
        {
            Pk2.SetMCLRTemp(true);     // assert /MCLR to prevent code execution before programming mode entered.
            Pk2.VddOn();

            if (!PE_DownloadAndConnect())
            {
                return false;
            }

            string statusWinText = "Checking if Device is blank:\n";
            UpdateStatusWinText(statusWinText);

            int progMemP32 = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem;
            int bootMemP32 = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].BootFlash;
            progMemP32 -= bootMemP32; // boot flash at upper end of prog mem.
            int bytesPerWord = Pk2.DevFile.Families[Pk2.GetActiveFamily()].BytesPerLocation;

            // Check Program Memory ====================================================================================
            statusWinText += "Program Flash... ";
            UpdateStatusWinText(statusWinText);

            if (!PEBlankCheck(KONST.P32_PROGRAM_FLASH_START_ADDR, (uint)(progMemP32 * bytesPerWord)))
            {
                statusWinText = "Program Flash is not blank";
                UpdateStatusWinText(statusWinText);
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                return false;
            }

            // Check Boot Memory ====================================================================================
            statusWinText += "Boot Flash... ";
            UpdateStatusWinText(statusWinText);

            if (!PEBlankCheck(KONST.P32_BOOT_FLASH_START_ADDR, (uint)(bootMemP32 * bytesPerWord)))
            {
                statusWinText = "Boot Flash is not blank";
                UpdateStatusWinText(statusWinText);
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                return false;
            }

            // Check Config Memory ====================================================================================
            statusWinText += "UserID & Config... ";
            UpdateStatusWinText(statusWinText);

            if (!PEBlankCheck(KONST.P32_BOOT_FLASH_START_ADDR
                + (uint)(bootMemP32 * bytesPerWord), (uint)16))
            {
                statusWinText = "ID / Config Memory is not blank";
                UpdateStatusWinText(statusWinText);
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                return false;
            }

            Pk2.RunScript(KONST.PROG_EXIT, 1);

            statusWinText = "Device is Blank.";
            UpdateStatusWinText(statusWinText);

            return true;
        }

        public static bool P32Write(bool verifyWrite, bool codeProtect)
        {
            Pk2.SetMCLRTemp(true);     // assert /MCLR to prevent code execution before programming mode entered.
            Pk2.VddOn();

            // Erase device first
            Pk2.RunScript(KONST.PROG_ENTRY, 1);
            Pk2.RunScript(KONST.ERASE_CHIP, 1);

            if (!PE_DownloadAndConnect())
            {
                return false;
            }

            // Erase device first
            Pk2.RunScript(KONST.ERASE_CHIP, 1);

            string statusWinText = "Writing device:\n";
            UpdateStatusWinText(statusWinText);

            // Write Program Memory ====================================================================================
            statusWinText += "Program Flash... ";
            UpdateStatusWinText(statusWinText);

            int progMemP32 = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem;
            int bootMemP32 = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].BootFlash;
            progMemP32 -= bootMemP32; // boot flash at upper end of prog mem.

            // Write 512 bytes (128 words) per memory row - so need 2 downloads per row.
            // MX1xx, MX2xx: (32 words) per memory row - one download per row.
            // timijk
            int wordsPerLoop;
//            if (Pk2.DevFile.PartsList[Pk2.ActivePart].PartName.IndexOf("TCHIP-USB-MX2") == 0 ||
//                Pk2.DevFile.PartsList[Pk2.ActivePart].PartName.IndexOf("PIC32MX2") == 0 ||
//                Pk2.DevFile.PartsList[Pk2.ActivePart].PartName.IndexOf("PIC32MX1") == 0)
            if ((Pk2.DevFile.PartsList[Pk2.ActivePart].ProgMemPanelBufs & 0xf0) == 0x20)
                 { wordsPerLoop = 32; }
            else { wordsPerLoop = 128; }

            // First, find end of used Program Memory
            int endOfBuffer = Pk2.FindLastUsedInBuffer(Pk2.DeviceBuffers.ProgramMemory,
                                            Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue, progMemP32 - 1);
            // align end on next loop boundary                 
            int writes = (endOfBuffer + 1) / wordsPerLoop;
            if (((endOfBuffer + 1) % wordsPerLoop) > 0)
            {
                writes++;
            }
            if (writes < 2)
                writes = 2; // 1024 bytes min

            ResetStatusBar(endOfBuffer / wordsPerLoop);

            // Send PROGRAM command header
            //timijk 
            if (wordsPerLoop == 128) PEProgramHeader(KONST.P32_PROGRAM_FLASH_START_ADDR, (uint)(writes * 512));
            else PEProgramHeader(KONST.P32_PROGRAM_FLASH_START_ADDR, (uint)(writes * 128));

            // First block of data
            int index = 0;
            //timijk
            if (wordsPerLoop == 128) PEProgramSendBlock(index, false); // no response
            else PEProgramSendBlock2(index, false);

            writes--;
            StepStatusBar();
            
            do
            {
                index += wordsPerLoop;
                //timijk
                if (wordsPerLoop == 128) PEProgramSendBlock(index, true); // response
                else PEProgramSendBlock2(index, true); // response
                StepStatusBar();
            } while (--writes > 0);

            // get last response
            byte[] commandArrayp = new byte[4];
            int commOffSet = 0;
            commandArrayp[commOffSet++] = KONST.CLR_UPLOAD_BUFFER;
            commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayp[commOffSet++] = 1;
            commandArrayp[commOffSet++] = KONST._JT2_GET_PE_RESP;
            Pk2.writeUSB(commandArrayp);

            // Write Boot Memory ====================================================================================
            statusWinText += "Boot Flash... ";
            UpdateStatusWinText(statusWinText);

            // Write 512 bytes (128 words) per memory row - so need 2 downloads per row.
            // MX1xx, MX2xx: (32 words) per memory row - one download per row.
            // timijk
            //if (Pk2.DevFile.PartsList[Pk2.ActivePart].PartName.IndexOf("TCHIP-USB-MX2") == 0 ||
            //    Pk2.DevFile.PartsList[Pk2.ActivePart].PartName.IndexOf("PIC32MX2") == 0 ||
            //    Pk2.DevFile.PartsList[Pk2.ActivePart].PartName.IndexOf("PIC32MX1") == 0)
            if ((Pk2.DevFile.PartsList[Pk2.ActivePart].ProgMemPanelBufs & 0xf0) == 0x20)
            { wordsPerLoop = 32; }
            else { wordsPerLoop = 128; }

            // First, find end of used Program Memory
            endOfBuffer = Pk2.FindLastUsedInBuffer(Pk2.DeviceBuffers.ProgramMemory,
                                            Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue, Pk2.DeviceBuffers.ProgramMemory.Length - 1);
            if (endOfBuffer < progMemP32)
                endOfBuffer = 1;
            else
                endOfBuffer -= progMemP32;
            // align end on next loop boundary                 
            writes = (endOfBuffer + 1) / wordsPerLoop;
            if (((endOfBuffer + 1) % wordsPerLoop) > 0)
            {
                writes++;
            }
            if (writes < 2)
                writes = 2; // 1024 bytes min

            ResetStatusBar(endOfBuffer / wordsPerLoop);

            // Send PROGRAM command header
            //timijk
            if (wordsPerLoop == 128) PEProgramHeader(KONST.P32_BOOT_FLASH_START_ADDR, (uint)(writes * 512));
            else PEProgramHeader(KONST.P32_BOOT_FLASH_START_ADDR, (uint)(writes * 128));

            // First block of data
            index = progMemP32;
            //timijk
            if (wordsPerLoop == 128) PEProgramSendBlock(index, false); // no response
            else PEProgramSendBlock2(index, false);
            writes--;
            StepStatusBar();

            do
            {
                index += wordsPerLoop;
                //timijk
                if (wordsPerLoop == 128) PEProgramSendBlock(index, true); // response
                else PEProgramSendBlock2(index, true);
                StepStatusBar();
            } while (--writes > 0);

            // get last response
            Pk2.writeUSB(commandArrayp);

            // Write Config Memory ====================================================================================
            statusWinText += "UserID & Config... ";
            UpdateStatusWinText(statusWinText);

            uint[] cfgBuf = new uint[4];
            cfgBuf[0] = Pk2.DeviceBuffers.UserIDs[0] & 0xFF;
            cfgBuf[0] |= (Pk2.DeviceBuffers.UserIDs[1] & 0xFF) << 8;

            //timijk: quick fix for PIC32MX1xx/2xx
            if (Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigAddr != Pk2.DevFile.PartsList[Pk2.ActivePart].UserIDAddr)
            {

                cfgBuf[0] |= 0xFFFF0000;

                cfgBuf[1] = (Pk2.DeviceBuffers.ConfigWords[0] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[0]) | ((Pk2.DeviceBuffers.ConfigWords[1] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[1]) << 16);
                cfgBuf[1] |= (~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[0] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[0])
                            | ((~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[1] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[1]) << 16);
                cfgBuf[2] = (Pk2.DeviceBuffers.ConfigWords[2] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[2]) | ((Pk2.DeviceBuffers.ConfigWords[3] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[3]) << 16);
                cfgBuf[2] |= (~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[2] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[2])
                            | ((~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[3] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[3]) << 16);
                cfgBuf[3] = (Pk2.DeviceBuffers.ConfigWords[4] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[4]) | ((Pk2.DeviceBuffers.ConfigWords[5] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[5]) << 16);
                cfgBuf[3] |= (~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[4] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[4])
                            | ((~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[5] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[5]) << 16);

            }
            else
            {
                cfgBuf[0] |= ((Pk2.DeviceBuffers.ConfigWords[1] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[1]) << 16);
                cfgBuf[0] |= ((~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[1] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[1]) << 16);

                cfgBuf[1] = (Pk2.DeviceBuffers.ConfigWords[2] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[2]) | ((Pk2.DeviceBuffers.ConfigWords[3] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[3]) << 16);
                cfgBuf[1] |= (~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[2] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[2])
                            | ((~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[3] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[3]) << 16);

                cfgBuf[2] = (Pk2.DeviceBuffers.ConfigWords[4] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[4]) | ((Pk2.DeviceBuffers.ConfigWords[5] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[5]) << 16);
                cfgBuf[2] |= (~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[4] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[4])
                            | ((~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[5] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[5]) << 16);

                cfgBuf[3] = (Pk2.DeviceBuffers.ConfigWords[6] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[6]) | ((Pk2.DeviceBuffers.ConfigWords[7] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[7]) << 16);
                cfgBuf[3] |= (~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[6] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[6])
                            | ((~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[7] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[7]) << 16);

            }

            if (codeProtect)
            {
                cfgBuf[3] &= ~((uint)Pk2.DevFile.PartsList[Pk2.ActivePart].CPMask << 16);
            }

            uint startAddress = KONST.P32_BOOT_FLASH_START_ADDR + (uint)(bootMemP32 * 4);

            byte[] commandArrayc = new byte[39];
            commOffSet = 0;
            commandArrayc[commOffSet++] = KONST.CLR_UPLOAD_BUFFER;
            commandArrayc[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayc[commOffSet++] = 36;
            commandArrayc[commOffSet++] = KONST._JT2_SENDCMD;
            commandArrayc[commOffSet++] = 0x0E;                 // ETAP_FASTDATA
            commandArrayc[commOffSet++] = KONST._JT2_XFRFASTDAT_LIT;
            commandArrayc[commOffSet++] = 0x00;
            commandArrayc[commOffSet++] = 0x00;
            commandArrayc[commOffSet++] = 0x03;     // WORD_PROGRAM
            commandArrayc[commOffSet++] = 0x00;
            commandArrayc[commOffSet++] = KONST._JT2_XFRFASTDAT_LIT;
            commandArrayc[commOffSet++] = (byte)(startAddress & 0xFF);
            commandArrayc[commOffSet++] = (byte)((startAddress >> 8) & 0xFF);
            commandArrayc[commOffSet++] = (byte)((startAddress >> 16) & 0xFF);
            commandArrayc[commOffSet++] = (byte)((startAddress >> 24) & 0xFF);
            commandArrayc[commOffSet++] = KONST._JT2_XFRFASTDAT_LIT;
            commandArrayc[commOffSet++] = (byte)(cfgBuf[0] & 0xFF);
            commandArrayc[commOffSet++] = (byte)((cfgBuf[0] >> 8) & 0xFF);
            commandArrayc[commOffSet++] = (byte)((cfgBuf[0] >> 16) & 0xFF);
            commandArrayc[commOffSet++] = (byte)((cfgBuf[0] >> 24) & 0xFF);
            commandArrayc[commOffSet++] = KONST._JT2_WAIT_PE_RESP;
            startAddress += 4;
            commandArrayc[commOffSet++] = KONST._JT2_SENDCMD;
            commandArrayc[commOffSet++] = 0x0E;                 // ETAP_FASTDATA
            commandArrayc[commOffSet++] = KONST._JT2_XFRFASTDAT_LIT;
            commandArrayc[commOffSet++] = 0x00;
            commandArrayc[commOffSet++] = 0x00;
            commandArrayc[commOffSet++] = 0x03;     // WORD_PROGRAM
            commandArrayc[commOffSet++] = 0x00;
            commandArrayc[commOffSet++] = KONST._JT2_XFRFASTDAT_LIT;
            commandArrayc[commOffSet++] = (byte)(startAddress & 0xFF);
            commandArrayc[commOffSet++] = (byte)((startAddress >> 8) & 0xFF);
            commandArrayc[commOffSet++] = (byte)((startAddress >> 16) & 0xFF);
            commandArrayc[commOffSet++] = (byte)((startAddress >> 24) & 0xFF);
            commandArrayc[commOffSet++] = KONST._JT2_XFRFASTDAT_LIT;
            commandArrayc[commOffSet++] = (byte)(cfgBuf[1] & 0xFF);
            commandArrayc[commOffSet++] = (byte)((cfgBuf[1] >> 8) & 0xFF);
            commandArrayc[commOffSet++] = (byte)((cfgBuf[1] >> 16) & 0xFF);
            commandArrayc[commOffSet++] = (byte)((cfgBuf[1] >> 24) & 0xFF);
            commandArrayc[commOffSet++] = KONST._JT2_WAIT_PE_RESP;
            startAddress += 4;
            Pk2.writeUSB(commandArrayc);

            commOffSet = 0;
            commandArrayc[commOffSet++] = KONST.CLR_UPLOAD_BUFFER;
            commandArrayc[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayc[commOffSet++] = 36;
            commandArrayc[commOffSet++] = KONST._JT2_SENDCMD;
            commandArrayc[commOffSet++] = 0x0E;                 // ETAP_FASTDATA
            commandArrayc[commOffSet++] = KONST._JT2_XFRFASTDAT_LIT;
            commandArrayc[commOffSet++] = 0x00;
            commandArrayc[commOffSet++] = 0x00;
            commandArrayc[commOffSet++] = 0x03;     // WORD_PROGRAM
            commandArrayc[commOffSet++] = 0x00;
            commandArrayc[commOffSet++] = KONST._JT2_XFRFASTDAT_LIT;
            commandArrayc[commOffSet++] = (byte)(startAddress & 0xFF);
            commandArrayc[commOffSet++] = (byte)((startAddress >> 8) & 0xFF);
            commandArrayc[commOffSet++] = (byte)((startAddress >> 16) & 0xFF);
            commandArrayc[commOffSet++] = (byte)((startAddress >> 24) & 0xFF);
            commandArrayc[commOffSet++] = KONST._JT2_XFRFASTDAT_LIT;
            commandArrayc[commOffSet++] = (byte)(cfgBuf[2] & 0xFF);
            commandArrayc[commOffSet++] = (byte)((cfgBuf[2] >> 8) & 0xFF);
            commandArrayc[commOffSet++] = (byte)((cfgBuf[2] >> 16) & 0xFF);
            commandArrayc[commOffSet++] = (byte)((cfgBuf[2] >> 24) & 0xFF);
            commandArrayc[commOffSet++] = KONST._JT2_WAIT_PE_RESP;
            startAddress += 4;
            commandArrayc[commOffSet++] = KONST._JT2_SENDCMD;
            commandArrayc[commOffSet++] = 0x0E;                 // ETAP_FASTDATA
            commandArrayc[commOffSet++] = KONST._JT2_XFRFASTDAT_LIT;
            commandArrayc[commOffSet++] = 0x00;
            commandArrayc[commOffSet++] = 0x00;
            commandArrayc[commOffSet++] = 0x03;     // WORD_PROGRAM
            commandArrayc[commOffSet++] = 0x00;
            commandArrayc[commOffSet++] = KONST._JT2_XFRFASTDAT_LIT;
            commandArrayc[commOffSet++] = (byte)(startAddress & 0xFF);
            commandArrayc[commOffSet++] = (byte)((startAddress >> 8) & 0xFF);
            commandArrayc[commOffSet++] = (byte)((startAddress >> 16) & 0xFF);
            commandArrayc[commOffSet++] = (byte)((startAddress >> 24) & 0xFF);
            commandArrayc[commOffSet++] = KONST._JT2_XFRFASTDAT_LIT;
            commandArrayc[commOffSet++] = (byte)(cfgBuf[3] & 0xFF);
            commandArrayc[commOffSet++] = (byte)((cfgBuf[3] >> 8) & 0xFF);
            commandArrayc[commOffSet++] = (byte)((cfgBuf[3] >> 16) & 0xFF);
            commandArrayc[commOffSet++] = (byte)((cfgBuf[3] >> 24) & 0xFF);
            commandArrayc[commOffSet++] = KONST._JT2_WAIT_PE_RESP;
            startAddress += 4;
            Pk2.writeUSB(commandArrayc);

            if (verifyWrite)
            {
                return P32Verify(true, codeProtect);
            }

            Pk2.RunScript(KONST.PROG_EXIT, 1);

            return true;
        }

        private static void PEProgramHeader(uint startAddress, uint lengthBytes)
        {
            byte[] commandArrayp = new byte[20];
            int commOffSet = 0;
            commandArrayp[commOffSet++] = KONST.CLR_UPLOAD_BUFFER;
            commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayp[commOffSet++] = 17;
            commandArrayp[commOffSet++] = KONST._JT2_SENDCMD;
            commandArrayp[commOffSet++] = 0x0E;                 // ETAP_FASTDATA
            commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_LIT;
            commandArrayp[commOffSet++] = 0x00;
            commandArrayp[commOffSet++] = 0x00;
            commandArrayp[commOffSet++] = 0x02;     // PROGRAM
            commandArrayp[commOffSet++] = 0x00;
            commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_LIT;
            commandArrayp[commOffSet++] = (byte)(startAddress & 0xFF);
            commandArrayp[commOffSet++] = (byte)((startAddress >> 8) & 0xFF);
            commandArrayp[commOffSet++] = (byte)((startAddress >> 16) & 0xFF);
            commandArrayp[commOffSet++] = (byte)((startAddress >> 24) & 0xFF);
            commandArrayp[commOffSet++] = KONST._JT2_XFRFASTDAT_LIT;
            commandArrayp[commOffSet++] = (byte)(lengthBytes & 0xFF);
            commandArrayp[commOffSet++] = (byte)((lengthBytes >> 8) & 0xFF);
            commandArrayp[commOffSet++] = (byte)((lengthBytes >> 16) & 0xFF);
            commandArrayp[commOffSet++] = (byte)((lengthBytes >> 24) & 0xFF);
            Pk2.writeUSB(commandArrayp);
        }

        private static void PEProgramSendBlock(int index, bool peResp)
        { // Assumes DL buffer is 256!
            byte[] downloadBuffer = new byte[256];
            uint memWord = 0;
            int dnldIndex = 0;
            int memMax = Pk2.DeviceBuffers.ProgramMemory.Length;

            // first half
            for (int i = 0; i < 64; i++)
            {
                if (index < memMax)
                    memWord = Pk2.DeviceBuffers.ProgramMemory[index++];
                else
                    memWord = 0xFFFFFFFF;
                downloadBuffer[dnldIndex++] = (byte)(memWord & 0xFF);
                downloadBuffer[dnldIndex++] = (byte)((memWord >> 8) & 0xFF);
                downloadBuffer[dnldIndex++] = (byte)((memWord >> 16) & 0xFF);
                downloadBuffer[dnldIndex++] = (byte)((memWord >> 24) & 0xFF);
            }
            // Download first half of block
            int dataIndex = Pk2.DataClrAndDownload(downloadBuffer, 0);
            while ((dnldIndex - dataIndex) > 62) // Pk2.DataDownload send 62 bytes per call
            {
                dataIndex = Pk2.DataDownload(downloadBuffer, dataIndex, downloadBuffer.Length);
            }
            // send rest of data with script cmd
            int length = dnldIndex - dataIndex;
            byte[] commandArray = new byte[5 + length];
            int commOffset = 0;
            commandArray[commOffset++] = KONST.DOWNLOAD_DATA;
            commandArray[commOffset++] = (byte)(length & 0xFF);
            for (int i = 0; i < length; i++)
            {
                commandArray[commOffset++] = downloadBuffer[dataIndex + i];
            }
            commandArray[commOffset++] = KONST.RUN_SCRIPT;
            commandArray[commOffset++] = KONST.PROGMEM_WR_PREP; // should not be remapped
            commandArray[commOffset++] = 1; // once
            Pk2.writeUSB(commandArray);

            // 2nd half
            dnldIndex = 0;
            for (int i = 0; i < 64; i++)
            {
                if (index < memMax)
                    memWord = Pk2.DeviceBuffers.ProgramMemory[index++];
                else
                    memWord = 0xFFFFFFFF;
                downloadBuffer[dnldIndex++] = (byte)(memWord & 0xFF);
                downloadBuffer[dnldIndex++] = (byte)((memWord >> 8) & 0xFF);
                downloadBuffer[dnldIndex++] = (byte)((memWord >> 16) & 0xFF);
                downloadBuffer[dnldIndex++] = (byte)((memWord >> 24) & 0xFF);
            }
            // Download 2nd half of block
            dataIndex = Pk2.DataClrAndDownload(downloadBuffer, 0);
            while ((dnldIndex - dataIndex) > 62) // Pk2.DataDownload send 62 bytes per call
            {
                dataIndex = Pk2.DataDownload(downloadBuffer, dataIndex, downloadBuffer.Length);
            }
            // send rest of data with script cmd
            length = dnldIndex - dataIndex;
            commOffset = 0;
            commandArray[commOffset++] = KONST.DOWNLOAD_DATA;
            commandArray[commOffset++] = (byte)(length & 0xFF);
            for (int i = 0; i < length; i++)
            {
                commandArray[commOffset++] = downloadBuffer[dataIndex + i];
            }
            commandArray[commOffset++] = KONST.RUN_SCRIPT;
            if (peResp)
                commandArray[commOffset++] = KONST.PROGMEM_WR; // should not be remapped
            else
                commandArray[commOffset++] = KONST.PROGMEM_WR_PREP; // should not be remapped
            commandArray[commOffset++] = 1; // once
            Pk2.writeUSB(commandArray);

        }

        private static void PEProgramSendBlock2(int index, bool peResp)
        {   //timijk Set dowdloadBuffer to only 128!
            byte[] downloadBuffer = new byte[128];
            uint memWord = 0;
            int dnldIndex = 0;
            int memMax = Pk2.DeviceBuffers.ProgramMemory.Length;

            // first half
            for (int i = 0; i < 32; i++)
            {
                if (index < memMax)
                    memWord = Pk2.DeviceBuffers.ProgramMemory[index++];
                else
                    memWord = 0xFFFFFFFF;
                downloadBuffer[dnldIndex++] = (byte)(memWord & 0xFF);
                downloadBuffer[dnldIndex++] = (byte)((memWord >> 8) & 0xFF);
                downloadBuffer[dnldIndex++] = (byte)((memWord >> 16) & 0xFF);
                downloadBuffer[dnldIndex++] = (byte)((memWord >> 24) & 0xFF);
            }
            // Download first half of block
            int dataIndex = Pk2.DataClrAndDownload(downloadBuffer, 0);
            while ((dnldIndex - dataIndex) > 62) // Pk2.DataDownload send 62 bytes per call
            {
                dataIndex = Pk2.DataDownload(downloadBuffer, dataIndex, downloadBuffer.Length);
            }
            // send rest of data with script cmd
            int length = dnldIndex - dataIndex;

            byte[] commandArray;

            if (peResp) commandArray = new byte[2 + length + 9];
            else commandArray = new byte[2 + length + 8];

            int commOffset = 0;
            commandArray[commOffset++] = KONST.DOWNLOAD_DATA;
            commandArray[commOffset++] = (byte)(length & 0xFF);
            for (int i = 0; i < length; i++)
            {
                commandArray[commOffset++] = downloadBuffer[dataIndex + i];
            }

            //timijk
            commandArray[commOffset++] = KONST.EXECUTE_SCRIPT;
            if (peResp) commandArray[commOffset++] = 7;
            else commandArray[commOffset++] = 6;  //bytes of script
            commandArray[commOffset++] = KONST._JT2_SENDCMD;
            commandArray[commOffset++] = 0x0E;    //ETAP_FASTDATA
            commandArray[commOffset++] = KONST._JT2_XFRFASTDAT_BUF;
            commandArray[commOffset++] = KONST._LOOP;  //Loop _JT2_XFRFASTDAT_BUF
            commandArray[commOffset++] = 0x01;
            commandArray[commOffset++] = 0x1F;   //total 0x20 * 4 bytes = 128 bytes

            if (peResp)
            {
                commandArray[commOffset++] = 0xB3; // KONST.JT2_PE_PROG_RESP;
            }

            Pk2.writeUSB(commandArray);

        }

        public static bool P32Verify(bool writeVerify, bool codeProtect)
        {
            if (!writeVerify)
            { // not necessary on post-program verify
                Pk2.SetMCLRTemp(true);     // assert /MCLR to prevent code execution before programming mode entered.
                Pk2.VddOn();

                if (!PE_DownloadAndConnect())
                {
                    return false;
                }
            }

            string statusWinText = "Verifying Device:\n";
            UpdateStatusWinText(statusWinText);

            int progMemP32 = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem;
            int bootMemP32 = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].BootFlash;
            progMemP32 -= bootMemP32; // boot flash at upper end of prog mem.
            int bytesPerWord = Pk2.DevFile.Families[Pk2.GetActiveFamily()].BytesPerLocation;

            // Verify Program Memory ====================================================================================
            statusWinText += "Program Flash... ";
            UpdateStatusWinText(statusWinText);

            int bufferCRC = p32CRC_buf(Pk2.DeviceBuffers.ProgramMemory, 0, (uint)progMemP32);

            int deviceCRC = PEGetCRC(KONST.P32_PROGRAM_FLASH_START_ADDR, (uint)(progMemP32 * bytesPerWord));

            if (bufferCRC != deviceCRC)
            {
                if (writeVerify)
                {
                    statusWinText = "Programming Program Flash Failed.";
                    UpdateStatusWinText(statusWinText);
                }
                else
                {
                    statusWinText = "Verify of Program Flash Failed.";
                    UpdateStatusWinText(statusWinText);
                }

                Pk2.RunScript(KONST.PROG_EXIT, 1);
                return false;
            }

            // Verify Boot Memory ====================================================================================
            statusWinText += "Boot Flash... ";
            UpdateStatusWinText(statusWinText);

            bufferCRC = p32CRC_buf(Pk2.DeviceBuffers.ProgramMemory, (uint)progMemP32, (uint)bootMemP32);

            deviceCRC = PEGetCRC(KONST.P32_BOOT_FLASH_START_ADDR, (uint)(bootMemP32 * bytesPerWord));

            if (bufferCRC != deviceCRC)
            {
                if (writeVerify)
                {
                    statusWinText = "Programming Boot Flash Failed.";
                    UpdateStatusWinText(statusWinText);
                }
                else
                {
                    statusWinText = "Verify of Boot Flash Failed.";
                    UpdateStatusWinText(statusWinText);
                }

                Pk2.RunScript(KONST.PROG_EXIT, 1);
                return false;
            }

            // Verify Config Memory ====================================================================================
            statusWinText += "ID/Config Flash... ";
            UpdateStatusWinText(statusWinText);

            uint[] cfgBuf = new uint[4];
            cfgBuf[0] = Pk2.DeviceBuffers.UserIDs[0] & 0xFF;
            cfgBuf[0] |= (Pk2.DeviceBuffers.UserIDs[1] & 0xFF) << 8;

            //timijk: quick fix for PIC32MX1xx/2xx
            if (Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigAddr != Pk2.DevFile.PartsList[Pk2.ActivePart].UserIDAddr)
            {

                cfgBuf[0] |= 0xFFFF0000;

                cfgBuf[1] = (Pk2.DeviceBuffers.ConfigWords[0] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[0]) | ((Pk2.DeviceBuffers.ConfigWords[1] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[1]) << 16);
                cfgBuf[1] |= (~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[0] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[0])
                            | ((~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[1] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[1]) << 16);
                cfgBuf[2] = (Pk2.DeviceBuffers.ConfigWords[2] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[2]) | ((Pk2.DeviceBuffers.ConfigWords[3] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[3]) << 16);
                cfgBuf[2] |= (~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[2] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[2])
                            | ((~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[3] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[3]) << 16);
                cfgBuf[3] = (Pk2.DeviceBuffers.ConfigWords[4] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[4]) | ((Pk2.DeviceBuffers.ConfigWords[5] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[5]) << 16);
                cfgBuf[3] |= (~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[4] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[4])
                            | ((~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[5] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[5]) << 16);

            }
            else
            {
                cfgBuf[0] |= ((Pk2.DeviceBuffers.ConfigWords[1] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[1]) << 16);
                cfgBuf[0] |= ((~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[1] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[1]) << 16);

                cfgBuf[1] = (Pk2.DeviceBuffers.ConfigWords[2] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[2]) | ((Pk2.DeviceBuffers.ConfigWords[3] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[3]) << 16);
                cfgBuf[1] |= (~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[2] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[2])
                            | ((~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[3] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[3]) << 16);

                cfgBuf[2] = (Pk2.DeviceBuffers.ConfigWords[4] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[4]) | ((Pk2.DeviceBuffers.ConfigWords[5] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[5]) << 16);
                cfgBuf[2] |= (~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[4] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[4])
                            | ((~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[5] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[5]) << 16);

                cfgBuf[3] = (Pk2.DeviceBuffers.ConfigWords[6] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[6]) | ((Pk2.DeviceBuffers.ConfigWords[7] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[7]) << 16);
                cfgBuf[3] |= (~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[6] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[6])
                            | ((~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[7] & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[7]) << 16);

            }

            if (codeProtect)
            {
                cfgBuf[3] &= ~((uint)Pk2.DevFile.PartsList[Pk2.ActivePart].CPMask << 16);
            }

            bufferCRC = p32CRC_buf(cfgBuf, (uint)0, (uint)4);

            deviceCRC = PEGetCRC(KONST.P32_BOOT_FLASH_START_ADDR + (uint)(bootMemP32 * bytesPerWord), (uint)(4 * bytesPerWord));

            if (bufferCRC != deviceCRC)
            {
                if (writeVerify)
                {
                    statusWinText = "Programming ID/Config Flash Failed.";
                    UpdateStatusWinText(statusWinText);
                }
                else
                {
                    statusWinText = "Verify of ID/Config Flash Failed.";
                    UpdateStatusWinText(statusWinText);
                }

                Pk2.RunScript(KONST.PROG_EXIT, 1);
                return false;
            }

            if (!writeVerify)
            {
                statusWinText = "Verification Successful.\n";
                UpdateStatusWinText(statusWinText);
            }
            else
            {
                statusWinText = "Programming Successful.\n";
                UpdateStatusWinText(statusWinText);
            }
            Pk2.RunScript(KONST.PROG_EXIT, 1);
            return true;
        }

        /*private static int p32CRC(uint A1, uint L1)
        {
            uint CRC_POLY = 0x11021;
            uint CRC_SEED = 0xFFFF; //0x84CF;
            
            uint A, B1, B2;
            uint CurByte;
            uint CurCRC = CRC_SEED;
            uint CurCRCHighBit;
            uint CurWord;

            uint bytesPerWord = (uint)Pk2.DevFile.Families[Pk2.GetActiveFamily()].BytesPerLocation;
            uint progMemP32 = Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem;
            uint bootMemP32 = Pk2.DevFile.PartsList[Pk2.ActivePart].BootFlash;
            progMemP32 -= bootMemP32; // boot flash at upper end of prog mem.
            
            if (A1 >= KONST.P32_BOOT_FLASH_START_ADDR)
            { // boot flash
                // starting index
                A1 = progMemP32 + ((A1 - KONST.P32_BOOT_FLASH_START_ADDR) / bytesPerWord);
            }
            else
            { // program flash
                // starting index
                A1 = (A1 - KONST.P32_PROGRAM_FLASH_START_ADDR) / bytesPerWord;
            }
            L1 /= bytesPerWord; // L1 in words

            // Loop through entire address range
            for (A = A1; A < L1; A++)
            {
                CurWord = Pk2.DeviceBuffers.ProgramMemory[A];

                // Process each byte in this word
                for (B1 = 0; B1 < bytesPerWord; B1++)
                {
                    CurByte = (CurWord & 0xFF) << 8;
                    CurWord >>= 8;

                    // Process each bit in this byte
                    for (B2 = 0; B2 < 8; B2++)
                    {
                        CurCRCHighBit = (CurCRC ^ CurByte) & 0x8000;
                        CurCRC <<= 1;
                        //CurCRC |= (CurByte >> 7) & 0x1;
                        CurByte <<= 1;
                        if (CurCRCHighBit > 0)
                            CurCRC ^= CRC_POLY;
                    }
                }
            }
            return (int)(CurCRC & 0xFFFF);

        }            */

        private static int p32CRC_buf(uint[] buffer, uint startIdx, uint len)
        {
            uint CRC_POLY = 0x11021;
            uint CRC_SEED = 0xFFFF; //0x84CF;

            uint A, B1, B2;
            uint CurByte;
            uint CurCRC = CRC_SEED;
            uint CurCRCHighBit;
            uint CurWord;

            uint bytesPerWord = (uint)Pk2.DevFile.Families[Pk2.GetActiveFamily()].BytesPerLocation;

            uint L1 = (uint)buffer.Length; ; // L1 in words

            // Loop through entire address range
            for (A = startIdx; A < (startIdx + len); A++)
            {
                CurWord = buffer[A];

                // Process each byte in this word
                for (B1 = 0; B1 < bytesPerWord; B1++)
                {
                    CurByte = (CurWord & 0xFF) << 8;
                    CurWord >>= 8;

                    // Process each bit in this byte
                    for (B2 = 0; B2 < 8; B2++)
                    {
                        CurCRCHighBit = (CurCRC ^ CurByte) & 0x8000;
                        CurCRC <<= 1;
                        //CurCRC |= (CurByte >> 7) & 0x1;
                        CurByte <<= 1;
                        if (CurCRCHighBit > 0)
                            CurCRC ^= CRC_POLY;
                    }
                }
            }

            return (int)(CurCRC & 0xFFFF);

        }

    }
}
