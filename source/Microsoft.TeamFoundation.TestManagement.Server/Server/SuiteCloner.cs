// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.SuiteCloner
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class SuiteCloner : WITCreator
  {
    private WorkItemTypeCategory m_suiteCategory;

    public SuiteCloner(TfsTestManagementRequestContext requestContext)
      : base((TestManagementRequestContext) requestContext)
    {
    }

    public Dictionary<int, int> CloneSuitesUsingServerObjectModel(
      List<int> suiteIds,
      int sourceRootSuiteId,
      bool cloneRootSuite = false,
      bool suppressNotifications = false,
      string targetAreaPath = null)
    {
      using (PerfManager.Measure(this.TcmRequestContext.RequestContext, "CrossService", TraceUtils.GetActionName("CloneSuites", "WorkItem")))
      {
        this.TcmRequestContext.TraceEnter("BusinessLayer", "SuiteCloner.CloneSuites");
        Dictionary<int, int> dictionary = new Dictionary<int, int>();
        Validator.CheckAndGetProjectFromName(this.TcmRequestContext, this.options.DestinationProjectName);
        TeamFoundationWorkItemService service = this.RequestContext.GetService<TeamFoundationWorkItemService>();
        int num1 = 0;
        if (targetAreaPath != null)
          num1 = this.RequestContext.GetService<WebAccessWorkItemService>().GetAreaId(this.RequestContext, targetAreaPath);
        for (int count = 0; count < suiteIds.Count; count += 200)
        {
          List<int> list1 = suiteIds.Skip<int>(count).Take<int>(200).ToList<int>();
          List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate> workItemUpdateList = new List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate>();
          List<Tuple<TCMWorkItemBase, WorkItemUpdateData>> tupleList = new List<Tuple<TCMWorkItemBase, WorkItemUpdateData>>();
          List<int> intList = new List<int>();
          foreach (int num2 in list1)
          {
            this.TcmRequestContext.TraceVerbose("BusinessLayer", "SuiteCloner.CloneSuites: Creating updatedata for suiteid:{0}", (object) num2);
            Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItemById;
            try
            {
              workItemById = service.GetWorkItemById(this.RequestContext, num2, true, true, false, WorkItemRetrievalMode.NonDeleted, false, false, new Guid?(), false, new DateTime?());
            }
            catch (WorkItemNotFoundException ex)
            {
              this.TcmRequestContext.TraceWarning("BusinessLayer", "SuiteCloner.CloneSuites: Ignorning deleted suiteId:{0}", (object) num2);
              continue;
            }
            int temporaryId = -num2;
            IDictionary<string, object> customizedFieldUpdates = this.GetCustomizedFieldUpdates(workItemById, num2);
            if (num1 > 0)
              customizedFieldUpdates["System.AreaId"] = (object) num1;
            List<WorkItemLinkUpdate> customizedLinkUpdates = this.GetCustomizedLinkUpdates(workItemById, num2, temporaryId);
            List<WorkItemResourceLinkUpdate> list2 = workItemById.ResourceLinks.Select<WorkItemResourceLinkInfo, WorkItemResourceLinkUpdate>((Func<WorkItemResourceLinkInfo, WorkItemResourceLinkUpdate>) (r => WITCreator.CreateResourceLinkUpdate(r, temporaryId))).ToList<WorkItemResourceLinkUpdate>();
            if (cloneRootSuite && num2 == sourceRootSuiteId)
              this.OverrideRootSuiteProperties(num2, customizedFieldUpdates);
            workItemUpdateList.Add(new Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate()
            {
              Id = temporaryId,
              Fields = (IEnumerable<KeyValuePair<string, object>>) customizedFieldUpdates,
              LinkUpdates = (IEnumerable<WorkItemLinkUpdate>) customizedLinkUpdates,
              ResourceLinkUpdates = (IEnumerable<WorkItemResourceLinkUpdate>) list2
            });
            intList.Add(num2);
          }
          List<WorkItemUpdateResult> list3 = service.UpdateWorkItems(this.TcmRequestContext.RequestContext, (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate>) workItemUpdateList, true, false, false, (IReadOnlyCollection<int>) null, suppressNotifications, false, false, false).ToList<WorkItemUpdateResult>();
          WorkItemUpdateResult itemUpdateResult1 = list3.Find((Predicate<WorkItemUpdateResult>) (r => r.Exception != null));
          if (itemUpdateResult1 != null)
          {
            this.TcmRequestContext.TraceError("BusinessLayer", itemUpdateResult1.Exception.Message);
            throw WitToTcmExceptionConverter.Convert((Exception) itemUpdateResult1.Exception);
          }
          foreach (int num3 in intList)
          {
            int oldId = num3;
            if (!dictionary.ContainsValue(oldId))
            {
              WorkItemUpdateResult itemUpdateResult2 = list3.Find((Predicate<WorkItemUpdateResult>) (r => r.UpdateId == -oldId));
              if (itemUpdateResult2 != null && !dictionary.ContainsKey(itemUpdateResult2.Id))
                dictionary.Add(itemUpdateResult2.Id, oldId);
            }
          }
        }
        this.TcmRequestContext.TraceLeave("BusinessLayer", "SuiteCloner.CloneSuites");
        return dictionary;
      }
    }

    public Dictionary<int, int> CloneSuites(
      List<int> suiteIds,
      int sourceRootSuiteId,
      bool cloneRootSuite = false,
      bool suppressNotifications = false)
    {
      using (PerfManager.Measure(this.TcmRequestContext.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (CloneSuites), "WorkItem")))
      {
        this.TcmRequestContext.TraceEnter("BusinessLayer", "SuiteCloner.CloneSuites");
        Dictionary<int, int> dictionary = new Dictionary<int, int>();
        Validator.CheckAndGetProjectFromName(this.TcmRequestContext, this.options.DestinationProjectName);
        IWorkItemRemotableService service = this.RequestContext.GetService<IWorkItemRemotableService>();
        for (int count = 0; count < suiteIds.Count; count += 200)
        {
          List<int> list1 = suiteIds.Skip<int>(count).Take<int>(200).ToList<int>();
          List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate> workItemUpdateList = new List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate>();
          List<Tuple<TCMWorkItemBase, WorkItemUpdateData>> tupleList = new List<Tuple<TCMWorkItemBase, WorkItemUpdateData>>();
          List<int> intList = new List<int>();
          List<WitBatchRequest> witBatchRequestList = new List<WitBatchRequest>();
          IList<CreateWitRequest> createWitRequests = (IList<CreateWitRequest>) new List<CreateWitRequest>();
          IList<JsonPatchDocument> jsonPatchDocumentList = (IList<JsonPatchDocument>) new List<JsonPatchDocument>();
          foreach (int num in list1)
          {
            this.TcmRequestContext.TraceVerbose("BusinessLayer", "SuiteCloner.CloneSuites: Creating updatedata for suiteid:{0}", (object) num);
            Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem;
            try
            {
              workItem = service.GetWorkItem(this.RequestContext, num, expand: WorkItemExpand.All);
            }
            catch (WorkItemNotFoundException ex)
            {
              this.TcmRequestContext.TraceWarning("BusinessLayer", "SuiteCloner.CloneSuites: Ignorning deleted suiteId:{0}", (object) num);
              continue;
            }
            int id = -num;
            string typeName = workItem.Fields[CoreFieldReferenceNames.WorkItemType].ToString();
            IDictionary<string, object> customizedFieldUpdates = this.GetCustomizedFieldUpdates(workItem, num);
            IList<WorkItemRelation> customizedRelations = this.GetCustomizedRelations(workItem, num);
            if (cloneRootSuite && num == sourceRootSuiteId)
              this.OverrideRootSuiteProperties(num, customizedFieldUpdates);
            JsonPatchDocument jsonPatchDocument = WorkHelper.ConvertToJsonPatchDocument(this.RequestContext, customizedFieldUpdates, (IList<WorkItemRelation>) customizedRelations.ToList<WorkItemRelation>());
            jsonPatchDocumentList.Add(jsonPatchDocument);
            createWitRequests.Add(this.CreateWorkItemBatchRequest(id, typeName, jsonPatchDocument));
            intList.Add(num);
          }
          List<WitBatchResponse> list2 = service.CreateWorkItems(this.TcmRequestContext.RequestContext, this.options.DestinationProjectName, createWitRequests, true, suppressNotifications).ToList<WitBatchResponse>();
          this.HandleException(service, (IEnumerable<WitBatchResponse>) list2);
          for (int index = 0; index < list2.Count; ++index)
          {
            Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem = JsonConvert.DeserializeObject<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>(list2[index].Body);
            dictionary.Add(workItem.Id.Value, intList[index]);
          }
        }
        this.TcmRequestContext.TraceLeave("BusinessLayer", "SuiteCloner.CloneSuites");
        return dictionary;
      }
    }

    private void HandleException(
      IWorkItemRemotableService workItemRemotableService,
      IEnumerable<WitBatchResponse> createResults)
    {
      int successCode = 200;
      WitBatchResponse witBatchResponse = createResults.ToList<WitBatchResponse>().Find((Predicate<WitBatchResponse>) (r => r.Code != successCode));
      if (witBatchResponse != null)
      {
        Exception e = JsonConvert.DeserializeObject<Exception>(witBatchResponse.Body);
        this.TcmRequestContext.TraceError("BusinessLayer", e.Message);
        throw WitToTcmExceptionConverter.Convert(e);
      }
    }

    private CreateWitRequest CreateWorkItemBatchRequest(
      int id,
      string typeName,
      JsonPatchDocument document)
    {
      if (string.IsNullOrEmpty(typeName))
        throw new ArgumentNullException(nameof (typeName));
      if (document == null)
        throw new ArgumentNullException(nameof (document));
      return new CreateWitRequest()
      {
        Id = id,
        TypeName = typeName,
        document = document
      };
    }

    public List<ServerTestSuite> CloneSuitesAndUpdateRelationships(
      List<int> suitesToClone,
      HashSet<int> suitesToUpdateRelationship,
      bool cloneRootSuite,
      int targetSuiteIdToUpdate,
      bool suppressNotifications,
      bool isEnableBulkUpdateUsingServerOM,
      string targetAreaPath,
      out UpdatedProperties targetSuiteProp)
    {
      Dictionary<int, int> source1 = new Dictionary<int, int>();
      Dictionary<int, int> source2 = new Dictionary<int, int>();
      if (suitesToClone != null && suitesToClone.Count > 0)
      {
        source2 = !isEnableBulkUpdateUsingServerOM ? this.CloneSuites(suitesToClone, suitesToUpdateRelationship.First<int>(), cloneRootSuite, suppressNotifications) : this.CloneSuitesUsingServerObjectModel(suitesToClone, suitesToUpdateRelationship.First<int>(), cloneRootSuite, suppressNotifications, targetAreaPath);
        source1 = source2.ToDictionary<KeyValuePair<int, int>, int, int>((Func<KeyValuePair<int, int>, int>) (m => m.Value), (Func<KeyValuePair<int, int>, int>) (m => m.Key));
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(this.RequestContext))
          planningDatabase.UpdateCloneRelationship(this.opId, source1.Where<KeyValuePair<int, int>>((Func<KeyValuePair<int, int>, bool>) (m => suitesToUpdateRelationship.Contains(m.Key))).ToDictionary<KeyValuePair<int, int>, int, int>((Func<KeyValuePair<int, int>, int>) (m => m.Key), (Func<KeyValuePair<int, int>, int>) (m => m.Value)), CloneItemType.Suites);
      }
      List<ServerTestSuite> serverTestSuiteList = TestSuiteWorkItem.FetchSuites(this.TcmRequestContext, this.options.DestinationProjectName, source1.Select<KeyValuePair<int, int>, int>((Func<KeyValuePair<int, int>, int>) (m => m.Value)).ToList<int>());
      foreach (ServerTestSuite serverTestSuite in serverTestSuiteList)
        serverTestSuite.SourceSuiteId = source2[serverTestSuite.Id];
      this.UpdateTargetSuite(targetSuiteIdToUpdate, suppressNotifications, out targetSuiteProp);
      return serverTestSuiteList;
    }

    protected override void PostInitialize(CloneOperationInformation options)
    {
      List<WorkItemTypeCategory> itemTypeCategories = this.RequestContext.GetService<IWitHelper>().GetWorkItemTypeCategories(this.RequestContext, this.options.DestinationProjectName, (IEnumerable<string>) new List<string>()
      {
        WitCategoryRefName.TestSuite
      });
      if (itemTypeCategories == null || itemTypeCategories.Count <= 0)
        return;
      this.m_suiteCategory = itemTypeCategories[0];
    }

    private void UpdateTargetSuite(
      int targetSuiteIdToUpdate,
      bool suppressNotifications,
      out UpdatedProperties targetSuiteProp)
    {
      ServerTestSuite suiteFromSuiteId = ServerTestSuite.GetSuiteFromSuiteId(this.TcmRequestContext, targetSuiteIdToUpdate, this.options.DestinationProjectName);
      targetSuiteProp = this.UpdateTargetSuiteRevision(suiteFromSuiteId, suppressNotifications);
    }

    private UpdatedProperties UpdateTargetSuiteRevision(
      ServerTestSuite targetSuiteToUpdate,
      bool suppressNotifications)
    {
      targetSuiteToUpdate.LastUpdatedBy = this.options.TeamFoundationUserId;
      targetSuiteToUpdate.LastUpdatedByName = this.options.TeamFoundationUserName;
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(this.TcmRequestContext, this.options.DestinationProjectName);
      string audit = SuiteAuditHelper.ConstructSuiteAuditForClone(this.opId);
      return SuiteAuditHelper.UpdateSuiteAudit(this.TcmRequestContext, this.options.DestinationProjectName, projectFromName, new IdAndRev(targetSuiteToUpdate.Id, targetSuiteToUpdate.Revision), audit, true, suppressNotifications);
    }

    private void FixQueryForQueryBasedSuite(IDictionary<string, object> oldTCFields)
    {
      using (PerfManager.Measure(this.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (FixQueryForQueryBasedSuite), "WorkItem")))
      {
        WorkItemField2 field = this.RequestContext.GetService<IWorkItemFieldsRemotableService>().GetField(this.RequestContext, TCMWitFields.QueryText, (string) null);
        if (oldTCFields == null || !oldTCFields.ContainsKey(field.ReferenceName))
          return;
        string oldTcField = oldTCFields[field.ReferenceName] as string;
        if (string.IsNullOrEmpty(oldTcField))
          return;
        oldTCFields[field.ReferenceName] = (object) WiqlTransformUtils.TransformNamesToIds(this.RequestContext, oldTcField, true);
      }
    }

    private IDictionary<string, object> GetCustomizedFieldUpdates(Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem wi, int oldId)
    {
      IDictionary<string, object> cloneableFieldData = (IDictionary<string, object>) this.GetCloneableFieldData(wi);
      this.FixQueryForQueryBasedSuite(cloneableFieldData);
      this.CustomizeFields(cloneableFieldData, oldId);
      return (IDictionary<string, object>) cloneableFieldData.ToDictionary<KeyValuePair<string, object>, string, object>((Func<KeyValuePair<string, object>, string>) (kvp => kvp.Key.ToString((IFormatProvider) CultureInfo.CurrentCulture)), (Func<KeyValuePair<string, object>, object>) (kvp => kvp.Value));
    }

    private IDictionary<string, object> GetCustomizedFieldUpdates(Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem wi, int oldId)
    {
      IDictionary<string, object> cloneableFieldData = (IDictionary<string, object>) this.GetCloneableFieldData(wi);
      this.FixQueryForQueryBasedSuite(cloneableFieldData);
      this.CustomizeFields(cloneableFieldData, oldId);
      return (IDictionary<string, object>) cloneableFieldData.ToDictionary<KeyValuePair<string, object>, string, object>((Func<KeyValuePair<string, object>, string>) (kvp => kvp.Key.ToString((IFormatProvider) CultureInfo.CurrentCulture)), (Func<KeyValuePair<string, object>, object>) (kvp => kvp.Value));
    }

    private void CustomizeFields(IDictionary<string, object> oldTCFields, int oldId)
    {
      this.TcmRequestContext.TraceEnter("BusinessLayer", "SuiteCloner.CustomizeFields");
      this.CustomizeExplicitlyOverriddenFields(oldTCFields, true);
      this.CustomizeWorkitemType(oldTCFields);
      this.UpdateSpecialFields(oldId, oldTCFields);
      this.UpdateIdentityFields(oldTCFields);
      this.FilterOutFieldsNotToCloneForSuite(oldTCFields);
      this.TcmRequestContext.TraceLeave("BusinessLayer", "SuiteCloner.CustomizeFields");
    }

    private void FilterOutFieldsNotToCloneForSuite(IDictionary<string, object> oldTCFields)
    {
      if (!oldTCFields.ContainsKey(TCMWitFields.SuiteAudit))
        return;
      oldTCFields.Remove(TCMWitFields.SuiteAudit);
    }

    private void CustomizeWorkitemType(IDictionary<string, object> suiteFields)
    {
      this.TcmRequestContext.TraceEnter("BusinessLayer", "SuiteCloner.CustomizeWorkitemType");
      string oldtype = Convert.ToString(suiteFields[WorkItemFieldRefNames.WorkItemType]);
      if (!this.m_suiteCategory.WorkItemTypes.Any<WorkItemTypeReference>((Func<WorkItemTypeReference, bool>) (type => string.Equals(type.Name, oldtype, StringComparison.OrdinalIgnoreCase))))
      {
        this.TcmRequestContext.TraceWarning("BusinessLayer", "SuiteCloner.CustomizeWorkitemType: {0} workItemType does not exist in suite category", (object) oldtype);
        suiteFields[WorkItemFieldRefNames.WorkItemType] = (object) this.m_suiteCategory.DefaultWorkItemType?.Name;
      }
      this.TcmRequestContext.TraceLeave("BusinessLayer", "SuiteCloner.CustomizeWorkitemType");
    }

    private IList<WorkItemRelation> GetCustomizedRelations(Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem wi, int oldID)
    {
      List<WorkItemRelation> customizedRelations = new List<WorkItemRelation>();
      if (wi.Relations == null)
        return (IList<WorkItemRelation>) customizedRelations;
      foreach (WorkItemRelation relation in (IEnumerable<WorkItemRelation>) wi.Relations)
      {
        if (relation.Rel.Equals("System.LinkTypes.Hierarchy-Forward", StringComparison.InvariantCultureIgnoreCase))
        {
          this.TcmRequestContext.TraceVerbose("BusinessLayer", "SuiteCloner.CustomizeLinks: Ignorning child link for suiteId:{0}", (object) oldID);
        }
        else
        {
          string str;
          if (string.CompareOrdinal(relation.Rel, "System.LinkTypes.Related") != 0 || relation.Attributes == null || !relation.Attributes.TryGetValue<string, string>("comment", out str) || !str.Contains("TF237027"))
          {
            WorkItemRelation workItemRelation = new WorkItemRelation();
            workItemRelation.Rel = relation.Rel;
            workItemRelation.Url = relation.Url;
            workItemRelation.Title = relation.Title;
            workItemRelation.Attributes = (IDictionary<string, object>) new Dictionary<string, object>();
            if (relation.Attributes != null)
            {
              foreach (KeyValuePair<string, object> attribute in (IEnumerable<KeyValuePair<string, object>>) relation.Attributes)
                relation.Attributes.Add(attribute.Key, attribute.Value);
            }
            customizedRelations.Add(relation);
          }
        }
      }
      return (IList<WorkItemRelation>) customizedRelations;
    }

    private List<WorkItemLinkUpdate> GetCustomizedLinkUpdates(Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem wi, int oldID, int tempId)
    {
      List<WorkItemLinkUpdate> links = new List<WorkItemLinkUpdate>();
      foreach (WorkItemLinkInfo workItemLink in wi.WorkItemLinks)
      {
        if (workItemLink.LinkType == this.linkDictionary.GetLinkTypeByReferenceName(this.RequestContext, "System.LinkTypes.Hierarchy").ForwardId)
          this.TcmRequestContext.TraceVerbose("BusinessLayer", "SuiteCloner.CustomizeLinks: Ignorning child link for suiteId:{0}", (object) oldID);
        else if (workItemLink.LinkType != 1 || string.IsNullOrEmpty(workItemLink.Comment) || !workItemLink.Comment.Contains("TF237027"))
          links.Add(WITCreator.CreateLinkUpdate(workItemLink, tempId));
      }
      this.AddRelatedLink(oldID, links, tempId);
      return links;
    }

    private void OverrideRootSuiteProperties(
      int oldSuiteId,
      IDictionary<string, object> suiteFields)
    {
      suiteFields["System.Title"] = (object) this.options.SourcePlanName;
    }
  }
}
