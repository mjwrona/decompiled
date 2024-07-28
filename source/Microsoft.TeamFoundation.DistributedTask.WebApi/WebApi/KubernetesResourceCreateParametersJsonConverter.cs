// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.KubernetesResourceCreateParametersJsonConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  public class KubernetesResourceCreateParametersJsonConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => typeof (KubernetesResourceCreateParameters).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

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
      if (reader.TokenType != JsonToken.StartObject || !(serializer.ContractResolver.ResolveContract(objectType) is JsonObjectContract))
        return existingValue;
      JObject jobject = JObject.Load(reader);
      JToken jtoken;
      KubernetesResourceCreateParameters target;
      if (jobject.TryGetValue("serviceEndpointId", StringComparison.OrdinalIgnoreCase, out jtoken) && jtoken.Type == JTokenType.String)
      {
        target = (KubernetesResourceCreateParameters) new KubernetesResourceCreateParametersExistingEndpoint();
      }
      else
      {
        if (!jobject.TryGetValue("endpoint", StringComparison.OrdinalIgnoreCase, out jtoken) || jtoken.Type != JTokenType.Object)
          throw new NotSupportedException("serviceEndpointId or endpoint field is mandatory for creating resource.");
        target = (KubernetesResourceCreateParameters) new KubernetesResourceCreateParametersNewEndpoint();
      }
      using (JsonReader reader1 = jobject.CreateReader())
        serializer.Populate(reader1, (object) target);
      return (object) target;
    }
  }
}
