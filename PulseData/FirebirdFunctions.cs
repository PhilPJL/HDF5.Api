using EntityFramework.Functions;

namespace PulseData
{
    public static class FirebirdFunctions
    {
        [Function(FunctionType.BuiltInFunction, "OCTET_LENGTH")]
        public static int OctetLength(this byte[] _) => Function.CallNotSupported<int>();
    }
}
