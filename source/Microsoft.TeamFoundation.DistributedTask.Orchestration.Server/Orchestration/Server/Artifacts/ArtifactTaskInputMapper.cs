// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ArtifactTaskInputMapper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public static class ArtifactTaskInputMapper
  {
    public static void PopulateMappedTaskInputs(
      IArtifactType artifactType,
      IDictionary<string, string> taskInputs,
      Guid? taskDefinitionId = null)
    {
      ArgumentUtility.CheckForNull<IArtifactType>(artifactType, nameof (artifactType));
      ArgumentUtility.CheckForNull<IDictionary<string, string>>(taskInputs, nameof (taskInputs));
      ArtifactTaskInputMapper.ProcessYamlInputMapping(artifactType, taskInputs);
      EndpointStringResolver endpointStringResolver = new EndpointStringResolver(JToken.FromObject((object) taskInputs));
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) (artifactType.GetTaskInputMapping(taskDefinitionId) ?? (IDictionary<string, string>) new Dictionary<string, string>()))
        taskInputs[keyValuePair.Key] = endpointStringResolver.ResolveVariablesInMustacheFormat(keyValuePair.Value);
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) (artifactType.TaskInputDefaultValues ?? (IDictionary<string, string>) new Dictionary<string, string>()))
      {
        if (!taskInputs.ContainsKey(keyValuePair.Key) || string.IsNullOrEmpty(taskInputs[keyValuePair.Key]))
          taskInputs[keyValuePair.Key] = keyValuePair.Value;
      }
    }

    public static void ProcessYamlInputMapping(
      IArtifactType artifactType,
      IDictionary<string, string> parameters)
    {
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) (artifactType.YamlInputMapping ?? (IDictionary<string, string>) new Dictionary<string, string>()))
      {
        string str;
        if (parameters.TryGetValue(keyValuePair.Key, out str))
          parameters[keyValuePair.Value] = str;
      }
    }
  }
}
