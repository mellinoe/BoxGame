using EngineCore.Components;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EngineCore.Services
{
    internal class ServiceInjector
    {
        private readonly Action<ServiceRegistry, Component> _injector;

        public ServiceInjector(Type componentType)
        {
            _injector = GetServiceInjectionDelegate(componentType);
        }

        public void InjectServices(ServiceRegistry registry, Component component)
        {
            _injector(registry, component);
        }

        private static Action<ServiceRegistry, Component> GetServiceInjectionDelegate(Type componentType)
        {
            var fields = componentType.GetFields().Where(HasInjectorProperty).ToArray();
            var properties = componentType.GetProperties().Where(HasInjectorProperty).ToArray();

            if (fields.Length == 0 && properties.Length == 0)
            {
                return (sr, o) => { };
            }

            Expression[] expressions = new Expression[fields.Length + properties.Length];

            var registryParamExpr = Expression.Parameter(typeof(ServiceRegistry));
            var componentParamExpr = Expression.Parameter(typeof(Component));

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                var typedParamExpr = Expression.Convert(componentParamExpr, componentType);
                var fieldExpr = Expression.Field(typedParamExpr, field);
                var fieldType = field.FieldType;
                var fieldTypeExpr = Expression.Constant(fieldType);
                var callExpr = Expression.Call(registryParamExpr, typeof(ServiceRegistry).GetMethod(nameof(ServiceRegistry.GetService)), fieldTypeExpr);
                var casted = Expression.Convert(callExpr, fieldType);
                var assignExpr = Expression.Assign(fieldExpr, casted);

                expressions[i] = assignExpr;
            }

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                var typedParamExpr = Expression.TypeAs(componentParamExpr, componentType);
                var propertyExpr = Expression.Property(typedParamExpr, property);
                var propertyType = property.PropertyType;
                var propertyTypeExpr = Expression.Constant(propertyType);
                var callExpr = Expression.Call(registryParamExpr, typeof(ServiceRegistry).GetMethod(nameof(ServiceRegistry.GetService)), propertyTypeExpr);
                var casted = Expression.Convert(callExpr, propertyType);
                var assignExpr = Expression.Assign(propertyExpr, casted);

                expressions[fields.Length + i] = assignExpr;
            }

            var allExpressions = Expression.Block(expressions);

            return Expression.Lambda<Action<ServiceRegistry, Component>>(allExpressions, registryParamExpr, componentParamExpr).Compile();
        }

        private static bool HasInjectorProperty(MemberInfo mi) => mi.IsDefined(typeof(AutoInjectAttribute));
    }
}
