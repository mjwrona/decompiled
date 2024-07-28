// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.BuildCoverage
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [ClassVisibility(ClientVisibility.Internal)]
  [SoapInclude(typeof (Coverage))]
  public class BuildCoverage : Coverage
  {
    [ClientProperty(ClientVisibility.Private)]
    public BuildConfiguration Configuration { get; set; }

    internal static List<BuildCoverage> Query(
      TestManagementRequestContext context,
      string projectName,
      string buildUri,
      CoverageQueryFlags flags)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return new List<BuildCoverage>();
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        return managementDatabase.QueryBuildCoverage(projectFromName.GuidId, buildUri, flags);
    }

    internal static void CreateBuildConfiguration(
      TestManagementRequestContext context,
      Guid projectId,
      BuildConfiguration config)
    {
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.CreateBuildConfiguration(projectId, config);
    }

    internal void Update(
      TestManagementRequestContext context,
      int coverageChangeId,
      Guid projectId)
    {
      context.IfNullThenTraceAndDebugFail("BusinessLayer", (object) this.Configuration, "Configuration");
      bool assertCondition = this.Configuration.BuildConfigurationId != 0 || this.Modules.Count <= 0;
      string message = "BuildConfigurationId == 0 and coverage data was reported";
      context.TraceAndDebugAssert("BusinessLayer", assertCondition, message);
      context.SecurityManager.CheckServiceAccount(context);
      this.AssignIds();
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.UpdateBuildCoverage(this.Configuration.BuildConfigurationId, (Coverage) this, coverageChangeId, projectId);
      context.RequestContext.Trace(1015929, TraceLevel.Info, "TestManagementJob", "BusinessLayer", "executed prc_UpdateBuildCoverage successfully");
      BuildCoverage.FireNotification(context, this.Configuration);
    }

    private static void FireNotification(
      TestManagementRequestContext context,
      BuildConfiguration configuration)
    {
      context.EventService.PublishNotification(context.RequestContext, (object) new BuildCoverageUpdatedNotification(configuration));
    }
  }
}
