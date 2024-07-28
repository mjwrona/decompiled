// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ReportingWorkItemLinksControllerBase
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  public abstract class ReportingWorkItemLinksControllerBase : WorkItemTrackingApiController
  {
    private const char WatermarkSeparator = ';';
    private const int TraceRange = 5917000;

    internal virtual int GetTraceRange() => 5917000;

    public override string TraceArea => "workItemLinks";

    internal virtual ApiResourceVersion GetApiResourceVersion() => this.Request.GetApiResourceVersion();

    internal ReportingWorkItemLinksBatch<T> ReadReportingLinksImpl<T>(
      ReportingWorkItemLinksRequest<T> request)
    {
      if (request.StartDateTime.HasValue && request.StartDateTime.Value.Kind == DateTimeKind.Unspecified)
        DateTime.SpecifyKind(request.StartDateTime.Value, DateTimeKind.Utc);
      return request.FromContinuationToken ? this.ReadReportingLinksImplContinuationToken<T>(request) : this.ReadReportingLinksImplWatermark<T>(request);
    }

    private ReportingWorkItemLinksBatch<T> ReadReportingLinksImplWatermark<T>(
      ReportingWorkItemLinksRequest<T> request)
    {
      long watermark;
      if (!long.TryParse(request.Watermark, out watermark) || watermark < 0L)
        throw new VssPropertyValidationException("watermark", ResourceStrings.QueryParameterOutOfRange((object) "watermark"));
      long topReadWatermark = watermark;
      int actualLinkCount = 0;
      int unfilteredRowCount = 0;
      DateTime? startDateTime = request.StartDateTime;
      if (startDateTime.HasValue)
      {
        if (topReadWatermark == 0L)
        {
          IDataAccessLayer dataAccessLayer = this.DataAccessLayer;
          startDateTime = request.StartDateTime;
          DateTime dateTime = startDateTime.Value;
          topReadWatermark = dataAccessLayer.GetLinkTimeStampForDateTime(dateTime);
        }
        else
          throw new ArgumentConflictException(new string[2]
          {
            "waterMark",
            "startDateTime"
          });
      }
      RegistryEntryCollection registryEntryCollection = this.TfsRequestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(this.TfsRequestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/Reporting/*");
      int bigLoopBatchSize = registryEntryCollection.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Reporting/LinksApiBigLoopBatchSize", 1000);
      int smallLoopBatchSize = registryEntryCollection.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Reporting/LinksApiSmallLoopBatchSize", 200);
      return (ReportingWorkItemLinksBatch<T>) new StreamingWorkItemLinksBatch<T>(bigLoopBatchSize, (Func<SmallLoopBatch<T>>) (() =>
      {
        int batchSize = Math.Min(bigLoopBatchSize - actualLinkCount, smallLoopBatchSize);
        ICollection<T> objs = (ICollection<T>) new List<T>();
        DateTime? createdDateWatermark = new DateTime?();
        DateTime? removedDateWatermark = new DateTime?();
        foreach (WorkItemLinkChange workItemLinkChange in this.DataAccessLayer.GetWorkItemLinkChanges(this.TfsRequestContext, topReadWatermark, batchSize, new Guid?(this.ProjectId), request.Types, (IEnumerable<string>) null, ref createdDateWatermark, ref removedDateWatermark, out topReadWatermark, out unfilteredRowCount))
        {
          bool includeRemoteUrl = ReportingLinkFactory.IsRemoteLinkTypes(this.TfsRequestContext, workItemLinkChange.LinkTypeId);
          if (!includeRemoteUrl || !this.ShouldFilterRemoteLinktype(workItemLinkChange.RemoteStatus.Value))
          {
            objs.Add(request.CreateLink(this.TfsRequestContext, workItemLinkChange, (IDictionary<Guid, IdentityReference>) null, includeRemoteUrl));
            ++actualLinkCount;
          }
        }
        this.TfsRequestContext.RequestTimer.SetTimeToFirstPageEnd();
        return new SmallLoopBatch<T>()
        {
          Values = (IEnumerable<T>) objs,
          IsFinalSmallBatch = unfilteredRowCount < batchSize,
          RawCount = unfilteredRowCount
        };
      }), (Func<int, string, string>) ((counter, nextWatermark) =>
      {
        string types = string.Join(",", request.Types ?? Enumerable.Empty<string>());
        TelemetryHelper.PublishGetReportingWorkItemLinks(this.TfsRequestContext, watermark.ToString(), nextWatermark, actualLinkCount);
        return WitUrlHelper.GetReportingLinksUrl(this.WitRequestContext, this.ProjectId, nextWatermark, types, (string) null, false, this.GetApiResourceVersion());
      }), (Func<string>) (() => topReadWatermark.ToString()));
    }

    private ReportingWorkItemLinksBatch<T> ReadReportingLinksImplContinuationToken<T>(
      ReportingWorkItemLinksRequest<T> request)
    {
      int actualLinkCount = 0;
      int unfilteredRowCount = 0;
      string nextContinuationToken = (string) null;
      IDalWorkItemLinksService workItemLinksService = this.TfsRequestContext.GetService<IDalWorkItemLinksService>();
      RegistryEntryCollection registryEntryCollection = this.TfsRequestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(this.TfsRequestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/Reporting/*");
      int bigLoopBatchSize = registryEntryCollection.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Reporting/LinksApiBigLoopBatchSize", 1000);
      int smallLoopBatchSize = registryEntryCollection.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Reporting/LinksApiSmallLoopBatchSize", 200);
      string continuationToken = request.ContinuationToken;
      return (ReportingWorkItemLinksBatch<T>) new StreamingWorkItemLinksBatch<T>(bigLoopBatchSize, (Func<SmallLoopBatch<T>>) (() => this.TfsRequestContext.TraceBlock<SmallLoopBatch<T>>(5917001, 5917002, 5917003, this.TraceArea, "ReadReportingLinksImplWatermark", "ReadReportingLinksImplWatermark", (Func<SmallLoopBatch<T>>) (() =>
      {
        int batchSize = Math.Min(bigLoopBatchSize - actualLinkCount, smallLoopBatchSize);
        WorkItemLinkChange[] workItemLinkChanges = workItemLinksService.GetWorkItemLinkChanges(this.TfsRequestContext, this.DataAccessLayer, new Guid?(this.ProjectId), request.Types, request.LinkTypes, batchSize, continuationToken == null ? request.StartDateTime : new DateTime?(), continuationToken, out nextContinuationToken, out unfilteredRowCount);
        IDictionary<Guid, IdentityReference> identityMap = !IdentityReferenceBuilder.ShouldUseProperIdentityRef(this.TfsRequestContext) ? (IDictionary<Guid, IdentityReference>) new Dictionary<Guid, IdentityReference>() : IdentityReferenceBuilder.Create(this.TfsRequestContext, (IEnumerable<Guid>) ((IEnumerable<WorkItemLinkChange>) workItemLinkChanges).Select<WorkItemLinkChange, Guid>((Func<WorkItemLinkChange, Guid>) (x => x.ChangedBy_TfId)).ToList<Guid>());
        List<T> objList = new List<T>();
        foreach (WorkItemLinkChange linkChange in workItemLinkChanges)
        {
          bool includeRemoteUrl = ReportingLinkFactory.IsRemoteLinkTypes(this.TfsRequestContext, linkChange.LinkTypeId);
          if (!includeRemoteUrl || !linkChange.RemoteStatus.HasValue || !this.ShouldFilterRemoteLinktype(linkChange.RemoteStatus.Value))
          {
            linkChange.LinkTypeString = linkChange.LinkTypeId == 0 ? linkChange.LinkType : WorkItemRelationFactory.GetWorkItemLinkType(linkChange.LinkTypeId, this.WitRequestContext);
            objList.Add(request.CreateLink(this.TfsRequestContext, linkChange, identityMap, includeRemoteUrl));
            ++actualLinkCount;
          }
        }
        continuationToken = nextContinuationToken;
        this.TfsRequestContext.RequestTimer.SetTimeToFirstPageEnd();
        return new SmallLoopBatch<T>()
        {
          Values = (IEnumerable<T>) objList,
          IsFinalSmallBatch = unfilteredRowCount < batchSize,
          RawCount = unfilteredRowCount
        };
      }))), (Func<int, string, string>) ((counter, _) =>
      {
        string types = string.Join(",", request.Types ?? Enumerable.Empty<string>());
        string linkTypes = string.Join(",", request.LinkTypes ?? Enumerable.Empty<string>());
        TelemetryHelper.PublishGetReportingWorkItemLinks(this.TfsRequestContext, continuationToken, nextContinuationToken, actualLinkCount);
        return WitUrlHelper.GetReportingLinksUrl(this.WitRequestContext, this.ProjectId, nextContinuationToken, types, linkTypes, true, this.GetApiResourceVersion());
      }), (Func<string>) (() => nextContinuationToken));
    }

    private bool ShouldFilterRemoteLinktype(Microsoft.TeamFoundation.WorkItemTracking.Common.RemoteStatus remoteStatus) => remoteStatus != Microsoft.TeamFoundation.WorkItemTracking.Common.RemoteStatus.Success && remoteStatus != Microsoft.TeamFoundation.WorkItemTracking.Common.RemoteStatus.PendingUpdate;
  }
}
