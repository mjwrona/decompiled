// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Common.ParameterDataInfoHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1401105B-6771-499A-8DF3-F3CBE1BB3AE4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ParameterDataInfoHelper
  {
    public static void RenameParameter(
      this TestCaseParameterDataInfo parameterDataInfo,
      string oldName,
      string newName)
    {
      if (parameterDataInfo.ParameterMap == null)
        return;
      foreach (ParameterDefinition parameter in parameterDataInfo.ParameterMap)
        parameter.LocalParamName = string.Equals(oldName, parameter.LocalParamName, StringComparison.OrdinalIgnoreCase) ? newName : parameter.LocalParamName;
    }

    public static void DeleteParameter(
      this TestCaseParameterDataInfo parameterDataInfo,
      string paramName)
    {
      if (parameterDataInfo.ParameterMap == null)
        return;
      for (int index = parameterDataInfo.ParameterMap.Count - 1; index >= 0; --index)
      {
        ParameterDefinition parameter = parameterDataInfo.ParameterMap[index];
        if (string.Equals(paramName, parameter.LocalParamName, StringComparison.OrdinalIgnoreCase))
        {
          parameterDataInfo.ParameterMap.RemoveAt(index);
          break;
        }
      }
    }

    public static void AddParameter(
      this TestCaseParameterDataInfo parameterDataInfo,
      string paramName,
      bool isSharedParameter = true)
    {
      if (parameterDataInfo.SharedParameterDataSetIds.Length == 0)
        return;
      int parameterDataSetId = parameterDataInfo.SharedParameterDataSetIds[0];
      ParameterDefinition parameterDefinition = (ParameterDefinition) new SharedParameterDefinition(paramName, "", parameterDataSetId);
      parameterDataInfo.ParameterMap.Add(parameterDefinition);
    }

    public static bool ContainsParam(
      this TestCaseParameterDataInfo parameterDataInfo,
      string localParamName)
    {
      if (parameterDataInfo.ParameterMap != null)
      {
        foreach (ParameterDefinition parameter in parameterDataInfo.ParameterMap)
        {
          if (string.Equals(parameter.LocalParamName, localParamName, StringComparison.OrdinalIgnoreCase))
            return true;
        }
      }
      return false;
    }

    public static List<string> GetLocalParamNames(this TestCaseParameterDataInfo parameterDataInfo)
    {
      List<ParameterDefinition> parameterMap = parameterDataInfo.ParameterMap;
      return parameterMap != null ? parameterMap.Select<ParameterDefinition, string>((Func<ParameterDefinition, string>) (paramDefinition => paramDefinition.LocalParamName)).ToList<string>() : (List<string>) null;
    }

    public static List<string> GetSharedParamNames(this TestCaseParameterDataInfo parameterDataInfo)
    {
      List<string> sharedParamNames = new List<string>();
      if (parameterDataInfo.ParameterMap != null)
      {
        foreach (ParameterDefinition parameter in parameterDataInfo.ParameterMap)
        {
          if (parameter is SharedParameterDefinition parameterDefinition)
            sharedParamNames.Add(parameterDefinition.SharedParameterName);
        }
      }
      return sharedParamNames;
    }
  }
}
