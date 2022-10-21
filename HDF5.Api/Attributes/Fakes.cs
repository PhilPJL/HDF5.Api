﻿// Fake attributes available in .NET7 but not .NET Standard

#if NETSTANDARD
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    internal class DisallowNull : Attribute
    {
    }
}

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    internal class AllowNull : Attribute
    {
    }
}
#endif