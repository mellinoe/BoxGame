using System;

namespace EngineCore.Services
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class AutoInjectAttribute : Attribute
    {
    }
}
