﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace HDF5.Api.Benchmark
{
    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine(H5Global.Describe());

            BenchmarkRunner.Run<HDF5>();
        }
    }

    [Config(typeof(HDF5))]
    public class HDF5 : ManualConfig
    {
        public HDF5()
        {
            AddJob(Job.RyuJitX64
                .WithBaseline(true)
                .WithRuntime(CoreRuntime.Core70));

            AddJob(Job.RyuJitX64
                .WithRuntime(CoreRuntime.Core60));

            AddJob(Job.RyuJitX64
                .WithRuntime(ClrRuntime.Net48));

            // TODO: x86
        }

        public const string TestFile = "Benchmark.h5";

        private H5File? _file;

        [GlobalSetup]
        public void Setup()
        {
            File.Delete(TestFile);

            _file = H5File.Create(TestFile);
            _file.WriteAttribute("DateTime", DateTime.UtcNow);
            _file.WriteAttribute("shortstring", "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789", 101);
            _file.WriteAttribute("longstring",
                "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" +
                "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" +
                "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" +
                "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" +
                "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" +
                "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" +
                "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" +
                "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" +
                "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" +
                "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789", 1001);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _file?.Dispose();
            _file = null;

            //File.Delete(TestFile);
        }

        [Benchmark]
        public void ReadDateTime()
        {
            _file?.ReadAttribute<DateTime>("DateTime");
        }

        [Benchmark]
        public void ReadShortString()
        {
            _file?.ReadAttribute<string>("shortstring");
        }

        [Benchmark]
        public void ReadLongString()
        {
            _file?.ReadAttribute<string>("longstring");
        }
    }

}
