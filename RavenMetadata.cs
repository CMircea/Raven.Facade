using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Abstractions.Data;
using Raven.Json.Linq;

namespace Raven.Facade
{
    public sealed class RavenMetadata
    {
        /*\ ***** ***** ***** ***** ***** Properties ***** ***** ***** ***** ***** \*/
        public string Key
        {
            get;
            set;
        }

        public Etag Etag
        {
            get;
            set;
        }

        public DateTimeOffset? LastModified
        {
            get;
            set;
        }

        public bool NonAuthoritativeInformation
        {
            get;
            set;
        }

        /*\ ***** ***** ***** ***** ***** Constructors ***** ***** ***** ***** ***** \*/
        public RavenMetadata(RavenJObject metadata)
        {
            Key = metadata.Value<string>("@id");

            var etag             = metadata.Value<string>("@etag");
            var lastModified     = metadata.Value<DateTimeOffset?>("Raven-Last-Modified") ?? metadata.Value<DateTimeOffset?>("Last-Modified");
            var nonAuthoritative = metadata.Value<bool?>("Non-Authoritative-Information");

            if (etag != null)
                Etag = new Etag(etag);

            if (lastModified != null)
                LastModified = lastModified.Value;

            if (nonAuthoritative != null)
                NonAuthoritativeInformation = nonAuthoritative.Value;
        }


        public RavenMetadata(Etag etag, RavenJObject metadata)
        {
            Etag = etag;

            if (metadata != null)
            {
                Key = metadata.Value<string>("@id");

                var lastModified     = metadata.Value<DateTimeOffset?>("Raven-Last-Modified") ?? metadata.Value<DateTimeOffset?>("Last-Modified");
                var nonAuthoritative = metadata.Value<bool?>("Non-Authoritative-Information");

                if (etag != null)
                    Etag = new Etag(etag);

                if (lastModified != null)
                    LastModified = lastModified.Value;

                if (nonAuthoritative != null)
                    NonAuthoritativeInformation = nonAuthoritative.Value;
            }
        }

        public RavenMetadata(IJsonDocumentMetadata metadata)
        {
            Key  = metadata.Key;
            Etag = metadata.Etag;

            if (metadata.LastModified != null)
                LastModified = metadata.LastModified.Value;

            if (metadata.NonAuthoritativeInformation != null)
                NonAuthoritativeInformation = metadata.NonAuthoritativeInformation.Value;
        }
    }
}
