using EngineCore.Components;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace EngineCore.Services
{
    internal class ServiceRegistry
    {
        private const int InitialServiceCapacity = 20;
        private readonly Dictionary<Type, object> s_registeredServices = new Dictionary<Type, object>(InitialServiceCapacity);
        private readonly Dictionary<Type, ServiceInjector> s_injectors = new Dictionary<Type, ServiceInjector>();

        public void RegisterService<T>(T service) => RegisterService(typeof(T), service);

        public void RegisterService(Type registrationType, object service)
        {
            if (!registrationType.GetTypeInfo().IsAssignableFrom(service.GetType().GetTypeInfo()))
            {
                throw new InvalidOperationException(
                    $"The service {service} was not compatible with registration type {registrationType.FullName}.");
            }

            if (s_registeredServices.ContainsKey(registrationType))
            {
                throw new InvalidOperationException(
                    $"A service of type {registrationType.FullName} is already registered.");
            }

            s_registeredServices.Add(registrationType, service);
        }

        public object GetService(Type serviceType)
        {
            object service;
            if (!s_registeredServices.TryGetValue(serviceType, out service))
            {
                throw new InvalidOperationException($"No service of type {serviceType.FullName} is registered.");
            }

            return service;
        }

        /*
        public T GetService<T>()
        {
            object service;
            if (!s_registeredServices.TryGetValue(typeof(T), out service))
            {
                throw new InvalidOperationException($"No service of type {typeof(T).FullName} is registered.");
            }

            return (T)service;
        }
        */

        public void InjectServices(Component component)
        {
            var injector = GetInjector(component.GetType());
            injector.InjectServices(this, component);
        }

        private ServiceInjector GetInjector(Type componentType)
        {
            ServiceInjector injector;
            if (!s_injectors.TryGetValue(componentType, out injector))
            {
                injector = new ServiceInjector(componentType);
                s_injectors.Add(componentType, injector);
            }

            return injector;
        }
    }
}
