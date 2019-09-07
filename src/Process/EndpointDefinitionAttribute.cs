namespace Process
{
    using System;

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
        public EndpointDefinitionAttribute(Method method, string endpoint)
        {
            Method = method;
            Endpoint = endpoint;
        }

        public Method Method { get; }
        public string Endpoint { get; }
    }
}
