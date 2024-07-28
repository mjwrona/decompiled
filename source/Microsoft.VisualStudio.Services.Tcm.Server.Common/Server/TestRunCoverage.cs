// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestRunCoverage
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  [SoapInclude(typeof (Coverage))]
  public class TestRunCoverage : Coverage
  {
    [XmlAttribute]
    public int TestRunId { get; set; }

    internal static List<TestRunCoverage> Query(
      TestManagementRequestContext context,
      string projectName,
      int testRunId,
      CoverageQueryFlags flags)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return new List<TestRunCoverage>();
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        return managementDatabase.QueryTestRunCoverage(projectFromName.GuidId, testRunId, flags);
    }

    internal void Update(TestManagementRequestContext context, Guid projectId)
    {
      context.TraceAndDebugAssert("BusinessLayer", this.TestRunId != 0, "Invalid TestRunId");
      context.SecurityManager.CheckServiceAccount(context);
      this.AssignIds();
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.UpdateTestRunCoverage(this.TestRunId, (Coverage) this, projectId);
      TestRunCoverage.FireNotification(context, this.TestRunId);
    }

    private static void FireNotification(TestManagementRequestContext context, int testRunId) => context.EventService.PublishNotification(context.RequestContext, (object) new TestRunCoverageUpdatedNotification(testRunId));
  }
}
