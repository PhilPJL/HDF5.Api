﻿using HDF5.Api.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace HDF5.Api.TestHarness
{
    internal static class Program
    {
        private static void Main()
        {
            H5Global.TryLoadLibraries(@"C:\Program Files\HDF_Group\HDF5\1.10.9_intel\bin");
            H5Global.TryLoadLibraries();

            const string fileName = @"C:\Users\passp\Downloads\ex_table_11.h5";
            using var file = H5File.Open(fileName);

            Console.WriteLine(fileName);

            Console.WriteLine(H5Global.GetLibraryVersion());
            DumpLocation(file, 1);
            Console.WriteLine(H5Global.GetLibraryVersion());
        }

        private static void DumpLocation<T>(H5Location<T> location, int indent) where T : H5Object<T>
        {
            string sIndent = new string(' ', indent * 2);

            location.Enumerate(HandleAttribute);

            foreach (var a in location.AttributeNames)
            {
                Console.WriteLine(sIndent + "A: " + a);

                using var att = location.OpenStringAttribute(a);
                using var type = att.GetH5Type();

                // TODO:
/*                switch (type.GetClass())
                {
                    case DataTypeClass.Integer:
                        Console.WriteLine(sIndent + "->V: " + att.Read<int>());
                        break;
                    case DataTypeClass.String:
                        Console.WriteLine(sIndent + "->V: " + att.Read());
                        break;
                    case DataTypeClass.Float:
                        Console.WriteLine(sIndent + "->V: " + att.Read<double>());
                        break;
                }
*/            }

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
        }

        static void HandleAttribute(H5Attribute attribute)
        {
            if(attribute is H5StringAttribute sa)
            {
                ProcessStringAttribute(sa);
            }
            else if(attribute is H5EnumAttribute<Test> ta) 
            {
                ProcessEnumAttribute(ta);
            }
            else if(attribute is H5PrimitiveAttribute<int> ti)
            {
                ProcessPrimitiveAttribute(ti);
            }

            void ProcessStringAttribute(H5StringAttribute attribute) { }
            void ProcessEnumAttribute<T>(H5EnumAttribute<T> attribute) where T : unmanaged, Enum{ }
            void ProcessPrimitiveAttribute<T>(H5PrimitiveAttribute<T> attribute) where T : unmanaged { }
        }
    }

    enum Test { }

    [H5Contract("AType")]
    public class SomeType
    {
        [H5Member(name: "AProperty")]
        public int IntProperty { get; set; }

        [H5StringMember(characterSet: CharacterSet.Utf8, allocatedStorageInBytes: 1000)]
        public string StringProperty { get; set; } = string.Empty;
    }
}