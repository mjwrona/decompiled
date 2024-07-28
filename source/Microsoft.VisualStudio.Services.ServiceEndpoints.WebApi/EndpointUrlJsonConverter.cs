// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.EndpointUrlJsonConverter
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  public class EndpointUrlJsonConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => typeof (EndpointUrl).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

    public override bool CanWrite => false;

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof (reader));
      if (serializer == null)
        throw new ArgumentNullException(nameof (serializer));
      EndpointUrl endpointUrl = (EndpointUrl) null;
      if (reader.TokenType == JsonToken.String && reader.Value is string uriString)
        endpointUrl = new EndpointUrl()
        {
          Value = new Uri(uriString)
        };
      if (reader.TokenType == JsonToken.StartObject)
      {
        JObject jobject = JObject.Load(reader);
        JToken jtoken1 = jobject.GetValue("DisplayName", StringComparison.OrdinalIgnoreCase);
        JToken jtoken2 = jobject.GetValue("HelpText", StringComparison.OrdinalIgnoreCase);
        JToken jtoken3 = jobject.GetValue("Value", StringComparison.OrdinalIgnoreCase);
        JToken jtoken4 = jobject.GetValue("IsVisible", StringComparison.OrdinalIgnoreCase);
        JToken jtoken5 = jobject.GetValue("DependsOn", StringComparison.OrdinalIgnoreCase);
        JToken jtoken6 = jobject.GetValue("Format", StringComparison.OrdinalIgnoreCase);
        endpointUrl = new EndpointUrl()
        {
          Value = jtoken3 != null ? new Uri(jtoken3.ToString()) : (Uri) null,
          DependsOn = jtoken5 != null ? JsonUtility.FromString<DependsOn>(jtoken5.ToString()) : (DependsOn) null,
          DisplayName = jtoken1?.ToString(),
          HelpText = jtoken2?.ToString(),
          IsVisible = jtoken4?.ToString(),
          Format = jtoken6?.ToString()
        };
      }
      return (object) endpointUrl;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
  }
}
