namespace MiCake.DDD.Domain
{
    /// <summary>
    /// <inheritdoc cref="IAggregateRoot{TKey}"/>
    /// </summary>
    public abstract class AggregateRoot : AggregateRoot<int>
    {
    }

    /// <summary>
    /// <inheritdoc cref="IAggregateRoot{TKey}"/>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot<TKey> where TKey : notnull
    {
    }
}
