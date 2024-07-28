// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Common.TelemetryHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Batch;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http.Controllers;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Common
{
  public static class TelemetryHelper
  {
    private const string AreaName = "WorkItem Tracking";
    private const string FeatureName = "RestApi";
    private const string RequestHeaderPropertyNameTemplate = "RequestHeader_{0}";

    public static void PublishGetWorkItemOptions(
      IVssRequestContext requestContext,
      WorkItemExpand expandOption)
    {
      CustomerIntelligenceData ciData = new CustomerIntelligenceData();
      ciData.Add("GetWorkItemExpand", expandOption.ToString());
      TelemetryHelper.Publish(requestContext, ciData);
    }

    public static void PublishGetWorkItemsOptions(
      IVssRequestContext requestContext,
      WorkItemExpand expandOption)
    {
      CustomerIntelligenceData ciData = new CustomerIntelligenceData();
      ciData.Add("GetWorkItemsExpand", expandOption.ToString());
      TelemetryHelper.Publish(requestContext, ciData);
    }

    public static void PublishGetWorkItemTemplateOptions(
      IVssRequestContext requestContext,
      WorkItemExpand expandOption)
    {
      CustomerIntelligenceData ciData = new CustomerIntelligenceData();
      ciData.Add("GetWorkItemTemplateExpand", expandOption.ToString());
      TelemetryHelper.Publish(requestContext, ciData);
    }

    public static void PublishGetWorkItemRevisionOptions(
      IVssRequestContext requestContext,
      WorkItemExpand expandOption)
    {
      CustomerIntelligenceData ciData = new CustomerIntelligenceData();
      ciData.Add("GetWorkItemRevisionExpand", expandOption.ToString());
      TelemetryHelper.Publish(requestContext, ciData);
    }

    internal static void PublishGetReportingWorkItemLinks(
      IVssRequestContext tfsRequestContext,
      string continuationToken,
      string nextContinuationToken,
      int count)
    {
      CustomerIntelligenceData ciData = new CustomerIntelligenceData();
      ciData.Add("GetReportingWorkItemLinksBaseWatermark", continuationToken);
      ciData.Add("GetReportingWorkItemLinksTopWatermark", nextContinuationToken);
      ciData.Add("GetReportingWorkItemLinksRowCount", (double) count);
      TelemetryHelper.Publish(tfsRequestContext, ciData);
    }

    public static void PublishGetWorkItemRevisionsOptions(
      IVssRequestContext requestContext,
      WorkItemExpand expandOption)
    {
      CustomerIntelligenceData ciData = new CustomerIntelligenceData();
      ciData.Add("GetWorkItemRevisionsExpand", expandOption.ToString());
      TelemetryHelper.Publish(requestContext, ciData);
    }

    public static void PublishGetClassificationNodesOptions(
      IVssRequestContext requestContext,
      int depth)
    {
      CustomerIntelligenceData ciData = new CustomerIntelligenceData();
      ciData.Add("GetClassificationNodesDepth", (double) depth);
      TelemetryHelper.Publish(requestContext, ciData);
    }

    public static void PublishGetQueryOptions(
      IVssRequestContext requestContext,
      QueryExpand expandOption,
      int depth,
      bool includeDeleted)
    {
      CustomerIntelligenceData ciData = new CustomerIntelligenceData();
      ciData.Add("GetQueryExpand", expandOption.ToString());
      ciData.Add("GetQueryDepth", (double) depth);
      ciData.Add("GetQueryIncludeDeleted", includeDeleted);
      TelemetryHelper.Publish(requestContext, ciData);
    }

    public static void PublishGetQueriesOptions(
      IVssRequestContext requestContext,
      QueryExpand expandOption,
      int depth,
      bool includeDeleted)
    {
      CustomerIntelligenceData ciData = new CustomerIntelligenceData();
      ciData.Add("GetQueriesExpand", expandOption.ToString());
      ciData.Add("GetQueriesDepth", (double) depth);
      ciData.Add("GetQueriesIncludeDeleted", includeDeleted);
      TelemetryHelper.Publish(requestContext, ciData);
    }

    internal static void PublishGetReportingWorkItemRevisions(
      IVssRequestContext tfsRequestContext,
      WorkItemIdRevisionPair watermark,
      IEnumerable<string> fields,
      IEnumerable<string> types,
      WorkItemIdRevisionPair newWatermark,
      int idRevPairCount,
      int revisionCount)
    {
      CustomerIntelligenceData ciData = new CustomerIntelligenceData();
      ciData.Add("GetReportingWorkItemRevisionsBaseWatermark", (object) watermark);
      ciData.Add("GetReportingWorkItemRevisionsFieldsCount", (double) (fields ?? Enumerable.Empty<string>()).Count<string>());
      ciData.Add("GetReportingWorkItemRevisionsTypesCount", (double) (types ?? Enumerable.Empty<string>()).Count<string>());
      ciData.Add("GetReportingWorkItemRevisionsTopWatermark", (object) newWatermark);
      ciData.Add("GetReportingWorkItemRevisionsIdRevPairCount", (double) idRevPairCount);
      ciData.Add("GetReportingWorkItemRevisionsRevisionCount", (double) revisionCount);
      TelemetryHelper.Publish(tfsRequestContext, ciData);
    }

    public static void PublishUndeleteQuery(
      IVssRequestContext requestContext,
      bool undeleteDescendants)
    {
      CustomerIntelligenceData ciData = new CustomerIntelligenceData();
      ciData.Add("UndeletQueryWithDescendants", undeleteDescendants);
      TelemetryHelper.Publish(requestContext, ciData);
    }

    public static void PublishBatchHandlerResults(
      IVssRequestContext requestContext,
      IList<BatchHttpRequestMessage> requests)
    {
      TelemetryHelper.TryPublish(requestContext, (Func<CustomerIntelligenceData>) (() =>
      {
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        intelligenceData.Add("BatchRequestCount", (double) requests.Count);
        foreach (IGrouping<string, BatchHttpRequestMessage> source in requests.Where<BatchHttpRequestMessage>((Func<BatchHttpRequestMessage, bool>) (r => !string.IsNullOrEmpty(r.Handler))).GroupBy<BatchHttpRequestMessage, string>((Func<BatchHttpRequestMessage, string>) (r => r.Handler)))
          intelligenceData.Add(source.Key, (double) source.Count<BatchHttpRequestMessage>());
        return intelligenceData;
      }));
    }

    public static void PublishRuleChangeCIEvent(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      bool updateExisting,
      FieldRuleModel fieldRuleModel)
    {
      TelemetryHelper.TryPublish(requestContext, (Func<CustomerIntelligenceData>) (() =>
      {
        CustomerIntelligenceData intelligenceData1 = new CustomerIntelligenceData();
        intelligenceData1.Add("ProcessId", processId.ToString());
        intelligenceData1.Add("WitRefName", witRefName);
        CustomerIntelligenceData intelligenceData2 = intelligenceData1;
        Guid? id = fieldRuleModel.Id;
        ref Guid? local = ref id;
        string str = local.HasValue ? local.GetValueOrDefault().ToString() : (string) null;
        intelligenceData2.Add("RuleId", str);
        intelligenceData1.Add("WitName", ((IEnumerable<string>) witRefName.Split('.')).Last<string>());
        intelligenceData1.Add(nameof (updateExisting), updateExisting);
        intelligenceData1.Add("FriendlyName", fieldRuleModel.FriendlyName);
        intelligenceData1.Add("Disabled", fieldRuleModel.IsDisabled);
        HashSet<string> hashSet1 = fieldRuleModel.Conditions.Select<RuleConditionModel, string>((Func<RuleConditionModel, string>) (c => c.ConditionType + ":" + c.Field)).ToHashSet<string>();
        if (hashSet1 != null && hashSet1.Any<string>())
          intelligenceData1.Add("Conditions", string.Join(",", (IEnumerable<string>) hashSet1));
        HashSet<string> hashSet2 = fieldRuleModel.Actions.Select<RuleActionModel, string>((Func<RuleActionModel, string>) (a => a.ActionType + ":" + a.TargetField + (a.ForVsId != Guid.Empty || a.NotVsId != Guid.Empty ? ":Vsid" : ""))).ToHashSet<string>();
        if (hashSet2 != null && hashSet2.Any<string>())
          intelligenceData1.Add("Actions", string.Join(",", (IEnumerable<string>) hashSet2));
        return intelligenceData1;
      }));
    }

    public static void PublishHttpRequestAttributes(HttpActionContext actionContext)
    {
      IVssRequestContext tfsRequestContext = (actionContext.ControllerContext.Controller as TfsApiController).TfsRequestContext;
      CustomerIntelligenceData ciData = new CustomerIntelligenceData();
      ciData.Add("ActivityId", tfsRequestContext.ActivityId.ToString());
      TelemetryHelper.FillHeadersData((HttpHeaders) actionContext.Request.Headers, ciData, "Origin");
      TelemetryHelper.FillHeadersData((HttpHeaders) actionContext.Request.Content.Headers, ciData, "Content-Type");
      TelemetryHelper.Publish(tfsRequestContext, ciData);
    }

    private static void Publish(IVssRequestContext requestContext, CustomerIntelligenceData ciData)
    {
      try
      {
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "WorkItem Tracking", "RestApi", ciData);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "wit", "RestApiTelemetry", ex);
      }
    }

    private static void TryPublish(
      IVssRequestContext requestContext,
      Func<CustomerIntelligenceData> getCiData)
    {
      try
      {
        CustomerIntelligenceData properties = getCiData();
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "WorkItem Tracking", "RestApi", properties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "wit", "RestApiTelemetry", ex);
      }
    }

    private static void FillHeadersData(
      HttpHeaders headers,
      CustomerIntelligenceData ciData,
      string headerName)
    {
      IEnumerable<string> values = (IEnumerable<string>) null;
      if (!headers.TryGetValues(headerName, out values) || values == null || values.Count<string>() <= 0)
        return;
      ciData.Add(string.Format("RequestHeader_{0}", (object) headerName), string.Join(",", values));
    }
  }
}
