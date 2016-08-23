using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Raven.Facade.Patching;

namespace Raven.Facade
{
    public static class RavenScript
    {
        [Pure]
        [NotNull]
        public static RavenScriptedPatch For([NotNull] string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return new RavenScriptedPatch(id);
        }
    }
}
