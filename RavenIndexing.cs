using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Raven.Client;
using Raven.Client.Indexes;

namespace Raven.Facade
{
    public static class RavenIndexing
    {
        public static Task CreateIndexesAsync([NotNull] IDocumentStore documentStore)
        {
            if (documentStore == null)
                throw new ArgumentNullException(nameof(documentStore));

            return IndexCreation.CreateIndexesAsync(Assembly.GetExecutingAssembly(), documentStore);
        }
    }
}
