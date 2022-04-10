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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
