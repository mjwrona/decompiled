// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.ApiWITController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 74AD14A4-225D-46D2-B154-945941A2D167
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Mail;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.Server.WebAccess.Utils.Performance;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Utilities;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking
{
  [ValidateInput(false)]
  [SupportedRouteArea("Api", NavigationContextLevels.Collection | NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  [SamplePerformanceData]
  public class ApiWITController : WorkItemsAreaController
  {
    internal const string WiovCategory = "Microsoft.LimitedModeCategory";
    private const int PayloadWorkItemIdOffset = 0;

    protected WebAccessWorkItemService WitService => this.TfsRequestContext.GetService<WebAccessWorkItemService>();

    protected WorkItemTrackingFieldService WitFieldService => this.TfsRequestContext.GetService<WorkItemTrackingFieldService>();

    protected ILocationService LocationService => this.TfsRequestContext.GetService<ILocationService>();

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(516000, 516010)]
    public ActionResult ConstantSets(IEnumerable<ConstantSetReference> ids)
    {
      Dictionary<string, string[]> result = WITDataSource.GetConstantSets(this.TfsRequestContext, ids);
      return this.ETag((Func<string>) (() => EtagHelper.GetConstantSetsETag(this.TfsRequestContext)), (Func<ActionResult>) (() =>
      {
        return (ActionResult) new SecureJsonResult()
        {
          Data = (object) result,
          JsonRequestBehavior = JsonRequestBehavior.AllowGet
        };
      }), new TimeSpan?(this.GetMaxAgeTimeSpan()));
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(516020, 516030)]
    public ActionResult LinkTypes() => this.ETag((Func<string>) (() => EtagHelper.GetLinkTypesETag(this.TfsRequestContext)), (Func<ActionResult>) (() => (ActionResult) this.Json((object) WITDataSource.LinkTypes(this.TfsRequestContext), JsonRequestBehavior.AllowGet)), new TimeSpan?(this.GetMaxAgeTimeSpan()));

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(516040, 516050)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    public ActionResult Nodes() => this.ETag((Func<string>) (() => EtagHelper.GetNodesETag(this.TfsRequestContext)), (Func<ActionResult>) (() =>
    {
      return (ActionResult) new DataContractJsonResult((object) this.WitService.GetNode(this.TfsRequestContext, this.WitService.GetProjectNodeId(this.TfsRequestContext, this.TfsWebContext.Project.Name)))
      {
        JsonRequestBehavior = JsonRequestBehavior.AllowGet
      };
    }), new TimeSpan?(this.GetMaxAgeTimeSpan()));

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(516060, 516070)]
    public ActionResult TeamProjects(
      IEnumerable<string> namesOrIds,
      bool includeFieldDefinitions = false,
      bool includeProcessId = true,
      bool includeProcessInfo = true,
      bool includeWorkItemTypes = true)
    {
      IEnumerable<Project> projects;
      JsObject teamObjects = WITDataSource.TeamProjects(this.TfsRequestContext, namesOrIds, out projects, includeFieldDefinitions, includeProcessId, includeProcessInfo, includeWorkItemTypes);
      return this.ETag((Func<string>) (() =>
      {
        using (WebPerformanceTimerHelpers.StartMeasure((WebContext) this.TfsWebContext, "ApiWITController.TeamProjects.GetTeamProjectsETag"))
          return EtagHelper.GetTeamProjectsETag(projects, this.TfsRequestContext);
      }), (Func<ActionResult>) (() => (ActionResult) this.Json((object) teamObjects, JsonRequestBehavior.AllowGet)), new TimeSpan?(this.GetMaxAgeTimeSpan()), (Action<HttpContextBase>) (httpContext => WITDataSource.PublishCIForTeamProjects(this.TfsRequestContext, namesOrIds, projects, includeFieldDefinitions, includeProcessId, includeProcessInfo, includeWorkItemTypes)));
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(516080, 516090)]
    public ActionResult WorkItemTypes(
      IEnumerable<string> typeNames,
      bool getSuggestedAsAllowedValuesForIdentity = false)
    {
      IList<IWorkItemType> workItemTypes = (IList<IWorkItemType>) null;
      return this.ETag((Func<string>) (() =>
      {
        using (WebPerformanceTimerHelpers.StartMeasure((WebContext) this.TfsWebContext, "ApiWITController.WorkItemTypes.GetWorkItemTypesETag"))
          return EtagHelper.GetWorkItemTypesETag(this.TfsRequestContext, this.TfsWebContext.Project.Id);
      }), (Func<ActionResult>) (() => (ActionResult) this.Json((object) WITDataSource.WorkItemTypes(this.TfsRequestContext, typeNames, getSuggestedAsAllowedValuesForIdentity, this.TfsWebContext.Project.Id, out workItemTypes), JsonRequestBehavior.AllowGet)), new TimeSpan?(this.GetMaxAgeTimeSpan()), (Action<HttpContextBase>) (httpContext => WITDataSource.PublishCIForWorkItemTypes(this.TfsRequestContext, typeNames, getSuggestedAsAllowedValuesForIdentity, this.TfsWebContext.Project, workItemTypes)));
    }

    [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
    [TfsTraceFilter(516100, 516110)]
    [TfsBypassAntiForgeryValidation]
    public JsonResult WorkItems(
      IEnumerable<int> ids,
      QueryResultFormat format = QueryResultFormat.Json,
      IEnumerable<string> fields = null,
      bool isDeleted = false,
      bool includeInRecentActivity = true)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      return WITDataSource.WorkItems(this.TfsRequestContext, ids, format, fields, isDeleted, includeInRecentActivity: includeInRecentActivity);
    }

    public override string GetActivityLogCommandPrefix() => this.GetCustomCommandPrefix();

    private string GetCustomCommandPrefix()
    {
      if (!this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment || this.NavigationContext?.CurrentAction == null || VssStringComparer.HttpRequestMethod.Compare(this.NavigationContext.CurrentAction, "workitemtypes") != 0)
        return (string) null;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      IWorkItemTrackingProcessService service = tfsRequestContext != null ? tfsRequestContext.GetService<IWorkItemTrackingProcessService>() : (IWorkItemTrackingProcessService) null;
      Guid? id = this.TfsWebContext?.Project?.Id;
      string customCommandPrefix = (string) null;
      ProcessDescriptor processDescriptor;
      if (service != null && id.HasValue && service.TryGetLatestProjectProcessDescriptor(this.TfsRequestContext, id.Value, out processDescriptor))
      {
        if (processDescriptor.IsDerived)
          customCommandPrefix = "Inherited";
        else if (processDescriptor.IsCustom)
          customCommandPrefix = "XML";
      }
      return customCommandPrefix;
    }

    [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
    [TfsTraceFilter(516800, 516810)]
    [TfsBypassAntiForgeryValidation]
    public JsonResult WorkItemsNoHistory(
      IEnumerable<int> ids,
      QueryResultFormat format = QueryResultFormat.Json,
      IEnumerable<string> fields = null,
      bool isDeleted = false,
      bool includeInRecentActivity = true)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      return WITDataSource.WorkItems(this.TfsRequestContext, ids, format, fields, isDeleted, false, includeInRecentActivity);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(516129, 516129)]
    public JsonResult ResourceLinks(IEnumerable<int> ids)
    {
      SecureJsonResult secureJsonResult = new SecureJsonResult();
      secureJsonResult.Data = (object) WITDataSource.ResourceLinks(this.TfsRequestContext, ids);
      secureJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (JsonResult) secureJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Collection | NavigationContextLevels.Project)]
    [TfsTraceFilter(516140, 516150)]
    public ActionResult AllowedValues(
      int? fieldId,
      string fieldReferenceName = null,
      IEnumerable<string> workItemTypeNames = null)
    {
      if (fieldReferenceName == null)
        ArgumentUtility.CheckForNull<int>(fieldId, nameof (fieldId));
      return this.ETag((Func<string>) (() => EtagHelper.GetAllowedValuesETag(this.TfsRequestContext)), (Func<ActionResult>) (() =>
      {
        FieldEntry fieldEntry = fieldReferenceName != null ? this.WitFieldService.GetField(this.TfsRequestContext, fieldReferenceName) : this.WitFieldService.GetFieldById(this.TfsRequestContext, fieldId.Value);
        string name = this.TfsWebContext.Project != null ? this.TfsWebContext.Project.Name : (string) null;
        IEnumerable<string> strings = fieldEntry == null ? Enumerable.Empty<string>() : this.WitService.GetAllowedValues(this.TfsRequestContext, fieldEntry.FieldId, name, workItemTypeNames, fieldEntry.IsIdentity);
        return (ActionResult) new SecureJsonResult()
        {
          Data = (object) strings,
          JsonRequestBehavior = JsonRequestBehavior.AllowGet
        };
      }), new TimeSpan?(this.GetMaxAgeTimeSpan()));
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(516160, 516170)]
    public ActionResult Groups(bool? includeGlobal)
    {
      int projectId = 0;
      if (this.TfsWebContext.Project != null)
        projectId = this.WitService.GetProjectNodeId(this.TfsRequestContext, this.TfsWebContext.Project.Name);
      return this.ETag((Func<string>) (() => this.CalculateMetadataETag((IEnumerable<MetadataTable>) new MetadataTable[3]
      {
        MetadataTable.Constants,
        MetadataTable.ConstantSets,
        MetadataTable.Rules
      }, projectId)), (Func<ActionResult>) (() =>
      {
        if (!includeGlobal.HasValue)
          includeGlobal = new bool?(false);
        return (ActionResult) this.Json((object) this.WitService.GetGlobalAndProjectGroups(this.TfsRequestContext, projectId, includeGlobal.Value), JsonRequestBehavior.AllowGet);
      }));
    }

    [TfsTraceFilter(516180, 516190)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult WorkItemTypeCategories() => this.ETag((Func<string>) (() => EtagHelper.GetWorkItemTypeCategoriesETag(this.TfsRequestContext)), (Func<ActionResult>) (() => (ActionResult) this.Json((object) this.WitService.GetWorkItemTypeCategories(this.TfsRequestContext, this.TfsWebContext.Project.Name).ToDictionary<WorkItemTypeCategory, string, JsObject>((Func<WorkItemTypeCategory, string>) (witCategory => witCategory.ReferenceName), (Func<WorkItemTypeCategory, JsObject>) (witCategory => witCategory.ToJson())), JsonRequestBehavior.AllowGet)), new TimeSpan?(this.GetMaxAgeTimeSpan()));

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [TfsTraceFilter(516200, 516210, Order = 1)]
    public ActionResult UpdateWorkItems([ModelBinder(typeof (WorkItemUpdateModelBinder))] IEnumerable<WorkItemUpdate> updatePackage)
    {
      this.Trace(516201, TraceLevel.Info, "Update WorkItems");
      return (ActionResult) this.Json((object) WITDataSource.UpdateWorkItems(this.TfsRequestContext, updatePackage), JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [TfsTraceFilter(516220, 516230)]
    public ActionResult UpdateQueries([ModelBinder(typeof (QueryItemModelBinder))] IEnumerable<QueryItem> queries)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) queries, nameof (queries));
      CommonUtility.CheckEnumerableElements<QueryItem>(queries, nameof (queries), (Action<QueryItem, string>) ((queryItem, paramName) =>
      {
        if (((IEnumerable<Guid>) AdhocQueryProvider.QueryIds).Contains<Guid>(queryItem.Id))
        {
          ArgumentUtility.CheckStringForNullOrEmpty(queryItem.QueryText, paramName + ".queryText");
        }
        else
        {
          ArgumentUtility.CheckForEmptyGuid(queryItem.ParentId, paramName + ".parentId");
          ArgumentUtility.CheckStringForNullOrEmpty(queryItem.Name, paramName + ".name");
        }
        if (queryItem.IsFolder)
          return;
        try
        {
          WiqlHelper.ParseSyntax(queryItem.QueryText);
        }
        catch (Exception ex)
        {
          throw new InvalidArgumentValueException(paramName + ".queryText", ex);
        }
      }));
      List<JsObject> data = new List<JsObject>();
      int num = 0;
      foreach (QueryItem query in queries)
      {
        try
        {
          if (query.Id == Guid.Empty)
          {
            Guid id = Guid.NewGuid();
            JsObject jsObject = new JsObject();
            jsObject["index"] = (object) num;
            jsObject["id"] = (object) id;
            this.Trace(516221, TraceLevel.Info, "Saving new query item [Id: {0}]", (object) id);
            this.WitService.CreateQueryItem(this.TfsRequestContext, id, query.ParentId, query.Name, query.QueryText);
            data.Add(jsObject);
          }
          else if (((IEnumerable<Guid>) AdhocQueryProvider.QueryIds).Contains<Guid>(query.Id))
          {
            this.Trace(516224, TraceLevel.Info, "Saving adhoc query [Id: {0}]", (object) query.Id);
            AdhocQueryProvider.SaveAdhocQuery(this.TfsRequestContext, this.TfsWebContext.CurrentProjectGuid, query.Id, query.QueryText);
          }
          else
          {
            this.Trace(516225, TraceLevel.Info, "Saving query item [Id: {0}]", (object) query.Id);
            this.WitService.UpdateQueryItem(this.TfsRequestContext, query.Id, query.ParentId, query.Name, query.QueryText);
          }
        }
        catch (Exception ex)
        {
          this.TraceException(516226, ex);
          JsObject jsObject = new JsObject();
          jsObject["index"] = (object) num;
          jsObject["error"] = (object) ex.ToJson();
          this.TraceException(599999, ex);
          data.Add(jsObject);
        }
        ++num;
      }
      return (ActionResult) this.Json((object) data);
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [TfsBypassAntiForgeryValidation]
    [TfsTraceFilter(516240, 516250)]
    public ActionResult PageWorkItems(
      string workItemIds,
      string fields,
      DateTime? asOf,
      bool? omitHeaders,
      bool isDeleted = false)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult();
      contractJsonResult.Data = (object) BoardsQueryWITDataSource.PageWorkItems(this.TfsRequestContext, this.TraceArea, workItemIds, fields, asOf, omitHeaders, isDeleted);
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [TfsTraceFilter(516700, 516710)]
    [TfsBypassAntiForgeryValidation]
    public JsonResult PageWorkItemsByIdRev(
      string workItemIds,
      string workItemRevisions,
      string fields,
      bool isDeleted = false)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult();
      contractJsonResult.Data = (object) BoardsQueryWITDataSource.PageWorkItemsByIdRev(this.TfsRequestContext, workItemIds, workItemRevisions, fields, isDeleted);
      return (JsonResult) contractJsonResult;
    }

    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(516260, 516270)]
    public ActionResult AdHocQueries() => this.ETag((Func<string>) (() => AdhocQueryProvider.Etags.GetAdHocQueriesETag(this.TfsRequestContext, this.TfsWebContext.CurrentProjectGuid)), (Func<ActionResult>) (() =>
    {
      AdHocQueriesResult hocQueriesResult = BoardsQueryWITDataSource.AdHocQueries(this.TfsRequestContext, this.TfsWebContext.CurrentProjectGuid);
      return (ActionResult) new SecureJsonResult()
      {
        Data = (object) new
        {
          recycleBin = hocQueriesResult.RecycleBin
        },
        JsonRequestBehavior = JsonRequestBehavior.AllowGet
      };
    }), new TimeSpan?(this.GetMaxAgeTimeSpan()));

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(516480, 516490)]
    public ActionResult UpdateAdHocQuery(Guid queryId, string wiql)
    {
      try
      {
        return (ActionResult) this.Json((object) BoardsQueryWITDataSource.UpdateAdHocQuery(this.TfsRequestContext, this.TfsWebContext.CurrentProjectGuid, this.TraceArea, queryId, wiql));
      }
      catch (ArgumentException ex)
      {
        return (ActionResult) this.HttpStatusCodeWithMessage(HttpStatusCode.NotFound, ex.Message);
      }
      catch (Exception ex)
      {
        return (ActionResult) this.HttpStatusCodeWithMessage(HttpStatusCode.BadRequest, ex.Message);
      }
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put | HttpVerbs.Delete)]
    [TfsTraceFilter(516280, 516290)]
    public ActionResult DeleteQueries(string[] queryIds)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) queryIds, nameof (queryIds));
      int length = queryIds.Length;
      List<JsObject> data = new List<JsObject>();
      for (int index = 0; index < length; ++index)
      {
        string queryId = queryIds[index];
        try
        {
          ArgumentUtility.CheckStringForNullOrEmpty(queryId, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "queryIds[{0}]", (object) index));
          this.Trace(516281, TraceLevel.Info, "DeleteQueries: queryId: {0}", (object) queryId);
          this.WitService.DeleteQueryItem(this.TfsRequestContext, new Guid(queryId));
        }
        catch (Exception ex)
        {
          JsObject jsObject = new JsObject();
          jsObject["index"] = (object) index;
          jsObject["error"] = (object) ex.ToJson();
          data.Add(jsObject);
          this.TraceException(599999, ex);
        }
      }
      return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(516320, 516330)]
    public ActionResult DownloadAttachment(
      int? attachmentId,
      Guid? fileGuid,
      string fileName,
      bool? contentOnly)
    {
      if (!this.TfsRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(this.TfsRequestContext, "WorkItemTracking.Server.AllowMVCDownloadAttachment"))
        throw new FeatureDisabledException(nameof (DownloadAttachment));
      ArgumentUtility.CheckForNull<string>(fileName, nameof (fileName));
      if (!fileGuid.HasValue && !attachmentId.HasValue)
        throw new ArgumentNullException(nameof (fileGuid));
      int fileId;
      try
      {
        fileId = !fileGuid.HasValue ? this.WitService.GetAttachmentFileId(this.TfsRequestContext, attachmentId.Value) : this.WitService.GetAttachmentFileId(this.TfsRequestContext, fileGuid.Value);
      }
      catch (AttachmentNotFoundException ex)
      {
        return (ActionResult) this.HttpNotFound(ex.Message);
      }
      if (!contentOnly.HasValue)
        contentOnly = new bool?(false);
      this.Trace(516321, TraceLevel.Info, "DownloadAttachment: fileId: [{0}]  fileName: [{1}] contentOnly: [{2}] ", (object) fileId, (object) fileName, (object) contentOnly.Value);
      this.TfsRequestContext.RequestTimeout = TimeSpan.FromMinutes(60.0);
      this.TfsRequestContext.RequestTimer.SetTimeToFirstPageEnd();
      return (ActionResult) new WorkItemAttachmentActionResult(this.TfsRequestContext, fileId, fileName, contentOnly.Value);
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [TfsTraceFilter(516360, 516370)]
    public ActionResult UpdateColumnOptions(Guid? persistenceId, IEnumerable<string> fields)
    {
      BoardsQueryWITDataSource.UpdateColumnOptions(this.TfsRequestContext, this.TfsWebContext.CurrentProjectGuid, persistenceId, fields);
      return (ActionResult) this.Json((object) new
      {
        success = true
      });
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(516380, 516390)]
    public ActionResult ColumnOptions(bool simpleMode = false) => (ActionResult) this.View((object) new ColumnOptionsModel()
    {
      SimpleMode = simpleMode
    });

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(516420, 516430)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult SearchIdentities(string searchTerm, SearchIdentityType identityType = SearchIdentityType.All)
    {
      IEnumerable<IdentityRef> source = this.TfsRequestContext.GetService<IWorkItemIdentityService>().SearchIdentities(this.TfsRequestContext, searchTerm, identityType);
      SecureJsonResult secureJsonResult = new SecureJsonResult();
      secureJsonResult.Data = (object) source.Select<IdentityRef, JsObject>((Func<IdentityRef, JsObject>) (identity => identity.ToJson()));
      secureJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) secureJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(516400, 516410)]
    public ActionResult Search(string searchText)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) BoardsQueryWITDataSource.Search(this.TfsRequestContext, this.TraceArea, this.TfsWebContext.CurrentProjectGuid, this.TfsWebContext.Project, this.TfsWebContext.Team, searchText));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [OutputCache(CacheProfile = "NoCache")]
    [TfsBypassAntiForgeryValidation]
    [TfsTraceFilter(516410, 516420)]
    public ActionResult Query(
      string wiql,
      IEnumerable<string> fields,
      IEnumerable<string> sortFields,
      DateTime? asOf,
      bool? runQuery,
      bool? includePayload,
      bool? includeEditInfo,
      Guid? persistenceId,
      int? top,
      IEnumerable<int> workItemIdFilter = null,
      QueryResultFormat format = QueryResultFormat.Json,
      bool isDirty = false)
    {
      if (format == QueryResultFormat.Html)
      {
        HtmlQueryResult htmlQueryResult = BoardsQueryWITDataSource.QueryHtmlFormat(this.TfsRequestContext, this.TraceArea, this.TfsWebContext.CurrentProjectGuid, this.TfsWebContext.Project, this.TfsWebContext.Team, this.TfsWebContext.FeatureContext.AreStandardFeaturesAvailable, wiql, fields, sortFields, asOf, runQuery, includePayload, includeEditInfo, persistenceId, top, workItemIdFilter, isDirty);
        SecureJsonResult secureJsonResult = new SecureJsonResult();
        secureJsonResult.Data = (object) new
        {
          WorkItemCount = htmlQueryResult.WorkItemCount,
          MaxWorkItemCount = htmlQueryResult.MaxWorkItemCount,
          Html = htmlQueryResult.Html
        };
        secureJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        return (ActionResult) secureJsonResult;
      }
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) BoardsQueryWITDataSource.QueryJsonFormat(this.TfsRequestContext, this.TraceArea, this.TfsWebContext.CurrentProjectGuid, this.TfsWebContext.Project, this.TfsWebContext.Team, this.TfsWebContext.FeatureContext.AreStandardFeaturesAvailable, wiql, fields, sortFields, asOf, runQuery, includePayload, includeEditInfo, persistenceId, top, workItemIdFilter, isDirty));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [RequireTeam]
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult TeamSettings()
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      TeamWITSettingsModel data;
      try
      {
        data = TeamWITSettingsModel.CreateTeamWITSettingsModel(this.TfsRequestContext, this.TfsWebContext.Project, this.TfsWebContext.Team, this.TfsRequestContext.GetCollectionTimeZone());
      }
      catch (ProjectSettingsException ex)
      {
        data = new TeamWITSettingsModel();
        this.TraceException(290233, (Exception) ex, TraceLevel.Warning);
      }
      catch (InvalidTeamSettingsException ex)
      {
        data = new TeamWITSettingsModel();
        this.TraceException(290232, (Exception) ex, TraceLevel.Warning);
      }
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) data);
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(516440, 516450)]
    public ActionResult FindWorkItem(bool? showContextMenu, bool? allowMultiSelect) => (ActionResult) this.View("WorkItemFinder", (object) new WorkItemFinderModel()
    {
      ShowContextMenu = (showContextMenu.HasValue && showContextMenu.Value),
      AllowMultipleSelection = (allowMultiSelect.HasValue && allowMultiSelect.Value)
    });

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(516461, 516469)]
    public ActionResult WorkItemTypeExtensions(IEnumerable<Guid> extensionIds)
    {
      IEnumerable<WorkItemTypeExtension> extensions;
      JsObject[] extensionsJsonObject = WITDataSource.WorkItemTypeExtensions(this.TfsRequestContext, extensionIds, out extensions);
      return this.ETag((Func<string>) (() => EtagHelper.GetWorkItemTypeExtensionETag(extensions, this.TfsRequestContext)), (Func<ActionResult>) (() => (ActionResult) this.Json((object) extensionsJsonObject, JsonRequestBehavior.AllowGet)));
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult QueryFavorites(Guid teamId)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      return (ActionResult) this.Json(BoardsQueryWITDataSource.QueryFavorites(this.TfsRequestContext, teamId), JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(516531, 516540)]
    public ActionResult GetLayoutProperties()
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      IWorkItemTrackingConfigurationInfo configurationInfo = this.TfsRequestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(this.TfsRequestContext);
      JsObject data = new JsObject();
      data.Add("maxAttachmentSize", (object) configurationInfo.MaxAttachmentSize);
      IUserPreferencesService service = this.TfsRequestContext.GetService<IUserPreferencesService>();
      data.Add("fieldChromeBorder", (object) service.GetUserPreferences(this.TfsRequestContext).WorkItemFormChromeBorder);
      return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    [ValidateInput(false)]
    [AcceptVerbs(HttpVerbs.Post)]
    [TfsTraceFilter(516541, 516550)]
    public void SendMailAsync(
      [ModelBinder(typeof (JsonModelBinder))] MailMessage message,
      IEnumerable<int> ids = null,
      string wiql = "",
      IEnumerable<string> fields = null,
      Guid? persistenceId = null,
      string tempQueryId = "",
      IEnumerable<string> sortFields = null,
      string projectId = "")
    {
      ArgumentUtility.CheckForNull<MailMessage>(message, nameof (message));
      ArgumentUtility.CheckForNull<EmailRecipients>(message.To, "message.To");
      ArgumentUtility.CheckForNull<EmailRecipients>(message.ReplyTo, "message.ReplyTo");
      if (this.TfsRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(this.TfsRequestContext, "WorkItemTracking.Server.DisableSendMailDataProvider"))
        MailSender.BeginSendMail(WITDataSource.ModifySendMailMessage(message, this.TfsRequestContext, this.TraceArea, this.TfsWebContext.Project, this.TfsWebContext.Team, ids, wiql, fields, persistenceId, tempQueryId, sortFields, projectId), this.TfsRequestContext, this.Request.RequestContext.TfsWebContext().IsHosted, this.AsyncManager);
      else
        WITDataSource.SendMail(message, this.TfsRequestContext, this.AsyncManager, this.TraceArea, this.TfsWebContext.Project, this.TfsWebContext.Team, this.Request.RequestContext.TfsWebContext().IsHosted, ids, wiql, fields, persistenceId, tempQueryId, sortFields, projectId);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(516551, 516560)]
    public ActionResult SendMailCompleted() => (ActionResult) this.Json((object) MailSender.SendMailCompleted(this.AsyncManager));

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(516581, 516590)]
    public ActionResult WorkItemStatesColorData()
    {
      try
      {
        ProjectInfo project = this.TfsWebContext.Project;
        IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>> projectStateColors = this.TfsRequestContext.GetService<IWorkItemMetadataFacadeService>().GetWorkItemStateColors(this.TfsRequestContext, project.Id);
        return this.ETag((Func<string>) (() => EtagHelper.GetStateColorETagPart(this.TfsRequestContext, project.Id)), (Func<ActionResult>) (() =>
        {
          return (ActionResult) this.Json((object) new JsObject()
          {
            ["projectName"] = (object) project.Name,
            ["projectId"] = (object) project.Id,
            ["workItemTypeStateColors"] = (object) projectStateColors.Select<KeyValuePair<string, IReadOnlyCollection<WorkItemStateColor>>, JsObject>((Func<KeyValuePair<string, IReadOnlyCollection<WorkItemStateColor>>, JsObject>) (item => item.Value.ToJson(item.Key))).ToArray<JsObject>()
          }, JsonRequestBehavior.AllowGet);
        }));
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(290005, "WebAccess", TfsTraceLayers.Controller, ex);
        throw;
      }
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(516591, 516600)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    public ActionResult GetMetadataCacheStamps()
    {
      Dictionary<string, string> metadataCacheStamps = EtagHelper.GetWorkItemMetadataCacheStamps(this.TfsWebContext.TfsRequestContext, this.TfsWebContext.Project.Id);
      string workItemTypesEtag = EtagHelper.GetWorkItemTypesETag(this.TfsRequestContext, this.TfsWebContext.Project.Id);
      int cacheMaxAgeInDays = this.TfsRequestContext.WitContext().ServerSettings.WorkItemMetadataCacheMaxAgeInDays;
      return (ActionResult) this.Json((object) JsonConvert.SerializeObject((object) new WorkItemMetadataCacheData()
      {
        WorkItemMetadataCacheMaxAgeInDays = cacheMaxAgeInDays,
        WorkItemMetadataCacheStamp = metadataCacheStamps,
        RawWorkItemTypesEtagForCI = workItemTypesEtag
      }).ToString(), JsonRequestBehavior.AllowGet);
    }

    private static string GetQueryRegistryPath(Guid projectId, Guid queryId) => "/Projects/" + projectId.ToString() + "/Queries/" + queryId.ToString() + "/Query";

    private string BuildPlatformIndependentDownloadUrl(Guid fileGuid, string fileName)
    {
      ArgumentUtility.CheckForEmptyGuid(fileGuid, nameof (fileGuid));
      ServiceDefinition serviceDefinition = this.LocationService.FindServiceDefinition(this.TfsRequestContext, "WorkitemAttachmentHandler", ServiceIdentifiers.WorkItemAttachmentHandler);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(this.LocationService.LocationForAccessMapping(this.TfsRequestContext, serviceDefinition, this.LocationService.DetermineAccessMapping(this.TfsRequestContext)));
      stringBuilder.AppendFormat("?FileNameGuid={0}", (object) fileGuid);
      if (!string.IsNullOrWhiteSpace(fileName))
        stringBuilder.AppendFormat("&FileName={0}", (object) Uri.EscapeDataString(Path.GetFileName(fileName)));
      return stringBuilder.ToString();
    }

    private string CalculateMetadataETag(IEnumerable<MetadataTable> tables, int projectId)
    {
      string metadataEtag = "";
      if (tables != null && tables.Any<MetadataTable>())
        metadataEtag = this.WitService.GetMetadataTableTimestamps(this.TfsRequestContext, tables, projectId).OrderBy<KeyValuePair<MetadataTable, long>, MetadataTable>((Func<KeyValuePair<MetadataTable, long>, MetadataTable>) (pair => pair.Key)).Select<KeyValuePair<MetadataTable, long>, long>((Func<KeyValuePair<MetadataTable, long>, long>) (pair => pair.Value)).Concat<long>((IEnumerable<long>) new long[1]
        {
          (long) this.TfsRequestContext.AuthenticatedUserName.GetStableHashCode()
        }).Select<long, string>((Func<long, string>) (stamp => Convert.ToString(stamp, 16))).StringJoin<string>('-');
      return metadataEtag;
    }

    private string CalculateQueriesEtag(int projectId) => string.Format("{0}-{1}", (object) this.WitService.GetQueryItemsTimestamps(this.TfsRequestContext, projectId).Select<long, string>((Func<long, string>) (stamp => Convert.ToString(stamp, 16))).StringJoin<string>('-'), (object) AdhocQueryProvider.Etags.CalculateAdhocQueryEtag(this.TfsRequestContext, this.TfsWebContext.CurrentProjectGuid));

    private TimeSpan GetMaxAgeTimeSpan() => string.IsNullOrEmpty(this.Request.Params["stamp"]) ? TimeSpan.Zero : TimeSpan.MaxValue;
  }
}
