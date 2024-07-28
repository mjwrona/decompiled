// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.Extensions.ReleaseDefinitionPipelineHelper
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DevSecOps.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.Extensions
{
  public class ReleaseDefinitionPipelineHelper : PipelineHelper
  {
    private static Regex s_preDeploymentGatesRegex = new Regex("(?i)environments\\[(?<E>\\d+)\\]\\.preDeploymentGates.gates\\[(?<G>\\d+)\\]\\.tasks\\[(?<T>\\d+)\\]\\.(?<N>\\w+)(?:\\.?(?<N2>\\w+))?", RegexOptions.Compiled);
    private static Regex s_deployPhaseAndWorkflowRegex = new Regex("(?i)environments\\[(?<E>\\d+)\\]\\.deployPhases\\[(?<D>\\d+)\\]\\.workflowTasks\\[(?<W>\\d+)\\]\\.(?<N>\\w+)(?:\\.?(?<N2>\\w+|'\\w+'|\\['(?<N3>.+)'\\]))?", RegexOptions.Compiled);

    public override bool IsUserDefined(Violation violation) => throw new NotSupportedException();

    public override string ComposeErrorMessage(
      IEnumerable<string> credentialLocations,
      bool usePrescriptiveBlockMessage = false)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (credentialLocations != null)
      {
        stringBuilder.Append(credentialLocations.Count<string>() > 0 ? "\r\n" : "");
        stringBuilder.Append(this.JoinCredentialLocations(credentialLocations));
      }
      return !usePrescriptiveBlockMessage ? DevSecOpsSdkResources.BlockReleaseDefinitionSaveWhenSecretsAreFound((object) stringBuilder.ToString()) : DevSecOpsSdkResources.BlockReleaseDefinitionSaveWhenHighConfidenceSecretsAreFound((object) stringBuilder.ToString());
    }

    public bool TryGetEnvironmentIndexAndVariableName(
      IVssRequestContext requestContext,
      string variableLocationJpath,
      int tracePoint,
      string traceArea,
      string traceLayer,
      out int? environmentIndex,
      out string variableName)
    {
      try
      {
        if (string.IsNullOrEmpty(variableLocationJpath))
        {
          environmentIndex = new int?();
          variableName = (string) null;
          return false;
        }
        int startIndex1;
        if (variableLocationJpath.StartsWith("variables.", StringComparison.Ordinal))
        {
          environmentIndex = new int?();
          startIndex1 = "variables.".Length;
        }
        else if (variableLocationJpath.StartsWith("variables['", StringComparison.Ordinal))
        {
          environmentIndex = new int?();
          startIndex1 = "variables['".Length;
        }
        else if (variableLocationJpath.StartsWith("environments[", StringComparison.Ordinal))
        {
          int length = "environments[".Length;
          int startIndex2 = variableLocationJpath.IndexOf(']');
          int result;
          if (int.TryParse(variableLocationJpath.Substring(length, startIndex2 - length), out result))
          {
            environmentIndex = new int?(result);
            int num = variableLocationJpath.IndexOf(".variables.", startIndex2, StringComparison.Ordinal);
            if (num > startIndex2)
            {
              startIndex1 = num + ".variables.".Length;
            }
            else
            {
              startIndex1 = variableLocationJpath.IndexOf(".variables['", startIndex2, StringComparison.Ordinal);
              if (startIndex1 > startIndex2)
                startIndex1 += ".variables['".Length;
            }
          }
          else
          {
            environmentIndex = new int?();
            variableName = (string) null;
            return false;
          }
        }
        else
        {
          environmentIndex = new int?();
          variableName = (string) null;
          return false;
        }
        int num1;
        if (variableLocationJpath.EndsWith("'].value", StringComparison.Ordinal))
          num1 = variableLocationJpath.Length - "'].value".Length;
        else if (variableLocationJpath.EndsWith(".value", StringComparison.Ordinal))
        {
          num1 = variableLocationJpath.Length - ".value".Length;
        }
        else
        {
          environmentIndex = new int?();
          variableName = (string) null;
          return false;
        }
        variableName = variableLocationJpath.Substring(startIndex1, num1 - startIndex1);
        return true;
      }
      catch (ArgumentOutOfRangeException ex)
      {
        requestContext.TraceException(tracePoint, traceArea, traceLayer, (Exception) ex);
        environmentIndex = new int?();
        variableName = (string) null;
        return false;
      }
    }

    public bool TryGetGateAndTaskIndices(
      IVssRequestContext requestContext,
      string preDeploymentGatesLocationJpath,
      int tracePoint,
      string traceArea,
      string traceLayer,
      out int? environmentIndex,
      out int? gateIndex,
      out int? taskIndex,
      out string fieldType,
      out string fieldName)
    {
      environmentIndex = new int?();
      gateIndex = new int?();
      taskIndex = new int?();
      fieldType = string.Empty;
      fieldName = string.Empty;
      string s1 = "-1";
      string s2 = "-1";
      string s3 = "-1";
      try
      {
        Match match = ReleaseDefinitionPipelineHelper.s_preDeploymentGatesRegex.Match(preDeploymentGatesLocationJpath);
        if (match.Success)
        {
          s1 = match.Groups["E"].Value;
          s2 = match.Groups["G"].Value;
          s3 = match.Groups["T"].Value;
          if (!match.Groups["N2"].Success)
          {
            fieldType = "Unknown";
            fieldName = match.Groups["N"].Value;
          }
          else
          {
            fieldType = match.Groups["N"].Value;
            fieldName = match.Groups["N2"].Value;
          }
        }
        if (!string.IsNullOrEmpty(fieldName))
        {
          if (!string.IsNullOrEmpty(fieldType))
          {
            int result1;
            if (int.TryParse(s1, out result1))
            {
              int result2;
              if (int.TryParse(s2, out result2))
              {
                int result3;
                if (int.TryParse(s3, out result3))
                {
                  environmentIndex = new int?(result1);
                  gateIndex = new int?(result2);
                  taskIndex = new int?(result3);
                  return true;
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(tracePoint, traceArea, traceLayer, ex);
      }
      return false;
    }

    public bool TryGetDeployPhaseAndWorkflowIndices(
      IVssRequestContext requestContext,
      string deployPhaseLocationJpath,
      int tracePoint,
      string traceArea,
      string traceLayer,
      out int? environmentIndex,
      out int? deployPhaseIndex,
      out int? workflowTaskIndex,
      out string fieldType,
      out string fieldName)
    {
      environmentIndex = new int?();
      deployPhaseIndex = new int?();
      workflowTaskIndex = new int?();
      fieldName = string.Empty;
      fieldType = string.Empty;
      string s1 = "-1";
      string s2 = "-1";
      string s3 = "-1";
      try
      {
        Match match = ReleaseDefinitionPipelineHelper.s_deployPhaseAndWorkflowRegex.Match(deployPhaseLocationJpath);
        if (match.Success)
        {
          s1 = match.Groups["E"].Value;
          s2 = match.Groups["D"].Value;
          s3 = match.Groups["W"].Value;
          if (!match.Groups["N3"].Success && !match.Groups["N2"].Success)
          {
            fieldType = "Unknown";
            fieldName = match.Groups["N"].Value;
          }
          else if (!match.Groups["N3"].Success)
          {
            fieldType = match.Groups["N"].Value;
            fieldName = match.Groups["N2"].Value;
          }
          else
          {
            fieldType = match.Groups["N"].Value;
            fieldName = match.Groups["N3"].Value;
          }
        }
        if (!string.IsNullOrEmpty(fieldName))
        {
          if (!string.IsNullOrEmpty(fieldType))
          {
            int result1;
            if (int.TryParse(s1, out result1))
            {
              int result2;
              if (int.TryParse(s2, out result2))
              {
                int result3;
                if (int.TryParse(s3, out result3))
                {
                  environmentIndex = new int?(result1);
                  deployPhaseIndex = new int?(result2);
                  workflowTaskIndex = new int?(result3);
                  return true;
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(tracePoint, traceArea, traceLayer, ex);
      }
      return false;
    }
  }
}
