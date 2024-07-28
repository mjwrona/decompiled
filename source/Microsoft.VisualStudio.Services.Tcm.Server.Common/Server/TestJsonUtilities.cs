// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestJsonUtilities
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public static class TestJsonUtilities
  {
    internal static readonly JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings();
    internal static readonly JsonSerializerSettings DefaultCultureInvariantSerializerSettings;

    static TestJsonUtilities()
    {
      TestJsonUtilities.DefaultSerializerSettings.Converters.Add((JsonConverter) new IsoDateTimeConverter());
      TestJsonUtilities.DefaultSerializerSettings.Converters.Add((JsonConverter) new StringEnumConverter());
      TestJsonUtilities.DefaultSerializerSettings.MaxDepth = new int?(10);
      TestJsonUtilities.DefaultCultureInvariantSerializerSettings = new JsonSerializerSettings();
      TestJsonUtilities.DefaultCultureInvariantSerializerSettings.Converters.Add((JsonConverter) new IsoDateTimeConverter()
      {
        Culture = CultureInfo.InvariantCulture
      });
      TestJsonUtilities.DefaultCultureInvariantSerializerSettings.Converters.Add((JsonConverter) new StringEnumConverter());
      TestJsonUtilities.DefaultCultureInvariantSerializerSettings.Culture = CultureInfo.InvariantCulture;
      TestJsonUtilities.DefaultCultureInvariantSerializerSettings.MaxDepth = new int?(10);
    }

    public static string Serialize<T>(this T obj, bool cultureInvariant = false) => TestJsonUtilities.Serialize((object) obj, cultureInvariant);

    public static string Serialize(object o, bool cultureInvariant = false)
    {
      ArgumentUtility.CheckForNull<object>(o, nameof (o));
      JsonSerializerSettings settings = cultureInvariant ? TestJsonUtilities.DefaultCultureInvariantSerializerSettings : TestJsonUtilities.DefaultSerializerSettings;
      return JsonConvert.SerializeObject(o, settings);
    }

    public static T Deserialize<T>(string json, bool cultureInvariant = false)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(json, nameof (json));
      JsonSerializerSettings settings = cultureInvariant ? TestJsonUtilities.DefaultCultureInvariantSerializerSettings : TestJsonUtilities.DefaultSerializerSettings;
      return JsonConvert.DeserializeObject<T>(json, settings);
    }

    public static object Deserialize(string json, Type type, bool cultureInvariant = false)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(json, nameof (json));
      JsonSerializerSettings settings = cultureInvariant ? TestJsonUtilities.DefaultCultureInvariantSerializerSettings : TestJsonUtilities.DefaultSerializerSettings;
      return JsonConvert.DeserializeObject(json, type, settings);
    }

    public static bool TryDeserialize<T>(string json, out T obj, bool cultureInvariant = false)
    {
      obj = default (T);
      if (string.IsNullOrWhiteSpace(json))
        return false;
      try
      {
        obj = TestJsonUtilities.Deserialize<T>(json, cultureInvariant);
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }
  }
}
