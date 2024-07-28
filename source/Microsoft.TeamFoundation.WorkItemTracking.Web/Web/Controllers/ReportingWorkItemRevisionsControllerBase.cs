// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ReportingWorkItemRevisionsControllerBase
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Comments.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  public abstract class ReportingWorkItemRevisionsControllerBase : WorkItemTrackingApiController
  {
    private const char WatermarkSeparator = ';';

    internal Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ReportingWorkItemRevisionsBatch ReadReportingRevisionsImpl(
      string continuationToken,
      DateTime? startDateTime,
      IEnumerable<string> fields,
      IEnumerable<string> types,
      bool? includeIdentityRef,
      bool? includeDeleted,
      bool? includeTagRef,
      bool? includeLatestOnly,
      bool? expandFields,
      bool? includeDiscussionChangesOnly,
      bool fromGet,
      bool fromContinuationToken,
      int? maxPageSize,
      bool? includeDiscussionHistory = null)
    {
      bool fromLegacyDiscussionContinuationToken;
      WorkItemIdRevisionPair workItemIdRevisionPair = this.ParseWorkItemIdRevisionPairFromContinuationToken(continuationToken, fromContinuationToken ? nameof (continuationToken) : "watermark", out fromLegacyDiscussionContinuationToken);
      if (workItemIdRevisionPair.Watermark == 0 && workItemIdRevisionPair.Id == 0 && workItemIdRevisionPair.Revision == 0 && startDateTime.HasValue)
        workItemIdRevisionPair = this.WarehouseService.GetWorkItemWatermarkForDate(this.TfsRequestContext, startDateTime.Value);
      RegistryEntryCollection settings = this.TfsRequestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(this.TfsRequestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/Reporting/*");
      int bigLoopBatchSize;
      int smallLoopBatchSize;
      ReportingWorkItemRevisionsControllerBase.ConfigureLoopBatchSizes(maxPageSize, settings, out bigLoopBatchSize, out smallLoopBatchSize);
      WorkItemIdRevisionPair topReadWorkItemIdRevisionPair = workItemIdRevisionPair;
      CustomerIntelligenceService service = this.TfsRequestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("projectId", (object) this.ProjectId);
      intelligenceData.Add(nameof (startDateTime), startDateTime.HasValue);
      intelligenceData.Add(nameof (fields), string.Join(",", fields ?? Enumerable.Empty<string>()));
      intelligenceData.Add(nameof (types), string.Join(",", types ?? Enumerable.Empty<string>()));
      intelligenceData.Add(nameof (includeIdentityRef), includeIdentityRef.GetValueOrDefault());
      intelligenceData.Add(nameof (includeDeleted), includeDeleted.GetValueOrDefault());
      intelligenceData.Add(nameof (includeTagRef), includeTagRef.GetValueOrDefault());
      intelligenceData.Add(nameof (includeLatestOnly), includeLatestOnly.GetValueOrDefault());
      intelligenceData.Add(nameof (expandFields), expandFields.GetValueOrDefault());
      intelligenceData.Add(nameof (includeDiscussionChangesOnly), includeDiscussionChangesOnly.GetValueOrDefault());
      intelligenceData.Add(nameof (includeDiscussionHistory), includeDiscussionHistory.GetValueOrDefault());
      intelligenceData.Add(nameof (fromGet), fromGet);
      intelligenceData.Add(nameof (fromContinuationToken), fromContinuationToken);
      intelligenceData.Add(nameof (maxPageSize), (object) maxPageSize);
      intelligenceData.Add("bigLoopBatchSize", (double) bigLoopBatchSize);
      intelligenceData.Add("smallLoopBatchSize", (double) smallLoopBatchSize);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(tfsRequestContext, "WIT", "ReportingRevisions", properties);
      IEnumerable<string> validatedFieldsList = this.GetValidatedFieldNames(fields, expandFields.GetValueOrDefault());
      IEnumerable<string> validatedFieldsListWithWatermark = this.GetFieldNamesWithWatermark(validatedFieldsList);
      int fetchedRowCount = 0;
      int fetchedRevisionsCount = 0;
      return (Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ReportingWorkItemRevisionsBatch) new StreamingWorkItemRevisionsBatch(bigLoopBatchSize, (Func<SmallLoopBatch<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>>) (() => this.TfsRequestContext.TraceBlock<SmallLoopBatch<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>>(15521, 15527, 15528, "WIT", "ReportingRevisions", nameof (ReadReportingRevisionsImpl), (Func<SmallLoopBatch<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>>) (() =>
      {
        int batchSize = Math.Min(bigLoopBatchSize - fetchedRowCount, smallLoopBatchSize);
        ICollection<WorkItemIdRevisionPair> itemIdRevisionPairs = !fromContinuationToken ? this.WarehouseService.GetChangedRevisions(this.TfsRequestContext, new Guid?(this.ProjectId), types, topReadWorkItemIdRevisionPair.Watermark, batchSize) : this.WarehouseService.GetChangedRevisionsPageable(this.TfsRequestContext, new Guid?(this.ProjectId), types, includeLatestOnly.GetValueOrDefault(), includeDiscussionChangesOnly.GetValueOrDefault(), fromLegacyDiscussionContinuationToken, topReadWorkItemIdRevisionPair, batchSize, includeDiscussionHistory.GetValueOrDefault());
        IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> workItems;
        bool flag;
        if (!itemIdRevisionPairs.Any<WorkItemIdRevisionPair>())
        {
          workItems = Enumerable.Empty<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>();
          flag = true;
        }
        else
        {
          topReadWorkItemIdRevisionPair = itemIdRevisionPairs.Max<WorkItemIdRevisionPair>();
          bool valueOrDefault = includeDeleted.GetValueOrDefault();
          List<WorkItemFieldData> list = this.WorkItemService.GetWorkItemFieldValues(this.TfsRequestContext, (IEnumerable<WorkItemIdRevisionPair>) itemIdRevisionPairs, validatedFieldsListWithWatermark, batchSize: itemIdRevisionPairs.Count, workItemRetrievalMode: valueOrDefault ? WorkItemRetrievalMode.All : WorkItemRetrievalMode.NonDeleted, suppressCustomerIntelligence: true, disableProjectionLevelThree: !WorkItemTrackingFeatureFlags.IsProjectionLevelThreeForReportingAPIsEnabled(this.TfsRequestContext), useWorkItemIdentity: includeIdentityRef.GetValueOrDefault()).ToList<WorkItemFieldData>();
          fetchedRevisionsCount += list.Count;
          if (includeDiscussionHistory.GetValueOrDefault())
            ReportingWorkItemRevisionsControllerBase.EnrichCommentVersionsWithText(this.TfsRequestContext, this.ProjectId, list);
          workItems = list.OrderBy<WorkItemFieldData, object>((Func<WorkItemFieldData, object>) (revisionData => revisionData.GetFieldValue(this.WitRequestContext, 7))).Select<WorkItemFieldData, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>((Func<WorkItemFieldData, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>) (revisionData => WorkItemFactory.Create(this.WitRequestContext, revisionData, fields: validatedFieldsList, includeURL: false, includeIdentityRef: includeIdentityRef.GetValueOrDefault(), includeIsDeletedField: includeDeleted.GetValueOrDefault(), includeTagRef: includeTagRef.GetValueOrDefault(), returnIdentityRef: includeIdentityRef.GetValueOrDefault(), extendCommentVersionRef: includeDiscussionHistory.GetValueOrDefault())));
          flag = itemIdRevisionPairs.Count < batchSize;
        }
        this.TfsRequestContext.RequestTimer.SetTimeToFirstPageEnd();
        fetchedRowCount += itemIdRevisionPairs.Count;
        return new SmallLoopBatch<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>()
        {
          Values = workItems,
          IsFinalSmallBatch = flag,
          RawCount = itemIdRevisionPairs.Count
        };
      }))), (Func<int, string, string>) ((counter, nextContinuationToken) =>
      {
        TelemetryHelper.PublishGetReportingWorkItemRevisions(this.TfsRequestContext, workItemIdRevisionPair, fields, types, topReadWorkItemIdRevisionPair, counter, fetchedRevisionsCount);
        this.TfsRequestContext.TraceConditionally(this.GetTraceRange(), TraceLevel.Verbose, "Services", "WorkItemService", (Func<string>) (() => "[ReportingApi.WorkItemRevisions.Filters]:[fields:" + string.Join(",", fields ?? Enumerable.Empty<string>()) + ";types:" + string.Join(",", types ?? Enumerable.Empty<string>()) + "]"));
        return WitUrlHelper.GetReportingRevisionsUrl(this.WitRequestContext, this.ProjectId, nextContinuationToken, fromContinuationToken, fromGet ? fields : (IEnumerable<string>) null, fromGet ? types : (IEnumerable<string>) null, fromGet ? includeIdentityRef : new bool?(), fromGet ? includeDeleted : new bool?(), fromGet ? includeTagRef : new bool?(), fromGet ? includeLatestOnly : new bool?(), fromGet ? includeDiscussionChangesOnly : new bool?(), expandFields, fromGet ? maxPageSize : new int?(), this.GetApiResourceVersion());
      }), (Func<string>) (() => this.GetContinuationTokenFromWorkItemIdRevision(topReadWorkItemIdRevisionPair, fromContinuationToken, fromLegacyDiscussionContinuationToken)));
    }

    private static void EnrichCommentVersionsWithText(
      IVssRequestContext requestContext,
      Guid projectId,
      List<WorkItemFieldData> revisions)
    {
      IList<WorkItemFieldData> list1 = (IList<WorkItemFieldData>) revisions.Where<WorkItemFieldData>((Func<WorkItemFieldData, bool>) (fv => fv.CommentVersion != null)).ToList<WorkItemFieldData>();
      if (!list1.Any<WorkItemFieldData>())
        return;
      IList<GetCommentVersion> list2 = (IList<GetCommentVersion>) list1.Select<WorkItemFieldData, GetCommentVersion>((Func<WorkItemFieldData, GetCommentVersion>) (fv => new GetCommentVersion(fv.Id.ToString(), fv.CommentVersion.CommentId, fv.CommentVersion.Version))).ToList<GetCommentVersion>();
      Dictionary<(int, int), Microsoft.TeamFoundation.Comments.Server.CommentVersion> dictionary = ReportingWorkItemRevisionsControllerBase.GetCommentsVersions(requestContext, projectId, list2).ToDictionary<Microsoft.TeamFoundation.Comments.Server.CommentVersion, (int, int), Microsoft.TeamFoundation.Comments.Server.CommentVersion>((Func<Microsoft.TeamFoundation.Comments.Server.CommentVersion, (int, int)>) (c => (c.CommentId, c.Version)), (Func<Microsoft.TeamFoundation.Comments.Server.CommentVersion, Microsoft.TeamFoundation.Comments.Server.CommentVersion>) (c => c));
      foreach (WorkItemFieldData workItemFieldData in (IEnumerable<WorkItemFieldData>) list1)
      {
        int commentId = workItemFieldData.CommentVersion.CommentId;
        int version = workItemFieldData.CommentVersion.Version;
        Microsoft.TeamFoundation.Comments.Server.CommentVersion commentVersion;
        if (dictionary.TryGetValue((commentId, version), out commentVersion))
        {
          workItemFieldData.CommentVersion.Text = commentVersion.Text;
          workItemFieldData.CommentVersion.IsDeleted = commentVersion.IsDeleted;
        }
      }
    }

    private static IList<Microsoft.TeamFoundation.Comments.Server.CommentVersion> GetCommentsVersions(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<GetCommentVersion> getCommentsVersions)
    {
      return requestContext.GetService<ICommentReportingService>().GetCommentsVersions(requestContext, projectId, WorkItemArtifactKinds.WorkItem, getCommentsVersions);
    }

    private static void ConfigureLoopBatchSizes(
      int? maxPageSize,
      RegistryEntryCollection settings,
      out int bigLoopBatchSize,
      out int smallLoopBatchSize)
    {
      bigLoopBatchSize = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Reporting/RevisionsApiBigLoopBatchSize", 1000);
      smallLoopBatchSize = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Reporting/RevisionsApiSmallLoopBatchSize", 200);
      LoopBatchSizeConfigurer.Configure(maxPageSize, ref bigLoopBatchSize, ref smallLoopBatchSize, 1000, 1, 200, 200);
    }

    internal virtual ApiResourceVersion GetApiResourceVersion() => this.Request.GetApiResourceVersion();

    internal abstract int GetTraceRange();

    private IEnumerable<string> GetValidatedFieldNames(
      IEnumerable<string> fields,
      bool expandFields)
    {
      IFieldTypeDictionary fieldDictionary = this.WitRequestContext.FieldDictionary;
      if (((fields == null ? 1 : (!fields.Any<string>() ? 1 : 0)) | (expandFields ? 1 : 0)) != 0)
        return (IEnumerable<string>) fieldDictionary.GetAllFields().Where<FieldEntry>((Func<FieldEntry, bool>) (field => (((field.Usage & InternalFieldUsages.WorkItemTypeExtension) != InternalFieldUsages.None || field.IsLongText ? (field.FieldId == 80 ? 1 : 0) : 1) | (expandFields ? 1 : 0)) != 0)).Select<FieldEntry, string>((Func<FieldEntry, string>) (field => field.ReferenceName)).ToArray<string>();
      foreach (string field in fields)
      {
        if (!fieldDictionary.TryGetField(field, out FieldEntry _))
          throw new VssPropertyValidationException(nameof (fields), Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.InvalidFieldName((object) field));
      }
      return fields;
    }

    private IEnumerable<string> GetFieldNamesWithWatermark(IEnumerable<string> fieldNames) => fieldNames.Contains<string>("System.Watermark", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? fieldNames : (IEnumerable<string>) fieldNames.Concat<string>(Enumerable.Repeat<string>("System.Watermark", 1)).ToArray<string>();

    private WorkItemIdRevisionPair ParseWorkItemIdRevisionPairFromContinuationToken(
      string continuationToken,
      string propertyName,
      out bool legacyWatermarkFormat)
    {
      legacyWatermarkFormat = false;
      WorkItemIdRevisionPair continuationToken1 = new WorkItemIdRevisionPair();
      if (!string.IsNullOrEmpty(continuationToken))
      {
        string[] strArray = continuationToken.Split(new char[1]
        {
          ';'
        }, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length != 1 && strArray.Length != 3 && strArray.Length != 4)
          throw new VssPropertyValidationException(propertyName, ResourceStrings.QueryParameterOutOfRange((object) propertyName));
        int result;
        if (!int.TryParse(strArray[0], out result))
          throw new VssPropertyValidationException(propertyName, ResourceStrings.QueryParameterOutOfRange((object) propertyName));
        continuationToken1.Watermark = result;
        if (strArray.Length >= 3)
        {
          if (!int.TryParse(strArray[1], out result))
            throw new VssPropertyValidationException(propertyName, ResourceStrings.QueryParameterOutOfRange((object) propertyName));
          continuationToken1.Id = result;
          if (!int.TryParse(strArray[2], out result))
            throw new VssPropertyValidationException(propertyName, ResourceStrings.QueryParameterOutOfRange((object) propertyName));
          continuationToken1.Revision = result;
        }
        legacyWatermarkFormat = strArray.Length != 4;
      }
      if (continuationToken1.Watermark < 0 || continuationToken1.Id < 0 || continuationToken1.Revision < 0)
        throw new VssPropertyValidationException(propertyName, ResourceStrings.QueryParameterOutOfRange((object) propertyName));
      return continuationToken1;
    }

    private string GetContinuationTokenFromWorkItemIdRevision(
      WorkItemIdRevisionPair workItemIdRevisionPair,
      bool fromContinuationToken,
      bool fromLegacyWatermarkFormat)
    {
      if (!fromContinuationToken)
        return string.Format("{0}", (object) workItemIdRevisionPair.Watermark);
      if (fromLegacyWatermarkFormat)
        return string.Format("{0};{1};{2}", (object) workItemIdRevisionPair.Watermark, (object) workItemIdRevisionPair.Id, (object) workItemIdRevisionPair.Revision);
      return string.Format("{0};{1};{2};{3}", (object) workItemIdRevisionPair.Watermark, (object) workItemIdRevisionPair.Id, (object) workItemIdRevisionPair.Revision, (object) "Discussion");
    }
  }
}
