// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.VariablesRequestValidator
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class VariablesRequestValidator : IBuildRequestValidator
  {
    private readonly HashSet<string> m_internalRuntimeVariables;

    public VariablesRequestValidator(HashSet<string> internalRuntimeVariables = null) => this.m_internalRuntimeVariables = internalRuntimeVariables != null ? new HashSet<string>((IEnumerable<string>) internalRuntimeVariables, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public BuildRequestValidationResult ValidateRequest(
      IVssRequestContext requestContext,
      BuildRequestValidationContext validationContext)
    {
      ArgumentUtility.CheckForNull<BuildRequestValidationContext>(validationContext, nameof (validationContext));
      ArgumentUtility.CheckForNull<BuildDefinition>(validationContext.Definition, "validationContext.Definition");
      ArgumentUtility.CheckForNull<BuildData>(validationContext.Build, "validationContext.Queue.Build");
      BuildDefinition definition = validationContext.Definition;
      BuildRequestValidationResult validationResult1 = new BuildRequestValidationResult()
      {
        Result = ValidationResult.OK,
        Message = string.Empty
      };
      if (string.IsNullOrEmpty(validationContext.Build.Parameters))
        return validationResult1;
      ProjectPipelineGeneralSettingsHelper generalSettingsHelper = new ProjectPipelineGeneralSettingsHelper(requestContext, validationContext.Build.ProjectId, true);
      if (!generalSettingsHelper.EnforceSettableVar && !generalSettingsHelper.AuditEnforceSettableVar)
        return validationResult1;
      BuildRequestValidationResult validationResult2 = validationResult1;
      IDictionary<string, string> queueTimeVariables = BuildRequestHelper.DeserializeParameters(validationContext.Build.Parameters);
      if (queueTimeVariables?.Keys != null)
      {
        string overriddenVariables = this.GetIncorrectlyOverriddenVariables(definition, queueTimeVariables);
        if (!string.IsNullOrEmpty(overriddenVariables))
        {
          if (!generalSettingsHelper.EnforceSettableVar && generalSettingsHelper.AuditEnforceSettableVar)
          {
            validationResult2.ShowAuditInformation = true;
            validationResult2.AuditInsecureSettableVariables = overriddenVariables;
            return validationResult2;
          }
          requestContext.TraceAlways(12030372, TraceLevel.Error, "Build2", "BuildRequestHelper", "Setting variables ({0}) is preventing this build from running.", (object) overriddenVariables);
          validationResult1 = new BuildRequestValidationResult()
          {
            Result = ValidationResult.Error,
            Message = BuildServerResources.EnforceSettableVarErrorMessage((object) overriddenVariables)
          };
        }
      }
      return validationResult1;
    }

    private string GetIncorrectlyOverriddenVariables(
      BuildDefinition definition,
      IDictionary<string, string> queueTimeVariables)
    {
      return string.Join(",", queueTimeVariables.Keys.Where<string>((Func<string, bool>) (variable => !string.IsNullOrEmpty(variable) && !VariablesRequestValidator.IsVariableSystemInternal(variable, this.m_internalRuntimeVariables) && !VariablesRequestValidator.IsVariableOverridable(variable, (IDictionary<string, BuildDefinitionVariable>) definition.Variables))));
    }

    private static bool IsVariableSystemInternal(
      string variable,
      HashSet<string> internalSystemVariables)
    {
      return internalSystemVariables.Contains(variable);
    }

    private static bool IsVariableOverridable(
      string variable,
      IDictionary<string, BuildDefinitionVariable> definitionVariables)
    {
      foreach (KeyValuePair<string, BuildDefinitionVariable> definitionVariable in (IEnumerable<KeyValuePair<string, BuildDefinitionVariable>>) definitionVariables)
      {
        if (string.Equals(definitionVariable.Key, variable, StringComparison.OrdinalIgnoreCase) && definitionVariable.Value.AllowOverride)
          return true;
      }
      return false;
    }
  }
}
