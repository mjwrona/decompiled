// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JsonUtilities
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.IO;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class JsonUtilities
  {
    internal static readonly JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings();
    internal static readonly JsonSerializerSettings DefaultCultureInvariantSerializerSettings;

    static JsonUtilities()
    {
      JsonUtilities.DefaultSerializerSettings.Converters.Add((JsonConverter) new IsoDateTimeConverter());
      JsonUtilities.DefaultSerializerSettings.Converters.Add((JsonConverter) new StringEnumConverter());
      JsonUtilities.DefaultCultureInvariantSerializerSettings = new JsonSerializerSettings();
      JsonUtilities.DefaultCultureInvariantSerializerSettings.Converters.Add((JsonConverter) new IsoDateTimeConverter()
      {
        Culture = CultureInfo.InvariantCulture
      });
      JsonUtilities.DefaultCultureInvariantSerializerSettings.Converters.Add((JsonConverter) new StringEnumConverter());
      JsonUtilities.DefaultCultureInvariantSerializerSettings.Culture = CultureInfo.InvariantCulture;
    }

    public static string Serialize<T>(this T obj, bool cultureInvariant = false) => JsonUtilities.Serialize((object) obj, cultureInvariant);

    public static string Serialize(object obj, bool cultureInvariant = false)
    {
      ArgumentUtility.CheckForNull<object>(obj, nameof (obj));
      JsonSerializerSettings settings = cultureInvariant ? JsonUtilities.DefaultCultureInvariantSerializerSettings : JsonUtilities.DefaultSerializerSettings;
      return JsonConvert.SerializeObject(obj, settings);
    }

    public static T Deserialize<T>(string json, bool cultureInvariant = false)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(json, nameof (json));
      JsonSerializerSettings settings = cultureInvariant ? JsonUtilities.DefaultCultureInvariantSerializerSettings : JsonUtilities.DefaultSerializerSettings;
      return JsonConvert.DeserializeObject<T>(json, settings);
    }

    public static object Deserialize(string json, Type type, bool cultureInvariant = false)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(json, nameof (json));
      JsonSerializerSettings settings = cultureInvariant ? JsonUtilities.DefaultCultureInvariantSerializerSettings : JsonUtilities.DefaultSerializerSettings;
      return JsonConvert.DeserializeObject(json, type, settings);
    }

    public static bool TryDeserialize<T>(string json, out T obj, bool cultureInvariant = false)
    {
      obj = default (T);
      if (string.IsNullOrWhiteSpace(json))
        return false;
      try
      {
        obj = JsonUtilities.Deserialize<T>(json, cultureInvariant);
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static JToken DeserializeTruncatedJson(string truncatedJson)
    {
      using (StringReader reader1 = new StringReader(truncatedJson))
      {
        using (JsonTextReader reader2 = new JsonTextReader((TextReader) reader1))
        {
          using (JTokenWriter jtokenWriter = new JTokenWriter())
          {
            try
            {
              jtokenWriter.WriteToken((JsonReader) reader2);
            }
            catch (Exception ex)
            {
            }
            return jtokenWriter.Token;
          }
        }
      }
    }
  }
}
