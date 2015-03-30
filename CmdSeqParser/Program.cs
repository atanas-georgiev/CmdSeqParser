namespace CmdSeqParser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;    

    public class Program
    {        
        private static void Main(string[] args)
        {
            try
            {
                XMLReader reader = new XMLReader("../../../CmdSeqParser.xml");

                // Remove output folder
                if (Directory.Exists(reader.OutputFolder))
                {
                    RemoveReadOnly(new DirectoryInfo(reader.OutputFolder));
                    Directory.Delete(reader.OutputFolder, true);
                }

                // Create new output folder
                Directory.CreateDirectory(reader.OutputFolder);
                foreach (var item in reader.Variants)
                {
                    Directory.CreateDirectory(reader.OutputFolder + "\\" + item);
                }

                foreach (var item in reader.Variants)
                {
                    Console.WriteLine(item);
                    var a = new IndigoVariant(reader.InputSequenceFolder, reader.InputParameterFolder + "\\" + item, reader.ParameterVersion);
                    a.CopyFiles(reader.OutputFolder + "\\" + item);
                    a.MergeFiles(reader.OutputFolder + "\\" + item, item);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void RemoveReadOnly(DirectoryInfo directory)
        {
            foreach (FileInfo fi in directory.GetFiles())
            {
                fi.IsReadOnly = false;
            }

            foreach (DirectoryInfo subdir in directory.GetDirectories())
            {
                RemoveReadOnly(subdir);
            }
        }
    }
}
