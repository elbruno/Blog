using System;

namespace ElBruno.PolarH7.Annotations
{
    /// <summary>
    /// Indicates that marked element should be localized or not.
    /// </summary>
    /// <example><code>
    /// [LocalizationRequiredAttribute(true)]
    /// class Foo {
    ///   string str = "my string"; // Warning: Localizable string
    /// }
    /// </code></example>
    [AttributeUsage(AttributeTargets.All)]
    public sealed class LocalizationRequiredAttribute : Attribute
    {
        public LocalizationRequiredAttribute() : this(true) { }
        public LocalizationRequiredAttribute(bool required)
        {
            Required = required;
        }

        public bool Required { get; private set; }
    }
}