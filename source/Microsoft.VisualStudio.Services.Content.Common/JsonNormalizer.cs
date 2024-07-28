// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.JsonNormalizer
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class JsonNormalizer
  {
    public static string NormalizeJsonString(string jsonstring) => JsonNormalizer.SortObjectProperties(JToken.Parse(jsonstring)).ToString(Formatting.None);

    public static JToken SortObjectProperties(JToken tok)
    {
      if (tok == null)
        return (JToken) null;
      switch (tok.Type)
      {
        case JTokenType.Object:
          JObject jobject = new JObject();
          foreach (JProperty jproperty in (IEnumerable<JProperty>) (tok as JObject).Properties().ToList<JProperty>().OrderBy<JProperty, string>((Func<JProperty, string>) (x => x.Name)))
          {
            if (jproperty.Value != null && jproperty.Value.Type != JTokenType.Null)
              jobject.Add(jproperty.Name, JsonNormalizer.SortObjectProperties(jproperty.Value));
          }
          return (JToken) jobject;
        case JTokenType.Array:
          JArray jarray = new JArray();
          foreach (JToken child in (tok as JArray).Children())
            jarray.Add(JsonNormalizer.SortObjectProperties(child));
          return (JToken) jarray;
        default:
          return tok;
      }
    }
  }
}
