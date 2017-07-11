using System;

namespace ElBruno.PolarH7.Annotations
{
    /// <summary>
    /// Indicates that IEnumerable, passed as parameter, is not enumerated.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class NoEnumerationAttribute : Attribute { }
}