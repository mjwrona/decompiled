// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.VstsVariableGroupValueProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal class VstsVariableGroupValueProvider : IVariableValueProvider
  {
    public string GroupType => "Vsts";

    public IList<TaskStep> GetSteps(
      IPipelineContext context,
      VariableGroupReference group,
      IEnumerable<string> keys)
    {
      return (IList<TaskStep>) Array.Empty<TaskStep>();
    }

    public IDictionary<string, VariableValue> GetValues(
      VariableGroup group,
      ServiceEndpoint endpoint,
      IEnumerable<string> keys,
      bool includeSecrets)
    {
      VariablesDictionary values = new VariablesDictionary();
      foreach (string key in keys)
      {
        VariableValue variableValue;
        if (group.Variables.TryGetValue(key, out variableValue) && (includeSecrets || !variableValue.IsSecret))
          values[key] = variableValue;
      }
      return (IDictionary<string, VariableValue>) values;
    }

    public bool ShouldGetValues(IPipelineContext context) => true;
  }
}
