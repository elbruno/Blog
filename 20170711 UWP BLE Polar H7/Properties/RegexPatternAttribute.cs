using System;

namespace ElBruno.PolarH7.Annotations
{
    /// <summary>
    /// Indicates that parameter is regular expression pattern.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class RegexPatternAttribute : Attribute { }
}