using HDF5Test.H5TypeHelpers;
using System;

namespace HDF5Test
{
    static class Program
    {
        static void Main()
        {
            try
            {
                // TODO: move to unit tests
                //using var intervalRecordType = IntervalRecordHelper.CreateH5Type();
                //using var measurementConfigurationType = MeasurementConfigurationHelper.CreateH5Type();
                //using var profileType = ProfileHelper.CreateH5Type();
                //using var rawRecordType = RawRecordHelper.CreateH5Type();

                CreateFileTestRealData2.CreateFile();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
