// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.VariableGroupConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class VariableGroupConverter
  {
    public static Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup ToServerVariableGroup(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup webApiGroup)
    {
      if (webApiGroup == null)
        return (Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup) null;
      Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup serverVariableGroup = new Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup()
      {
        Id = webApiGroup.Id,
        Type = webApiGroup.Type,
        Name = webApiGroup.Name,
        Description = webApiGroup.Description,
        CreatedBy = webApiGroup.CreatedBy,
        CreatedOn = webApiGroup.CreatedOn,
        ModifiedBy = webApiGroup.ModifiedBy,
        ModifiedOn = webApiGroup.ModifiedOn
      };
      if (webApiGroup.Variables != null && webApiGroup.Variables.Count > 0)
      {
        List<string> list = webApiGroup.Variables.GroupBy<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableValue>, string>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableValue>, string>) (x => x.Key.Trim()), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Where<IGrouping<string, KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableValue>>>((Func<IGrouping<string, KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableValue>>, bool>) (x => x.Count<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableValue>>() > 1)).Select<IGrouping<string, KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableValue>>, string>((Func<IGrouping<string, KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableValue>>, string>) (x => x.Key)).ToList<string>();
        if (list.Count > 0)
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.DuplicateKeyInVariableGroup, (object) webApiGroup.Name, (object) string.Join<string>(", ", (IEnumerable<string>) list)));
      }
      switch (webApiGroup.Type)
      {
        case "Vsts":
          if (webApiGroup.Variables != null && webApiGroup.Variables.Count > 0)
          {
            foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableValue> variable in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableValue>>) webApiGroup.Variables)
            {
              Microsoft.TeamFoundation.DistributedTask.WebApi.VariableValue variableValue = new Microsoft.TeamFoundation.DistributedTask.WebApi.VariableValue();
              if (variable.Value != null)
              {
                variableValue.IsSecret = variable.Value.IsSecret;
                variableValue.Value = variable.Value.Value;
              }
              serverVariableGroup.Variables.Add(variable.Key.Trim(), variableValue);
            }
          }
          serverVariableGroup.ProviderData = JsonUtility.Deserialize<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroupProviderData>(JsonUtility.Serialize((object) webApiGroup.ProviderData));
          break;
        case "AzureKeyVault":
          if (webApiGroup.Variables != null && webApiGroup.Variables.Count > 0)
          {
            foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableValue> variable in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableValue>>) webApiGroup.Variables)
            {
              Microsoft.TeamFoundation.DistributedTask.WebApi.AzureKeyVaultVariableValue vaultVariableValue1 = new Microsoft.TeamFoundation.DistributedTask.WebApi.AzureKeyVaultVariableValue();
              if (variable.Value != null && variable.Value is Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.AzureKeyVaultVariableValue vaultVariableValue2)
              {
                vaultVariableValue1.IsSecret = vaultVariableValue2.IsSecret;
                vaultVariableValue1.Value = vaultVariableValue2.Value;
                vaultVariableValue1.Enabled = vaultVariableValue2.Enabled;
                vaultVariableValue1.ContentType = vaultVariableValue2.ContentType;
                vaultVariableValue1.Expires = vaultVariableValue2.Expires;
              }
              serverVariableGroup.Variables.Add(variable.Key.Trim(), (Microsoft.TeamFoundation.DistributedTask.WebApi.VariableValue) vaultVariableValue1);
            }
          }
          serverVariableGroup.ProviderData = (Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroupProviderData) JsonUtility.Deserialize<Microsoft.TeamFoundation.DistributedTask.WebApi.AzureKeyVaultVariableGroupProviderData>(JsonUtility.Serialize((object) webApiGroup.ProviderData));
          break;
        default:
          throw new ReleaseManagementServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.UnsupportedVariableGroupType, (object) webApiGroup.Type));
      }
      return serverVariableGroup;
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup ToWebApiVariableGroup(
      Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup serverGroup)
    {
      if (serverGroup == null)
        return (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup) null;
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup apiVariableGroup = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup()
      {
        Id = serverGroup.Id,
        Type = serverGroup.Type,
        Name = serverGroup.Name,
        Description = serverGroup.Description,
        CreatedBy = serverGroup.CreatedBy,
        CreatedOn = serverGroup.CreatedOn,
        ModifiedBy = serverGroup.ModifiedBy,
        ModifiedOn = serverGroup.ModifiedOn
      };
      switch (serverGroup.Type)
      {
        case "Vsts":
          if (serverGroup.Variables != null && serverGroup.Variables.Count > 0)
          {
            foreach (KeyValuePair<string, Microsoft.TeamFoundation.DistributedTask.WebApi.VariableValue> variable in (IEnumerable<KeyValuePair<string, Microsoft.TeamFoundation.DistributedTask.WebApi.VariableValue>>) serverGroup.Variables)
            {
              Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableValue variableValue = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableValue();
              if (variable.Value != null)
              {
                variableValue.IsSecret = variable.Value.IsSecret;
                variableValue.Value = variable.Value.Value;
              }
              apiVariableGroup.Variables.Add(variable.Key, variableValue);
            }
          }
          apiVariableGroup.ProviderData = JsonUtility.Deserialize<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroupProviderData>(JsonUtility.Serialize((object) serverGroup.ProviderData));
          break;
        case "AzureKeyVault":
          if (serverGroup.Variables != null && serverGroup.Variables.Count > 0)
          {
            foreach (KeyValuePair<string, Microsoft.TeamFoundation.DistributedTask.WebApi.VariableValue> variable in (IEnumerable<KeyValuePair<string, Microsoft.TeamFoundation.DistributedTask.WebApi.VariableValue>>) serverGroup.Variables)
            {
              Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.AzureKeyVaultVariableValue vaultVariableValue1 = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.AzureKeyVaultVariableValue();
              if (variable.Value != null && variable.Value is Microsoft.TeamFoundation.DistributedTask.WebApi.AzureKeyVaultVariableValue vaultVariableValue2)
              {
                vaultVariableValue1.IsSecret = vaultVariableValue2.IsSecret;
                vaultVariableValue1.Value = vaultVariableValue2.Value;
                vaultVariableValue1.Enabled = vaultVariableValue2.Enabled;
                vaultVariableValue1.ContentType = vaultVariableValue2.ContentType;
                vaultVariableValue1.Expires = vaultVariableValue2.Expires;
              }
              apiVariableGroup.Variables.Add(variable.Key, (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableValue) vaultVariableValue1);
            }
          }
          apiVariableGroup.ProviderData = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroupProviderData) JsonUtility.Deserialize<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.AzureKeyVaultVariableGroupProviderData>(JsonUtility.Serialize((object) serverGroup.ProviderData));
          break;
        default:
          throw new ReleaseManagementServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.UnsupportedVariableGroupType, (object) serverGroup.Type));
      }
      return apiVariableGroup;
    }
  }
}
