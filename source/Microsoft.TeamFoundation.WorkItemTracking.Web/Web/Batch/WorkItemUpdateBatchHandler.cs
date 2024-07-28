// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Batch.WorkItemUpdateBatchHandler
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.Azure.Boards.WebApi.Common.Helpers;
using Microsoft.Azure.Devops.Work.PlatformServices.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Batch
{
  public class WorkItemUpdateBatchHandler : BatchHandler
  {
    private static readonly HttpMethod Patch = new HttpMethod("PATCH");
    private static readonly IEnumerable<MediaTypeFormatter> MediaTypeFormatters = (IEnumerable<MediaTypeFormatter>) Enumerable.Repeat<VssJsonPatchMediaTypeFormatter>(new VssJsonPatchMediaTypeFormatter(), 1);
    protected static readonly string[] SupportedParam = new string[3]
    {
      "api-version",
      "bypassrules",
      "suppressnotifications"
    };
    protected static readonly string updateRouteTemplate = "_apis/{area}/{resource}/{id}";
    protected static readonly string createRouteTemplate = "{project}/_apis/{area}/{resource}/${type}";

    public WorkItemUpdateBatchHandler(HttpServer server)
      : base(server)
    {
    }

    public override IEnumerable<HttpResponseMessage> ProcessBatch(
      IVssRequestContext requestContext,
      IEnumerable<BatchHttpRequestMessage> requests)
    {
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate> workItemUpdateList = new List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate>();
      List<BatchHttpRequestMessage> handledRequests = new List<BatchHttpRequestMessage>();
      Dictionary<int, WorkItemUpdateBatchHandler.WorkItemUpdateType> dictionary = new Dictionary<int, WorkItemUpdateBatchHandler.WorkItemUpdateType>();
      List<HttpResponseMessage> responses = new List<HttpResponseMessage>();
      WorkItemController controller = new WorkItemController();
      bool flag1 = false;
      BatchRequestParameters batchRequestParameters;
      if (!this.IsBatchRequestValid(requestContext, requests, controller, responses, out batchRequestParameters))
        return (IEnumerable<HttpResponseMessage>) responses;
      bool flag2 = true;
      List<int> idList = new List<int>();
      List<BatchHttpRequestMessage> httpRequestMessageList = new List<BatchHttpRequestMessage>();
      foreach (BatchHttpRequestMessage request in requests)
      {
        try
        {
          IHttpRouteData routeData = request.GetRouteData();
          if (this.ValidateRequest(request, routeData))
          {
            if (!string.Equals(routeData.Route.RouteTemplate, WorkItemUpdateBatchHandler.updateRouteTemplate, StringComparison.OrdinalIgnoreCase))
            {
              flag2 = false;
              break;
            }
            int result;
            if (int.TryParse((string) routeData.Values["id"], out result))
            {
              idList.Add(result);
              httpRequestMessageList.Add(request);
            }
          }
        }
        catch (Exception ex1)
        {
          flag1 = true;
          request.Handled = true;
          Exception ex2 = WitUpdateHelper.TranslateUpdateResultException(ex1);
          responses.Add(request.CreateErrorResponse(controller.MapException(ex2), controller.TranslateException(ex2).Message));
        }
      }
      if (flag2)
      {
        IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem> source = WitUpdateHelper.PrepareUpdateWorkItems(requestContext, (IEnumerable<int>) idList, batchRequestParameters.UseIdentityRef);
        int i = 0;
        foreach (BatchHttpRequestMessage request in httpRequestMessageList)
        {
          try
          {
            dictionary[request.RequestId] = WorkItemUpdateBatchHandler.WorkItemUpdateType.Update;
            PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> patchDocument = this.ReadPatchDocument((HttpRequestMessage) request);
            if (patchDocument == null)
              throw new VssPropertyValidationException("document", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingPatchDocument());
            Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem serverWorkItem = source.ElementAtOrDefault<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem>(i) ?? source.FirstOrDefault<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem, bool>) (it => it.Id == idList.ElementAt<int>(i)));
            Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate updateRequest = this.CreateUpdateRequest(requestContext, workItemUpdateList, batchRequestParameters.UseLegacyIndexHandling, batchRequestParameters.UseIdentityRef, request, patchDocument, serverWorkItem);
            workItemUpdateList.Add(updateRequest);
            handledRequests.Add(request);
          }
          catch (Exception ex3)
          {
            Exception ex4 = WitUpdateHelper.TranslateUpdateResultException(ex3);
            responses.Add(request.CreateErrorResponse(controller.MapException(ex4), controller.TranslateException(ex4).Message));
            flag1 = true;
            request.Handled = true;
          }
          i++;
        }
      }
      else
      {
        foreach (BatchHttpRequestMessage request in requests)
        {
          IHttpRouteData routeData = request.GetRouteData();
          if (this.ValidateRequest(request, routeData))
          {
            try
            {
              PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> patchDocument;
              Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem serverWorkItem;
              if (string.Equals(routeData.Route.RouteTemplate, WorkItemUpdateBatchHandler.createRouteTemplate, StringComparison.OrdinalIgnoreCase))
              {
                object obj1;
                if (routeData.Values.TryGetValue("project", out obj1))
                {
                  if (obj1 is string)
                  {
                    object obj2;
                    if (routeData.Values.TryGetValue("type", out obj2))
                    {
                      if (obj2 is string)
                      {
                        dictionary[request.RequestId] = WorkItemUpdateBatchHandler.WorkItemUpdateType.Create;
                        patchDocument = this.ReadPatchDocument((HttpRequestMessage) request);
                        serverWorkItem = WitUpdateHelper.PrepareUpdateWorkItem(requestContext, HttpUtility.UrlDecode((string) obj1), HttpUtility.UrlDecode((string) obj2), patchDocument, batchRequestParameters.UseIdentityRef);
                      }
                      else
                        continue;
                    }
                    else
                      continue;
                  }
                  else
                    continue;
                }
                else
                  continue;
              }
              else if (string.Equals(routeData.Route.RouteTemplate, WorkItemUpdateBatchHandler.updateRouteTemplate, StringComparison.OrdinalIgnoreCase))
              {
                int result;
                if (int.TryParse((string) routeData.Values["id"], out result))
                {
                  dictionary[request.RequestId] = WorkItemUpdateBatchHandler.WorkItemUpdateType.Update;
                  patchDocument = this.ReadPatchDocument((HttpRequestMessage) request);
                  serverWorkItem = WitUpdateHelper.PrepareUpdateWorkItem(requestContext, result, patchDocument, batchRequestParameters.UseIdentityRef);
                }
                else
                  continue;
              }
              else
                continue;
              Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate updateRequest = this.CreateUpdateRequest(requestContext, workItemUpdateList, batchRequestParameters.UseLegacyIndexHandling, batchRequestParameters.UseIdentityRef, request, patchDocument, serverWorkItem);
              workItemUpdateList.Add(updateRequest);
              handledRequests.Add(request);
            }
            catch (Exception ex)
            {
              responses.Add(request.CreateErrorResponse(controller.MapException(ex), controller.TranslateException(ex).Message));
              flag1 = true;
              request.Handled = true;
            }
          }
        }
      }
      if (!flag1 && workItemUpdateList.Any<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate>())
        this.CommitChanges(requestContext, workItemUpdateList, handledRequests, responses, controller, batchRequestParameters);
      return (IEnumerable<HttpResponseMessage>) responses;
    }

    protected bool IsBatchRequestValid(
      IVssRequestContext requestContext,
      IEnumerable<BatchHttpRequestMessage> requests,
      WorkItemController controller,
      List<HttpResponseMessage> responses,
      out BatchRequestParameters batchRequestParameters)
    {
      BatchRequestParameters batchRequestParameters1;
      int num = this.ValidateConsistentBatchParameters(requestContext, requests, out batchRequestParameters1) ? 1 : 0;
      batchRequestParameters = batchRequestParameters1;
      if (num != 0)
        return true;
      ArgumentException ex = new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.InconsistentParametersForBatchRequest(batchRequestParameters1.HasBypassRules ? (object) WorkItemUpdateBatchHandler.SupportedParam[1] : (object) WorkItemUpdateBatchHandler.SupportedParam[2]));
      foreach (BatchHttpRequestMessage request in requests)
      {
        request.Handled = true;
        responses.Add(request.CreateErrorResponse(controller.MapException((Exception) ex), controller.TranslateException((Exception) ex).Message));
      }
      return false;
    }

    protected void CommitChanges(
      IVssRequestContext requestContext,
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate> workItemUpdates,
      List<BatchHttpRequestMessage> handledRequests,
      List<HttpResponseMessage> responses,
      WorkItemController controller,
      BatchRequestParameters args)
    {
      try
      {
        List<WorkItemUpdateResult> updateResults = this.GetUpdateResults(requestContext, workItemUpdates, args.HasBypassRules, args.HasSuppressNotifications, args.UseIdentityRef);
        List<(int, HttpResponseMessage)> source = new List<(int, HttpResponseMessage)>(updateResults.Count);
        for (int index = 0; index < updateResults.Count; ++index)
        {
          WorkItemUpdateResult itemUpdateResult = updateResults[index];
          BatchHttpRequestMessage handledRequest = handledRequests[index];
          if (itemUpdateResult.Exception != null)
          {
            Exception ex = WitUpdateHelper.TranslateUpdateResultException((Exception) itemUpdateResult.Exception, true);
            if (ex is RuleValidationException)
              source.Add((itemUpdateResult.Id, handledRequest.CreateResponse<Exception>(controller.MapException(ex), ex)));
            else
              source.Add((itemUpdateResult.Id, handledRequest.CreateErrorResponse(controller.MapException(ex), controller.TranslateException(ex).Message)));
          }
          else
            source.Add((itemUpdateResult.Id, (HttpResponseMessage) null));
        }
        IEnumerable<int> ids = source.Where<(int, HttpResponseMessage)>((Func<(int, HttpResponseMessage), bool>) (wir => wir.Response == null)).Select<(int, HttpResponseMessage), int>((Func<(int, HttpResponseMessage), int>) (wir => wir.WorkItemId));
        Dictionary<int?, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> dictionary = WitUpdateHelper.GetWorkItems(requestContext, ids, false, args.UseIdentityRef, args.ReturnProjectScopedUrl).ToDictionary<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem, int?>((Func<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem, int?>) (wi => wi.Id));
        for (int index = 0; index < updateResults.Count; ++index)
        {
          int num = source[index].Item1;
          if (source[index].Item2 == null)
            source[index] = (num, handledRequests[index].CreateResponse<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>(dictionary[new int?(num)]));
          responses.Add(source[index].Item2);
        }
      }
      catch (Exception ex1)
      {
        Exception ex2 = WitUpdateHelper.TranslateUpdateResultException(ex1);
        responses.Add(handledRequests[0].CreateErrorResponse(controller.MapException(ex2), controller.TranslateException(ex2).Message));
      }
    }

    protected Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate CreateUpdateRequest(
      IVssRequestContext requestContext,
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate> workItemUpdates,
      bool useLegacyIndexHandling,
      bool useIdentityRef,
      BatchHttpRequestMessage request,
      PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> patchDocument,
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem serverWorkItem)
    {
      request.Handled = true;
      request.Handler = nameof (WorkItemUpdateBatchHandler);
      WorkItemPatchDocument itemPatchDocument = new WorkItemPatchDocument(requestContext.WitContext(), useLegacyIndexHandling, (IPatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>) patchDocument, serverWorkItem, useIdentityRef);
      itemPatchDocument.Evaluate();
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate workItemUpdate = itemPatchDocument.GetWorkItemUpdate(requestContext.WitContext(), useIdentityRef);
      if (workItemUpdates.Any<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate, bool>) (u => u.Id == workItemUpdate.Id)))
        throw new PatchOperationFailedException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.WorkItemPatchDocument_DuplicateWorkItemId((object) workItemUpdate.Id));
      return workItemUpdate;
    }

    protected PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> ReadPatchDocument(
      HttpRequestMessage request)
    {
      try
      {
        return request.Content.ReadAsAsync<PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>>(WorkItemUpdateBatchHandler.MediaTypeFormatters).Result;
      }
      catch (AggregateException ex) when (ex.InnerException is AggregateException && ex.InnerException.InnerException is JsonSerializationException)
      {
        throw new InvalidBatchWorkItemUpdateJsonException(ex.InnerException.InnerException as JsonSerializationException);
      }
      catch (AggregateException ex) when (ex.InnerException != null)
      {
        throw ex.InnerException;
      }
    }

    protected bool ValidateConsistentBatchParameters(
      IVssRequestContext requestContext,
      IEnumerable<BatchHttpRequestMessage> requests,
      out BatchRequestParameters batchRequestParameters)
    {
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int num4 = 0;
      batchRequestParameters = new BatchRequestParameters()
      {
        UseLegacyIndexHandling = true
      };
      foreach (BatchHttpRequestMessage request in requests)
      {
        IHttpController controllerFromRequest = this.GetControllerFromRequest((HttpRequestMessage) request);
        if (this.ShouldUseIdentityRefForWorkItemFieldValues(requestContext, controllerFromRequest))
        {
          batchRequestParameters.UseIdentityRef = true;
          ++num4;
        }
        if (this.ShouldReturnProjectScopedUrl(requestContext, controllerFromRequest))
          batchRequestParameters.ReturnProjectScopedUrl = true;
        NameValueCollection queryString = request.RequestUri.ParseQueryString();
        foreach (string str in (NameObjectCollectionBase) queryString)
        {
          if (StringComparer.OrdinalIgnoreCase.Equals(str, WorkItemUpdateBatchHandler.SupportedParam[0]) && request.GetApiVersion() > WorkItemPatchDocument.MaximumSupportedLegacyIndexHandlingVersion)
          {
            batchRequestParameters.UseLegacyIndexHandling = false;
            ++num3;
          }
          bool result;
          bool.TryParse(queryString[str], out result);
          if (StringComparer.OrdinalIgnoreCase.Equals(str, WorkItemUpdateBatchHandler.SupportedParam[1]) & result)
          {
            batchRequestParameters.HasBypassRules = true;
            ++num1;
          }
          if (StringComparer.OrdinalIgnoreCase.Equals(str, WorkItemUpdateBatchHandler.SupportedParam[2]) & result)
          {
            batchRequestParameters.HasSuppressNotifications = true;
            ++num2;
          }
        }
      }
      if (num3 != 0 && num3 != requests.Count<BatchHttpRequestMessage>() || num1 != 0 && num1 != requests.Count<BatchHttpRequestMessage>())
        return false;
      if (num2 == 0)
        return true;
      if (num2 != requests.Count<BatchHttpRequestMessage>())
        return false;
      return num4 == 0 || num4 == requests.Count<BatchHttpRequestMessage>();
    }

    private bool ValidateQueryParams(BatchHttpRequestMessage request)
    {
      NameValueCollection queryString = request.RequestUri.ParseQueryString();
      if (queryString.Count > WorkItemUpdateBatchHandler.SupportedParam.Length)
        return false;
      foreach (string str in (NameObjectCollectionBase) queryString)
      {
        if (!((IEnumerable<string>) WorkItemUpdateBatchHandler.SupportedParam).Contains<string>(str.ToLowerInvariant()))
          return false;
      }
      return true;
    }

    protected List<WorkItemUpdateResult> GetUpdateResults(
      IVssRequestContext requestContext,
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate> workItemUpdates,
      bool hasByPassRule,
      bool hasSuppressNotifications,
      bool useIdentityRef)
    {
      ITeamFoundationWorkItemService service = requestContext.GetService<ITeamFoundationWorkItemService>();
      WorkItemUpdateRuleExecutionMode ruleExecutionMode = hasByPassRule ? WorkItemUpdateRuleExecutionMode.Bypass : WorkItemUpdateRuleExecutionMode.Full;
      IVssRequestContext requestContext1 = requestContext;
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate> workItemUpdateList = workItemUpdates;
      int num1 = (int) ruleExecutionMode;
      bool flag = hasSuppressNotifications;
      int num2 = !hasSuppressNotifications ? 1 : 0;
      int num3 = flag ? 1 : 0;
      int num4 = hasByPassRule ? 1 : 0;
      int num5 = useIdentityRef ? 1 : 0;
      return service.UpdateWorkItems(requestContext1, (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate>) workItemUpdateList, (WorkItemUpdateRuleExecutionMode) num1, includeInRecentActivity: num2 != 0, suppressNotifications: num3 != 0, isPermissionCheckRequiredForBypassRules: num4 != 0, useWorkItemIdentity: num5 != 0).ToList<WorkItemUpdateResult>();
    }

    protected bool ValidateRequest(BatchHttpRequestMessage request, IHttpRouteData route)
    {
      object b1;
      object b2;
      return !(request.Method != WorkItemUpdateBatchHandler.Patch) && route.Values.TryGetValue("area", out b1) && string.Equals("wit", b1 as string, StringComparison.InvariantCultureIgnoreCase) && route.Values.TryGetValue("resource", out b2) && string.Equals("workItems", b2 as string, StringComparison.InvariantCultureIgnoreCase) && this.ValidateQueryParams(request);
    }

    protected enum WorkItemUpdateType
    {
      Create,
      Update,
    }
  }
}
