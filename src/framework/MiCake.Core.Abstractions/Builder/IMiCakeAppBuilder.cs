﻿using Microsoft.Extensions.DependencyInjection;
using System;

namespace MiCake.Core.Builder
{
    /// <summary>
    /// a builder for <see cref="IMiCakeApplication"/>
    /// </summary>
    public interface IMiCakeAppBuilder
    {
        /// <summary>
        /// Build an <see cref="IMiCakeApplication"/>
        /// </summary>
        /// <returns></returns>
        IMiCakeApplication Build();

        /// <summary>
        /// Adds a delegate for configuring micake application.
        /// </summary>
        /// <param name="configureServices">A delegate for configuring the <see cref="IMiCakeApplication"/>.</param>
        /// <returns>The <see cref="IMiCakeAppBuilder"/>.</returns>
        IMiCakeAppBuilder ConfigureApplication(Action<IMiCakeApplication> configureApp);

        /// <summary>
        /// Adds a delegate for configuring micake application and services.
        /// </summary>
        /// <param name="configureApp">A delegate for configuring the <see cref="IMiCakeApplication"/> and <see cref="IServiceCollection"/></param>
        /// <returns><see cref="IMiCakeAppBuilder"/></returns>
        IMiCakeAppBuilder ConfigureApplication(Action<IMiCakeApplication, IServiceCollection> configureApp);
    }
}
