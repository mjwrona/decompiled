// Decompiled with JetBrains decompiler
// Type: WebGrease.Extensions.JsonExtensions
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace WebGrease.Extensions
{
  internal static class JsonExtensions
  {
    private static readonly Newtonsoft.Json.JsonSerializerSettings DefaultJsonSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings();
    private static readonly Lazy<Newtonsoft.Json.JsonSerializerSettings> JsonSerializerSettings = new Lazy<Newtonsoft.Json.JsonSerializerSettings>((Func<Newtonsoft.Json.JsonSerializerSettings>) (() =>
    {
      DefaultContractResolver contractResolver = new DefaultContractResolver();
      contractResolver.DefaultMembersSearchFlags |= BindingFlags.NonPublic;
      return new Newtonsoft.Json.JsonSerializerSettings()
      {
        ContractResolver = (IContractResolver) contractResolver
      };
    }));

    internal static T FromJson<T>(this string json, bool nonPublic = false) => JsonConvert.DeserializeObject<T>(json, JsonExtensions.GetJsonSerializationSettings(nonPublic));

    internal static string ToJson(this object value, bool nonPublic = false) => JsonConvert.SerializeObject(value, Formatting.None, JsonExtensions.GetJsonSerializationSettings(nonPublic));

    private static Newtonsoft.Json.JsonSerializerSettings GetJsonSerializationSettings(
      bool nonPublic)
    {
      return !nonPublic ? JsonExtensions.DefaultJsonSerializerSettings : JsonExtensions.JsonSerializerSettings.Value;
    }
  }
}
