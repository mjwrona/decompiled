// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ProcessParametersExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Common.Contracts;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class ProcessParametersExtensions
  {
    private const string ProcessParameterPrefix = "Parameters.";

    public static Dictionary<string, string> GetProcessParametersInputs(
      this ProcessParameters processParameters)
    {
      Dictionary<string, string> parametersInputs = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (processParameters != null && processParameters.Inputs != null && processParameters.Inputs.Count > 0)
      {
        foreach (TaskInputDefinitionBase input in (IEnumerable<TaskInputDefinitionBase>) processParameters.Inputs)
        {
          if (input != null && !string.IsNullOrEmpty(input.Name))
          {
            string key = "Parameters." + input.Name;
            parametersInputs[key] = input.DefaultValue;
          }
        }
      }
      return parametersInputs;
    }

    public static Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> GetProcessParametersAsDataModelVariables(
      this ProcessParameters processParameters)
    {
      Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> dataModelVariables = new Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>();
      foreach (KeyValuePair<string, string> processParametersInput in processParameters.GetProcessParametersInputs())
        dataModelVariables[processParametersInput.Key] = new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue()
        {
          IsSecret = false,
          Value = processParametersInput.Value
        };
      return dataModelVariables;
    }

    public static Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> GetProcessParametersAsWebContractVariables(
      this ProcessParameters processParameters)
    {
      Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> contractVariables = new Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>();
      foreach (KeyValuePair<string, string> processParametersInput in processParameters.GetProcessParametersInputs())
        contractVariables[processParametersInput.Key] = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue()
        {
          IsSecret = false,
          Value = processParametersInput.Value
        };
      return contractVariables;
    }
  }
}
