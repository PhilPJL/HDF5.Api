using System;

namespace HDF5Test
{
    static class Program
    {
        static void Main()
        {
            try
            {
                CreateFileTestRealData.CreateFile();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
