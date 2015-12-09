using System;
using System.Collections.Generic;
using System.Reflection;

namespace EngineCore.Services
{
    internal class ServiceRegistry
    {
        private const int InitialServiceCapacity = 20;
        private readonly Dictionary<Type, Service> s_registeredServices = new Dictionary<Type, Service>(InitialServiceCapacity);

        public void RegisterService<T>(Service service)
        {
            Type registrationType = typeof(T);
            if (!registrationType.GetTypeInfo().IsAssignableFrom(service.GetType().GetTypeInfo()))
            {
                throw new InvalidOperationException($"The service {service} was not compatible with type {registrationType.FullName}.");
            }

            if (s_registeredServices.ContainsKey(registrationType))
            {
                throw new InvalidOperationException($"A service of type {registrationType.FullName} is already registered.");
            }

            s_registeredServices.Add(registrationType, service);
        }

        public T GetService<T>()
        {
            Service service;
            if (!s_registeredServices.TryGetValue(typeof(T), out service))
            {
                throw new InvalidOperationException($"No service of type {typeof(T).FullName} is registered.");
            }

            return (T)(object)service;
        }
    }
}
