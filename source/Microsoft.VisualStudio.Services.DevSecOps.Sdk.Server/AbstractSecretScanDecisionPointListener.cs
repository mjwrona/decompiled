// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.AbstractSecretScanDecisionPointListener
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.DevSecOps.WebApi;
using System;
using System.Diagnostics;
using System.IO;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server
{
  public abstract class AbstractSecretScanDecisionPointListener : ISubscriber
  {
    public SubscriberPriority Priority => SubscriberPriority.Normal;

    public abstract string Name { get; }

    public abstract EventNotificationStatus ProcessEvent(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      object notificationEventArgs,
      out int statusCode,
      out string statusMessage,
      out ExceptionPropertyCollection properties);

    public abstract Type[] SubscribedTypes();

    public abstract string Area { get; }

    public abstract string Feature { get; }

    protected ScanResult PerformScan(
      IVssRequestContext requestContext,
      Guid projectId,
      string auditAction,
      string pipelineId,
      int? revision,
      PipelineType pipelineType,
      object eventData)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      using (MemoryStream inputStream = new MemoryStream(JsonFormatter.Serialize(eventData)))
      {
        ScanResult scanResult = requestContext.GetService<IStreamScannerService>().ScanPipelineDefinitionStream(requestContext, (Stream) inputStream, projectId, pipelineType, pipelineId, revision);
        stopwatch.Stop();
        this.PublishClientTrace(requestContext, projectId, auditAction, pipelineId, revision, scanResult, stopwatch);
        return scanResult;
      }
    }

    private void PublishClientTrace(
      IVssRequestContext requestContext,
      Guid projectId,
      string auditAction,
      string pipelineId,
      int? revision,
      ScanResult scanResult,
      Stopwatch stopwatch)
    {
      ClientTraceData properties = new ClientTraceData();
      properties.Add("ProjectId", (object) projectId);
      properties.Add("AuditAction", (object) auditAction);
      properties.Add("PipelineId", (object) pipelineId);
      properties.Add("Revision", (object) revision.GetValueOrDefault());
      properties.Add("ViolationCount", (object) scanResult.Violations.Count);
      properties.Add("ScanDurationInMs", (object) stopwatch.ElapsedMilliseconds);
      requestContext.GetService<ClientTraceService>().Publish(requestContext, this.Area, this.Feature, properties);
    }
  }
}
