// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskLocalization
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class TaskLocalization
  {
    public const string LocalizationPrefix = "ms-resource:";
    private static readonly Encoding s_UTF8NoBOM = (Encoding) new UTF8Encoding(false, true);

    public static void MergeResourcesDocuments(JObject targetDocument, JObject sourceDocument)
    {
      foreach (JProperty property in sourceDocument.Properties())
      {
        if (!targetDocument.TryGetValue(property.Name, StringComparison.InvariantCulture, out JToken _))
          targetDocument.Add(property.Name, property.Value);
      }
    }

    public static Stream LocalizeDocument(JObject localizationDocument, JObject resourcesDocument)
    {
      TaskLocalization.Localize(localizationDocument, resourcesDocument);
      Stream stream = (Stream) new MemoryStream();
      using (StreamWriter streamWriter = new StreamWriter(stream, TaskLocalization.s_UTF8NoBOM, 4096, true))
      {
        using (JsonTextWriter writer = new JsonTextWriter((TextWriter) streamWriter))
          localizationDocument.WriteTo((JsonWriter) writer);
      }
      stream.Seek(0L, SeekOrigin.Begin);
      return stream;
    }

    public static JObject ReadJsonStream(Stream stream)
    {
      using (StreamReader reader1 = new StreamReader(stream, TaskLocalization.s_UTF8NoBOM, true))
      {
        using (JsonTextReader reader2 = new JsonTextReader((TextReader) reader1))
          return JObject.Load((JsonReader) reader2);
      }
    }

    private static void Localize(JObject jObject, JObject resourcesDocument)
    {
      foreach (JProperty property in jObject.Properties())
        TaskLocalization.Localize(property, resourcesDocument);
    }

    private static void Localize(JArray array, JObject resourcesDocument)
    {
      for (int index = 0; index < array.Count; ++index)
      {
        JToken array1 = array[index];
        switch (array1.Type)
        {
          case JTokenType.Object:
            TaskLocalization.Localize((JObject) array1, resourcesDocument);
            break;
          case JTokenType.Array:
            TaskLocalization.Localize((JArray) array1, resourcesDocument);
            break;
          case JTokenType.String:
            string parameterName = array1.Value<string>();
            string str;
            if (parameterName.StartsWith("ms-resource:", StringComparison.InvariantCulture) && resourcesDocument.TryGetValue<string>(parameterName, out str))
            {
              array[index] = (JToken) str;
              break;
            }
            break;
        }
      }
    }

    private static void Localize(JProperty property, JObject resourcesDocument)
    {
      switch (property.Value.Type)
      {
        case JTokenType.Object:
          TaskLocalization.Localize((JObject) property.Value, resourcesDocument);
          break;
        case JTokenType.Array:
          TaskLocalization.Localize((JArray) property.Value, resourcesDocument);
          break;
        case JTokenType.String:
          string str1 = property.Value.Value<string>();
          string str2;
          if (!str1.StartsWith("ms-resource:", StringComparison.InvariantCulture) || !resourcesDocument.TryGetValue<string>(str1.Substring("ms-resource:".Length), out str2))
            break;
          property.Value = (JToken) str2;
          break;
      }
    }
  }
}
