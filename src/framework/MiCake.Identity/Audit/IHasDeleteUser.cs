﻿using MiCake.Identity;

namespace MiCake.Audit
{
    /// <summary>
    /// Represents has a delete user. If audit is enabled, it will be assigned to the audited system later.
    /// </summary>
    /// <typeparam name="TKey">the primary key type of user.Must be consistent with <see cref="IMiCakeUser{TKey}"/></typeparam>
    public interface IHasDeleteUser<TKey> : IHasAuditUser
    {
        /// <summary>
        /// The primary key for user.
        /// </summary>
        TKey DeleteUserID { get; set; }
    }

    /// <summary>
    /// Represents has a delete user. If audit is enabled, it will be assigned to the audited system later.
    /// </summary>
    /// <typeparam name="TKey">the primary key type of user.Must be consistent with <see cref="IMiCakeUser{TKey}"/></typeparam>
    public interface IMayHasDeleteUser<TKey> : IHasAuditUser where TKey : struct
    {
        /// <summary>
        /// The primary key for user.
        /// </summary>
        TKey? DeleteUserID { get; set; }
    }
}
