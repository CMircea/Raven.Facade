using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Raven.Facade.Patching
{
    public sealed class RavenScriptedPatch
    {
        private readonly string _documentId;

        /*\ ***** ***** ***** ***** ***** Constructor ***** ***** ***** ***** ***** \*/
        internal RavenScriptedPatch([NotNull] string documentId)
        {
            if (documentId == null)
                throw new ArgumentNullException(nameof(documentId));

            _documentId = documentId;
        }

        /*\ ***** ***** ***** ***** ***** Public Methods ***** ***** ***** ***** ***** \*/
        [Pure]
        [NotNull]
        public RavenScriptedPatchWithScript Script([NotNull] string script)
        {
            if (script == null)
                throw new ArgumentNullException(nameof(script));

            return new RavenScriptedPatchWithScript(_documentId, script);
        }
    }
}
