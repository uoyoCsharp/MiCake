﻿using MiCake.Core.Util;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MiCake.Core.Handlers
{
    /// <summary>
    /// A micake handler that can use other services from an <see cref="IServiceProvider"/>.
    /// </summary>
    public class ServiceHandler : IMiCakeHandlerFactory
    {
        public bool IsReusable { get; private set; }

        public Type HandlerType { get; set; }

        public ServiceHandler(Type handlerType)
        {
            CheckValue.NotNull(handlerType, nameof(handlerType));

            HandlerType = handlerType;
        }

        public IMiCakeHandler CreateInstance(IServiceProvider serviceProvider)
        {
            CheckValue.NotNull(serviceProvider, nameof(serviceProvider));

            var service = serviceProvider.GetRequiredService(HandlerType);

            var handler = service as IMiCakeHandler;
            if (handler == null)
            {
                throw new InvalidOperationException($"The service typpe : [{HandlerType.FullName}] is not in IServiceProvider");
            }

            return handler;
        }
    }
}
