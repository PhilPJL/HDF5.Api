using CommandLine;
using HDF5Api;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HDF5Test
{
    static class Program
    {
        public class Options
        {
            [Option('m', Required = true, HelpText = "One or more measurement Ids")]
            public IEnumerable<int> MeasIds { get; set; }

            [Option('t', Required = false, HelpText = "Minutes of data to retrieve", Default = 1)]
            public int Minutes { get; set; }
        }

        static async Task<int> Main(string[] args)
        {
            await Parser.Default
                .ParseArguments<Options>(args)
                .WithParsedAsync(o => RunAsync(o.MeasIds, o.Minutes));

            return 0;
        }

        static async Task RunAsync(IEnumerable<int> measurementIds, int minutes)
        {
            try
            {
                Console.WriteLine($"Version={H5Global.GetLibraryVersion()}, Is thread safe={H5Global.IsThreadSafe()}");
                Console.WriteLine();

                await CreateFileTest.CreateFileAsync(measurementIds, minutes);

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
