using System;

namespace Process
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EndpointDefinitionAttribute : Attribute
    {
        public string Method { get; }
        public string Endpoint { get; }

        public EndpointDefinitionAttribute(string method, string endpoint)
        {
            Method = method;
            Endpoint = endpoint;
        }
    }
}