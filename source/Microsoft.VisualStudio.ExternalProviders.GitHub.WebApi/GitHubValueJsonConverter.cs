// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.GitHubValueJsonConverter
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi
{
  internal class GitHubValueJsonConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => typeof (GitHubData.V3.Value).IsAssignableFrom(objectType);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      GitHubData.V3.Value obj = new GitHubData.V3.Value();
      JObject jobject = (JObject) null;
      if (reader.TokenType != JsonToken.StartObject)
        return (object) null;
      reader.Read();
      while (reader.TokenType == JsonToken.PropertyName)
      {
        switch ((reader.Value as string).ToLowerInvariant())
        {
          case "name":
            obj.Name = reader.ReadAsString();
            break;
          case "type":
            obj.Type = reader.ReadAsString();
            break;
          case "attributes":
            if (reader.Read())
            {
              jobject = JObject.Load(reader);
              break;
            }
            break;
        }
        reader.Read();
      }
      if (jobject != null)
      {
        foreach (Type type in typeof (GitHubData.V3.Attributes).GetTypeInfo().Assembly.GetTypes())
        {
          GitHubData.V3.ValueAttributesTypeAttribute customAttribute = type.GetTypeInfo().GetCustomAttribute<GitHubData.V3.ValueAttributesTypeAttribute>();
          if (customAttribute != null && string.Equals(customAttribute.Name, obj.Type, StringComparison.OrdinalIgnoreCase))
          {
            obj.Attributes = jobject.ToObject(type, serializer) as GitHubData.V3.Attributes;
            break;
          }
        }
      }
      return (object) obj;
    }

    public override bool CanWrite => true;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      base.WriteJson(writer, value, serializer);
      if (value == null)
        return;
      writer.WriteValue(value.ToString());
    }
  }
}
