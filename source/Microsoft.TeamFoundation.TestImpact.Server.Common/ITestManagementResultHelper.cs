// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.ITestManagementResultHelper
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [CLSCompliant(false)]
  public interface ITestManagementResultHelper
  {
    IList<TestCaseResult> QueryTestResultsByRun(
      TestImpactRequestContext requestContext,
      Guid project,
      int testRunId);

    IList<ShallowTestCaseResult> GetTestResultDetailsForBuild(
      TestImpactRequestContext requestContext,
      Guid project,
      int buildId);

    IList<ShallowTestCaseResult> GetTestResultDetailsForRelease(
      TestImpactRequestContext requestContext,
      Guid project,
      int releaseId,
      int releaseEnvId);
  }
}
