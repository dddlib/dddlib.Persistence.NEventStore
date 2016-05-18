// <copyright file="NEventStore.cs" company="dddlib contributors">
//  Copyright (c) dddlib contributors. All rights reserved.
// </copyright>

namespace dddlib.Persistence.NEventStore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using dddlib.Persistence.Sdk;
    using global::NEventStore;

    /// <summary>
    /// Represents the NEventStore event store.
    /// </summary>
    public sealed class NEventStore : IEventStore
    {
        private readonly IStoreEvents eventStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="NEventStore"/> class.
        /// </summary>
        /// <param name="eventStore">The NEventStore event storage implementation.</param>
        [CLSCompliant(false)]
        public NEventStore(IStoreEvents eventStore)
        {
            Guard.Against.Null(() => eventStore);

            this.eventStore = eventStore;
        }

        /// <summary>
        /// Gets the events for a stream.
        /// </summary>
        /// <param name="streamId">The stream identifier.</param>
        /// <param name="streamRevision">The stream revision to get the events from.</param>
        /// <param name="state">The state of the steam.</param>
        /// <returns>The events.</returns>
        public IEnumerable<object> GetStream(Guid streamId, int streamRevision, out string state)
        {
            using (var stream = this.eventStore.OpenStream(streamId, streamRevision, int.MaxValue))
            {
                state = stream.CommitSequence.ToString();

                return stream.CommittedEvents.Select(@event => @event.Body);
            }
        }

        /// <summary>
        /// Commits the events to a stream.
        /// </summary>
        /// <param name="streamId">The stream identifier.</param>
        /// <param name="events">The events to commit.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="preCommitState">The pre-commit state of the stream.</param>
        /// <param name="postCommitState">The post-commit state of stream.</param>
        public void CommitStream(Guid streamId, IEnumerable<object> events, Guid correlationId, string preCommitState, out string postCommitState)
        {
            Guard.Against.Null(() => events);

            using (var stream = this.eventStore.OpenStream(streamId, 0, int.MaxValue))
            {
                if (preCommitState != null && preCommitState != stream.CommitSequence.ToString())
                {
                    throw new ConcurrencyException();
                }

                foreach (var @event in events)
                {
                    stream.Add(new EventMessage { Body = @event });
                }

                stream.CommitChanges(correlationId);

                postCommitState = stream.CommitSequence.ToString();
            }
        }
    }
}
