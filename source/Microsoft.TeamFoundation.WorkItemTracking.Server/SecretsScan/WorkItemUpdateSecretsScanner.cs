// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.SecretsScan.WorkItemUpdateSecretsScanner
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.UpdateState;
using Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server;
using Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.Extensions;
using Microsoft.VisualStudio.Services.DevSecOps.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.SecretsScan
{
  internal class WorkItemUpdateSecretsScanner : SecretScanBatchBase<WorkItemUpdateState>
  {
    private readonly HashSet<string> DoNotScanFieldsSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal)
    {
      "Microsoft.VSTS.Common.Priority",
      "Microsoft.VSTS.Common.Severity"
    };

    public WorkItemUpdateSecretsScanner(CancellationToken cancellationToken)
      : base("WorkItemService", "SecretScanWorkItems", cancellationToken)
    {
    }

    protected override bool ShouldScan(
      IVssRequestContext requestContext,
      IDictionary<int, WorkItemUpdateState> eventArgs)
    {
      return WorkItemTrackingFeatureFlags.IsWorkItemTrackingSecretsScanningEnabled(requestContext) && base.ShouldScan(requestContext, eventArgs);
    }

    protected override object GetSerializableScanContent(
      IVssRequestContext requestContext,
      WorkItemUpdateState workItemUpdate,
      out int contentSize)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      contentSize = 0;
      if (workItemUpdate == null || workItemUpdate.IsEmpty() || workItemUpdate.FieldData == null || workItemUpdate.FieldUpdates.IsNullOrEmpty<KeyValuePair<int, object>>() || !workItemUpdate.HasFieldUpdates || workItemUpdate.HasFieldUpdate(-404))
        return (object) null;
      int revision = workItemUpdate.FieldData.Revision;
      TelemetryHelper.AddToResults(this.ClientTraceData, "WorkItemIdentity", string.Format("{0}:{1}:{2}:{3}", (object) workItemUpdate.FieldData.GetProjectGuid(requestContext), (object) workItemUpdate.FieldData.WorkItemType, (object) workItemUpdate.Id, (object) revision));
      WorkItemTrackingRequestContext trackingRequestContext = requestContext.WitContext();
      Dictionary<string, object> enumerable = new Dictionary<string, object>();
      foreach (KeyValuePair<int, object> fieldUpdate in workItemUpdate.FieldUpdates)
      {
        FieldEntry field;
        if (trackingRequestContext.FieldDictionary.TryGetField(fieldUpdate.Key, out field) && field != null && !string.IsNullOrWhiteSpace(field.ReferenceName) && fieldUpdate.Value != null && this.ShouldScanField(field))
        {
          enumerable.Add(field.ReferenceName, fieldUpdate.Value);
          contentSize += field.ReferenceName.Length;
          contentSize += string.Format("{0}", fieldUpdate.Value).Length;
        }
      }
      TelemetryHelper.AddToResults(this.ClientTraceData, "WorkItemScannableContentLength", string.Format("{0}:{1}", (object) workItemUpdate.Id, (object) contentSize));
      if (!enumerable.IsNullOrEmpty<KeyValuePair<string, object>>())
        this.ClientTraceData.AddRangeToResults("WorkItemScannableFields", (IEnumerable<string>) enumerable.Keys);
      return (object) enumerable;
    }

    protected override ScanResult PerformScan(
      IVssRequestContext requestContext,
      IDictionary<int, WorkItemUpdateState> workItemUpdates,
      IDictionary<int, object> requestContent)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (workItemUpdates == null)
        throw new ArgumentNullException(nameof (workItemUpdates));
      ScanResult scanResult = (ScanResult) null;
      if (requestContent.IsNullOrEmpty<KeyValuePair<int, object>>())
        return scanResult;
      IStreamScannerService service = requestContext.GetService<IStreamScannerService>();
      requestContext.WitContext();
      Guid guid = workItemUpdates.First<KeyValuePair<int, WorkItemUpdateState>>().Value?.FieldData?.GetProjectGuid(requestContext) ?? Guid.Empty;
      string str = string.Join(",", requestContent.Keys.Select<int, string>((Func<int, string>) (k => k.ToString((IFormatProvider) CultureInfo.InvariantCulture))));
      Stopwatch stopwatch1 = Stopwatch.StartNew();
      using (MemoryStream memoryStream = new MemoryStream(JsonFormatter.Serialize((object) requestContent)))
      {
        Stopwatch stopwatch2 = Stopwatch.StartNew();
        this.CancellationToken.ThrowIfCancellationRequested();
        IStreamScannerService streamScannerService = service;
        IVssRequestContext requestContext1 = requestContext;
        MemoryStream inputStream = memoryStream;
        Guid projectId = guid;
        string pipelineId = str;
        CancellationToken cancellationToken1 = this.CancellationToken;
        int? revision = new int?();
        CancellationToken cancellationToken2 = cancellationToken1;
        scanResult = streamScannerService.ScanPipelineDefinitionStream(requestContext1, (Stream) inputStream, projectId, PipelineType.WorkItem, pipelineId, revision, cancellationToken: cancellationToken2);
        this.CancellationToken.ThrowIfCancellationRequested();
        stopwatch2.Stop();
        TelemetryHelper.AddToResults(this.ClientTraceData, "SecretScanDurationInMilliseconds", string.Format("{0}:{1}", (object) str, (object) stopwatch2.ElapsedMilliseconds));
      }
      stopwatch1.Stop();
      TelemetryHelper.AddToResults(this.ClientTraceData, "SecretSerializeAndScanDurationInMilliseconds", string.Format("{0}:{1}", (object) str, (object) stopwatch1.ElapsedMilliseconds));
      return scanResult;
    }

    protected override string ComposeErrorMessge(
      IVssRequestContext requestContext,
      WorkItemUpdateState workItemUpdate,
      IReadOnlyList<string> userFacingLocations)
    {
      WorkItemPipelineHelper itemPipelineHelper = new WorkItemPipelineHelper();
      WorkItemTrackingRequestContext trackingRequestContext = requestContext.WitContext();
      List<string> credentialLocations = new List<string>();
      if (userFacingLocations != null)
      {
        foreach (string userFacingLocation in (IEnumerable<string>) userFacingLocations)
        {
          string nameFromLocation = this.GetReferenceNameFromLocation(userFacingLocation);
          FieldEntry field;
          if (!string.IsNullOrWhiteSpace(nameFromLocation) && trackingRequestContext.FieldDictionary.TryGetField(nameFromLocation, out field))
          {
            credentialLocations.Add(field.Name + " (" + nameFromLocation + ")");
          }
          else
          {
            credentialLocations.Add(userFacingLocation);
            TelemetryHelper.AddToResults(this.ClientTraceData, "InvalidComposeMessageLocations", userFacingLocation);
          }
        }
      }
      return itemPipelineHelper.ComposeErrorMessage((IEnumerable<string>) credentialLocations, false);
    }

    protected override bool IsBlockingEnabled(
      IVssRequestContext requestContext,
      IDictionary<int, WorkItemUpdateState> workItemUpdates)
    {
      return WorkItemTrackingFeatureFlags.IsWorkItemTrackingSecretsBlockingEnabled(requestContext);
    }

    protected override bool IsPrescriptiveBlockingEnabled(IVssRequestContext requestContext) => !WorkItemTrackingFeatureFlags.IsWorkItemTrackingSecretsPrescriptiveBlockingDisabled(requestContext);

    protected override BypassType DetermineBypassType(
      IVssRequestContext requestContext,
      WorkItemUpdateState workItemUpdate)
    {
      return workItemUpdate != null && workItemUpdate.HasFieldUpdate(54) ? this.DetermineBypassTypeFromComment(workItemUpdate.FieldUpdates.First<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (item => item.Key == 54)).Value?.ToString()) : BypassType.None;
    }

    protected override bool IsReportableLocation(
      IVssRequestContext requestContext,
      WorkItemUpdateState workItemUpdate,
      Violation violation,
      out string reportableLocation)
    {
      if (workItemUpdate == null)
        throw new ArgumentNullException(nameof (workItemUpdate));
      reportableLocation = violation != null ? violation.LogicalLocation : throw new ArgumentNullException(nameof (violation));
      return !string.IsNullOrWhiteSpace(reportableLocation);
    }

    private string GetReferenceNameFromLocation(string location)
    {
      if (string.IsNullOrWhiteSpace(location))
        return location;
      int startIndex = location.IndexOf('[');
      if (startIndex > 0)
        location = location.Substring(startIndex);
      location = location.Trim('[', ']', '\'');
      return location;
    }

    private bool ShouldScanField(FieldEntry fieldEntry)
    {
      int num = fieldEntry.FieldType == InternalFieldType.String || fieldEntry.FieldType == InternalFieldType.Html || fieldEntry.FieldType == InternalFieldType.PlainText || fieldEntry.FieldType == InternalFieldType.PicklistString ? 1 : (fieldEntry.FieldType == InternalFieldType.History ? 1 : 0);
      bool flag = fieldEntry.IsIdentity || fieldEntry.FieldId == 2 || fieldEntry.FieldId == 22 || fieldEntry.FieldId == 25 || this.DoNotScanFieldsSet.Contains(fieldEntry.ReferenceName);
      return num != 0 && !flag && !fieldEntry.IsComputed && !fieldEntry.IsIgnored;
    }
  }
}
