using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public class encoder
    {
        public static int[] xors = new int[] { 32, 96, 1064, 28012, 30000, 45640, 60000, 110987, 127658, 254688, 335792, 409600, 599999, 1000110, 1008611, 3276856 };
        public static int[] shifts = new int[] {1,2,4,1,3,8,6,9,7,6,5 };
        public static int[] shiftOffset = new int[] {44,68,910,1204,1468,9902,10057,18804,398722,3276800 };
        public static int  blockSize = 128;
        private static int Main(string[] args)
        {
            args = new string[] { "Assembly-CSharp.dll", "Encode" };
            //args = new string[] { "Assembly-CSharp.dll_copy", "Decode" };
            int ret = 0;
            if (args == null)
            {
                Console.WriteLine("no args was send");
                return 1;
            }
            if (args.Length == 0)
            {
                Console.WriteLine("no args was send");
                return 2;
            }
            string file = args[0];
            string tag = "Encode";
            if (args.Length > 1)
            {
                tag = args[1];
            }
            Console.WriteLine("arg is " + file);
            if (!File.Exists(file))
                return 3;
            FileStream stream = File.OpenRead(file);
            if (stream != null)
            {
                byte[] bytes = new byte[stream.Length];
                int x = stream.Read(bytes, 0, bytes.Length);
                if (tag == "Decode")
                {
                    //xor
                    for (int idx = 0; idx < xors.Length; idx++)
                    {
                        if (bytes.Length > xors[idx])
                        {
                            bytes[xors[idx]] = (byte) (0xff^bytes[xors[idx]]);
                        }
                    }
                    //shift
                    for (int idx = 0; idx < shiftOffset.Length; idx++)
                    {
                        if (bytes.Length > shiftOffset[idx])
                        {
                            bytes[shiftOffset[idx]] = left_shift(bytes[shiftOffset[idx]],shifts[idx]);
                        }
                    }
                }
                else
                {
                    //shift
                    for (int idx = 0; idx < shiftOffset.Length; idx++)
                    {
                        if (bytes.Length > shiftOffset[idx])
                        {
                            bytes[shiftOffset[idx]] = right_shift(bytes[shiftOffset[idx]], shifts[idx]);
                        }
                    }
                    //xor
                    for (int idx = 0; idx < xors.Length; idx++)
                    {
                        if (bytes.Length > xors[idx])
                        {
                            bytes[xors[idx]] = (byte)(0xff ^ bytes[xors[idx]]);
                        }
                    }
                }
                File.WriteAllBytes(file+"_copy",bytes);
                stream.Close();
            }
            return ret;
        }
        static byte left_shift(byte data,int offset)
        {
            int tmp = 0;
            Console.WriteLine("data org " + Convert.ToString(data, 16) + "," + offset);
            for (int i = 0; i < offset; i++)
            {
                //保存高位
                tmp = (data >> 7) & 0x01;
                data =(byte) (data << 1);
                data = (byte)((data&0xfe) | tmp);
            }
            Console.WriteLine("data shifted " + Convert.ToString(data, 16));
            return data;
        }
        static byte right_shift(byte data, int offset)
        {
            int tmp = 0;
            Console.WriteLine("data org " + Convert.ToString(data, 16) + "," +offset);
            for (int i = 0; i < offset; i++)
            {
                //保存低位
                tmp = data & 0x01;
                data = (byte)(data >> 1);
                data = (byte)((data & 0x7f) | (tmp<<7 & 0x80));
            }
            Console.WriteLine("data shifted " + Convert.ToString(data,16));
            return data;
        }
    }
}
