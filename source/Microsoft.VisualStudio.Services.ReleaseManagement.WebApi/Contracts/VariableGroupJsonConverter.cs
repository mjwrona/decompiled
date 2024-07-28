// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroupJsonConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
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
      JToken variablesJsonToken = jobject.GetValue("Variables", StringComparison.OrdinalIgnoreCase);
      JToken providerDataJsonToken = jobject.GetValue("ProviderData", StringComparison.OrdinalIgnoreCase);
      VariableGroup variableGroup = new VariableGroup();
      using (JsonReader reader1 = jobject.CreateReader())
        serializer.Populate(reader1, (object) variableGroup);
      if (string.IsNullOrEmpty(variableGroup.Type))
        variableGroup.Type = "Vsts";
      variableGroup.PopulateVariablesAndProviderData(variablesJsonToken, providerDataJsonToken);
      return (object) variableGroup;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
  }
}
