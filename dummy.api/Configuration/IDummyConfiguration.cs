using dummy.api.Exceptions;
using Flurl;
using Microsoft.Extensions.Configuration;

namespace dummy.api.Configuration
{
    public interface IDummyConfiguration
    {
        Url SomeHost { get; }
    }

    public class DummyConfiguration : IDummyConfiguration
    {
        public DummyConfiguration(IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration[PropertyName.Hosts.Some]))
            {
                throw new MissingConfigurationException(PropertyName.Hosts.Some);
            }

            this.SomeHost = configuration[PropertyName.Hosts.Some];
        }

        public Url SomeHost { get; }

        private static class PropertyName
        {
            internal static class Hosts
            {
                internal const string Some = "Hosts_Some";
            }
        }
    }
}