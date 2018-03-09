using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CoffeeUniversal.Helpers
{
    public sealed class HexConverter
    {
        private HexConverter() { }

        /* Takes a string of decimal or hexadecimal numbers, delimited by one or more of 
        ',' ' ' or ';' and outputs a byte array of these numbers. If any byte entry is 
        unable to be processed as a number, that byte is instead processed as an ascii 
        string (specified delimeters are still removed).
        */
        public static byte[] StringToByteArray(string send)
        {
            string[] separators = { ",", ";", " " };
            string[] output = send.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            int size = output.Count();

            List<byte> buffer = new List<byte>();

            for (int i = 0; i < size; i++)
            {
                string s = output[i];

                string[] bytesToOr;
                bytesToOr = s.Split('|');

                foreach (string b in bytesToOr)
                {
                    if (b.Length == 0)
                        continue;
                    try
                    {
                        if (b.ToUpper().StartsWith("0X", StringComparison.OrdinalIgnoreCase))
                        {
                            var b_new = b.Substring(2);
                            buffer.Add(Convert.ToByte(Int32.Parse(b_new, System.Globalization.NumberStyles.HexNumber)));
                        }
                        else
                        {
                            buffer.Add(Convert.ToByte(b));
                        }
                    }
                    catch
                    {
                        var ascii = Encoding.ASCII.GetBytes(b);
                        foreach (byte by in ascii)
                            buffer.Add(by);
                    }
                }
            }
            return buffer.ToArray();
        }

        /* Takes a byte array and outputs a hexadecimal-style string.
        */
        public static string ByteToString(byte[] byteArray)
        {
            StringBuilder hex = new StringBuilder(byteArray.Length * 6);
            foreach (byte b in byteArray)
            {
                hex.AppendFormat("0x{0:x2}, ", b);
            }
            char[] trim = { ' ', ',' };
            return hex.ToString().TrimEnd(trim);
        }

        public static string ByteToString(int i)
        {
            byte[] array = { (byte)i };
            return ByteToString(array);
        }
    }
}