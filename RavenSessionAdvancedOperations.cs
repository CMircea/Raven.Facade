using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Abstractions.Commands;
using Raven.Abstractions.Data;
using Raven.Abstractions.Util;
using Raven.Client;
using Raven.Client.Indexes;
using Raven.Client.Linq;
using Raven.Facade.Documents;

namespace Raven.Facade
{
    public sealed class RavenSessionAdvancedOperations
    {
        private readonly IAsyncDocumentSession _session;

        /*\ ***** ***** ***** ***** ***** Constructor ***** ***** ***** ***** ***** \*/
        public RavenSessionAdvancedOperations(IAsyncDocumentSession session)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            _session = session;
        }

        /*\ ***** ***** ***** ***** ***** Public Methods ***** ***** ***** ***** ***** \*/
        public void Defer(params ICommandData[] commands)
        {
            if (commands == null)
                throw new ArgumentNullException(nameof(commands));

            _session.Advanced.Defer(commands);
        }

        public void Defer(IEnumerable<ICommandData> commands)
        {
            if (commands == null)
                throw new ArgumentNullException(nameof(commands));

            Defer(commands.ToArray());
        }

        public RavenMetadata GetMetadataFor<T>(T document)
            where T : IDocument
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            var etag     = _session.Advanced.GetEtagFor(document);
            var metadata = _session.Advanced.GetMetadataFor(document);

            if (etag == null && metadata == null)
            {
                return null;
            }
            else
            {
                return new RavenMetadata(etag, metadata);
            }
        }

        public IRavenQueryable<T> LinqQuery<T>()
            where T : IDocument
        {
            return _session.Query<T>();
        }

        public IRavenQueryable<T> LinqQuery<T, TIndex>()
            where TIndex : AbstractIndexCreationTask, new()
        {
            return _session.Query<T, TIndex>();
        }

        public IAsyncDocumentQuery<T> LuceneQuery<T>()
            where T : IDocument
        {
            return _session.Advanced.AsyncDocumentQuery<T>();
        }

        public IAsyncDocumentQuery<T> LuceneQuery<T, TIndex>()
            where TIndex : AbstractIndexCreationTask, new()
        {
            return _session.Advanced.AsyncDocumentQuery<T, TIndex>();
        }

        public Task RefreshAsync<T>(T document)
            where T : IDocument
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            return _session.Advanced.RefreshAsync(document);
        }

        public Task<IAsyncEnumerator<StreamResult<T>>> StreamAsync<T>(IQueryable<T> query)
        {
            return _session.Advanced.StreamAsync(query);
        }

        public Task<IAsyncEnumerator<StreamResult<T>>> StreamAsync<T>(IAsyncDocumentQuery<T> query)
        {
            return _session.Advanced.StreamAsync(query);
        }

        public Task<IAsyncEnumerator<StreamResult<T>>> StreamAsync<T>(string keyPrefix)
        {
            return _session.Advanced.StreamAsync<T>(keyPrefix);
        }
    }
}
