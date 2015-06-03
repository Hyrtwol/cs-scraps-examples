using System;
using System.ComponentModel.Design;

namespace Scraps
{
    public static class ServiceLocatorExtensions
    {
        public static T GetService<T>(this IServiceProvider serviceProvider) where T : class
        {
            return serviceProvider.GetService(typeof(T)) as T;
        }

        public static T GetServiceOrThrow<T>(this IServiceProvider serviceProvider) where T : class
        {
            var result = serviceProvider.GetService(typeof(T)) as T;
            if (result == null) throw new Exception(string.Format("Service Not Found: {0}", typeof(T)));
            return result;
        }

        /// <summary>
        /// Adds the specified service to the service container. 
        /// </summary>
        public static void AddService<T>(this IServiceContainer gameServiceContainer, T serviceInstance) where T : class
        {
            gameServiceContainer.AddService(typeof(T), serviceInstance);
        }

        /// <summary>
        /// Adds the specified service to the service container, and optionally promotes the service to any parent service containers.
        /// </summary>
        public static void AddService<T>(this IServiceContainer gameServiceContainer, T serviceInstance, bool promote) where T : class
        {
            gameServiceContainer.AddService(typeof(T), serviceInstance, promote);
        }

        /// <summary>
        /// Adds the specified service to the service container.
        /// </summary>
        public static void AddService<T>(this IServiceContainer gameServiceContainer, ServiceCreatorCallback callback) where T : class
        {
            gameServiceContainer.AddService(typeof(T), callback);
        }

        /// <summary>
        /// Adds the specified service to the service container, and optionally promotes the service to parent service containers.
        /// </summary>
        public static void AddService<T>(this IServiceContainer gameServiceContainer, ServiceCreatorCallback callback, bool promote) where T : class
        {
            gameServiceContainer.AddService(typeof(T), callback, promote);
        }

        /// <summary>
        /// Removes the specified service type from the service container. 
        /// </summary>
        public static void RemoveService<T>(this IServiceContainer gameServiceContainer) where T : class
        {
            gameServiceContainer.RemoveService(typeof(T));
        }

        /// <summary>
        /// Removes the specified service type from the service container, and optionally promotes the service to parent service containers.
        /// </summary>
        public static void RemoveService<T>(this IServiceContainer gameServiceContainer, bool promote) where T : class
        {
            gameServiceContainer.RemoveService(typeof(T), promote);
        }
    }
}