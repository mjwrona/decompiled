// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataAccess.JsonUtilities
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Microsoft.VisualStudio.Services.Licensing.DataAccess
{
  public static class JsonUtilities
  {
    internal static readonly JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings();

    static JsonUtilities()
    {
      JsonUtilities.DefaultSerializerSettings.Converters.Add((JsonConverter) new IsoDateTimeConverter());
      JsonUtilities.DefaultSerializerSettings.Converters.Add((JsonConverter) new StringEnumConverter());
    }

    internal static string Serialize<T>(this T obj) => JsonUtilities.Serialize((object) obj);

    internal static string Serialize(object o)
    {
      ArgumentUtility.CheckForNull<object>(o, nameof (o));
      return JsonConvert.SerializeObject(o, JsonUtilities.DefaultSerializerSettings);
    }

    internal static T Deserialize<T>(string json)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(json, nameof (json));
      return JsonConvert.DeserializeObject<T>(json, JsonUtilities.DefaultSerializerSettings);
    }

    internal static object Deserialize(string json, Type type)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(json, nameof (json));
      return JsonConvert.DeserializeObject(json, type, JsonUtilities.DefaultSerializerSettings);
    }
  }
}
