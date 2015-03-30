namespace CmdSeqParser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    
    public class IndigoVariant
    {
        private const ulong MaxSizeMergedFile = 32 * 1024;
        private string[] seqFiles;
        private string[] parFiles;
        private int[] version;

        public IndigoVariant(string strSeqFolder, string strParFolder, int[] versionF)
        {
            if (!Directory.Exists(strSeqFolder))
            {
                throw new ArgumentException("Folder " + strSeqFolder + " do not exist!");
            }
            else if (!Directory.Exists(strParFolder))
            {
                throw new ArgumentException("Folder " + strParFolder + " do not exist!");
            }
            else
            {
                this.version = versionF;
                this.seqFiles = Directory.GetFiles(strSeqFolder);
                this.parFiles = Directory.GetFiles(strParFolder);
            }
        }

        public void CopyFiles(string outputFolder)
        {
            string frontEndParameterData = "FrontEndParameterData_" + this.version[0] + "_" + this.version[1] + ".gdc32dat";
            string frontEndProductionData = "FrontEndProductionData_" + this.version[0] + "_" + this.version[1] + ".gdc32dat";
            string frontEndSupplierSecuredData = "FrontEndSupplierSecuredData_" + this.version[0] + "_" + this.version[1] + ".gdc32dat";

            var fileFrontEndParameterData = this.parFiles.Where(x => x.EndsWith(frontEndParameterData)).ToArray();
            if (fileFrontEndParameterData.Length == 0)
            {
                throw new ArgumentException("File " + frontEndParameterData + " do not exist!");
            }
            else
            {
                File.Copy(fileFrontEndParameterData[0], outputFolder + "\\0x017F4000_FrontEndParameterData.gdc32dat");
            }

            var fileFrontEndProductionData = this.parFiles.Where(x => x.EndsWith(frontEndProductionData)).ToArray();
            if (fileFrontEndProductionData.Length == 0)
            {
                throw new ArgumentException("File " + frontEndProductionData + " do not exist!");
            }
            else
            {
                File.Copy(fileFrontEndProductionData[0], outputFolder + "\\0x017F6000_FrontEndProductionData.gdc32dat");
            }

            var fileFrontEndSupplierSecuredData = this.parFiles.Where(x => x.EndsWith(frontEndSupplierSecuredData)).ToArray();
            if (fileFrontEndSupplierSecuredData.Length == 0)
            {
                throw new ArgumentException("File " + frontEndSupplierSecuredData + " do not exist!");
            }
            else
            {
                File.Copy(fileFrontEndSupplierSecuredData[0], outputFolder + "\\0x017F7000_FrontEndSupplierSecuredData.gdc32dat");
            }

            foreach (var item in this.seqFiles)
            {
                File.Copy(item, outputFolder + "\\" + Path.GetFileName(item));
            }
        }

        public void MergeFiles(string outputFolder, string name)
        {
            string[] filesToMerge = Directory.GetFiles(outputFolder, "0x*.gdc32dat").OrderBy(x => x).ToArray();
            ulong offset = this.GetAddress(filesToMerge[0]);
            var lastFile = new FileInfo(filesToMerge[filesToMerge.Length - 1]);
            ulong end = this.GetAddress(lastFile.FullName) + (ulong)lastFile.Length;
            if (end - offset > MaxSizeMergedFile)
            {
                throw new ArgumentException("Merged file more than 32K!");
            }

            var arrayData = Enumerable.Repeat<byte>(0xFF, (int)(end - offset)).ToList<byte>();

            foreach (string file in filesToMerge)
            {
                using (BinaryReader b = new BinaryReader(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    ulong address = this.GetAddress(file);
                    int pos = 0;
                    int length = (int)b.BaseStream.Length;

                    while (pos < length)
                    {
                        if (arrayData[(int)address + (int)pos - (int)offset] != 0xFF)
                        {
                            throw new ArgumentException(string.Format("Address 0x{0:X} overlapps!", address + (ulong)pos));
                        }

                        arrayData[(int)address + (int)pos - (int)offset] = b.ReadByte();
                        pos += sizeof(byte);
                    }
                }

                using (BinaryWriter b = new BinaryWriter(File.Open(outputFolder + "\\" + name + ".gdc32dat", FileMode.Create)))
                {
                    foreach (byte data in arrayData)
                    {
                        b.Write(data);
                    }
                }

                if (name == "PRODUCTION")
                {
                    MHXWriter mhx = new MHXWriter(outputFolder + "\\PRODUCTION.mhx", arrayData, 0x2000);
                    SourceWritter src = new SourceWritter(outputFolder + "\\..\\source.c", arrayData);
                }
            }
        }

        private ulong GetAddress(string filename)
        {
            return Convert.ToUInt32(Path.GetFileName(filename).Substring(2, 8), 16);
        }
    }
}
