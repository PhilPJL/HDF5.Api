using HDF5Api;
using System;

namespace HDF5Test
{
    static class Program
    {
        static void Main()
        {
            try
            {
                CreateFileTest.CreateFile();

                Console.WriteLine($"Unclosed handles: {H5Handle.HandleCount}.");

                if(H5Handle.HandleCount > 0)
                {
                    foreach (var item in H5Handle.Handles)
                    {
                        Console.WriteLine("-----------------------------");
                        Console.WriteLine($"{item.Key}");
                        Console.WriteLine(item.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
