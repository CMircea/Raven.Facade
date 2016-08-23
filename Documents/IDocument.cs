using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven.Facade.Documents
{
    public interface IDocument
    {
        string Id
        {
            get;
            set;
        }
    }
}
