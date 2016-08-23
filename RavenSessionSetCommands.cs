using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Raven.Abstractions.Data;
using Raven.Client.Connection.Async;
using Raven.Client.Linq;
using Raven.Json.Linq;

namespace Raven.Facade
{
    public sealed class RavenSessionSetCommands
    {
        private readonly IAsyncDatabaseCommands _commands;

        /*\ ***** ***** ***** ***** ***** Constructor ***** ***** ***** ***** ***** \*/
        public RavenSessionSetCommands(IAsyncDatabaseCommands commands)
        {
            if (commands == null)
                throw new ArgumentNullException(nameof(commands));

            _commands = commands;
        }

        /*\ ***** ***** ***** ***** ***** Public Methods ***** ***** ***** ***** ***** \*/
        public async Task<IList<string>> DeleteAsync<T>(IRavenQueryable<T> query, bool allowStale)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var operation = await _commands.DeleteByIndexAsync(GetIndexName(query), GetIndexQuery(query), new BulkOperationOptions { AllowStale =  allowStale });
            var token     = await operation.WaitForCompletionAsync();
            var results   = token.Value<RavenJArray>();

            var ids = from document in results.Values<RavenJObject>()
                      where document.ContainsKey("Document")
                      select document["Document"].Value<string>();

            return ids.Distinct()
                      .ToList();
        }

        public Task<IList<string>> PatchAsync<T>(IRavenQueryable<T> query, PatchRequest patch, bool allowStale)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (patch == null)
                throw new ArgumentNullException(nameof(patch));

            return PatchAsync(query, new[] { patch }, allowStale);
        }

        public async Task<IList<string>> PatchAsync<T>(IRavenQueryable<T> query, IEnumerable<PatchRequest> patches, bool allowStale)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (patches == null)
                throw new ArgumentNullException(nameof(patches));

            var operation = await _commands.UpdateByIndexAsync(GetIndexName(query), GetIndexQuery(query), patches.ToArray(), new BulkOperationOptions { AllowStale = allowStale });
            var token     = await operation.WaitForCompletionAsync();
            var results   = token.Value<RavenJArray>();

            var ids = from document in results.Values<RavenJObject>()
                      where document.ContainsKey("Document")
                      select document["Document"].Value<string>();

            return ids.Distinct()
                      .ToList();
        }

        public async Task<IList<string>> PatchAsync<T>(IRavenQueryable<T> query, ScriptedPatchRequest patch, bool allowStale)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (patch == null)
                throw new ArgumentNullException(nameof(patch));

            var operation = await _commands.UpdateByIndexAsync(GetIndexName(query), GetIndexQuery(query), patch, new BulkOperationOptions { AllowStale =  allowStale });
            var token     = await operation.WaitForCompletionAsync();
            var results   = token.Value<RavenJArray>();

            var ids = from document in results.Values<RavenJObject>()
                      where document.ContainsKey("Document")
                      select document["Document"].Value<string>();

            return ids.Distinct()
                      .ToList();
        }

        /*\ ***** ***** ***** ***** ***** Private Methods ***** ***** ***** ***** ***** \*/
        private static string GetIndexName<T>([NotNull] IRavenQueryable<T> queryable)
        {
            return ((RavenQueryInspector<T>) queryable).AsyncIndexQueried;
        }

        private static IndexQuery GetIndexQuery<T>([NotNull] IRavenQueryable<T> queryable)
        {
            return ((RavenQueryInspector<T>) queryable).GetIndexQuery();
        }
    }
}
