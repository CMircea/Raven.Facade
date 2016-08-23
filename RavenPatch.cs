using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Raven.Facade.Patching;

namespace Raven.Facade
{
    public static class RavenPatch
    {
        [Pure]
        [NotNull]
        public static RavenDocumentPatch<T> For<T>([NotNull] string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            var serializer = RavenStore.Instance.Conventions.CreateSerializer();

            return new RavenDocumentPatch<T>(id, serializer);
        }
    }
}
