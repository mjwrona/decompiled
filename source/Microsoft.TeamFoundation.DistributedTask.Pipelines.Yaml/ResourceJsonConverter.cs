// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ResourceJsonConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml
{
  internal class ResourceJsonConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => objectType.Equals(typeof (RepositoryResource));

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      base.WriteJson(writer, value, serializer);
      Resource resource = value as Resource;
      Dictionary<string, JToken> dictionary = new Dictionary<string, JToken>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      dictionary.Add("alias", (JToken) (resource.Alias ?? string.Empty));
      if (resource.Endpoint != null)
        dictionary.Add("endpoint", JToken.FromObject((object) resource.Endpoint, serializer));
      if (resource.Properties != null)
      {
        foreach (KeyValuePair<string, JToken> keyValuePair in (IEnumerable<KeyValuePair<string, JToken>>) resource.Properties.Items)
        {
          if (!dictionary.ContainsKey(keyValuePair.Key))
            dictionary.Add(keyValuePair.Key, keyValuePair.Value);
        }
      }
      serializer.Serialize(writer, (object) dictionary);
    }

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override bool CanWrite => true;
  }
}
