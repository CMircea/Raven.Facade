using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Raven.Facade.Documents;

namespace Raven.Facade
{
    public sealed class RavenSession : IDisposable
    {
        private readonly IAsyncDocumentSession _async;

        /*\ ***** ***** ***** ***** ***** Properties ***** ***** ***** ***** ***** \*/
        public RavenSessionAdvancedOperations Advanced
        {
            get;
            private set;
        }

        public RavenSessionCommands Commands
        {
            get;
            private set;
        }

        /*\ ***** ***** ***** ***** ***** Constructor ***** ***** ***** ***** ***** \*/
        public RavenSession(IDocumentStore store)
        {
            _async = store.OpenAsyncSession();
            _async.Advanced.UseOptimisticConcurrency         = true;
            _async.Advanced.AllowNonAuthoritativeInformation = false;

            Advanced = new RavenSessionAdvancedOperations(_async);
            Commands = new RavenSessionCommands(store.AsyncDatabaseCommands);
        }

        /*\ ***** ***** ***** ***** ***** Public Methods ***** ***** ***** ***** ***** \*/
        public IAsyncLoaderWithInclude<T> Include<T>(Expression<Func<T, object>> path)
            where T : IDocument
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return _async.Include(path);
        }

        public Task<T> LoadAsync<T>(string id)
            where T : IDocument
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return _async.LoadAsync<T>(id);
        }

        public Task<T> LoadAsync<T, TTransformer>(string id)
            where TTransformer : AbstractTransformerCreationTask, new()
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return _async.LoadAsync<TTransformer, T>(id);
        }

        public Task<IList<T>> LoadAsync<T>(params string[] ids)
            where T : IDocument
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            return LoadAsync<T>((IEnumerable<string>) ids);
        }

        public Task<IList<T>> LoadAsync<T, TTransformer>(params string[] ids)
            where TTransformer : AbstractTransformerCreationTask, new()
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            return LoadAsync<T, TTransformer>((IEnumerable<string>) ids);
        }

        public async Task<IList<T>> LoadAsync<T>(IEnumerable<string> ids)
            where T : IDocument
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _async.LoadAsync<T>(ids.Distinct());

            return result.ToList();
        }

        public async Task<IList<T>> LoadAsync<T, TTransformer>(IEnumerable<string> ids)
            where TTransformer : AbstractTransformerCreationTask, new()
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _async.LoadAsync<TTransformer, T>(ids.Distinct().ToArray());

            return result.ToList();
        }

        public async Task<IList<T>> LoadStartingWithAsync<T>(string keyPrefix, int pageSize)
            where T : IDocument
        {
            if (keyPrefix == null)
                throw new ArgumentNullException(nameof(keyPrefix));

            var result = await _async.Advanced.LoadStartingWithAsync<T>(keyPrefix, pageSize: pageSize);

            return result.ToList();
        }

        public IQueryable<T> Query<T>()
            where T : IDocument
        {
            return _async.Query<T>();
        }

        public IQueryable<T> Query<T, TIndex>()
            where TIndex : AbstractIndexCreationTask, new()
        {
            return _async.Query<T, TIndex>();
        }

        public void DeleteOnSave<T>(T document)
            where T : IDocument
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            _async.Delete(document);
        }

        public void DeleteOnSave(string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            _async.Delete(id);
        }

        public Task SaveChangesAsync()
        {
            return _async.SaveChangesAsync();
        }

        public Task StoreAsync<T>(T document)
            where T : IDocument
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            return _async.StoreAsync(document);
        }

        public void Dispose()
        {
            _async.Dispose();
        }
    }
}
