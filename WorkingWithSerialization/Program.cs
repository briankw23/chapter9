using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using Packt.Shared;
using System.Threading.Tasks;
using NuJson = System.Text.Json.JsonSerializer;

using static System.Console;
using static System.Environment;
using static System.IO.Path;

namespace WorkingWithSerialization
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // create a object graph
            var people = new List<Person>
            {
                new Person(30000M) { FirstName = "Alice", LastName = "Smith",
                    DateOfBirth = new DateTime(1974, 3, 14) },
                new Person(40000M) { FirstName = "Bob", LastName = "Jones",
                    DateOfBirth = new DateTime(1969, 11, 23) },
                new Person(20000M) { FirstName = "Charlie", LastName = "Cox",
                    DateOfBirth = new DateTime(1984, 5, 4), Children = new HashSet<Person>
                    { new Person(0M) { FirstName = "Sally", LastName = "Cox",
                    DateOfBirth = new DateTime(2000, 7, 12) }}}
            };

            //create object that will format a List of Persons as XML
            var xs = new XmlSerializer(typeof(List<Person>));

            //Create a file to write to - path
            string path = Combine(CurrentDirectory, "people.xml");

            using (FileStream stream = File.Create(path))
            {
                //serialize the graph to the stream
                xs.Serialize(stream, people);
            }

            WriteLine("Written {0:N0} bytes of XML to {1}", arg0: new FileInfo(path).Length,
                arg1: path);
            WriteLine();
            // Display the serialized object graph 
            WriteLine(File.ReadAllText(path));


            using (FileStream xmlLoad = File.Open(path, FileMode.Open)) 
            {
            // deserialize and cast the object graph into a List of Person 
            var loadedPeople = (List<Person>)xs.Deserialize(xmlLoad);
                foreach (var item in loadedPeople)
                {
                WriteLine("{0} has {1} children.", item.LastName, item.Children.Count);
                } 
            }


            //create a file to write to
            string jsonPath = Combine(CurrentDirectory, "people.json");
            using (StreamWriter jsonStream = File.CreateText(jsonPath))
            {
                //create object that will foramt as json
                var jss = new Newtonsoft.Json.JsonSerializer();
                //serialize the object graph into a string
                jss.Serialize(jsonStream, people);
            }

            WriteLine();
            WriteLine("Written {0:N0} bytes of JSON to: {1}",
                arg0: new FileInfo(jsonPath).Length, arg1: jsonPath);
            // Display the serialized object graph 
            WriteLine(File.ReadAllText(jsonPath));


            //NUJSON
            using (FileStream jsonLoad = File.Open(jsonPath, FileMode.Open))
            {
                //deserialize object graph into a list of Person
                var loadedPeople = (List<Person>) 
                await NuJson.DeserializeAsync(utf8Json: jsonLoad,
                    returnType: typeof(List<Person>));

                    foreach (var item in loadedPeople)
                    {
                       WriteLine("{0} has {1} children.", item.LastName, item.Children?.Count); 
                    }
            }
        }
    }
}
