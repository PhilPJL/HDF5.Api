namespace HDF5.Api.TestHarness
{
    internal class Program
    {
        static void Main()
        {
            H5Global.TryLoadLibraries(@"C:\Program Files\HDF_Group\HDF5\1.10.9_intel\bin");
            H5Global.TryLoadLibraries();

            string fileName = @"C:\Users\passp\Downloads\ex_table_11.h5";
            using var file = H5File.Open(fileName);

            Console.WriteLine(fileName);

            Console.WriteLine(H5Global.GetLibraryVersion());
            DumpLocation(file, fileName, 1);
            Console.WriteLine(H5Global.GetLibraryVersion());
        }

        static void DumpLocation<T>(H5Location<T> location, string locationName, int indent) where T : H5Object<T>
        {
            string sIndent = new string(' ', indent * 2);

            foreach (var a in location.AttributeNames)
            {
                Console.WriteLine(sIndent + "A: " + a);

                using var att = location.OpenAttribute(a);
                using var type = att.GetH5Type();

                switch (type.GetClass())
                {
                    case H5Class.Integer:
                        Console.WriteLine(sIndent + "->V: " + att.Read<int>());
                        break;
                    case H5Class.String:
                        Console.WriteLine(sIndent + "->V: " + att.ReadString());
                        break;
                    case H5Class.Float:
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

                DumpLocation(gp, g, indent + 2);
            }
        }
    }
}