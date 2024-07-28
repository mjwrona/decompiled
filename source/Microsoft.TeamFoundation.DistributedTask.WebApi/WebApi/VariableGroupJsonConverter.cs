// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroupJsonConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  internal sealed class VariableGroupJsonConverter : VssSecureJsonConverter
  {
    public override bool CanRead => true;

    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType) => typeof (VariableGroup).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      JObject jobject = JObject.Load(reader);
      JToken jtoken1 = jobject.GetValue("Variables", StringComparison.OrdinalIgnoreCase);
      JToken jtoken2 = jobject.GetValue("ProviderData", StringComparison.OrdinalIgnoreCase);
      string variablesJson = (string) null;
      if (jtoken1 != null)
        variablesJson = jtoken1.ToString();
      string providerDataJson = (string) null;
      if (jtoken2 != null)
        providerDataJson = jtoken2.ToString();
      VariableGroup variableGroup = new VariableGroup();
      using (JsonReader reader1 = jobject.CreateReader())
        serializer.Populate(reader1, (object) variableGroup);
      if (string.IsNullOrEmpty(variableGroup.Type))
        variableGroup.Type = "Vsts";
      variableGroup.PopulateVariablesAndProviderData(variablesJson, providerDataJson);
      return (object) variableGroup;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
  }
}
