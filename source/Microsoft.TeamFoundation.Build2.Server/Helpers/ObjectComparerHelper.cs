// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Helpers.ObjectComparerHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server.Helpers
{
  internal static class ObjectComparerHelper
  {
    public static bool AreSameObjects(
      object first,
      object second,
      ICollection<string> fieldsToIgnore,
      IVssRequestContext requestContext)
    {
      using (new TraceWatch(requestContext, 12030381, TraceLevel.Warning, TimeSpan.FromSeconds(1.0), "Build2", nameof (ObjectComparerHelper), "ObjectComparerHelper took too long (Threshold 1s)", Array.Empty<object>()))
      {
        if (first == null && second == null)
          return true;
        return first != null && second != null && !ObjectComparerHelper.GetDifferences(JToken.FromObject(first), JToken.FromObject(second), fieldsToIgnore).Any<string>();
      }
    }

    private static List<string> GetDifferences(
      JToken first,
      JToken second,
      ICollection<string> fieldsToIgnore)
    {
      List<string> differences = new List<string>();
      ObjectComparerHelper.GetDifferencesRecursive(first, second, differences, string.Empty, fieldsToIgnore);
      return differences;
    }

    private static void GetDifferencesRecursive(
      JToken first,
      JToken second,
      List<string> differences,
      string basePath,
      ICollection<string> fieldsToIgnore)
    {
      if (first.Type != second.Type)
      {
        differences.Add(string.Format("{0}: type mismatch: {1} != {2}", (object) basePath, (object) first.Type, (object) second.Type));
      }
      else
      {
        switch (first)
        {
          case JObject jobject2:
            JObject jobject1 = second as JObject;
            HashSet<string> properties = new HashSet<string>();
            foreach (JProperty property in jobject2.Properties())
            {
              JProperty jproperty = jobject1.Property(property.Name);
              properties.Add(property.Name);
              if (!fieldsToIgnore.Contains(property.Path))
              {
                if (jproperty == null)
                  differences.Add(basePath + property.Name + ": missing in second");
                else
                  ObjectComparerHelper.GetDifferencesRecursive(property.Value, jproperty.Value, differences, basePath + "." + property.Name, fieldsToIgnore);
              }
            }
            using (IEnumerator<JProperty> enumerator = jobject1.Properties().Where<JProperty>((Func<JProperty, bool>) (x => !properties.Contains(x.Name))).GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                JProperty current = enumerator.Current;
                JProperty jproperty = jobject2.Property(current.Name);
                if (!fieldsToIgnore.Contains(current.Path))
                {
                  if (jproperty == null)
                    differences.Add(basePath + current.Name + ": missing in first");
                  else
                    ObjectComparerHelper.GetDifferencesRecursive(current.Value, jproperty.Value, differences, basePath + "." + current.Name, fieldsToIgnore);
                }
              }
              break;
            }
          case JArray jarray2:
            JArray jarray1 = second as JArray;
            if (jarray2.Count != jarray1.Count)
            {
              differences.Add(string.Format("{0}: array length mismatch: {1} != {2}", (object) basePath, (object) jarray2.Count, (object) jarray1.Count));
              break;
            }
            for (int index = 0; index < jarray2.Count; ++index)
              ObjectComparerHelper.GetDifferencesRecursive(jarray2[index], jarray1[index], differences, basePath + string.Format("[{0}]", (object) index), fieldsToIgnore);
            break;
          case JValue jvalue:
            JValue other = second as JValue;
            if (jvalue.Equals(other))
              break;
            differences.Add(string.Format("{0}: value mismatch: {1} != {2}", (object) basePath, (object) jvalue, (object) other));
            break;
          default:
            differences.Add(string.Format("{0}: unknown type: {1}", (object) basePath, (object) first.Type));
            break;
        }
      }
    }
  }
}
