using HDF5.Api.Attributes;
using HDF5.Api.H5Types;
using System.Diagnostics;

namespace HDF5.Api.TestHarness
{
    internal static class Program
    {
        private static void Main()
        {
            //H5Global.TryLoadLibraries(@"C:\Program Files\HDF_Group\HDF5\1.10.9_intel\bin");
            //H5Global.TryLoadLibraries();

            //const string fileName = @"C:\Users\passp\Downloads\ex_table_11.h5";
            const string fileName = @"C:\Users\passp\source\repos\PythonApplication1\PythonApplication1\arraytest-c#.h5";
            Console.WriteLine(fileName);

            //file.WriteAttribute<bool>("bool-2", true);
            //Console.WriteLine(file.ReadAttribute<bool>("bool-false"));

            //file.WriteAttribute("md-array-2x2x3", new List<string> {
            //    "one", "two", "three", "four", "five", "six",
            //    "one2", "two2", "three2", "four2", "five2", "six2"
            //}, new long[] { 2, 3, 2 }, 10, writeBehaviour: AttributeWriteBehaviour.OverwriteIfAlreadyExists);

            //arr3d = np.array([[[2,17], [45, 78]], [[88, 92], [60, 76]],[[76,33],[20,18]]])

            using var file = H5File.Create(fileName, versionBounds: new(LibraryVersion.Version110, LibraryVersion.Latest));
            Console.WriteLine(file.GetLibraryVersionBounds());
            //file.SetLibraryVersionBounds(LibraryVersion.Latest);
            using var group = file.CreateGroup("arrays");
            Random r = new Random(Environment.TickCount);
            int d = 2000;

            var sw = new Stopwatch();
            var list = Enumerable.Range(0, d * d).Select(i => r.NextDouble()).ToArray();
            sw.Start();
            for (int i = 0; i < 1; i++)
            {
                file.WriteUnmanagedAttribute($"c#1-{d}x{d}-{i}", list, new long[] { d, d });
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            sw.Restart();

            var list2 = file.ReadAttribute<double>($"c#1-{d}x{d}-{0}", d, d);

            Console.WriteLine(sw.ElapsedMilliseconds);

            if(list.Zip(list2, (l, m) => new {l, m}).Any(x => x.l != x.m))
            {
                Console.WriteLine("Fail");
            }
            else
            {
                Console.WriteLine("Succeed");
            }

            //for (int i = 0; i < 1000; i++)
            //{
            //    file.WriteAttribute("c#-array", list, new long[] { d, d }, writeBehaviour: AttributeWriteBehaviour.OverwriteIfAlreadyExists);
            //}
            //Console.WriteLine(sw.ElapsedMilliseconds);

            //file.WriteAttribute("c#-array-3x2x2 (2)", new string[3, 2, 2] {
            //     {{"1", "2"}, {"3", "4"}}, {{"5", "6"}, {"7", "8"}}, {{"9", "10"}, {"11", "12"}}
            //}, 10);

            //file.WriteAttribute("md-array-2x3", new List<string> {
            //    "one", "two", "three", "four", "five", "six"
            //}, new long[] { 2, 3 }, 0, writeBehaviour: AttributeWriteBehaviour.OverwriteIfAlreadyExists);

            //file.WriteAttribute("md-array-3x2", new List<string> {
            //    "one", "two", "three", "four", "five", "six"
            //}, new long[] { 3, 2 }, 10, writeBehaviour: AttributeWriteBehaviour.OverwriteIfAlreadyExists);

            //file.WriteAttribute("md-array-6x1", new List<string> {
            //    "one", "two", "three", "four", "five", "six"
            //}, new long[] { 6, 1 }, 10, writeBehaviour: AttributeWriteBehaviour.OverwriteIfAlreadyExists);

            //file.WriteAttribute("md-array-1x6", new List<string> {
            //    "one", "two", "three", "four", "five", "six"
            //}, new long[] { 1, 6 }, 10, writeBehaviour: AttributeWriteBehaviour.OverwriteIfAlreadyExists);

            //file.WriteAttribute("md-array-1x3x2", new List<string> {
            //    "one", "two", "three", "four", "five", "six"
            //}, new long[] { 1, 3, 2 }, 10, writeBehaviour: AttributeWriteBehaviour.OverwriteIfAlreadyExists);

            //file.WriteAttribute("md-array-1x2x3", new List<string> {
            //    "one", "two", "three", "four", "five", "six"
            //}, new long[] { 1, 2, 3 }, 10, writeBehaviour: AttributeWriteBehaviour.OverwriteIfAlreadyExists);

            //file.WriteAttribute("md-array-2x3-a", (Array)new string[2, 3] 
            //{
            //    { "one", "two", "three" },
            //    {"four", "five", "six" }
            //}, 
            //10, writeBehaviour: AttributeWriteBehaviour.OverwriteIfAlreadyExists);

            //file.WriteAttribute("md-array-3x2-a", (Array)new string[3, 2] 
            //{
            //    { "one", "two" }, 
            //    {"three", "four" }, 
            //    {"five", "six" }
            //}, 
            //10, writeBehaviour: AttributeWriteBehaviour.OverwriteIfAlreadyExists);

            //file.WriteAttribute("md-array-fixed2", (Array)new[,,] {
            //    {
            //        { "a-one", "a-two" },
            //        { "a-three", "a-four" },
            //        { "a-five", "a-six" },
            //        { "a-seven", "a-eight" },
            //        { "a-nine", "a-ten" },
            //        { "a-eleven", "a-twelve" },
            //        { "a-thirteen", "a-fourteen" },
            //        { "a-fifteen", "a-sixteen" }
            //    },
            //    {
            //        { "b-one", "b-two" },
            //        { "b-three", "b-four" },
            //        { "b-five", "b-six" },
            //        { "b-seven", "b-eight" },
            //        { "b-nine", "b-ten" },
            //        { "b-eleven", "b-twelve" },
            //        { "b-13", "b-14" },
            //        { "b-15", "b-16" }
            //    },
            //    {
            //        { "c-one", "c-two" },
            //        { "c-three", "c-four" },
            //        { "c-five", "c-six" },
            //        { "c-seven", "c-eight" },
            //        { "c-nine", "c-ten" },
            //        { "c-eleven", "c-twelve" },
            //        { "c-13", "c-14" },
            //        { "c-15", "c-16" }
            //    }
            //}, 12, writeBehaviour: AttributeWriteBehaviour.OverwriteIfAlreadyExists);

            //Console.WriteLine(file.ReadAttribute<bool>("bool-false"));

            //using var space = H5Space.CreateScalar();
            //using var type = H5Type.GetEquivalentNativeType<int>();
            //throw new NotImplementedException();
            //            using var att = file.CreateAttribute("null", type, space);
            //            H5AAdapter.WriteNull<int>(att, type);

            //Console.WriteLine(H5Global.GetLibraryVersion());
            //DumpLocation(file, 1);
            //Console.WriteLine(H5Global.GetLibraryVersion());
        }

        private static void DumpLocation<T>(H5Location<T> location, int indent) where T : H5Location<T>
        {
            throw new NotImplementedException();
            /*            string sIndent = new string(' ', indent * 2);

                        foreach (var a in location.AttributeNames)
                        {
                            Console.WriteLine(sIndent + "A: " + a);

                            using var att = location.OpenAttribute(a);
                            using var type = att.GetH5Type();

                            switch (type.GetClass())
                            {
                                case DataTypeClass.Integer:
                                    Console.WriteLine(sIndent + "->V: " + att.Read<int>());
                                    break;
                                case DataTypeClass.String:
                                    Console.WriteLine(sIndent + "->V: " + att.Read<string>());
                                    break;
                                case DataTypeClass.Float:
                                    Console.WriteLine(sIndent + "->V: " + att.Read<double>());
                                    break;
                            }
                        }

                        foreach (var d in location.DataSetNames)
                        {
                            using var ds = location.OpenDataSet(d);

                            Console.WriteLine(sIndent + "DS: " + d);

                            using var type = ds.GetH5Type();
                        }

                        foreach (var g in location.GroupNames)
                        {
                            using var gp = location.OpenGroup(g);

                            Console.WriteLine(sIndent + "G: " + g);

                            DumpLocation(gp, indent + 2);
                        }
            */
        }
    }

    [H5Contract("AType")]
    public class SomeType
    {
        [H5Member(name: "AProperty")]
        public int IntProperty { get; set; }

        [H5StringMember(characterSet: CharacterSet.Utf8, allocatedStorageInBytes: 1000)]
        public string StringProperty { get; set; } = string.Empty;
    }
}