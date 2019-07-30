using System;

namespace dummy.api.Exceptions
{
    public class ConfigurationDeserializationException : DummyException
    {
        public ConfigurationDeserializationException(string configPropertyName, Exception exception)
            : base($"Configuration deserialization error for configuration property {configPropertyName}", exception)
        {
        }
    }
}
