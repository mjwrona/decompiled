// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.VariableGroupUtility
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  public static class VariableGroupUtility
  {
    private static Type keyVaultVariableType = typeof (AzureKeyVaultVariableValue);

    public static VariableValue Clone(this VariableValue value) => VariableGroupUtility.keyVaultVariableType == value.GetType() ? (VariableValue) new AzureKeyVaultVariableValue((AzureKeyVaultVariableValue) value) : new VariableValue(value);

    public static void PopulateVariablesAndProviderData(
      this VariableGroup group,
      JToken variablesJsonToken,
      JToken providerDataJsonToken)
    {
      switch (group.Type)
      {
        case "Vsts":
          if (variablesJsonToken != null)
          {
            IDictionary<string, VariableValue> dictionary = variablesJsonToken.ToObject<IDictionary<string, VariableValue>>();
            if (dictionary != null)
            {
              foreach (KeyValuePair<string, VariableValue> keyValuePair in (IEnumerable<KeyValuePair<string, VariableValue>>) dictionary)
                group.Variables[keyValuePair.Key] = keyValuePair.Value;
            }
          }
          if (providerDataJsonToken == null)
            break;
          group.ProviderData = providerDataJsonToken.ToObject<VariableGroupProviderData>();
          break;
        case "AzureKeyVault":
          if (variablesJsonToken != null)
          {
            IDictionary<string, AzureKeyVaultVariableValue> dictionary = variablesJsonToken.ToObject<IDictionary<string, AzureKeyVaultVariableValue>>();
            if (dictionary != null)
            {
              foreach (KeyValuePair<string, AzureKeyVaultVariableValue> keyValuePair in (IEnumerable<KeyValuePair<string, AzureKeyVaultVariableValue>>) dictionary)
                group.Variables[keyValuePair.Key] = (VariableValue) keyValuePair.Value;
            }
          }
          if (providerDataJsonToken == null)
            break;
          group.ProviderData = (VariableGroupProviderData) providerDataJsonToken.ToObject<AzureKeyVaultVariableGroupProviderData>();
          break;
      }
    }
  }
}
