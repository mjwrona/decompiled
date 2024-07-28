// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageStatusCheckResultHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public static class CoverageStatusCheckResultHelper
  {
    public static CoverageStatusCheckResult GetCoverageStatusCheckResult(
      string source,
      string name,
      CoverageStatusCheckState coverageStatusCheckState,
      string description = null)
    {
      CoverageStatusCheckResult statusCheckResult = new CoverageStatusCheckResult()
      {
        Source = source,
        Name = name,
        State = coverageStatusCheckState
      };
      if (!string.IsNullOrEmpty(description))
      {
        statusCheckResult.Description = description;
        return statusCheckResult;
      }
      statusCheckResult.Description = CoverageStatusCheckResultHelper.GetCoverageStatusCheckResultDescription(coverageStatusCheckState, source);
      return statusCheckResult;
    }

    public static string GetCoverageStatusCheckResultDescription(
      CoverageStatusCheckState state,
      string source)
    {
      string str = string.Empty;
      switch (state)
      {
        case CoverageStatusCheckState.Error:
          str = CoverageResources.CoverageStatusCheckError;
          break;
        case CoverageStatusCheckState.Failed:
          str = CoverageResources.CoverageStatusCheckFailed;
          break;
        case CoverageStatusCheckState.InProgress:
          str = CoverageResources.CoverageStatusCheckInProgress;
          break;
        case CoverageStatusCheckState.Queued:
          str = CoverageResources.CoverageStatusCheckQueued;
          break;
        case CoverageStatusCheckState.Succeeded:
          str = CoverageResources.CoverageStatusCheckSucceeded;
          break;
        case CoverageStatusCheckState.NotApplicable:
          str = CoverageResources.CoverageStatusCheckNotApplicable;
          break;
      }
      return str + " " + source;
    }
  }
}
