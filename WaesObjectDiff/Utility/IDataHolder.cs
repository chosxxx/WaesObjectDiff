using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaesObjectDiff.Models;

namespace WaesObjectDiff.Utility
{
    public interface IDataHolder
    {
        Diff Get(string id);
        void Set(string id, Diff data);
    }
}
