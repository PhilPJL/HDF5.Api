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

#if DEBUG
                if(H5Handle.Handles.Count > 0)
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
