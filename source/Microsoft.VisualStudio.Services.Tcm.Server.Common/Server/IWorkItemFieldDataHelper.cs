// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.IWorkItemFieldDataHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public interface IWorkItemFieldDataHelper
  {
    void ValidateParamsAndUpdateResultsWithTestCasePropertiesIfRequired(
      TestManagementRequestContext context,
      GuidAndString projectId,
      TestCaseResult[] results,
      bool populateDataRowCount);

    void PopulateResultsFromTestCases(
      TestManagementRequestContext context,
      GuidAndString projectId,
      TestCaseResult[] results,
      int[] testCaseIds,
      bool populateDataRowCount);

    Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] PopulateTestResultFromWorkItem(
      TestManagementRequestContext tcmRequestContext,
      string projectName,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] testCaseResults,
      int testPlanId);
  }
}
