// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.Extensions.BuildDefinitionPipelineHelper
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DevSecOps.WebApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.Extensions
{
  public class BuildDefinitionPipelineHelper : PipelineHelper
  {
    public override bool IsUserDefined(Violation violation) => throw new NotSupportedException();

    public override string ComposeErrorMessage(
      IEnumerable<string> credentialLocations,
      bool usePrescriptiveBlockMessage = false)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (credentialLocations != null)
        stringBuilder.Append(this.JoinCredentialLocations(credentialLocations));
      return !usePrescriptiveBlockMessage ? DevSecOpsSdkResources.BlockBuildDefinitionSaveWhenSecretsAreFound((object) stringBuilder.ToString()) : DevSecOpsSdkResources.BlockBuildDefinitionSaveWhenHighConfidenceSecretsAreFound((object) stringBuilder.ToString());
    }

    public bool TryGetVariableName(
      IVssRequestContext requestContext,
      string variableLocationJpath,
      int tracePoint,
      string traceArea,
      string traceLayer,
      out string variableName)
    {
      try
      {
        if (string.IsNullOrEmpty(variableLocationJpath))
        {
          variableName = (string) null;
          return false;
        }
        int length;
        if (variableLocationJpath.StartsWith("variables.", StringComparison.Ordinal))
          length = "variables.".Length;
        else if (variableLocationJpath.StartsWith("variables['", StringComparison.Ordinal))
        {
          length = "variables['".Length;
        }
        else
        {
          variableName = (string) null;
          return false;
        }
        int num;
        if (variableLocationJpath.EndsWith("'].value", StringComparison.Ordinal))
          num = variableLocationJpath.Length - "'].value".Length;
        else if (variableLocationJpath.EndsWith(".value", StringComparison.Ordinal))
        {
          num = variableLocationJpath.Length - ".value".Length;
        }
        else
        {
          variableName = (string) null;
          return false;
        }
        variableName = variableLocationJpath.Substring(length, num - length);
        return true;
      }
      catch (ArgumentOutOfRangeException ex)
      {
        requestContext.TraceException(tracePoint, traceArea, traceLayer, (Exception) ex);
        variableName = (string) null;
        return false;
      }
    }

    public bool TryGetPhaseAndStepIndices(
      IVssRequestContext requestContext,
      string stepLocationJpath,
      int tracePoint,
      string traceArea,
      string traceLayer,
      out int phaseIndex,
      out int stepIndex,
      out string fieldType,
      out string fieldName)
    {
      phaseIndex = -1;
      stepIndex = -1;
      fieldType = "";
      fieldName = "";
      try
      {
        string s1 = "";
        string s2 = "";
        if (stepLocationJpath.StartsWith("process.phases["))
        {
          int length = "process.phases[".Length;
          int num1 = stepLocationJpath.IndexOf("]", length + 1);
          s1 = stepLocationJpath.Substring(length, num1 - length);
          if (stepLocationJpath.Substring(num1 + 1).StartsWith(".steps["))
          {
            int startIndex = num1 + ".steps[".Length + 1;
            int num2 = stepLocationJpath.IndexOf("]", startIndex + 1);
            s2 = stepLocationJpath.Substring(startIndex, num2 - startIndex);
            string str = stepLocationJpath.Substring(num2 + 1);
            if (str.StartsWith(".inputs."))
            {
              fieldType = "inputs";
              fieldName = str.Substring(".inputs.".Length);
            }
            else if (str.StartsWith(".environment."))
            {
              fieldType = "environment";
              fieldName = str.Substring(".environment.".Length);
            }
            else if (str.StartsWith(".environment['"))
            {
              fieldType = "environment";
              fieldName = str.Substring(".environment['".Length, str.Length - ".environment['".Length - "']".Length);
            }
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
                phaseIndex = result1;
                stepIndex = result2;
                return true;
              }
            }
          }
        }
      }
      catch (ArgumentOutOfRangeException ex)
      {
        requestContext.TraceException(tracePoint, traceArea, traceLayer, (Exception) ex);
      }
      return false;
    }
  }
}
