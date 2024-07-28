// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.AzureKeyVaultValueProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal class AzureKeyVaultValueProvider : IVariableValueProvider
  {
    private readonly Guid m_projectId;
    private readonly IVssRequestContext m_requestContext;
    public static readonly Guid AzureKeyVaultTaskId = new Guid("{1E244D32-2DD4-4165-96FB-B7441CA9331E}");

    public AzureKeyVaultValueProvider(IVssRequestContext requestContext, Guid projectId)
    {
      this.m_requestContext = requestContext;
      this.m_projectId = projectId;
    }

    public string GroupType => "AzureKeyVault";

    public bool ShouldGetValues(IPipelineContext context) => context is JobExecutionContext executionContext && executionContext.Phase.Definition is Phase definition && definition.Target.Type == PhaseTargetType.Server;

    public IList<TaskStep> GetSteps(
      IPipelineContext context,
      VariableGroupReference group,
      IEnumerable<string> keys)
    {
      if (group?.SecretStore == null || group.SecretStore.Endpoint == null)
        return (IList<TaskStep>) Array.Empty<TaskStep>();
      TaskDefinition taskDefinition = context.TaskStore.ResolveTask(AzureKeyVaultValueProvider.AzureKeyVaultTaskId, "1.*");
      TaskStep taskStep1 = new TaskStep();
      taskStep1.Enabled = true;
      taskStep1.DisplayName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, PipelineStrings.AzureKeyVaultTaskName((object) group.SecretStore.StoreName));
      taskStep1.Reference = new TaskStepDefinitionReference()
      {
        Id = taskDefinition.Id,
        Name = taskDefinition.Name,
        Version = (string) taskDefinition.Version
      };
      TaskStep taskStep2 = taskStep1;
      string str = "*";
      if (keys.Any<string>())
        str = string.Join(",", keys);
      taskStep2.Inputs["ConnectedServiceName"] = group.SecretStore.Endpoint.Id.ToString();
      taskStep2.Inputs["KeyVaultName"] = group.SecretStore.StoreName;
      taskStep2.Inputs["SecretsFilter"] = str;
      return (IList<TaskStep>) new TaskStep[1]{ taskStep2 };
    }

    public IDictionary<string, VariableValue> GetValues(
      VariableGroup group,
      ServiceEndpoint endpoint,
      IEnumerable<string> keys,
      bool includeSecrets)
    {
      if (!includeSecrets)
        return (IDictionary<string, VariableValue>) null;
      AzureKeyVaultVariableGroupProviderData providerData = group.ProviderData as AzureKeyVaultVariableGroupProviderData;
      return new KeyVaultProvider(this.m_requestContext, endpoint, providerData.Vault, this.m_projectId).GetSecrets(keys, true, out string _);
    }
  }
}
