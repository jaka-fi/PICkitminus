using System;
using System.Collections.Generic;
using System.Text;

namespace PICkit2V2
{
    public class Utilities
    {
        public static int Convert_Value_To_Int(string p_value)
        {
            // takes strings that represent numeric values and converts
            // them to a four byte int
            // assumes hex values are of form: 0x1234, or 0X1234
            // assumes binary values are of form 0b1234, or 0B1234
            //
            uint[] l_binary_adder = {0, 0, 0x80000000, 0x40000000, 0x20000000, 0x10000000, 0x8000000, 0x4000000, 0x2000000, 0x1000000, 
                                          0x800000,   0x400000,   0x200000,   0x100000,   0x80000,   0x40000,   0x20000,   0x10000, 
                                          0x8000,     0x4000,     0x2000,     0x1000,     0x800,     0x400,     0x200,     0x100, 
                                          0x80,       0x40,       0x20,       0x10,       0x8,       0x4,       0x2,       0x1};
            uint[] l_hex_adder = { 0, 0, 0x10000000, 0x1000000, 0x100000, 0x10000, 0x1000, 0x100, 0x10, 0x1 };
            int l_return_value = 0;
            int l_mult;
            int l_end_pos, l_start_pos;
            int l_x;

            if (p_value[0] == 0)
            {
                l_return_value = 0;
            }
            else if (p_value[0] == 'Y' || p_value[0] == 'y')
            {
                // boolean TRUE
                l_return_value = 1;
            }
            else if (p_value[0] == 'N' || p_value[0] == 'n')
            {
                // boolean FALSE
                l_return_value = 0;
            }
            else if (p_value.Length > 1)
            {
                if ((p_value[0] == '0' && (p_value[1] == 'b' || p_value[1] == 'B')) ||
                     (p_value[0] == 'b' || p_value[0] == 'B'))
                {
                    // binary
                    l_end_pos = p_value.Length - 1;
                    if (p_value[0] == '0')
                    {
                        // must change starting point for tokens that look like 0b10101010
                        l_start_pos = 2;
                    }
                    else
                    {
                        l_start_pos = 1;
                    }
                    for (l_x = l_start_pos; l_x <= l_end_pos; l_x++)
                    {
                        if (p_value[l_x] == '1')
                        {
                            l_mult = 1;
                        }
                        else
                        {
                            l_mult = 0;
                        }
                        l_return_value += (int)l_binary_adder[l_x + 34 - p_value.Length] * l_mult;
                    } // end for
                }
                else if (p_value[0] == '0' && (p_value[1] == 'x' || p_value[1] == 'X'))
                {
                    // hex
                    // 6 bits
                    l_end_pos = p_value.Length - 1;
                    for (l_x = 2; l_x <= l_end_pos; l_x++)
                    {
                        switch (p_value[l_x])
                        {

                            case 'A':
                            case 'a':
                                l_mult = 10;
                                break;
                            case 'B':
                            case 'b':
                                l_mult = 11;
                                break;
                            case 'C':
                            case 'c':
                                l_mult = 12;
                                break;
                            case 'D':
                            case 'd':
                                l_mult = 13;
                                break;
                            case 'E':
                            case 'e':
                                l_mult = 14;
                                break;
                            case 'F':
                            case 'f':
                                l_mult = 15;
                                break;
                            default:
                                string l_temp = p_value[l_x].ToString();
                                if (!int.TryParse(l_temp, out l_mult))
                                {
                                    l_mult = 0;
                                }
                                break;
                        }  // end switch
                        l_return_value += (int)l_hex_adder[l_x + 10 - p_value.Length] * l_mult;
                    } // end for
                }
                else
                {
                    // decimal
                    if (!int.TryParse(p_value, out l_return_value))
                    {
                        // use tryparse cause just parse will exception on non-numeric values
                        l_return_value = 0;
                    }
                }
            }
            else
            {
                // decimal
                if (!int.TryParse(p_value, out l_return_value))
                {
                    // use tryparse cause just parse will exception on non-numeric values
                    l_return_value = 0;
                }
            }

            return l_return_value;
        }  // end convert_value_to_int

        public static String ConvertIntASCII(int toConvert, int numBytes)
        {


            byte[] convertArray = new byte[numBytes];
            for (int i = numBytes; i > 0; i--)
            {
                convertArray[i - 1] = (byte)toConvert;
                if ((convertArray[i - 1] < 0x20) || (convertArray[i - 1] > 0x7E))
                {
                    convertArray[i - 1] = 0x2E; // "."
                }
                toConvert >>= 8;
            }

            return Encoding.ASCII.GetString(convertArray);

        }

        public static String ConvertIntASCIIReverse(int toConvert, int numBytes)
        { // ASCII characters are in reverse order from bytes in "toConvert"

            numBytes += (numBytes - 1);

            byte[] convertArray = new byte[numBytes];
            for (int i = 0; i < numBytes; i++)
            {
                if ((i % 2) == 1)
                { // add spaces
                    convertArray[i] = 0x20; // " " space
                }
                else
                {
                    convertArray[i] = (byte)toConvert;
                    if ((convertArray[i] < 0x20) || (convertArray[i] > 0x7E))
                    {
                        convertArray[i] = 0x2E; // "."
                    }
                    toConvert >>= 8;
                }
            }

            return Encoding.ASCII.GetString(convertArray);

        } 
                
    }
}
