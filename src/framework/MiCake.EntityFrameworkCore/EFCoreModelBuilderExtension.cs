﻿using MiCake.Core.DependencyInjection;
using MiCake.DDD.Extensions.Store.Configure;
using MiCake.EntityFrameworkCore.Internal;
using MiCake.EntityFrameworkCore.Interprets;
using Microsoft.EntityFrameworkCore;

namespace MiCake.EntityFrameworkCore
{
    public static class EFCoreModelBuilderExtension
    {
        /// <summary>
        /// Add MiCake manage model for EFCore.
        /// If you don't inherit <see cref="MiCakeDbContext"/>, you can use this extension method in your DbContent OnModelCreating().
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ModelBuilder AddMiCakeModel(this ModelBuilder builder)
        {
            //EFCore will cached model info.This method only called once
            var storeModelExpression = new EFModelExpressionProvider().GetExpression();
            storeModelExpression.Interpret(StoreConfig.Instance.GetStoreModel(), builder);

            return builder;
        }

        /// <summary>
        /// Add MiCake configure for EFCore.(include repository lifetime etc.)
        /// If you don't inherit <see cref="MiCakeDbContext"/>, you can use this extension method in your DbContent OnConfiguring().
        /// </summary>
        /// <param name="optionsBuilder"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder AddMiCakeConfigure(this DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(new MiCakeEFCoreInterceptor(ServiceLocator.Instance.Locator));
            return optionsBuilder;
        }
    }
}
