// <copyright file="NEventStoreRepository.cs" company="dddlib contributors">
//  Copyright (c) dddlib contributors. All rights reserved.
// </copyright>

namespace dddlib.Persistence.NEventStore
{
    using System;
    using dddlib.Persistence.Sdk;
    using dddlib.Persistence.SqlServer;
    using global::NEventStore;

    /// <summary>
    /// Represents an NEventStore event store repository.
    /// </summary>
    /// <seealso cref="dddlib.Persistence.Sdk.EventStoreRepository" />
    public sealed class NEventStoreRepository : EventStoreRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NEventStoreRepository"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="store">The NEventStore.</param>
        public NEventStoreRepository(string connectionString, IStoreEvents store)
            : base(
                new SqlServerIdentityMap(connectionString),
                new NEventStore(store),
                new SqlServerSnapshotStore(connectionString))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NEventStoreRepository"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="store">The NEventStore.</param>
        public NEventStoreRepository(string connectionString, string schema, IStoreEvents store)
            : base(
                new SqlServerIdentityMap(connectionString, schema),
                new NEventStore(store), 
                new SqlServerSnapshotStore(connectionString, schema))
        {
        }
    }
}
