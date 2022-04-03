using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;

namespace NewsWidget
{
    public struct Feed
    {
        public string Source;
        public SyndicationItem Item;
    }
}
