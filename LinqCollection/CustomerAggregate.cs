using System;
using System.Collections.Generic;
using System.Text;

namespace LinqCollectionPref
{
    public class CustomerAggregate
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public CustomerPreference Prefernce { get; set; }
    }
}
