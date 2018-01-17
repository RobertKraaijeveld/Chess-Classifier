using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;


namespace Classifiers
{
    public abstract class AbstractRecordCreator
    {
        public abstract List<Record> CreateRecords(IEnumerable<string> lines);
    }
}