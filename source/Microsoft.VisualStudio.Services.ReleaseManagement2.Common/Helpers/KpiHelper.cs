// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers.KpiHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers
{
  public static class KpiHelper
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public static void CreateAutoTriggeredReleaseCreationFailureEvent(
      IVssRequestContext requestContext,
      string featureName,
      string kpiScope,
      int failedCount,
      Exception exception,
      string errorMessage = "")
    {
      Action publishAction = (Action) (() => KpiHelper.PublishKpi(requestContext, "ReleaseManagementService", kpiScope, new KpiMetric()
      {
        Name = "RmKpiAutoTriggeredReleaseCreationFailure",
        Value = (double) failedCount,
        TimeStamp = DateTime.UtcNow
      }));
      KpiHelper.SafeExecute(requestContext, publishAction, 1990001);
      string str = TeamFoundationExceptionFormatter.FormatException(exception, false);
      requestContext.Trace(1976486, TraceLevel.Error, "ReleaseManagementService", "JobLayer", "HostId: {0}, FailedCount: {1}, Feature: {2}, errorMessage: {3}, exception: {4}", (object) kpiScope, (object) failedCount, (object) featureName, (object) errorMessage, (object) str);
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("Exception", str);
      IVssRequestContext requestContext1 = requestContext;
      string feature = featureName;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(requestContext1, "ReleaseManagementService", feature, properties);
    }

    public static void CreateAutoTriggerReleaseCreationFailedEventBecauseOfUser(
      IVssRequestContext requestContext,
      string kpiScope,
      int failedCount)
    {
      KpiHelper.SafeExecute(requestContext, (Action) (() => KpiHelper.PublishKpi(requestContext, "ReleaseManagementService", kpiScope, new KpiMetric()
      {
        Name = "RmKpiAutoTriggerReleaseCreationFailedBecauseOfUser",
        Value = (double) failedCount,
        TimeStamp = DateTime.UtcNow
      })), 1990001);
    }

    public static void CreateRunPlanFailedTimelineEventPassedEvent(
      IVssRequestContext requestContext,
      string kpiScope,
      int failedCount)
    {
      KpiHelper.SafeExecute(requestContext, (Action) (() => KpiHelper.PublishKpi(requestContext, "ReleaseManagementService", kpiScope, new KpiMetric()
      {
        Name = "RmKpiReleaseFailureRunPlanFailedTimelineEventPassed",
        Value = (double) failedCount,
        TimeStamp = DateTime.UtcNow
      })), 1990003);
    }

    public static void CreateWorkflowFailedEvent(
      IVssRequestContext requestContext,
      string kpiScope,
      int failedCount)
    {
      KpiHelper.SafeExecute(requestContext, (Action) (() => KpiHelper.PublishKpi(requestContext, "ReleaseManagementService", kpiScope, new KpiMetric()
      {
        Name = "RmReleaseFailureWorkflowFailed",
        Value = (double) failedCount,
        TimeStamp = DateTime.UtcNow
      })), 1990004);
    }

    public static void CreateDownloadTaskFailedEvent(
      IVssRequestContext requestContext,
      string kpiScope,
      int failedCount)
    {
      KpiHelper.SafeExecute(requestContext, (Action) (() => KpiHelper.PublishKpi(requestContext, "ReleaseManagementService", kpiScope, new KpiMetric()
      {
        Name = "RmReleaseFailureDownloadTaskFailed",
        Value = (double) failedCount,
        TimeStamp = DateTime.UtcNow
      })), 1990005);
    }

    private static void PublishKpi(
      IVssRequestContext requestContext,
      string kpiArea,
      string kpiScope,
      KpiMetric kpiData)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      KpiService service = vssRequestContext.GetService<KpiService>();
      service.EnsureKpiIsRegistered(vssRequestContext, kpiArea, kpiData.Name, kpiScope, kpiData.Name, (string) null);
      service.Publish(vssRequestContext, kpiArea, KpiHelper.GetAccountHostId(requestContext), kpiScope, kpiData);
    }

    private static Guid GetAccountHostId(IVssRequestContext requestContext) => requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? requestContext.ServiceHost.ParentServiceHost.InstanceId : requestContext.ServiceHost.InstanceId;

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Analytics api shuld not throw")]
    private static void SafeExecute(
      IVssRequestContext requestContext,
      Action publishAction,
      int tracepoint)
    {
      try
      {
        publishAction();
      }
      catch (Exception ex)
      {
        requestContext.Trace(tracepoint, TraceLevel.Error, "ReleaseManagementService", "Analytics", "Failed to publish KPI data. Exception {0}", (object) ex);
      }
    }
  }
}
