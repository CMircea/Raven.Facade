using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Raven.Abstractions.Data;
using Raven.Abstractions.Replication;
using Raven.Client;
using Raven.Client.Document;
using Raven.Facade.Attributes;
using Raven.Imports.Newtonsoft.Json;

namespace Raven.Facade
{
    public static class RavenStore
    {
        private static IDocumentStore _instance;

        /*\ ***** ***** ***** ***** ***** Properties ***** ***** ***** ***** ***** \*/
        [NotNull]
        public static IDocumentStore Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("The document store has not been initialized.");

                return _instance;
            }
        }

        /*\ ***** ***** ***** ***** ***** Public Methods ***** ***** ***** ***** ***** \*/
        public static IDocumentStore Initialize([NotNull] string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            var options = ParseConnectionString(connectionString);
            var store   = CreateUninitializedDocumentStore();

            if (options.ResourceManagerId != Guid.Empty)
                store.ResourceManagerId = options.ResourceManagerId;

            if (options.Credentials != null)
                store.Credentials = options.Credentials;

            if (!String.IsNullOrEmpty(options.Url))
                store.Url = options.Url;

            if (!String.IsNullOrEmpty(options.DefaultDatabase))
                store.DefaultDatabase = options.DefaultDatabase;

            if (!String.IsNullOrEmpty(options.ApiKey))
                store.ApiKey = options.ApiKey;

            if (options.FailoverServers != null)
                store.FailoverServers = options.FailoverServers;

            store.EnlistInDistributedTransactions = options.EnlistInDistributedTransactions;
            store.Initialize();

            return _instance = store;
        }

        public static IDocumentStore Initialize([NotNull] string url, [NotNull] string databaseName, [CanBeNull] string apiKey)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            if (databaseName == null)
                throw new ArgumentNullException(nameof(databaseName));

            var store = CreateUninitializedDocumentStore();

            store.Url             = url;
            store.ApiKey          = apiKey;
            store.DefaultDatabase = databaseName;

            store.Initialize();

            return _instance = store;
        }

        /*\ ***** ***** ***** ***** ***** Private Methods ***** ***** ***** ***** ***** \*/
        private static DocumentStore CreateUninitializedDocumentStore()
        {
            return new DocumentStore
            {
                Conventions =
                {
                    FailoverBehavior = FailoverBehavior.FailImmediately,

                    FindTypeTagName                         = FindTypeTagName,
                    TransformTypeTagNameToDocumentKeyPrefix = TransformTypeTagNameToDocumentKeyPrefix,

                    CustomizeJsonSerializer = serializer =>
                    {
                        serializer.DateTimeZoneHandling  = DateTimeZoneHandling.Utc;
                        serializer.DateFormatHandling    = DateFormatHandling.IsoDateFormat;
                        serializer.MissingMemberHandling = MissingMemberHandling.Ignore;
                        serializer.NullValueHandling     = NullValueHandling.Ignore;
                    },
                },
            };
        }

        private static RavenConnectionStringOptions ParseConnectionString([NotNull] string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            var parser = ConnectionStringParser<RavenConnectionStringOptions>.FromConnectionString(connectionString);

            try
            {
                parser.Parse();
            }
            catch (ArgumentException e)
            {
                throw new ArgumentOutOfRangeException(nameof(connectionString), connectionString, e.Message);
            }

            return parser.ConnectionStringOptions;
        }

        private static string FindTypeTagName(Type type)
        {
            var collectionAttribute = type.GetCustomAttribute<CollectionAttribute>();

            if (collectionAttribute == null)
            {
                return DocumentConvention.DefaultTypeTagName(type);
            }
            else
            {
                return collectionAttribute.Name;
            }
        }

        private static string TransformTypeTagNameToDocumentKeyPrefix(string name)
        {
            var builder = new StringBuilder(name);

            for (int i = 0; i < builder.Length; i++)
            {
                if (Char.IsUpper(builder[i]))
                {
                    builder[i] = Char.ToLower(builder[i]);

                    if (i != 0 && Char.IsLower(builder[i - 1]))
                        builder.Insert(i, '-');
                }
            }

            return builder.ToString();
        }
    }
}
