// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.ObjectModel.PipelineConfigurationJsonConverter
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace Microsoft.Azure.Pipelines.Server.ObjectModel
{
  public class PipelineConfigurationJsonConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => typeof (PipelineConfiguration).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

    public override bool CanWrite => false;

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType != JsonToken.StartObject)
        return (object) null;
      if (!(serializer.ContractResolver.ResolveContract(objectType) is JsonObjectContract jsonObjectContract))
        return existingValue;
      PipelineConfiguration target = (PipelineConfiguration) null;
      if (objectType == typeof (YamlConfiguration))
        target = (PipelineConfiguration) new YamlConfiguration();
      else if (objectType == typeof (DesignerJsonConfiguration))
        target = (PipelineConfiguration) new DesignerJsonConfiguration();
      JObject jobject = JObject.Load(reader);
      if (target == null)
      {
        JsonProperty closestMatchProperty1 = jsonObjectContract.Properties.GetClosestMatchProperty("Type");
        JToken jtoken1;
        if (closestMatchProperty1 == null || !jobject.TryGetValue(closestMatchProperty1.PropertyName, StringComparison.OrdinalIgnoreCase, out jtoken1))
          return existingValue;
        ConfigurationType result;
        if (jtoken1.Type == JTokenType.Integer)
          result = (ConfigurationType) (int) jtoken1;
        else if (jtoken1.Type != JTokenType.String || !Enum.TryParse<ConfigurationType>((string) jtoken1, true, out result))
          return existingValue;
        int? version = new int?();
        JsonProperty closestMatchProperty2 = jsonObjectContract.Properties.GetClosestMatchProperty("Version");
        JToken jtoken2;
        if (closestMatchProperty2 != null && jobject.TryGetValue(closestMatchProperty2.PropertyName, StringComparison.OrdinalIgnoreCase, out jtoken2) && jtoken2.Type == JTokenType.Integer)
          version = new int?((int) jtoken2);
        switch (result)
        {
          case ConfigurationType.Yaml:
            target = (PipelineConfiguration) this.CreateYamlConfiguration(version);
            break;
          case ConfigurationType.DesignerJson:
            target = (PipelineConfiguration) this.CreateDesignerJsonConfiguration(version);
            break;
        }
      }
      if (target != null)
      {
        using (JsonReader reader1 = jobject.CreateReader())
          serializer.Populate(reader1, (object) target);
      }
      return (object) target;
    }

    private YamlConfiguration CreateYamlConfiguration(int? version) => new YamlConfiguration();

    private DesignerJsonConfiguration CreateDesignerJsonConfiguration(int? version) => new DesignerJsonConfiguration();
  }
}
