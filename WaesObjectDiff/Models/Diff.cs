using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;

namespace WaesObjectDiff.Models
{
    public static class DiffResultMessages
    {
        public static string MissingElements = "One or more elements are missing.";
        public static string EqualElements = "Both elements are Equal.";
        public static string DifferentSize = "Both elements are of different size.";
        public static string PropertiesMismatch = "Elements' properties does not match.";
        public static string PropertiesDiffer = "Elements differ each other in the following properties:";
    };

    public class Diff
    {
        public string Id { get; set; }
        public JObject Left { get; set; }
        public JObject Right { get; set; }
        public string DiffResult
        {
            get
            {
                if (Left == null || Right == null)
                    return DiffResultMessages.MissingElements;
                
                if (Left.Count != Right.Count)
                    return DiffResultMessages.DifferentSize;

                bool propertiesDiffer = false;
                var diffResult = new StringBuilder(DiffResultMessages.PropertiesDiffer);
                foreach (var p in Left.Properties())
                {
                    if (!((IDictionary<string, JToken>)Right).ContainsKey(p.Name))
                        return DiffResultMessages.PropertiesMismatch;

                    var leftValue = p.Value;
                    var rightValue = Right[p.Name];
                    if (!Equals(leftValue, rightValue))
                    {
                        propertiesDiffer = true;
                        diffResult.AppendLine($"    {p.Name}");
                        diffResult.AppendLine($"        Left value size: {leftValue}");
                        diffResult.AppendLine($"        Right value size: {rightValue}");
                    }
                }
                if (propertiesDiffer)
                    return diffResult.ToString();

                return DiffResultMessages.EqualElements;
            }
        }
    }
}