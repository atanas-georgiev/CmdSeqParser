namespace CmdSeqParser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;    

    public class MHXWriter
    {
        public MHXWriter(string filename, List<byte> data, int offset)
        {
            List<byte> localData = data;

            using (StreamWriter b = new StreamWriter(File.Open(filename, FileMode.Create)))
            {
                b.WriteLine("S009000047414C4550335A");

                for (int i = 0; i < data.Count; i += 16)
                {
                    if (data.Count - i >= 16)
                    {
                        b.Write("S113");
                        b.Write(string.Format("{0:X4}", i + offset));

                        foreach (byte item in data.GetRange(i, 16))
                        {
                            b.Write(string.Format("{0:X2}", item));
                        }

                        b.Write(string.Format("{0:X2}", this.CalculateCheckSum(i + offset, data.GetRange(i, 16))));

                        b.Write("\n");
                    }
                    else
                    {
                        b.Write("S1");
                        b.Write(string.Format("{0:X2}", data.Count - i + 3));
                        b.Write(string.Format("{0:X4}", i + offset));

                        foreach (byte item in data.GetRange(i, data.Count - i))
                        {
                            b.Write(string.Format("{0:X2}", item));
                        }

                        b.Write(string.Format("{0:X2}", this.CalculateCheckSum(i + offset, data.GetRange(i, data.Count - i))));

                        b.Write("\n");
                    }
                }

                b.WriteLine("S9030000FC");
            }
        }

        private byte CalculateCheckSum(int address, List<byte> data)
        {
            byte result = (byte)(data.Sum<byte>(x => x) + (address & 0xFF) + ((address >> 8) & 0xFF) + (data.Count + 3));
            return (byte)(0xFF - result);
        }
    }
}
