using System;

namespace dummy.api.Exceptions
{
    /// <summary>
    /// Missing Configuration Exception.
    /// </summary>
    [Serializable]
    public class MissingConfigurationException : DummyException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingConfigurationException"/> class.
        /// </summary>
        /// <param name="configPropertyName">Configuration Property Name.</param>
        public MissingConfigurationException(string configPropertyName)
            : base($"Missing configuration property {configPropertyName}")
        {
        }
    }
}