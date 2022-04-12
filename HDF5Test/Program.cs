using CommandLine;
using HDF5Api;
using System;

namespace HDF5Test
{
    static class Program
    {
        public class Options
        {
            [Option('m', Required = true, HelpText = "The measurement Id")]
            public int MeasId { get; set; }

            [Option('r', Required = false, HelpText = "Max rows to retrieve", Default = 100)]
            public int MaxRows { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed(o => Run(o.MeasId, o.MaxRows));
        }

        static void Run(int measurementId, int maxRows)
        {
            try
            {
                CreateFileTest.CreateFile(measurementId, maxRows);

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
