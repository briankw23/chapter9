using System;
using System.IO;
using System.Xml;
using System.IO.Compression;

using static System.Console;
using static System.Environment;
using static System.IO.Path; 

namespace WorkingWithStreams
{
    class Program
    {
        static void Main(string[] args)
        {
            //WorkingWithText();
            WorkingWithXml();
            WorkingWithCompression();
        }

        static void WorkingWithText()
        {
            // define an array of Viper pilot call signs static 
            string[] callsigns = new string[] 
            {"Husker", "Starbuck", "Apollo", "Boomer", "Bulldog", "Athena", "Helo", "Racetrack" }; 

            // define a file to write to
            string textFile = Combine(CurrentDirectory, "streams.text");

            // creat a text file and return a helper writer
            StreamWriter text = File.CreateText(textFile);

            // enumerate the strings, writing each one 
            // to the stream on a separate line 
            foreach (string item in callsigns)
            {
            text.WriteLine(item); 
            }
            text.Close(); 
            // release resources
            // output the contents of the file 
            WriteLine("{0} contains {1:N0} bytes.",
            arg0: textFile,
            arg1: new FileInfo(textFile).Length);

            WriteLine(File.ReadAllText(textFile));
        }

        static void WorkingWithXml()
        {
            string[] callsigns = new string[] 
            {"Husker", "Starbuck", "Apollo", "Boomer", "Bulldog", "Athena", "Helo", "Racetrack" }; 

            FileStream xmlFileStream = null;
            XmlWriter xml = null;

            try
            {
            // define a file to write to 
            string xmlFile = Combine(CurrentDirectory, "streams.xml");

            // Create a filestream
            xmlFileStream = File.Create(xmlFile);

            // Wrap the stream in a XML writer helper
            // And automatically indent nested elements 
            xml = XmlWriter.Create(xmlFileStream, new XmlWriterSettings {Indent = true});

            // write the XML declaration
            xml.WriteStartElement("callsigns");

            // enumerate the strings writing each one to the stream
            foreach (var item in callsigns)
            {
                xml.WriteElementString("callsign", item );
            }

            // write the close root element
            xml.WriteEndElement();

            // Close helper and stream
            xml.Close();
            xmlFileStream.Close();

            //output all the contents of the file
            WriteLine("{0} contains {1:NO} bytes.", arg0: xmlFile, arg1: new FileInfo(xmlFile).Length);
            WriteLine(File.ReadAllText(xmlFile));
            }
            catch (Exception ex)
            {
                //if the path doesn't exist the exception will caught
                WriteLine($"{ex.GetType()} says {ex.Message}");
            }
            finally
            {
                if (xml != null)
                {
                    xml.Dispose();
                    WriteLine("The XML writer's unmanaged resources have been disposed.");
                }
                if (xmlFileStream != null)
                {
                    xmlFileStream.Dispose();
                    WriteLine("The File stream's unmanaged resources have been disposed.");
                }
            }
        }
    
        static void WorkingWithCompression()
        {
            string[] callsigns = new string[] 
            {"Husker", "Starbuck", "Apollo", "Boomer", "Bulldog", "Athena", "Helo", "Racetrack" }; 
            // compress the XML output (path)
            string gzipFilePath = Combine(CurrentDirectory, "streams.gzip");
            //File
            FileStream gzipFile = File.Create(gzipFilePath);

            using(GZipStream compressor = new GZipStream(gzipFile, CompressionMode.Compress))
            {
                using(XmlWriter xmlGzip = XmlWriter.Create(compressor))
                {
                    xmlGzip.WriteStartDocument();
                    xmlGzip.WriteStartElement("callsigns");

                    foreach (var item in callsigns)
                    {
                        xmlGzip.WriteElementString("callsigns", item);
                    }
                    // the normal call to WriteEndElement is not necessary 
                    // because when the XmlWriter disposes, it will
                    // automatically end any elements of any depth
                } // also closes the underlying stream

                // output all the contents of the compressed file 
                WriteLine("{0} contains {1:N0} bytes.", gzipFilePath, new FileInfo(gzipFilePath).Length);
                WriteLine($"The compressed contents:"); 
                WriteLine(File.ReadAllText(gzipFilePath));
                // read a compressed file
                WriteLine("Reading the compressed XML file:"); 
                gzipFile = File.Open(gzipFilePath, FileMode.Open);

                using(GZipStream decompressor = new GZipStream(gzipFile, CompressionMode.Decompress))
                {
                    using(XmlReader reader = XmlReader.Create(decompressor))
                    {
                        while(reader.Read()) // read the next XML Node
                        {
                            //check if we are on an element node named callsign
                            if((reader.NodeType == XmlNodeType.Element) && (reader.Name == "callsign"))
                            {
                                reader.Read(); //move to the next element
                                WriteLine($"{reader.Value}"); //read its value
                            }
                        }
                    }
                }
            }

        }
    }
}
