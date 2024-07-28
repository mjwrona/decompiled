// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.AzureKeyVaultVariableGroupProviderExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal static class AzureKeyVaultVariableGroupProviderExtensions
  {
    public static void PopulateKeyVaultSecretValues(
      this VariableGroup group,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      if (group == null)
        throw new ArgumentNullException(nameof (group));
      if (group.ProviderData == null)
        throw new ArgumentNullException("ProviderData");
      if (!(group.ProviderData is AzureKeyVaultVariableGroupProviderData providerData))
        throw new DistributedTaskException(TaskResources.InvalidAzureKeyVaultVariableGroupProviderData());
      ServiceEndpoint serviceEndpoint = DistributedTaskEndpointServiceHelper.GetServiceEndpoint(requestContext, projectId, providerData.ServiceEndpointId);
      if (serviceEndpoint == null)
        throw new DistributedTaskException(TaskResources.ServiceEndPointNotFound((object) providerData.ServiceEndpointId));
      if (serviceEndpoint.IsDisabled && requestContext.IsFeatureEnabled("ServiceEndpoints.AllowDisablingServiceEndpoints"))
        throw new DistributedTaskException(TaskResources.ServiceEndpointDisabled((object) serviceEndpoint.Id));
      serviceEndpoint.GetAdditionalServiceEndpointDetails(requestContext);
      string errorMessage;
      foreach (KeyValuePair<string, VariableValue> secret in (IEnumerable<KeyValuePair<string, VariableValue>>) new KeyVaultProvider(requestContext, serviceEndpoint, providerData.Vault, projectId).GetSecrets(group.Variables.Where<KeyValuePair<string, VariableValue>>((Func<KeyValuePair<string, VariableValue>, bool>) (x => x.Value is AzureKeyVaultVariableValue)).Select<KeyValuePair<string, VariableValue>, string>((Func<KeyValuePair<string, VariableValue>, string>) (x => x.Key)), false, out errorMessage))
      {
        VariableValue variableValue;
        if (group.Variables.TryGetValue(secret.Key, out variableValue))
        {
          variableValue.IsSecret = true;
          variableValue.Value = secret.Value?.Value;
        }
      }
      if (!string.IsNullOrEmpty(errorMessage))
        throw new DistributedTaskException(errorMessage);
    }
  }
}
