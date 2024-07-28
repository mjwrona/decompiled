// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.JsonSerializer
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Newtonsoft.Json;
using System.Collections;
using System.IO;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class JsonSerializer
  {
    public static readonly Newtonsoft.Json.JsonSerializer Serializer;
    private static readonly JsonSerializerSettings Settings = JsonSerializer.MakeDefaultSettings();

    static JsonSerializer() => JsonSerializer.Serializer = new Newtonsoft.Json.JsonSerializer()
    {
      DateTimeZoneHandling = JsonSerializer.Settings.DateTimeZoneHandling,
      DateFormatHandling = JsonSerializer.Settings.DateFormatHandling,
      DateParseHandling = JsonSerializer.Settings.DateParseHandling,
      EqualityComparer = JsonSerializer.Settings.EqualityComparer
    };

    public static JsonSerializerSettings MakeDefaultSettings() => new JsonSerializerSettings()
    {
      DateTimeZoneHandling = DateTimeZoneHandling.Utc,
      DateFormatHandling = DateFormatHandling.IsoDateFormat,
      DateParseHandling = DateParseHandling.None,
      EqualityComparer = (IEqualityComparer) new JsonSerializer.ReferenceEqualityComparer()
    };

    public static string Serialize<T>(T dataContractObject) => JsonSerializer.Serialize<T>(dataContractObject, JsonSerializer.Settings);

    public static string Serialize<T>(T dataContractObject, JsonSerializerSettings settings) => JsonConvert.SerializeObject((object) dataContractObject, settings);

    public static HttpContent SerializeToContent<T>(T dataContractObject) => (object) dataContractObject != null ? (HttpContent) new StringContent(JsonConvert.SerializeObject((object) dataContractObject, JsonSerializer.Settings), StrictEncodingWithoutBOM.UTF8, "application/json") : (HttpContent) null;

    public static T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, JsonSerializer.Settings);

    public static T Deserialize<T>(string json, JsonSerializerSettings settings) => !string.IsNullOrWhiteSpace(json) ? JsonConvert.DeserializeObject<T>(json, settings) : throw new JsonReaderException("The empty or null string isn't valid JSON");

    public static T Deserialize<T>(Stream stream)
    {
      using (StreamReader reader1 = new StreamReader(stream))
      {
        using (JsonTextReader reader2 = new JsonTextReader((TextReader) reader1))
          return JsonSerializer.Serializer.Deserialize<T>((JsonReader) reader2);
      }
    }

    private class ReferenceEqualityComparer : IEqualityComparer
    {
      public bool Equals(object x, object y) => x == y;

      public int GetHashCode(object obj) => this.GetHashCode(obj);
    }
  }
}
