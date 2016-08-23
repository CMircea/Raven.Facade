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
    public sealed class RavenScriptedPatchWithParameters
    {
        private readonly string _script;
        private readonly string _documentId;
        private readonly IEnumerable<KeyValuePair<string, object>> _parameters;

        /*\ ***** ***** ***** ***** ***** Constructor ***** ***** ***** ***** ***** \*/
        internal RavenScriptedPatchWithParameters([NotNull] string documentId, [NotNull] string script, [NotNull] IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (documentId == null)
                throw new ArgumentNullException(nameof(documentId));

            if (script == null)
                throw new ArgumentNullException(nameof(script));

            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            _script     = script;
            _documentId = documentId;
            _parameters = parameters;
        }

        /*\ ***** ***** ***** ***** ***** Public Methods ***** ***** ***** ***** ***** \*/
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
            var patch = new ScriptedPatchRequest
            {
                Script = _script,
            };

            foreach (var pair in _parameters)
                patch.Values.Add(pair.Key, pair.Value);

            return new ScriptedPatchCommandData
            {
                Key   = _documentId,
                Etag  = etag,
                Patch = patch,
            };
        }
    }
}
