// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WITDataSource
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Mail;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.ResourceLinks;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Async;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class WITDataSource
  {
    public const int WebAccessExceptionEaten = 599999;

    public static JsObject[] WorkItemTypeExtensions(
      IVssRequestContext tfsRequestContext,
      IEnumerable<Guid> extensionIds,
      out IEnumerable<WorkItemTypeExtension> extensions)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) extensionIds, nameof (extensionIds));
      IWorkItemTypeExtensionService service = tfsRequestContext.GetService<IWorkItemTypeExtensionService>();
      extensions = service.GetExtensions(tfsRequestContext, extensionIds);
      return extensions.Select<WorkItemTypeExtension, JsObject>((Func<WorkItemTypeExtension, JsObject>) (e => e.ToJson(tfsRequestContext))).ToArray<JsObject>();
    }

    public static IEnumerable<JsObject> UpdateWorkItems(
      IVssRequestContext tfsRequestContext,
      IEnumerable<WorkItemUpdate> updatePackage)
    {
      ArgumentUtility.CheckForNull<IEnumerable<WorkItemUpdate>>(updatePackage, nameof (updatePackage));
      IEnumerable<WorkItemUpdateResult> source = tfsRequestContext.GetService<ITeamFoundationWorkItemService>().UpdateWorkItems(tfsRequestContext, updatePackage, includeInRecentActivity: true);
      if (updatePackage.Count<WorkItemUpdate>() == 1)
      {
        foreach (WorkItemUpdate workItemUpdate in updatePackage)
        {
          if (workItemUpdate.Id == -1)
          {
            KeyValuePair<string, object> keyValuePair = workItemUpdate.Fields.FirstOrDefault<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (field => string.Equals(field.Key, CoreFieldReferenceNames.WorkItemType)));
            if (keyValuePair.Value != null && string.Equals(keyValuePair.Value as string, "Test Plan"))
            {
              CustomerIntelligenceData properties = new CustomerIntelligenceData();
              properties.Add("TestPlanWIT", (double) source.Single<WorkItemUpdateResult>().Id);
              tfsRequestContext.GetService<CustomerIntelligenceService>().Publish(tfsRequestContext, CustomerIntelligenceArea.WorkItemTracking, "CreateTestPlanWIT", properties);
            }
          }
        }
      }
      return source.Select<WorkItemUpdateResult, JsObject>((Func<WorkItemUpdateResult, JsObject>) (ur => ur.ToJson(tfsRequestContext, DateTime.UtcNow)));
    }

    public static Dictionary<string, string[]> GetConstantSets(
      IVssRequestContext tfsRequestContext,
      IEnumerable<ConstantSetReference> ids)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) ids, nameof (ids));
      IDictionary<ConstantSetReference, SetRecord[]> setMap = tfsRequestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>().GetConstantSets(tfsRequestContext, ids);
      SetRecord[] source;
      return ids.ToDictionary<ConstantSetReference, string, string[]>((Func<ConstantSetReference, string>) (cref => cref.ToString()), (Func<ConstantSetReference, string[]>) (cref => setMap.TryGetValue(cref, out source) ? ((IEnumerable<SetRecord>) source).Select<SetRecord, string>((Func<SetRecord, string>) (sr => sr.Item)).Distinct<string>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase).ToArray<string>() : Array.Empty<string>()));
    }

    public static JsObject TeamProjects(
      IVssRequestContext tfsRequestContext,
      IEnumerable<string> namesOrIds,
      out IEnumerable<Project> projects,
      bool includeFieldDefinitions = false,
      bool includeProcessId = true,
      bool includeProcessInfo = true,
      bool includeWorkItemTypes = true)
    {
      if (namesOrIds != null)
        CommonUtility.CheckEnumerableElements<string>(namesOrIds, nameof (namesOrIds), (Action<string, string>) ((nameOrId, paramName) => ArgumentUtility.CheckStringForNullOrEmpty(nameOrId, paramName)));
      WebAccessWorkItemService service1 = tfsRequestContext.GetService<WebAccessWorkItemService>();
      IEnumerable<Project> projects1;
      using (PerformanceTimer.StartMeasure(tfsRequestContext, "WITDataSource.TeamProjects.GetProjects"))
        projects1 = service1.GetProjects(tfsRequestContext, includeProcessId);
      projects = projects1;
      if (namesOrIds != null && namesOrIds.Any<string>())
      {
        Dictionary<string, Project> nameMap = projects1.ToDictionary<Project, string>((Func<Project, string>) (p => p.Name), (IEqualityComparer<string>) TFStringComparer.TeamProjectName);
        Dictionary<string, Project> idMap = projects1.ToDictionary<Project, string>((Func<Project, string>) (p => p.Guid.ToString()), (IEqualityComparer<string>) TFStringComparer.TeamProjectGuid);
        projects = (IEnumerable<Project>) namesOrIds.Select<string, Project>((Func<string, Project>) (nameOrId =>
        {
          Project project;
          if (!nameMap.TryGetValue(nameOrId, out project) && !idMap.TryGetValue(nameOrId, out project))
            throw new InvalidArgumentValueException(nameof (namesOrIds));
          return project;
        })).ToArray<Project>();
      }
      ILegacyApiWitControllerHelper service2 = tfsRequestContext.GetService<ILegacyApiWitControllerHelper>();
      using (PerformanceTimer.StartMeasure(tfsRequestContext, "WITDataSource.TeamProjects.GetTeamProjectsInternal"))
        return service2.GetTeamProjectsInternal(tfsRequestContext, (IEnumerable<object>) projects, includeFieldDefinitions, includeProcessInfo, includeWorkItemTypes);
    }

    private static JsObject GetTeamProjectsInternal(
      IVssRequestContext requestContext,
      IEnumerable<object> objectProjects,
      bool includeFieldDefinition = false)
    {
      IEnumerable<Project> source1 = objectProjects.Select<object, Project>((Func<object, Project>) (proj => (Project) proj));
      ILookup<int, IWorkItemType> workItemTypesLookupByProject = requestContext.GetService<WebAccessWorkItemService>().GetWorkItemTypes(requestContext, source1.Select<Project, Guid>((Func<Project, Guid>) (p => p.Guid))).ToLookup<IWorkItemType, int>((Func<IWorkItemType, int>) (wit => wit.ProjectId));
      bool addFieldEnabled = EtagHelper.IsAddFieldEnabled(requestContext);
      List<FieldDefinition> fieldDefinitions = new List<FieldDefinition>();
      Func<Project, JsObject> ProjectToJson = (Func<Project, JsObject>) (project =>
      {
        IEnumerable<IWorkItemType> source2 = workItemTypesLookupByProject[project.Id];
        IEnumerable<string> workItemTypeNames = source2.Select<IWorkItemType, string>((Func<IWorkItemType, string>) (wit => wit.Name));
        IEnumerable<int> fieldIds;
        if (includeFieldDefinition)
        {
          IEnumerable<IGrouping<int, FieldDefinition>> source3 = source2.SelectMany<IWorkItemType, FieldDefinition>((Func<IWorkItemType, IEnumerable<FieldDefinition>>) (wit => (IEnumerable<FieldDefinition>) wit.GetFields(requestContext))).GroupBy<FieldDefinition, int>((Func<FieldDefinition, int>) (fd => fd.Id));
          fieldIds = source3.Select<IGrouping<int, FieldDefinition>, int>((Func<IGrouping<int, FieldDefinition>, int>) (fd => fd.Key));
          fieldDefinitions.AddRange((IEnumerable<FieldDefinition>) source3.Select<IGrouping<int, FieldDefinition>, FieldDefinition>((Func<IGrouping<int, FieldDefinition>, FieldDefinition>) (fd => fd.First<FieldDefinition>())).ToList<FieldDefinition>());
        }
        else
          fieldIds = source2.SelectMany<IWorkItemType, int>((Func<IWorkItemType, IEnumerable<int>>) (wit => wit.GetFields(requestContext).Select<FieldDefinition, int>((Func<FieldDefinition, int>) (fd => fd.Id)))).Distinct<int>();
        JsObject json = project.ToJson(workItemTypeNames, fieldIds, (object) null);
        IWorkItemTrackingProcessService service = requestContext.GetService<IWorkItemTrackingProcessService>();
        ProcessDescriptor processDescriptor;
        if (addFieldEnabled && service.TryGetLatestProjectProcessDescriptor(requestContext, project.Guid, out processDescriptor))
          json["process"] = (object) new ProcessDescriptorModel(requestContext, processDescriptor, (ISecuredObject) null).ToJson();
        return json;
      });
      List<Project> list = source1.ToList<Project>();
      list.Sort();
      JsObject projectsInternal = new JsObject();
      projectsInternal["projects"] = (object) list.Select<Project, JsObject>((Func<Project, JsObject>) (p => ProjectToJson(p)));
      if (includeFieldDefinition)
      {
        IEnumerable<JsObject> first = fieldDefinitions.GroupBy<FieldDefinition, int>((Func<FieldDefinition, int>) (fd => fd.Id)).Select<IGrouping<int, FieldDefinition>, JsObject>((Func<IGrouping<int, FieldDefinition>, JsObject>) (fd => fd.First<FieldDefinition>().ToJson()));
        IEnumerable<JsObject> ignoredCoreFields = WITDataSource.GetIgnoredCoreFields(requestContext);
        projectsInternal["fields"] = first != null ? (object) first.Concat<JsObject>(ignoredCoreFields) : (object) (IEnumerable<JsObject>) null;
      }
      return projectsInternal;
    }

    private static IEnumerable<JsObject> GetIgnoredCoreFields(IVssRequestContext requestContext) => requestContext.WitContext().FieldDictionary.GetCoreFields().Where<FieldEntry>((Func<FieldEntry, bool>) (fld => fld.Usage == InternalFieldUsages.WorkItem && fld.IsIgnored)).Select<FieldEntry, JsObject>((Func<FieldEntry, JsObject>) (fld => fld.ToJson()));

    public static void PublishCIForTeamProjects(
      IVssRequestContext tfsRequestContext,
      IEnumerable<string> namesOrIds,
      IEnumerable<Project> projects,
      bool includeFieldDefinitions,
      bool includeProcessId,
      bool includeProcessInfo,
      bool includeWorkItemTypes)
    {
      if (!WITDataSource.IsElapsedTimeAboveThreshold(tfsRequestContext, "/Service/WorkItemTracking/Settings/TraceTeamProjectsAboveThreshold", 300))
        return;
      PerformanceTimer.SendCustomerIntelligenceData(tfsRequestContext, (Action<CustomerIntelligenceData>) (ciData =>
      {
        ciData.Add("Timings", tfsRequestContext.GetTraceTimingAsString());
        ciData.Add("GivenProjectNames", (object) namesOrIds);
        ciData.Add("ActualProjectNames", (object) projects.Select<Project, string>((Func<Project, string>) (p => p.Name)));
        ciData.Add("ProjectsCount", (double) projects.Count<Project>());
        ciData.Add("IncludeFieldDefinitions", includeFieldDefinitions);
        ciData.Add("IncludeProcessId", includeProcessId);
        ciData.Add("IncludeProcessInfo", includeProcessInfo);
        ciData.Add("IncludeWorkItemTypes", includeWorkItemTypes);
      }));
    }

    public static Dictionary<string, object> LinkTypes(IVssRequestContext tfsRequestContext)
    {
      WebAccessWorkItemService service = tfsRequestContext.GetService<WebAccessWorkItemService>();
      return new Dictionary<string, object>()
      {
        ["witLinkTypes"] = (object) service.GetLinkTypes(tfsRequestContext).ToJson(),
        ["registeredLinkTypes"] = (object) service.GetRegisteredLinkTypes(tfsRequestContext).Select<RegisteredLinkType, JsObject>((Func<RegisteredLinkType, JsObject>) (rlt => rlt.ToJson()))
      };
    }

    public static string GetMetadataCacheStamps(
      IVssRequestContext tfsRequestContext,
      Guid projectId)
    {
      Dictionary<string, string> metadataCacheStamps = EtagHelper.GetWorkItemMetadataCacheStamps(tfsRequestContext, projectId);
      string workItemTypesEtag = EtagHelper.GetWorkItemTypesETag(tfsRequestContext, projectId);
      int cacheMaxAgeInDays = tfsRequestContext.WitContext().ServerSettings.WorkItemMetadataCacheMaxAgeInDays;
      return JsonConvert.SerializeObject((object) new WorkItemMetadataCacheData()
      {
        WorkItemMetadataCacheMaxAgeInDays = cacheMaxAgeInDays,
        WorkItemMetadataCacheStamp = metadataCacheStamps,
        RawWorkItemTypesEtagForCI = workItemTypesEtag
      }).ToString();
    }

    public static IEnumerable<JsObject> WorkItemTypes(
      IVssRequestContext tfsRequestContext,
      IEnumerable<string> typeNames,
      bool getSuggestedAsAllowedValuesForIdentity,
      Guid projectId,
      out IList<IWorkItemType> workItemTypes)
    {
      WebAccessWorkItemService service = tfsRequestContext.GetService<WebAccessWorkItemService>();
      workItemTypes = (IList<IWorkItemType>) null;
      using (PerformanceTimer.StartMeasure(tfsRequestContext, "WITDataSource.WorkItemTypes.GetWorkItemTypes"))
        workItemTypes = (IList<IWorkItemType>) service.GetWorkItemTypes(tfsRequestContext, projectId).ToList<IWorkItemType>();
      IList<IWorkItemType> source = workItemTypes;
      if (typeNames != null && typeNames.Any<string>())
      {
        Dictionary<string, IWorkItemType> map = workItemTypes.ToDictionary<IWorkItemType, string>((Func<IWorkItemType, string>) (t => t.Name), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
        source = (IList<IWorkItemType>) typeNames.Select<string, IWorkItemType>((Func<string, IWorkItemType>) (typeName =>
        {
          IWorkItemType workItemType;
          if (!map.TryGetValue(typeName, out workItemType))
            throw new InvalidArgumentValueException(nameof (typeNames));
          return workItemType;
        })).ToArray<IWorkItemType>();
      }
      return source.Select<IWorkItemType, JsObject>((Func<IWorkItemType, JsObject>) (wit => wit.ToJson(tfsRequestContext, projectId, getSuggestedAsAllowedValuesForIdentity)));
    }

    public static void PublishCIForWorkItemTypes(
      IVssRequestContext tfsRequestContext,
      IEnumerable<string> typeNames,
      bool getSuggestedAsAllowedValuesForIdentity,
      ProjectInfo project,
      IList<IWorkItemType> workItemTypes)
    {
      if (!WITDataSource.IsElapsedTimeAboveThreshold(tfsRequestContext, "/Service/WorkItemTracking/Settings/TraceWorkItemTypesAboveThreshold", 200))
        return;
      PerformanceTimer.SendCustomerIntelligenceData(tfsRequestContext, (Action<CustomerIntelligenceData>) (ciData =>
      {
        ciData.Add("Timings", tfsRequestContext.GetTraceTimingAsString());
        ciData.Add("TypeNames", (object) typeNames);
        ciData.Add("GetSuggestedAsAllowedValuesForIdentity", getSuggestedAsAllowedValuesForIdentity);
        ciData.Add("ProjectName", project.Name);
        ciData.Add("ProjectId", (object) project.Id);
        ciData.Add("TotalTypesCount", (object) workItemTypes?.Count);
        CustomerIntelligenceData intelligenceData = ciData;
        IEnumerable<string> source = typeNames;
        // ISSUE: variable of a boxed type
        __Boxed<int?> local = (ValueType) (source != null ? new int?(source.Count<string>()) : new int?());
        intelligenceData.Add("ReturnTypesCount", (object) local);
      }));
    }

    public static IEnumerable<JsObject> ResourceLinks(
      IVssRequestContext tfsRequestContext,
      IEnumerable<int> ids)
    {
      return tfsRequestContext.GetService<ResourceLinkService>().GetResourceLinks(tfsRequestContext, ids).Select<ResourceLink, JsObject>((Func<ResourceLink, JsObject>) (rl => rl.ToJson()));
    }

    private static bool IsElapsedTimeAboveThreshold(
      IVssRequestContext tfsRequestContext,
      string thresholdRegistryKey,
      int thresholdDefaultMillis)
    {
      int num = tfsRequestContext.GetService<IVssRegistryService>().GetValue<int>(tfsRequestContext, (RegistryQuery) thresholdRegistryKey, true, thresholdDefaultMillis);
      return tfsRequestContext.LastTracedBlockElapsedMilliseconds() > (double) num;
    }

    public static JsonResult WorkItems(
      IVssRequestContext tfsRequestContext,
      IEnumerable<int> ids,
      QueryResultFormat format = QueryResultFormat.Json,
      IEnumerable<string> fields = null,
      bool isDeleted = false,
      bool includeHistory = true,
      bool includeInRecentActivity = true)
    {
      if (format == QueryResultFormat.Html)
      {
        SecureJsonResult secureJsonResult = new SecureJsonResult();
        secureJsonResult.Data = (object) WITDataSource.WorkItemsHtmlInternal(tfsRequestContext, ids, fields, isDeleted, includeInRecentActivity);
        secureJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        return (JsonResult) secureJsonResult;
      }
      IWorkItemTrackingConfigurationInfo configurationInfo = tfsRequestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(tfsRequestContext);
      SecureJsonResult secureJsonResult1 = new SecureJsonResult();
      secureJsonResult1.Data = (object) WITDataSource.WorkItemsJsonInternal(tfsRequestContext, ids, isDeleted, includeHistory, includeInRecentActivity);
      secureJsonResult1.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      secureJsonResult1.MaxJsonLength = new int?(configurationInfo.WorkItemMaxJsonLength);
      return (JsonResult) secureJsonResult1;
    }

    public static WorkItemsHtmlResultData WorkItemsHtmlInternal(
      IVssRequestContext tfsRequestContext,
      IEnumerable<int> ids,
      IEnumerable<string> fields = null,
      bool isDeleted = false,
      bool includeInRecentActivity = true)
    {
      WITDataSource.WorkItemsInternalCheckIds(ids);
      int num = ids.Count<int>();
      IEnumerable<WorkItemModel> workItems;
      try
      {
        workItems = WITDataSource.GetWorkItems(tfsRequestContext, ids.Take<int>(200), isDeleted, includeInRecentActivity: includeInRecentActivity);
      }
      catch (WorkItemUnauthorizedAccessException ex)
      {
        throw new HttpException(404, ex.Message, (Exception) ex);
      }
      Dictionary<int, WorkItemModel> workItemDictionary = workItems.ToDictionary<WorkItemModel, int>((Func<WorkItemModel, int>) (wi => wi.Id));
      IEnumerable<WorkItemModel> array = (IEnumerable<WorkItemModel>) ids.Where<int>((Func<int, bool>) (id => workItemDictionary.ContainsKey(id))).Select<int, WorkItemModel>((Func<int, WorkItemModel>) (id => workItemDictionary[id])).ToArray<WorkItemModel>();
      string htmlForWorkItems = new QueryResultHtmlFormatter(tfsRequestContext).GenerateHtmlForWorkItems((IEnumerable<WorkItemModel>) array.ToList<WorkItemModel>(), fields);
      return new WorkItemsHtmlResultData()
      {
        WorkItemCount = num,
        MaxWorkItemCount = 200,
        Html = htmlForWorkItems
      };
    }

    public static IEnumerable<JsObject> WorkItemsJsonInternal(
      IVssRequestContext tfsRequestContext,
      IEnumerable<int> ids,
      bool isDeleted = false,
      bool includeHistory = true,
      bool includeInRecentActivity = true,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail)
    {
      WITDataSource.WorkItemsInternalCheckIds(ids);
      IEnumerable<WorkItemModel> workItems;
      try
      {
        workItems = WITDataSource.GetWorkItems(tfsRequestContext, ids, isDeleted, includeHistory, includeInRecentActivity, errorPolicy);
      }
      catch (WorkItemUnauthorizedAccessException ex)
      {
        throw new HttpException(404, ex.Message, (Exception) ex);
      }
      return workItems.Select<WorkItemModel, JsObject>((Func<WorkItemModel, JsObject>) (wi => wi.ToJson()));
    }

    private static void WorkItemsInternalCheckIds(IEnumerable<int> ids)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) ids, nameof (ids));
      CommonUtility.CheckEnumerableElements<int>(ids, nameof (ids), (Action<int, string>) ((id, paramName) => ArgumentUtility.CheckForOutOfRange(id, paramName, 1)));
    }

    public static IEnumerable<WorkItemModel> GetWorkItems(
      IVssRequestContext tfsRequestContext,
      IEnumerable<int> ids,
      bool isDeleted = false,
      bool includeHistory = false,
      bool includeInRecentActivity = true,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail)
    {
      TeamFoundationWorkItemService service = tfsRequestContext.GetService<TeamFoundationWorkItemService>();
      try
      {
        TeamFoundationWorkItemService foundationWorkItemService = service;
        IVssRequestContext requestContext = tfsRequestContext;
        IEnumerable<int> workItemIds = ids;
        int num1 = includeHistory ? 1 : 0;
        int num2 = isDeleted ? 2 : 1;
        bool flag = includeInRecentActivity;
        int errorPolicy1 = (int) errorPolicy;
        int num3 = flag ? 1 : 0;
        DateTime? revisionsSince = new DateTime?();
        IEnumerable<WorkItem> workItems = foundationWorkItemService.GetWorkItems(requestContext, workItemIds, true, true, num1 != 0, true, (WorkItemRetrievalMode) num2, (WorkItemErrorPolicy) errorPolicy1, num3 != 0, true, false, revisionsSince);
        WorkItemTrackingRequestContext witRequestContext = tfsRequestContext.WitContext();
        DateTime loadDate = DateTime.UtcNow;
        return (IEnumerable<WorkItemModel>) workItems.Where<WorkItem>((Func<WorkItem, bool>) (wi => wi != null)).Select<WorkItem, WorkItemModel>((Func<WorkItem, WorkItemModel>) (wi => WorkItemModelFactory.Create(witRequestContext, wi, loadDate, (ISecuredObject) null))).ToArray<WorkItemModel>();
      }
      catch (WorkItemUnauthorizedAccessException ex)
      {
        ex.LogException = false;
        throw ex;
      }
    }

    public static JsObject SendMail(
      [ModelBinder(typeof (JsonModelBinder))] MailMessage message,
      IVssRequestContext requestContext,
      AsyncManager asyncManager,
      string traceArea,
      ProjectInfo projectInfo,
      WebApiTeam team,
      bool requestIsHosted,
      IEnumerable<int> ids = null,
      string wiql = "",
      IEnumerable<string> fields = null,
      Guid? persistenceId = null,
      string tempQueryId = "",
      IEnumerable<string> sortFields = null,
      string projectId = "")
    {
      return MailSender.BeginSendMail(WITDataSource.ModifySendMailMessage(message, requestContext, traceArea, projectInfo, team, ids, wiql, fields, persistenceId, tempQueryId, sortFields, projectId), requestContext, requestIsHosted, asyncManager);
    }

    public static MailMessage ModifySendMailMessage(
      [ModelBinder(typeof (JsonModelBinder))] MailMessage message,
      IVssRequestContext requestContext,
      string traceArea,
      ProjectInfo projectInfo,
      WebApiTeam team,
      IEnumerable<int> ids = null,
      string wiql = "",
      IEnumerable<string> fields = null,
      Guid? persistenceId = null,
      string tempQueryId = "",
      IEnumerable<string> sortFields = null,
      string projectId = "")
    {
      ILegacyApiWitControllerHelper service = requestContext.GetService<ILegacyApiWitControllerHelper>();
      Microsoft.VisualStudio.Services.Identity.Identity[] array1 = ((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) service.ResolveAllIdentities(requestContext, message.ReplyTo.TfIds, message.ReplyTo.UnresolvedEntityIds, false)).Concat<Microsoft.VisualStudio.Services.Identity.Identity>((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
      {
        requestContext.GetUserIdentity()
      }).Distinct<Microsoft.VisualStudio.Services.Identity.Identity>((IEqualityComparer<Microsoft.VisualStudio.Services.Identity.Identity>) IdentityComparer.Instance).ToArray<Microsoft.VisualStudio.Services.Identity.Identity>();
      Microsoft.VisualStudio.Services.Identity.Identity[] identityArray1 = service.ResolveAllIdentities(requestContext, message.To.TfIds, message.To.UnresolvedEntityIds, true);
      message.To.TfIds = ((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray1).Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (identity => identity.Id)).Distinct<Guid>().ToArray<Guid>();
      Microsoft.VisualStudio.Services.Identity.Identity[] identityArray2 = service.ResolveAllIdentities(requestContext, message.CC.TfIds, message.CC.UnresolvedEntityIds, true);
      message.CC.TfIds = ((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray2).Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (identity => identity.Id)).Distinct<Guid>().ToArray<Guid>();
      Microsoft.VisualStudio.Services.Identity.Identity[] array2 = ((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray1).Concat<Microsoft.VisualStudio.Services.Identity.Identity>((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray2).ToArray<Microsoft.VisualStudio.Services.Identity.Identity>();
      string str1 = "";
      int fieldsCount = fields == null ? 0 : fields.Count<string>();
      int length = message.Body == null ? 0 : message.Body.Length;
      if (ids != null && ids.Any<int>())
      {
        int workItemsCount = ids.Count<int>();
        WITDataSource.CheckUsersWITReadPermissions(requestContext, ids, array1, array2);
        WITDataSource.RecordEmailSelectedWorkItemsTelemetry(requestContext, traceArea, length, fieldsCount, workItemsCount);
        str1 = str1 + WITDataSource.ConstructNotesText(requestContext, message.Body, false, workItemsCount == 1) + WITDataSource.ContructSelectedWorkItemsHtml(requestContext, ids, fields);
      }
      else if (!string.IsNullOrEmpty(wiql))
      {
        WITDataSource.RecordEmailSelectedWorkItemsTelemetry(requestContext, traceArea, length, fieldsCount, -1);
        str1 = str1 + WITDataSource.ConstructNotesText(requestContext, message.Body, true) + WITDataSource.ConstructQueriedWorkItemsHtml(requestContext, traceArea, projectInfo, team, wiql, fields, sortFields, persistenceId, array1, array2);
      }
      Guid result1;
      Guid result2;
      if (!string.IsNullOrEmpty(tempQueryId) && !string.IsNullOrEmpty(projectId) && Guid.TryParse(tempQueryId, out result1) && Guid.TryParse(projectId, out result2))
      {
        ProjectInfo projectFromId = TfsProjectHelpers.GetProjectFromId(requestContext, result2);
        string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}/_workitems?tempQueryId={2:D}", (object) requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, Guid.Empty, AccessMappingConstants.ClientAccessMappingMoniker), (object) projectFromId.Name, (object) result1);
        str1 += string.Format("<br><a href='{0}' target='_blank' rel='noopener noreferrer'>{1}</a>", (object) str2, (object) Resources.TempQueryUrlText);
      }
      message.Body = str1;
      return message;
    }

    private static void CheckUsersWITReadPermissions(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      Microsoft.VisualStudio.Services.Identity.Identity[] resolvedSenderIdentities,
      Microsoft.VisualStudio.Services.Identity.Identity[] resolvedRecipientIdentities)
    {
      IEnumerable<int> ints = workItemIds.Distinct<int>();
      WITDataSource.EnsureFieldsSnapshotInitialized(requestContext);
      IEnumerable<WorkItemFieldData> workItemFieldValues = requestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItemFieldValues(requestContext, ints, (IEnumerable<string>) new string[2]
      {
        CoreFieldReferenceNames.Id,
        CoreFieldReferenceNames.AreaId
      });
      if (ints.Count<int>() != workItemFieldValues.Count<WorkItemFieldData>() || !WITDataSource.AreAllIdentitiesAuthorizedToAccessAllWorkItems(requestContext, workItemFieldValues, resolvedSenderIdentities))
        throw new SenderWorkItemAccessDeniedException();
      if (!WITDataSource.AreAllIdentitiesAuthorizedToAccessAllWorkItems(requestContext, workItemFieldValues, resolvedRecipientIdentities))
        throw new RecipientWorkItemAccessDeniedException();
    }

    private static void EnsureFieldsSnapshotInitialized(IVssRequestContext requestContext) => requestContext.WitContext().FieldDictionary.TryGetField(-33, out FieldEntry _);

    private static bool AreAllIdentitiesAuthorizedToAccessAllWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemFieldData> workItems,
      Microsoft.VisualStudio.Services.Identity.Identity[] resolvedIdentities)
    {
      bool accessAllWorkItems = true;
      if (resolvedIdentities != null)
      {
        WorkItemTrackingTreeService service = requestContext.GetService<WorkItemTrackingTreeService>();
        HashSet<string> stringSet = new HashSet<string>();
        foreach (Microsoft.VisualStudio.Services.Identity.Identity resolvedIdentity in resolvedIdentities)
        {
          if (resolvedIdentity != null)
          {
            stringSet.Clear();
            foreach (WorkItemFieldData workItem in workItems)
            {
              Guid projectGuid = workItem.GetProjectGuid(requestContext);
              int areaId = workItem.AreaId;
              string str = string.Format("{0},{1}", (object) projectGuid, (object) areaId);
              if (!stringSet.Contains(str) && !service.HasAreaPathPermissions(requestContext, resolvedIdentity, projectGuid, areaId, 16))
              {
                accessAllWorkItems = false;
                break;
              }
              stringSet.Add(str);
            }
            if (!accessAllWorkItems)
              break;
          }
        }
      }
      return accessAllWorkItems;
    }

    private static void RecordEmailSelectedWorkItemsTelemetry(
      IVssRequestContext requestContext,
      string traceArea,
      int notesLength,
      int fieldsCount,
      int workItemsCount)
    {
      try
      {
        Dictionary<string, object> data = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
        data["NotesLength"] = (object) notesLength;
        data["FieldsCount"] = (object) fieldsCount;
        if (workItemsCount > -1)
          data["WorkitemsCount"] = (object) workItemsCount;
        EmailTelemetryUtils.TraceData(requestContext, CustomerIntelligenceArea.WebAccessWorkItemTracking, data);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(599999, traceArea, nameof (RecordEmailSelectedWorkItemsTelemetry), ex);
      }
    }

    private static string ConstructNotesText(
      IVssRequestContext requestContext,
      string body,
      bool isQuery,
      bool isSingleWorkItem = false)
    {
      string str1 = SafeHtmlWrapper.MakeSafeWithHtmlEncode(body, true);
      int num = str1 == null ? 0 : (str1.Trim().Length > 0 ? 1 : 0);
      string emailWithNote = num != 0 ? Resources.EmailWithNote : "";
      string str2 = "";
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (userIdentity != null)
      {
        string property1 = (string) userIdentity.Properties["Mail"];
        string property2 = (string) userIdentity.Properties["Account"];
        str2 = !isQuery ? (!isSingleWorkItem ? str2 + string.Format(Resources.EmailMultipleWorkItemsPrependFormat, (object) property2, (object) property1, (object) emailWithNote) : str2 + string.Format(Resources.EmailSingleWorkItemsPrependFormat, (object) property2, (object) property1, (object) emailWithNote)) : str2 + string.Format(Resources.EmailWorkItemQueryPrependFormat, (object) property2, (object) property1, (object) emailWithNote);
      }
      if (num != 0)
        str2 += string.Format(Resources.EmailNotes, (object) str1);
      return str2;
    }

    private static string ContructSelectedWorkItemsHtml(
      IVssRequestContext requestContext,
      IEnumerable<int> ids,
      IEnumerable<string> fields)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) ids, nameof (ids));
      CommonUtility.CheckEnumerableElements<int>(ids, nameof (ids), (Action<int, string>) ((id, paramName) => ArgumentUtility.CheckForOutOfRange(id, paramName, 1)));
      IEnumerable<WorkItemModel> workItems = WITDataSource.GetWorkItems(requestContext, ids.Take<int>(200));
      return new QueryResultHtmlFormatter(requestContext).GenerateHtmlForWorkItems((IEnumerable<WorkItemModel>) workItems.ToList<WorkItemModel>(), fields);
    }

    private static string ConstructQueriedWorkItemsHtml(
      IVssRequestContext requestContext,
      string traceArea,
      ProjectInfo projectInfo,
      WebApiTeam team,
      string wiql,
      IEnumerable<string> fields,
      IEnumerable<string> sortFields,
      Guid? persistenceId,
      Microsoft.VisualStudio.Services.Identity.Identity[] senderIdentities,
      Microsoft.VisualStudio.Services.Identity.Identity[] recipientIdentities)
    {
      QueryResultModel queryResultModel = BoardsQueryWITDataSource.ExecuteQuery(requestContext, traceArea, projectInfo.Id, projectInfo, team, true, wiql, fields, sortFields, new DateTime?(), new bool?(true), new bool?(), new bool?(), persistenceId, new int?(), (IEnumerable<int>) null, (IEnumerable<string>) new string[1]
      {
        CoreFieldReferenceNames.Title
      });
      WITDataSource.CheckUsersWITReadPermissions(requestContext, queryResultModel.TargetIds, senderIdentities, recipientIdentities);
      return new QueryResultHtmlFormatter(requestContext).GenerateHtmlForQueryResult(queryResultModel, persistenceId, false);
    }

    public static IEnumerable<JsObject> WorkItemColorData(IVssRequestContext requestContext)
    {
      IReadOnlyCollection<WorkItemColor> workItemTypeColors = WITDataSource.GetWorkItemTypeColors(requestContext);
      return workItemTypeColors != null ? workItemTypeColors.Select<WorkItemColor, JsObject>((Func<WorkItemColor, JsObject>) (pC => pC.ToJson())) : (IEnumerable<JsObject>) new List<JsObject>();
    }

    public static IReadOnlyCollection<WorkItemColor> GetWorkItemTypeColors(
      IVssRequestContext requestContext)
    {
      try
      {
        ProjectInfo project = requestContext.GetService<IRequestProjectService>().GetProject(requestContext);
        IReadOnlyDictionary<Guid, IReadOnlyCollection<WorkItemColor>> colorsByProjectIds = requestContext.GetService<IWorkItemMetadataFacadeService>().GetWorkItemTypeColorsByProjectIds(requestContext, (IReadOnlyCollection<Guid>) new List<Guid>()
        {
          project.Id
        });
        IReadOnlyCollection<WorkItemColor> workItemTypeColors = (IReadOnlyCollection<WorkItemColor>) null;
        Guid id = project.Id;
        ref IReadOnlyCollection<WorkItemColor> local = ref workItemTypeColors;
        if (colorsByProjectIds.TryGetValue(id, out local))
          return workItemTypeColors;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(290005, "WebAccess", TfsTraceLayers.Controller, ex);
      }
      return (IReadOnlyCollection<WorkItemColor>) null;
    }
  }
}
