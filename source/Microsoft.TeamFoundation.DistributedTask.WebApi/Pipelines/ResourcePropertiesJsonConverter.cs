// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourcePropertiesJsonConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  internal class ResourcePropertiesJsonConverter : VssSecureJsonConverter
  {
    public override bool CanWrite => true;

    public override bool CanConvert(Type objectType) => typeof (IDictionary<string, JToken>).GetTypeInfo().IsAssignableFrom(objectType);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      return (object) new ResourceProperties(serializer.Deserialize<IDictionary<string, JToken>>(reader));
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      base.WriteJson(writer, value, serializer);
      ResourceProperties resourceProperties = (ResourceProperties) value;
      serializer.Serialize(writer, (object) resourceProperties?.Items);
    }
  }
}
