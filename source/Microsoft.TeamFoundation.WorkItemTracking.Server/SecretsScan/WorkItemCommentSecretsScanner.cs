// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.SecretsScan.WorkItemCommentSecretsScanner
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server;
using Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.Extensions;
using Microsoft.VisualStudio.Services.DevSecOps.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.SecretsScan
{
  internal class WorkItemCommentSecretsScanner : SecretScanStringBase<WorkItemCommentScanData>
  {
    public WorkItemCommentSecretsScanner(CancellationToken cancellationToken)
      : base("WorkItemService", "SecretScanWorkItemComments", cancellationToken)
    {
    }

    protected override bool ShouldScan(
      IVssRequestContext requestContext,
      WorkItemCommentScanData eventArgs)
    {
      return WorkItemTrackingFeatureFlags.IsWorkItemTrackingSecretsScanningEnabled(requestContext) && base.ShouldScan(requestContext, eventArgs);
    }

    protected override ScanResult PerformScan(
      IVssRequestContext requestContext,
      WorkItemCommentScanData eventArgs,
      string content)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (eventArgs == null)
        throw new ArgumentNullException(nameof (eventArgs));
      if (string.IsNullOrEmpty(content))
        throw new ArgumentNullException(nameof (content));
      ScanResult scanResult = (ScanResult) null;
      TelemetryHelper.AddToResults(this.ClientTraceData, "WorkItemIdentity", string.Format("{0}:{1}:{2}", (object) eventArgs.ProjectId, (object) eventArgs.WorkItemId, (object) eventArgs.RevisionId));
      IStreamScannerService service = requestContext.GetService<IStreamScannerService>();
      using (MemoryStream inputStream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        this.CancellationToken.ThrowIfCancellationRequested();
        scanResult = service.ScanPipelineDefinitionStream(requestContext, (Stream) inputStream, eventArgs.ProjectId, PipelineType.WorkItemComment, eventArgs.WorkItemId.ToString((IFormatProvider) CultureInfo.InvariantCulture), new int?(eventArgs.RevisionId), cancellationToken: this.CancellationToken);
        this.CancellationToken.ThrowIfCancellationRequested();
        stopwatch.Stop();
        this.ClientTraceData.AddToResults("SecretScanDurationInMilliseconds", (object) stopwatch.ElapsedMilliseconds);
      }
      return scanResult;
    }

    protected override string ComposeErrorMessage(
      IVssRequestContext requestContext,
      WorkItemCommentScanData eventArg,
      IReadOnlyList<string> userFacingLocations)
    {
      return new WorkItemPipelineHelper().ComposeErrorMessage((IEnumerable<string>) userFacingLocations, false);
    }

    protected override bool IsBlockingEnabled(IVssRequestContext requestContext) => WorkItemTrackingFeatureFlags.IsWorkItemTrackingSecretsBlockingEnabled(requestContext);

    protected override bool IsPrescriptiveBlockingEnabled(IVssRequestContext requestContext) => true;

    protected override BypassType DetermineBypassType(
      IVssRequestContext requestContext,
      WorkItemCommentScanData eventArgs,
      string commentText)
    {
      return eventArgs != null ? this.DetermineBypassTypeFromComment(commentText) : BypassType.None;
    }

    protected override bool IsReportableLocation(
      IVssRequestContext requestContext,
      WorkItemCommentScanData eventArgs,
      Violation violation,
      out string reportableLocation)
    {
      reportableLocation = "History (System.History)";
      return true;
    }
  }
}
