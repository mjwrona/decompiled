// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.GitHub.InputQueryValueUtilities
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.TeamFoundation.Common;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.GitHub
{
  public static class InputQueryValueUtilities
  {
    public static string ToFriendlyName(this string artifactConst)
    {
      string empty = string.Empty;
      string friendlyName;
      switch (artifactConst)
      {
        case "branch":
          friendlyName = "head";
          break;
        case "definition":
          friendlyName = "repositoryName";
          break;
        case "defaultVersionSpecific":
          friendlyName = "sha";
          break;
        case "name":
          friendlyName = "searchText";
          break;
        case "itemPath":
          friendlyName = "path";
          break;
        default:
          friendlyName = artifactConst;
          break;
      }
      return friendlyName;
    }

    public static string ToVerboseError(this string artifactConst)
    {
      string verboseError;
      switch (artifactConst)
      {
        case "branch":
          verboseError = Resources.BranchDetailsNotAvailable;
          break;
        case "definition":
          verboseError = Resources.RepositoryDetailsNotAvailable;
          break;
        case "connection":
          verboseError = Resources.ServiceEndPointIdNotPresent;
          break;
        default:
          verboseError = Resources.InputsMissing;
          break;
      }
      return verboseError;
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "By design")]
    [SuppressMessage("Microsoft.Design", "CA1062", Justification = "By design")]
    public static bool GetRequiredParameters(
      IDictionary<string, string> currentInputValues,
      IReadOnlyCollection<string> requiredParameters,
      out Dictionary<string, string> dataSourceParameters,
      out string errorMessage)
    {
      dataSourceParameters = new Dictionary<string, string>();
      if (!requiredParameters.IsNullOrEmpty<string>())
      {
        foreach (string requiredParameter in (IEnumerable<string>) requiredParameters)
        {
          string str;
          if (currentInputValues.TryGetValue(requiredParameter, out str))
          {
            dataSourceParameters.Add(requiredParameter.ToFriendlyName(), str);
          }
          else
          {
            errorMessage = requiredParameter.ToVerboseError();
            return false;
          }
        }
      }
      errorMessage = string.Empty;
      return true;
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "This is required to mock the external methods")]
    public static bool IsInputValueNotNull(
      this IDictionary<string, string> currentInputValues,
      string inputName)
    {
      string enumerable;
      return currentInputValues != null && currentInputValues.TryGetValue(inputName, out enumerable) && !enumerable.IsNullOrEmpty<char>();
    }
  }
}
