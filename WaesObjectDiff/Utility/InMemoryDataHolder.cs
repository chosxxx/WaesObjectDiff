using System.Collections.Generic;
using WaesObjectDiff.Models;

namespace WaesObjectDiff.Utility
{
    public class InMemoryDataHolder : IDataHolder
    {
        private static IDictionary<string, Diff> inMemoryData = new Dictionary<string, Diff>();
        public Diff Get(string id)
        {
            if (inMemoryData.ContainsKey(id)) return inMemoryData[id];
            return null;
        }

        public void Set(string id, Diff data)
        {
            if (inMemoryData.ContainsKey(id)) inMemoryData[id] = data;
            else inMemoryData.Add(id, data);
        }
    }
}