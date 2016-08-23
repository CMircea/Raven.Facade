using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Raven.Abstractions.Commands;
using Raven.Abstractions.Data;

namespace Raven.Facade.Patching
{
    public sealed class RavenScriptedPatchWithScript
    {
        private readonly string _script;
        private readonly string _documentId;

        /*\ ***** ***** ***** ***** ***** Constructor ***** ***** ***** ***** ***** \*/
        internal RavenScriptedPatchWithScript([NotNull] string documentId, [NotNull] string script)
        {
            if (documentId == null)
                throw new ArgumentNullException(nameof(documentId));

            if (script == null)
                throw new ArgumentNullException(nameof(script));

            _script     = script;
            _documentId = documentId;
        }

        /*\ ***** ***** ***** ***** ***** Public Methods ***** ***** ***** ***** ***** \*/
        [Pure]
        [NotNull]
        public RavenScriptedPatchWithParameters Parameters([NotNull] object parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            return new RavenScriptedPatchWithParameters(_documentId, _script, parameters: EnumerateObjectProperties(parameters));
        }

        [Pure]
        [NotNull]
        public RavenScriptedPatchWithParameters Parameters([NotNull] IDictionary<string, object> parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            return new RavenScriptedPatchWithParameters(_documentId, _script, parameters);
        }

        [Pure]
        [NotNull]
        public RavenScriptedPatchWithParameters Parameters([NotNull] IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            return new RavenScriptedPatchWithParameters(_documentId, _script, parameters);
        }

        [Pure]
        [NotNull]
        public ICommandData Build()
        {
            return Build(etag: null);
        }

        [Pure]
        [NotNull]
        public ICommandData Build([CanBeNull] Etag etag)
        {
            return new ScriptedPatchCommandData
            {
                Key   = _documentId,
                Etag  = etag,
                Patch = new ScriptedPatchRequest
                {
                    Script = _script,
                },
            };
        }

        /*\ ***** ***** ***** ***** ***** Private Methods ***** ***** ***** ***** ***** \*/
        [Pure]
        [NotNull]
        private IEnumerable<KeyValuePair<string, object>> EnumerateObjectProperties([NotNull] object parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            return from property in parameters.GetType().GetProperties()
                   let name  = property.Name
                   let value = property.GetValue(parameters)
                   select new KeyValuePair<string, object>(name, value);
        }
    }
}
