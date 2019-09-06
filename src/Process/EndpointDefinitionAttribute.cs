using System;

namespace Process
{
    public enum Method
    {
        Get,
        Post,
        Put,
        Delete
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class EndpointDefinitionAttribute : Attribute
    {
        public Method Method { get; }
        public string Endpoint { get; }

        public EndpointDefinitionAttribute(Method method, string endpoint)
        {
            Method = method;
            Endpoint = endpoint;
        }
    }
}
