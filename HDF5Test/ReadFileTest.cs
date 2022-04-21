using HDF5Api;
using System;
using System.Threading.Tasks;

namespace HDF5Test
{
    internal static class ReadFileTest
    {
        internal static async Task ReadFileAsync(string path)
        {
            await Task.Yield();

            using (var file = H5File.OpenReadOnly(path))
            {
                {
                    using var classAttribute = file.OpenAttribute("CLASS");
                    using var formatVersionAttribute = file.OpenAttribute("PYTABLES_FORMAT_VERSION");
                    using var titleAttribute = file.OpenAttribute("TITLE");
                    using var versionAttribute = file.OpenAttribute("VERSION");

                    Console.WriteLine(classAttribute.ReadString());
                    Console.WriteLine(formatVersionAttribute.ReadString());
                    Console.WriteLine(titleAttribute.ReadString());
                    Console.WriteLine(versionAttribute.ReadString());
                }

                foreach (var groupName in file.GetChildNames())
                {
                    Console.WriteLine(groupName);

                    using (var group = file.OpenGroup(groupName))
                    {
                        {
                            using var classAttribute = group.OpenAttribute("CLASS");
                            using var titleAttribute = group.OpenAttribute("TITLE");
                            using var versionAttribute = group.OpenAttribute("VERSION");
                            using var deltaFreqAttribute = group.OpenAttribute("delta_freq");
                            using var identifierAttribute = group.OpenAttribute("identifier");
                            using var originIdAttribute = group.OpenAttribute("orig_id");

                            Console.WriteLine(classAttribute.ReadString());
                            Console.WriteLine(titleAttribute.ReadString());
                            Console.WriteLine(versionAttribute.ReadString());
                            Console.WriteLine(deltaFreqAttribute.ReadDouble());
                            Console.WriteLine(identifierAttribute.ReadString());
                            Console.WriteLine(originIdAttribute.ReadInt32());
                        }

                        foreach (var dataSetName in group.GetChildNames())
                        {
                            Console.WriteLine($"/{groupName}/{dataSetName}");

                            using (var dataSet = group.OpenDataSet(dataSetName))
                            {
                                using var classAttribute = dataSet.OpenAttribute("CLASS");
                                using var flavorAttribute = dataSet.OpenAttribute("FLAVOR");
                                using var titleAttribute = dataSet.OpenAttribute("TITLE");
                                using var versionAttribute = dataSet.OpenAttribute("VERSION");
                                using var profileValueAttribute = dataSet.OpenAttribute("profile_value");

                                Console.WriteLine(classAttribute.ReadString());
                                Console.WriteLine(flavorAttribute.ReadString());
                                Console.WriteLine(titleAttribute.ReadString());
                                Console.WriteLine(versionAttribute.ReadString());
                                Console.WriteLine(profileValueAttribute.ReadDouble());
                            }
                        }
                    }
                }
            }
        }
    }
}

