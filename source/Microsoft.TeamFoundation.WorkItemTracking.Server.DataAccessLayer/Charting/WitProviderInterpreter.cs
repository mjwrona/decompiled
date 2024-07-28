// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Charting.WitProviderInterpreter
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices;
using Microsoft.TeamFoundation.Reporting.DataServices.FeatureUtilities;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Charting
{
  internal class WitProviderInterpreter : 
    IInterpretTimedData<DatedWorkItemFieldData>,
    IInterpretRecord<DatedWorkItemFieldData>,
    IProvideFilteredData<DatedWorkItemFieldData>,
    IInterpretQueryText
  {
    protected WorkItemTrackingRequestContext m_witRequestContext;
    protected IVssRequestContext m_requestContext;
    protected Dictionary<string, int> m_fieldsMap;
    protected Dictionary<int, bool> m_identityFieldsMap;

    public virtual IEnumerable<DatedWorkItemFieldData> GetFilteredData(
      IVssRequestContext requestContext,
      TransformSecurityInformation transformSecurityInformation,
      IEnumerable<TransformInstructions<DatedWorkItemFieldData>> instructions)
    {
      requestContext.TraceEnter(901612, "WITCharting", "WitQueryProvider", nameof (GetFilteredData));
      try
      {
        IEnumerable<TransformOptions> transformOptionses = instructions.Select<TransformInstructions<DatedWorkItemFieldData>, TransformOptions>((Func<TransformInstructions<DatedWorkItemFieldData>, TransformOptions>) (o => o.Options));
        if (this.m_witRequestContext == null)
        {
          this.m_witRequestContext = requestContext.WitContext();
          this.m_requestContext = requestContext;
        }
        this.m_fieldsMap = WitChartingDalWrapper.GetFieldIdMap(requestContext, transformOptionses);
        this.m_identityFieldsMap = WitChartingDalWrapper.GetIdentityFieldsMap(requestContext, transformOptionses);
        IDictionary filterContextWithUser = this.GetFilterContextWithUser(this.m_requestContext, transformOptionses);
        QueryItem validatedQuery = this.GetValidatedQuery(this.m_requestContext, transformOptionses, filterContextWithUser);
        IEnumerable<DateTime> dateTimes = instructions.SelectMany<TransformInstructions<DatedWorkItemFieldData>, DateTime>((Func<TransformInstructions<DatedWorkItemFieldData>, IEnumerable<DateTime>>) (o => o.HistorySamplePoints)).Distinct<DateTime>();
        TelemetryHelper.PublishFilterProviderRequestDetail(requestContext, FeatureProviderScopes.WorkItemQueries, validatedQuery.Id.ToString(), dateTimes.Count<DateTime>());
        requestContext.Items["queryId"] = (object) validatedQuery.Id;
        requestContext.Items["AsOfHistoryRange"] = (object) transformOptionses.First<TransformOptions>().HistoryRange;
        requestContext.Items["QueryText"] = (object) validatedQuery.QueryText;
        requestContext.Items["ProjectId"] = (object) transformSecurityInformation.ProjectId;
        return WitProviderInterpreter.GetWorkItemFieldValues(this.m_requestContext, transformSecurityInformation.ProjectId, validatedQuery.QueryText, dateTimes, filterContextWithUser, (IEnumerable<string>) this.m_fieldsMap.Keys);
      }
      finally
      {
        requestContext.TraceLeave(901613, "WITCharting", "WitQueryProvider", nameof (GetFilteredData));
      }
    }

    public bool IsValidQueryText(
      IVssRequestContext requestContext,
      string queryText,
      string projectId)
    {
      try
      {
        requestContext.GetService<IWorkItemQueryService>().ValidateWiql(requestContext, queryText, Guid.Parse(projectId));
      }
      catch (Exception ex)
      {
        TelemetryHelper.PublishChartingRequestFailure(requestContext, FeatureProviderScopes.WorkItemQueries, nameof (IsValidQueryText), ex);
        return false;
      }
      return true;
    }

    protected virtual IDictionary GetFilterContextWithUser(
      IVssRequestContext requestContext,
      IEnumerable<TransformOptions> options)
    {
      IDictionary filterContextWithUser = options.First<TransformOptions>().FilterContext ?? (IDictionary) new Dictionary<string, string>();
      filterContextWithUser[(object) "me"] = (object) requestContext.GetUserIdentity().GetLegacyDistinctDisplayName();
      return filterContextWithUser;
    }

    protected static IEnumerable<DatedWorkItemFieldData> GetWorkItemFieldValues(
      IVssRequestContext requestContext,
      Guid? filterUnderProjectId,
      string queryText,
      IEnumerable<DateTime> asOfDates,
      IDictionary queryContext,
      IEnumerable<string> fields)
    {
      requestContext.GetService<ITeamFoundationWorkItemService>();
      bool flag = true;
      IEnumerable<DatedWorkItemFieldData> workItemFieldValues;
      try
      {
        if (asOfDates.Count<DateTime>() == 1)
          flag = false;
        if (!flag)
        {
          DateTime date = asOfDates.First<DateTime>();
          workItemFieldValues = WitProviderInterpreter.GetSnapshotResults(requestContext, filterUnderProjectId, queryText, queryContext, fields, date);
        }
        else
          workItemFieldValues = WitProviderInterpreter.GetTrendDataResults(requestContext, filterUnderProjectId, queryText, queryContext, fields, asOfDates);
      }
      catch (NotSupportedException ex)
      {
        if (ex.Message == Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.AsOfWorkItemQueriesNotSupported() || ex.Message == Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemQueriesOnly())
          throw new NotSupportedException(DalResourceStrings.Get("ChartingFlatQueriesOnly"), (Exception) ex);
        throw;
      }
      catch (WorkItemTrackingTrendQueryResultSizeLimitExceededException ex)
      {
        throw new ChartDataExceedsAllowedLimitsException((Exception) ex);
      }
      return workItemFieldValues;
    }

    protected virtual QueryItem GetValidatedQuery(
      IVssRequestContext requestContext,
      IEnumerable<TransformOptions> options,
      IDictionary queryContext)
    {
      IDataAccessLayer dataAccessLayer = (IDataAccessLayer) new DataAccessLayerImpl(requestContext);
      Guid queryGuid = WitChartingDalWrapper.GetQueryGuid(options.First<TransformOptions>());
      QueryItem query = WitChartingDalWrapper.GetQuery(requestContext, dataAccessLayer, queryGuid, queryContext);
      WitChartingDalWrapper.ValidateQuery(query, queryContext);
      return query;
    }

    public virtual bool SamplePoint(
      DatedWorkItemFieldData record,
      TransformInstructions<DatedWorkItemFieldData> instructions,
      out AggregatedRecord sampledRecord)
    {
      ArgumentUtility.CheckForNull<Dictionary<string, int>>(this.m_fieldsMap, "m_fieldsMap");
      ArgumentUtility.CheckForNull<WorkItemTrackingRequestContext>(this.m_witRequestContext, "m_witRequestContext");
      ArgumentUtility.CheckForNull<TransformInstructions<DatedWorkItemFieldData>>(instructions, nameof (instructions));
      sampledRecord = (AggregatedRecord) null;
      int num = instructions.HistorySamplePoints.Contains<DateTime>(record.Date) ? 1 : 0;
      if (num == 0)
        return num != 0;
      sampledRecord = new AggregatedRecord();
      WorkItemFieldData workItemFieldData = record.Value;
      ChartDimensionality chartDimensionality = instructions.ChartDimensionality;
      TransformOptions options = instructions.Options;
      if (chartDimensionality.IsTrend)
      {
        if (!string.IsNullOrWhiteSpace(options.GroupBy))
        {
          sampledRecord.Group = this.InterpretGroupingValue(record, this.m_fieldsMap[options.GroupBy], instructions.LocalTimeZone);
          sampledRecord.Series = (object) record.Date;
        }
        else
          sampledRecord.Group = (object) record.Date;
      }
      else
      {
        sampledRecord.Group = this.InterpretGroupingValue(record, this.m_fieldsMap[options.GroupBy], instructions.LocalTimeZone);
        if (chartDimensionality.TotalDimensions == 2)
          sampledRecord.Series = this.InterpretGroupingValue(record, this.m_fieldsMap[options.Series], instructions.LocalTimeZone);
      }
      sampledRecord.Measure = instructions.AggregationStrategy.GetValue(record);
      return num != 0;
    }

    public virtual object GetRecordValue(string recordPropertyName, DatedWorkItemFieldData record) => record.Value.GetFieldValue(this.m_witRequestContext, this.m_fieldsMap[recordPropertyName]);

    int IInterpretRecord<DatedWorkItemFieldData>.GetWorkIemId(DatedWorkItemFieldData record) => record.Value.Id;

    public object InterpretGroupingValue(
      DatedWorkItemFieldData record,
      int fieldId,
      TimeZoneInfo timeZone)
    {
      object originalDate = record.Value.GetFieldValue(this.m_witRequestContext, fieldId);
      int num = fieldId == -7 ? 1 : (fieldId == -105 ? 1 : 0);
      bool flag1 = fieldId == 80;
      if (num == 0)
      {
        if (flag1 && originalDate != null)
        {
          char ch = ';';
          return (object) ((IEnumerable<string>) originalDate.ToString().Split(ch)).Select<string, string>((Func<string, string>) (tag => tag.Trim())).ToArray<string>();
        }
        bool flag2;
        if (this.m_identityFieldsMap.TryGetValue(fieldId, out flag2) & flag2)
        {
          if (originalDate is WorkItemIdentity workItemIdentity)
          {
            originalDate = (object) workItemIdentity.IdentityRef?.DisplayName;
          }
          else
          {
            string distinctDisplayName = originalDate as string;
            if (!string.IsNullOrWhiteSpace(distinctDisplayName))
              originalDate = (object) IdentityHelper.GetDisplayNameFromDistinctDisplayName(distinctDisplayName);
          }
        }
        else if (originalDate is DateTime)
          originalDate = (object) ((DateTime) originalDate).FromUtc(timeZone).Date;
      }
      return originalDate;
    }

    private static IEnumerable<DatedWorkItemFieldData> GetSnapshotResults(
      IVssRequestContext requestContext,
      Guid? filterUnderProjectId,
      string queryText,
      IDictionary queryContext,
      IEnumerable<string> fields,
      DateTime date)
    {
      QueryResult queryResult = requestContext.GetService<WorkItemQueryService>().ExecuteQuery(requestContext, queryText, queryContext, filterUnderProjectId, int.MaxValue, WITQueryApplicationIntentOverride.Default, false, QuerySource.Charting);
      IEnumerable<int> workItemIds = queryResult.ResultType == QueryResultType.WorkItem ? queryResult.WorkItemIds : throw new InvalidOperationException(DalResourceStrings.Get("ChartingRequiresWorkItemResponse"));
      return requestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItemFieldValues(requestContext, workItemIds, fields, 0, useWorkItemIdentity: true).Select<WorkItemFieldData, DatedWorkItemFieldData>((Func<WorkItemFieldData, DatedWorkItemFieldData>) (o => new DatedWorkItemFieldData(date, o)));
    }

    internal static IEnumerable<DatedWorkItemFieldData> GetTrendDataResults(
      IVssRequestContext requestContext,
      Guid? filterUnderProjectId,
      string queryText,
      IDictionary queryContext,
      IEnumerable<string> fields,
      IEnumerable<DateTime> asOfDates)
    {
      IDictionary<WorkItemIdRevisionPair, IEnumerable<DateTime>> idRevPairToRelevantTimesDictionary = requestContext.GetService<WorkItemQueryService>().ExecuteQueryAsOfTimes(requestContext, queryText, queryContext, asOfDates, filterUnderProjectId, QuerySource.Charting).QueryResultsByIdRevPair;
      return requestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItemFieldValues(requestContext, (IEnumerable<WorkItemIdRevisionPair>) idRevPairToRelevantTimesDictionary.Keys, fields, 0, workItemRetrievalMode: WorkItemRetrievalMode.All, useWorkItemIdentity: true).SelectMany<WorkItemFieldData, DatedWorkItemFieldData>((Func<WorkItemFieldData, IEnumerable<DatedWorkItemFieldData>>) (fd => idRevPairToRelevantTimesDictionary[new WorkItemIdRevisionPair()
      {
        Id = fd.Id,
        Revision = fd.Revision
      }].Select<DateTime, DatedWorkItemFieldData>((Func<DateTime, DatedWorkItemFieldData>) (date => new DatedWorkItemFieldData(date, fd)))));
    }
  }
}
