// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestReporting.Helpers.ReportingOptionsHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server.TestReporting.Models;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server.TestReporting.Helpers
{
  [CLSCompliant(false)]
  public static class ReportingOptionsHelper
  {
    private static readonly ReportingOptions DefaultReportingOptions = new ReportingOptions()
    {
      IncludeUnreliableTestResults = false,
      GroupByCategory = string.Empty,
      NotReportedOutcomes = new HashSet<Microsoft.TeamFoundation.TestManagement.Client.TestOutcome>()
    };
    private static readonly ReportingOptions CustomizedReportingOptions = new ReportingOptions()
    {
      IncludeUnreliableTestResults = true,
      GroupByCategory = "OutcomeConfidence",
      NotReportedOutcomes = new HashSet<Microsoft.TeamFoundation.TestManagement.Client.TestOutcome>()
      {
        Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.NotExecuted
      }
    };

    public static ReportingOptions GetReportingOptions(
      TestManagementRequestContext requestContext,
      GuidAndString projectId)
    {
      return requestContext.IsFeatureEnabled("TestManagement.Server.TRIReportCustomization") && ReportingOptionsHelper.GetRegistrySettingsEnabled(requestContext.RequestContext, "/Service/TestManagement/Settings/CustomReportingOptions") ? ReportingOptionsHelper.CustomizedReportingOptions : ReportingOptionsHelper.DefaultReportingOptions;
    }

    public static bool GetRegistrySettingsEnabled(
      IVssRequestContext requestContext,
      string registryName)
    {
      return requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/TestManagement/Settings/CustomReportingOptions", 0) == 1;
    }

    public static bool ShouldPublishFlakiness(
      TestManagementRequestContext requestContext,
      GuidAndString projectId)
    {
      if (requestContext.IsFeatureEnabled("TestManagement.Server.EnableTestProjectSettings"))
      {
        ProjectInfo projectFromGuid = requestContext.ProjectServiceHelper.GetProjectFromGuid(projectId.GuidId);
        TestResultsSettings testResultsSettings = requestContext.RequestContext.GetService<ITeamFoundationTestManagementTestResultsSettingsService>().GetTestResultsSettings(requestContext, projectFromGuid, TestResultsSettingsType.Flaky);
        FlakySettings flakySettings = testResultsSettings.FlakySettings;
        return (flakySettings != null ? (flakySettings.FlakyInSummaryReport.HasValue ? 1 : 0) : 0) != 0 && !testResultsSettings.FlakySettings.FlakyInSummaryReport.Value;
      }
      return requestContext.IsFeatureEnabled("TestManagement.Server.ShouldByPassFlakyFromSummary");
    }
  }
}
