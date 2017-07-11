using System;

namespace ElBruno.PolarH7.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AspTypePropertyAttribute : Attribute
    {
        public bool CreateConstructorReferences { get; private set; }

        public AspTypePropertyAttribute(bool createConstructorReferences)
        {
            CreateConstructorReferences = createConstructorReferences;
        }
    }
}