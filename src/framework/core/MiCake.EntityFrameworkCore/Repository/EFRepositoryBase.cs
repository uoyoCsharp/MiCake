﻿using MiCake.DDD.Domain;
using MiCake.EntityFrameworkCore.Uow;
using MiCake.Uow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MiCake.EntityFrameworkCore.Repository
{
    /// <summary>
    /// a base repository for EFCore
    /// </summary>
    public abstract class EFRepositoryBase<TDbContext, TEntity, TKey>
         where TEntity : class, IEntity<TKey>
         where TDbContext : DbContext
    {
        /// <summary>
        /// Use to get need services.
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }

        protected TDbContext? CurrentDbContext { get; private set; }

        protected DbSet<TEntity> DbSet => CurrentDbContext!.Set<TEntity>();

        public EFRepositoryBase(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;

            var currentTransactionObjs = serviceProvider.GetService<IUnitOfWorkManager>()?.GetCurrentUnitOfWork()?.GetTransactionObjects();
            if (currentTransactionObjs == null || currentTransactionObjs.Count == 0)
            {
                throw new InvalidOperationException($"Can not get {nameof(TDbContext)} from {nameof(IUnitOfWorkManager)},please check the {nameof(EFRepositoryBase<TDbContext, TEntity, TKey>)} is used in the right way.");
            }

            var currentEfCoreDbContext = currentTransactionObjs.Where(s => s is EFCoreTransactionObject).FirstOrDefault();
            if (currentTransactionObjs == null)
            {
                throw new InvalidOperationException($"Get {currentTransactionObjs!.Count} transaction object,but there have no {nameof(EFCoreTransactionObject)}.");
            }

            CurrentDbContext = (TDbContext)currentEfCoreDbContext!.TransactionInstance;
        }
    }
}
