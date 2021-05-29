using System;
using System.Collections.Generic;
using System.Text;

namespace LitJson
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class JsonSkipAttribute : Attribute
    {
    }
}