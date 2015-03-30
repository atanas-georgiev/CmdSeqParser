namespace CmdSeqParser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;    

    public class XMLReader
    {
        private string inputParameterFolder;
        private string inputSequenceFolder;
        private string outputFolder;
        private int[] parameterVersion = new int[2];
        private List<string> variants = new List<string>();

        public XMLReader(string xmlConfiguration)
        {
            try
            {
                XDocument xmlDoc = XDocument.Load(xmlConfiguration);
                this.inputParameterFolder = xmlDoc.Root.Elements("Global").Elements("InputParameterFolder").First().Value;
                this.inputSequenceFolder = xmlDoc.Root.Elements("Global").Elements("InputSequenceFolder").First().Value;
                this.outputFolder = xmlDoc.Root.Elements("Global").Elements("OutputFolder").First().Value;
                this.parameterVersion[0] = int.Parse(xmlDoc.Root.Elements("Global").Elements("ParameterVersionMajor").First().Value);
                this.parameterVersion[1] = int.Parse(xmlDoc.Root.Elements("Global").Elements("ParameterVersionMinor").First().Value);

                foreach (var item in xmlDoc.Root.Elements("Variants").Elements())
                {
                    this.variants.Add(item.Value);
                }                
            }
            catch (IOException e)
            {
                Console.WriteLine("Error opening file: " + e.Message);
            }
        }

        public List<string> Variants
        {
            get { return this.variants; }
        }

        public string InputParameterFolder
        {
            get { return this.inputParameterFolder; }
        }

        public string InputSequenceFolder
        {
            get { return this.inputSequenceFolder; }
        }

        public int[] ParameterVersion
        {
            get { return this.parameterVersion; }
        }

        public string OutputFolder
        {
            get { return this.outputFolder; }
        }
    }
}
