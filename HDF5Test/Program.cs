using CommandLine;
using HDF5Api;
using System;
using System.Collections.Generic;

namespace HDF5Test
{
    static class Program
    {
        public class Options
        {
            [Option('m', Required = true, HelpText = "One or more measurement Ids")]
            public IEnumerable<int> MeasIds { get; set; }

            [Option('r', Required = false, HelpText = "Max rows to retrieve", Default = 100)]
            public int MaxRows { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed(o => Run(o.MeasIds, o.MaxRows));
        }

        static void Run(IEnumerable<int> measurementIds, int maxRows)
        {
            try
            {
                CreateFileTest.CreateFile(measurementIds, maxRows);

#if DEBUG
                if (H5Handle.Handles.Count > 0)
                {
                    foreach (var item in H5Handle.Handles)
                    {
                        Console.WriteLine("-----------------------------");
                        Console.WriteLine($"{item.Key}");
                        Console.WriteLine(item.Value);
                    }
                }
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
