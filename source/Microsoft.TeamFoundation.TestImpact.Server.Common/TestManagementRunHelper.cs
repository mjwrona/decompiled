// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.TestManagementRunHelper
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  internal class TestManagementRunHelper : ITestManagementRunHelper
  {
    public TestRun GetTestRun(TestImpactRequestContext context, int testRunId, Guid project)
    {
      try
      {
        return context.TestResultsHttpClient.GetTestRunByIdAsync(project, testRunId).Result;
      }
      catch (Exception ex)
      {
        context.RequestContext.Trace(6200450, TraceLevel.Error, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.CIServiceLayer, string.Format("Failed to get test run information {0}: error {1}", (object) testRunId, (object) ex));
        throw;
      }
    }
  }
}
