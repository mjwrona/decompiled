// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITestManagementObjectHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public interface ITestManagementObjectHelper
  {
    TeamProjectReference GetProjectReference(
      string projectIdentifier,
      IVssRequestContext requestContext);

    TestRun CreateTestRun(
      TestRun run,
      TestManagementRequestContext requestContext,
      TestSettings settings,
      TestCaseResult[] results,
      bool populateDataRowCount,
      string teamProjectName);

    Microsoft.TeamFoundation.Build.WebApi.Build GetBuildDetailFromUri(
      string buildUri,
      IVssRequestContext requestContext,
      Guid projectId,
      IBuildServiceHelper buildServiceHelper);

    void CheckForViewTestResultPermission(
      string projectName,
      TestManagementRequestContext requestContext);

    TestSettings GetTestSettingdById(
      TestManagementRequestContext requestContext,
      int testSettingId,
      string projectName);

    List<TestRunStatistic> QueryTestRunStatistics(
      TestManagementRequestContext context,
      string projectName,
      int testRunId);
  }
}
