namespace CmdSeqParser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class SourceWritter
    {
        private const int Block0Address = 0x0;
        private const int Block1Address = 0x2000;
        private const int Block2Address = 0x4000;
        private const int Block3Address = 0x6000;

        public SourceWritter(string filename, List<byte> data)
        {
            using (StreamWriter b = new StreamWriter(File.Open(filename, FileMode.Create)))
            {
                int i;

                for (i = Block1Address - 1; i >= Block0Address; i--)
                {
                    if (data[i] != 0xFF)
                    {
                        break;
                    }
                }

                b.Write("#define cBlock0Size {0}\n", i + 1);
                b.Write("const uint8 u8FeBoot_Block0[cBlock0Size] =\n{");

                for (int j = 0; j <= i; j++)
                {
                    if (j % 16 == 0)
                    {
                        b.Write("\n    ");
                    }

                    b.Write(string.Format("0x{0:X2}", data[j]));
                    if (j != i)
                    {
                        b.Write(", ");
                    }
                }

                b.WriteLine("\n};\n");

                b.Write("#define cBlock3Size {0}\n", data.Count - Block3Address);
                b.Write("const uint8 u8FeBoot_Block3[cBlock3Size] =\n{");

                for (int j = Block3Address; j < data.Count; j++)
                {
                    if (j % 16 == 0)
                    {
                        b.Write("\n    ");
                    }

                    b.Write(string.Format("0x{0:X2}", data[j]));
                    if (j != data.Count - 1)
                    {
                        b.Write(", ");
                    }
                }

                b.WriteLine("\n};\n");
            }
        }
    }
}
