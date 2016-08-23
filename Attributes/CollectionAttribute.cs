using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven.Facade.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CollectionAttribute : Attribute
    {
        /*\ ***** ***** ***** ***** ***** Properties ***** ***** ***** ***** ***** \*/
        public string Name
        {
            get;
        }

        /*\ ***** ***** ***** ***** ***** Constructor ***** ***** ***** ***** ***** \*/
        public CollectionAttribute(string name)
        {
            Name = name;
        }
    }
}
