// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors.CheckToGateConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.Azure.Pipelines.TaskCheck.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors
{
  internal class CheckToGateConverter
  {
    private readonly IVssRequestContext requestContext;
    private readonly Guid projectId;
    private readonly IServiceEndpointGuidMappingProvider serviceEndpointGuidProvider;

    public CheckToGateConverter(
      IVssRequestContext requestContext,
      Guid projectId,
      IServiceEndpointGuidMappingProvider serviceEndpointGuidProvider)
    {
      this.requestContext = requestContext;
      this.projectId = projectId;
      this.serviceEndpointGuidProvider = serviceEndpointGuidProvider;
    }

    public ReleaseDefinitionGate ConvertCheckToGate(
      HashSet<string> existingGateNames,
      CheckConfiguration config)
    {
      TaskCheckConfig configurationSettings = config.GetCheckConfigurationSettings() as TaskCheckConfig;
      string gateDisplayName = CheckToGateConverter.GenerateGateDisplayName(existingGateNames, configurationSettings.DisplayName);
      this.UpdateCheckConfigurationInputs(configurationSettings.Inputs);
      return new ReleaseDefinitionGate()
      {
        Tasks = (IList<WorkflowTask>) new List<WorkflowTask>()
        {
          new WorkflowTask()
          {
            AlwaysRun = true,
            ContinueOnError = false,
            CheckConfig = new CheckConfigurationReference()
            {
              Id = config.Id,
              Version = config.Version,
              ResourceName = config.Resource.Name,
              ResourceType = CheckToGateConverter.GetResourceType(config.Resource.Type)
            },
            DefinitionType = "task",
            Enabled = true,
            Inputs = (Dictionary<string, string>) configurationSettings.Inputs,
            Name = gateDisplayName,
            RefName = configurationSettings.DefinitionRef.Name,
            RetryCountOnTaskFailure = 0,
            TaskId = configurationSettings.DefinitionRef.Id,
            TimeoutInMinutes = 0,
            Version = configurationSettings.DefinitionRef.Version
          }
        },
        IsGenerated = true
      };
    }

    private static string GenerateGateDisplayName(
      HashSet<string> existingGateNames,
      string checkConfigName)
    {
      int num = 0;
      string gateDisplayName;
      for (gateDisplayName = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.CheckInjectedAsGateDisplayName, (object) checkConfigName); existingGateNames.Contains(gateDisplayName); gateDisplayName = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.CheckInjectedAsGateDisplayNameWithIndex, (object) checkConfigName, (object) num))
        ++num;
      existingGateNames.Add(gateDisplayName);
      return gateDisplayName;
    }

    private static string GetResourceType(string resourceType)
    {
      if (string.Equals("queue", resourceType, StringComparison.OrdinalIgnoreCase))
        return "Agent Pool";
      return string.Equals("endpoint", resourceType, StringComparison.OrdinalIgnoreCase) ? "Service Connection" : resourceType.ToUpperInvariant();
    }

    private void UpdateCheckConfigurationInputs(IDictionary<string, string> inputs)
    {
      IDictionary<string, Guid> dictionary = this.serviceEndpointGuidProvider.Get(this.requestContext, this.projectId);
      string key1;
      Guid guid1;
      if (inputs.TryGetValue("connectedServiceName", out key1) && dictionary.TryGetValue(key1, out guid1))
        inputs["connectedServiceName"] = guid1.ToString();
      string key2;
      Guid guid2;
      if (!inputs.TryGetValue("connectedServiceNameARM", out key2) || !dictionary.TryGetValue(key2, out guid2))
        return;
      inputs["connectedServiceNameARM"] = guid2.ToString();
    }

    internal static class TaskCheckConfigurationInputs
    {
      internal const string ConnectedServiceName = "connectedServiceName";
      internal const string ConnectedServiceNameARM = "connectedServiceNameARM";
    }
  }
}
