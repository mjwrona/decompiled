// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanWorkItem
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestPlanWorkItem : TCMWorkItemBase
  {
    private readonly string[] planWitFields = new string[15]
    {
      "System.Id",
      "System.Rev",
      "System.Title",
      "System.WorkItemType",
      "System.CreatedBy",
      "System.ChangedBy",
      "System.ChangedDate",
      "System.AreaPath",
      "System.AssignedTo",
      "System.IterationPath",
      "System.State",
      "System.TeamProject",
      "System.Description",
      TCMWitFields.StartDate,
      TCMWitFields.EndDate
    };

    internal TestPlanWorkItem()
    {
      this.CategoryRefName = WitCategoryRefName.TestPlan;
      this.Id = -1;
      this.TCMObjectType = ObjectTypes.TestPlan;
    }

    protected override StateTypeEnum ToMetaState(byte tcmState)
    {
      switch ((TestPlanState) tcmState)
      {
        case TestPlanState.Active:
          return StateTypeEnum.InProgress;
        case TestPlanState.Inactive:
          return StateTypeEnum.Complete;
        default:
          return StateTypeEnum.InProgress;
      }
    }

    internal override byte FromMetaState(StateTypeEnum metaState)
    {
      if (metaState == StateTypeEnum.InProgress)
        return 1;
      return metaState == StateTypeEnum.Complete ? (byte) 2 : (byte) 0;
    }

    internal override List<StateTypeEnumAndStateString> GetDefaultStatesMap() => new List<StateTypeEnumAndStateString>()
    {
      new StateTypeEnumAndStateString((byte) 2, ServerResources.TestPlanActiveWorkItemState),
      new StateTypeEnumAndStateString((byte) 3, ServerResources.TestPlanInactiveWorkItemState)
    };

    protected override StateTypeEnum GetMetaStatesFromDefaultMap(
      TestManagementRequestContext context,
      string workItemState)
    {
      if (string.Equals(workItemState, ServerResources.TestPlanActiveWorkItemState, StringComparison.CurrentCultureIgnoreCase))
        return StateTypeEnum.InProgress;
      if (string.Equals(workItemState, ServerResources.TestPlanInactiveWorkItemState, StringComparison.CurrentCultureIgnoreCase))
        return StateTypeEnum.Complete;
      throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.CouldNotFindAValidPlanStateMapping, (object) workItemState));
    }

    protected override StateTypeEnum GetMetaStatesFromProcessConfiguration(
      TestManagementRequestContext context,
      TestManagementProcessConfig projProcConfig,
      string workItemState)
    {
      int processConfiguration = (int) base.GetMetaStatesFromProcessConfiguration(context, projProcConfig, workItemState);
      return processConfiguration != 0 ? (StateTypeEnum) processConfiguration : throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.CouldNotFindAValidPlanStateMapping, (object) workItemState));
    }

    internal override string GetDefaultWorkItemState(
      TestManagementRequestContext context,
      byte tcmState)
    {
      string defaultWorkItemState = (string) null;
      switch (tcmState)
      {
        case 1:
          defaultWorkItemState = ServerResources.TestPlanActiveWorkItemState;
          break;
        case 2:
          defaultWorkItemState = ServerResources.TestPlanInactiveWorkItemState;
          break;
      }
      return defaultWorkItemState;
    }

    internal override byte GetDefaultTcmState(
      TestManagementRequestContext context,
      string workItemState)
    {
      if (string.Equals(workItemState, ServerResources.TestPlanActiveWorkItemState, StringComparison.CurrentCultureIgnoreCase))
        return 1;
      return string.Equals(workItemState, ServerResources.TestPlanInactiveWorkItemState, StringComparison.CurrentCultureIgnoreCase) ? (byte) 2 : (byte) 0;
    }

    internal override IList<string> GetDefaultWorkItemStates(TestManagementRequestContext context) => (IList<string>) new List<string>()
    {
      ServerResources.TestPlanActiveWorkItemState,
      ServerResources.TestPlanInactiveWorkItemState
    };

    protected override Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State[] GetMappedWitStates(
      TestManagementRequestContext context,
      TestManagementProcessConfig projProcConfig)
    {
      context.TraceEnter("BusinessLayer", "TestPlanWorkItem.GetMappedWitStates");
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State[] mappedWitStates = Array.Empty<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State>();
      if (projProcConfig.TestPlanStates != null)
        mappedWitStates = projProcConfig.TestPlanStates;
      context.TraceLeave("BusinessLayer", "TestPlanWorkItem.GetMappedWitStates");
      return mappedWitStates;
    }

    internal override byte GetTCMState(
      IReadOnlyCollection<WorkItemStateDefinition> possibleStates,
      string workItemState)
    {
      if (possibleStates != null)
      {
        switch (possibleStates.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (possibleState => possibleState.Name == workItemState)).First<WorkItemStateDefinition>().StateCategory)
        {
          case WorkItemStateCategory.InProgress:
            return 1;
          case WorkItemStateCategory.Completed:
            return 2;
        }
      }
      return 0;
    }

    protected override void CreateWorkItem(
      TestManagementRequestContext context,
      string teamProjectName,
      GuidAndString projectId,
      IList<TestExternalLink> externalLinks,
      IList<WorkItemLinkInfo> witLinks,
      bool byPass = false)
    {
      if (this.TCMPlan.State == (byte) 2)
        this.CreateTestPlanInInactiveState(context, teamProjectName, projectId, externalLinks);
      else
        base.CreateWorkItem(context, teamProjectName, projectId, externalLinks, witLinks, byPass);
      this.CreateRootSuite(context, teamProjectName, projectId);
    }

    private void CreateTestPlanInInactiveState(
      TestManagementRequestContext context,
      string teamProjectName,
      GuidAndString projectId,
      IList<TestExternalLink> links)
    {
      ProcessConfigurationHelper.ValidateProcessConfiguration(context, projectId.String, teamProjectName, this.WitTypeName, this.CategoryRefName, true);
      string str = this.TCMPlan.Status;
      this.State = this.GetStartState(context, teamProjectName);
      this.CreateNewOrUpdateExistingWorkItem(context, teamProjectName, projectId, true, links, (IList<WorkItemLinkInfo>) null, false, false, WitOperationType.WitFieldUpdate);
      if (string.IsNullOrWhiteSpace(str))
        str = this.ToWorkItemState(context, projectId.String, this.TCMPlan.State, true);
      this.State = str;
      try
      {
        this.CreateNewOrUpdateExistingWorkItem(context, teamProjectName, projectId, false, (IList<TestExternalLink>) null, (IList<WorkItemLinkInfo>) null, true, false, WitOperationType.WitFieldUpdate);
      }
      catch (Exception ex)
      {
        context.TraceVerbose("BusinessLayer", "TestPlan could not be moved to state:" + this.TCMPlan.Status);
      }
    }

    protected override void PopulateWitFields(
      TestManagementRequestContext context,
      WitOperationType witOperationType,
      bool byPass)
    {
      if (byPass && this.TCMPlan != null)
      {
        this.OwnerName = this.TCMPlan.OwnerName;
        this.OwnerDistinctName = IdentityHelper.ResolveIdentityToName(context, this.TCMPlan.Owner, true);
        this.CreatedByName = this.TCMPlan.CreatedByName;
        this.CreatedByDistinctName = this.TCMPlan.CreatedByDistinctName;
        this.LastUpdatedBy = this.TCMPlan.LastUpdatedBy;
        this.LastUpdatedByName = this.TCMPlan.LastUpdatedByName;
        this.LastUpdatedByDistinctName = IdentityHelper.ResolveIdentityToName(context, this.TCMPlan.LastUpdatedBy, true);
      }
      else
        base.PopulateWitFields(context, witOperationType, byPass);
      if (witOperationType == WitOperationType.TcmFieldUpdate)
        return;
      this.Title = this.TCMPlan.Name;
      this.Description = this.TCMPlan.Description;
      this.AreaPath = this.TCMPlan.AreaPath;
      this.Iteration = this.TCMPlan.Iteration;
      this.StartDate = this.TCMPlan.StartDate;
      this.EndDate = this.TCMPlan.EndDate;
      this.OwnerName = IdentityHelper.ResolveIdentityToName(context, this.TCMPlan.Owner);
      this.OwnerDistinctName = IdentityHelper.ResolveIdentityToName(context, this.TCMPlan.Owner, true);
      this.TeamFieldName = this.TCMPlan.TeamFieldName;
      this.TeamFieldDefaultValue = this.TCMPlan.TeamFieldDefaultValue;
    }

    protected override void OnWitUpdateComplete(
      TestManagementRequestContext context,
      int id,
      int revision,
      DateTime lastUpdated)
    {
      base.OnWitUpdateComplete(context, id, revision, lastUpdated);
      this.TCMPlan.PlanId = this.Id;
      this.TCMPlan.Revision = this.Revision;
      this.TCMPlan.LastUpdated = this.LastUpdated;
      this.TCMPlan.OwnerName = this.OwnerName;
      this.TCMPlan.CreatedByName = this.CreatedByName;
      this.TCMPlan.CreatedByDistinctName = this.CreatedByDistinctName;
      this.TCMPlan.LastUpdatedByName = this.LastUpdatedByName;
      this.TCMPlan.Status = this.State;
    }

    protected override Dictionary<string, object> CreateWitFields(
      TestManagementRequestContext context,
      string teamprojectName,
      GuidAndString targetProject,
      bool witStateChanged,
      bool isNew,
      WitOperationType witOperationType,
      bool isUpgrade = false,
      bool isSuiteRenameScenario = false)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "TestPlanWorkItem.CreateWitFields");
        WITCreator witCreator = new WITCreator(context);
        Dictionary<string, object> witFields = base.CreateWitFields(context, teamprojectName, targetProject, witStateChanged, isNew, witOperationType, isUpgrade);
        if (witOperationType == WitOperationType.WitFieldUpdate)
        {
          this.CheckValueAndAddWitField(witCreator, context, witFields, "System.AssignedTo", this.OwnerDistinctName, targetProject);
          if (this.StartDate != new DateTime())
            witFields[TCMWitFields.StartDate] = (object) this.StartDate;
          if (this.EndDate != new DateTime())
            witFields[TCMWitFields.EndDate] = (object) this.EndDate;
          if (!string.IsNullOrEmpty(this.TeamFieldName) && !string.IsNullOrEmpty(this.TeamFieldDefaultValue))
            this.CheckValueAndAddWitField(witCreator, context, witFields, this.TeamFieldName, this.TeamFieldDefaultValue, targetProject);
        }
        return witFields;
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestPlanWorkItem.CreateWitFields");
      }
    }

    protected override void FromWorkItem(
      TestManagementRequestContext context,
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItemFieldData)
    {
      DateTime valueFromPayload1 = this.ExtractFieldValueFromPayload<DateTime>(context, TCMWitFields.StartDate, workItemFieldData);
      if (valueFromPayload1 != new DateTime())
        this.StartDate = valueFromPayload1;
      DateTime valueFromPayload2 = this.ExtractFieldValueFromPayload<DateTime>(context, TCMWitFields.EndDate, workItemFieldData);
      if (valueFromPayload2 != new DateTime())
        this.EndDate = valueFromPayload2;
      base.FromWorkItem(context, workItemFieldData);
    }

    protected override void PopulateWitId(
      TestManagementRequestContext context,
      int id,
      int revision)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "TCMWorkItemBase.PopulateWitId");
        this.Id = id;
        this.Revision = revision;
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TCMWorkItemBase.PopulateWitId");
      }
    }

    protected override void PopulateDataAndUpdatePackage(
      WorkItemUpdateContext witUpdateContext,
      WorkItemUpdateData witUpdateData,
      MigrationLogger logger,
      out Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate updateData)
    {
      this.WitTypeName = witUpdateData.WitTypeName;
      this.TCMPlan.Status = witUpdateData.WitCreationState;
      if (witUpdateData.ExternalLinks == null || witUpdateData.ExternalLinks.Count == 0)
        witUpdateData.ExternalLinks = (IList<TestExternalLink>) this.TCMPlan.Links;
      base.PopulateDataAndUpdatePackage(witUpdateContext, witUpdateData, logger, out updateData);
    }

    protected override void ValidateStateTransition(
      TestManagementRequestContext context,
      string teamProjectName,
      string fromState,
      out bool witStateChanged)
    {
      context.TraceEnter("BusinessLayer", "TestPlanWorkItem.ValidateStateTransition");
      base.ValidateStateTransition(context, teamProjectName, fromState, out witStateChanged);
      if (!string.Equals(fromState, this.State, StringComparison.CurrentCultureIgnoreCase) && !witStateChanged)
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.NoValidStateTransitionExistsForPlan, (object) fromState, (object) this.State));
      context.TraceLeave("BusinessLayer", "TestPlanWorkItem.ValidateStateTransition");
    }

    protected override void PopulateFieldsBeforeUpdate(
      TestManagementRequestContext context,
      string teamProjectName,
      ref CoreWorkItemUpdateFields existingWorkItemFieldValues,
      WitOperationType witOperationType,
      bool byPass)
    {
      base.PopulateFieldsBeforeUpdate(context, teamProjectName, ref existingWorkItemFieldValues, witOperationType, byPass);
      if (witOperationType == WitOperationType.TcmFieldUpdate)
        return;
      this.State = TCMWorkItemBase.GetFirstNonEmptyValue(this.TCMPlan.Status, existingWorkItemFieldValues.State);
    }

    internal void CreateRootSuite(
      TestManagementRequestContext context,
      string teamProjectName,
      GuidAndString projectId)
    {
      TestSuiteWorkItem testSuiteWorkItem = new TestSuiteWorkItem();
      testSuiteWorkItem.ParentId = this.TCMPlan.PlanId;
      testSuiteWorkItem.ParentCategory = WitCategoryRefName.TestPlan;
      testSuiteWorkItem.ParentName = this.TCMPlan.Name;
      testSuiteWorkItem.AreaPath = this.TCMPlan.AreaPath;
      testSuiteWorkItem.Iteration = this.TCMPlan.Iteration;
      testSuiteWorkItem.Create(context, teamProjectName, projectId, (IList<TestExternalLink>) null, (IList<int>) null);
      this.TCMPlan.RootSuiteStatus = testSuiteWorkItem.State;
      this.TCMPlan.RootSuiteId = testSuiteWorkItem.Id;
    }

    protected override void CreateWitLinks(
      TestManagementRequestContext context,
      IList<TestExternalLink> externalLinks,
      IList<WorkItemLinkInfo> witLinks,
      bool isNew,
      MigrationLogger logger,
      out List<WorkItemLinkUpdate> linkUpdateData,
      out List<WorkItemResourceLinkUpdate> resourceLinkUpdateData)
    {
      context.TraceEnter("BusinessLayer", "TestPlanWorkItem.CreateWitLinks");
      try
      {
        base.CreateWitLinks(context, externalLinks, witLinks, isNew, logger, out linkUpdateData, out resourceLinkUpdateData);
        if (isNew)
        {
          if (externalLinks == null)
            return;
          resourceLinkUpdateData.AddRange((IEnumerable<WorkItemResourceLinkUpdate>) this.CreateExternalLinks(context, externalLinks.ToList<TestExternalLink>()));
        }
        else
          resourceLinkUpdateData.AddRange((IEnumerable<WorkItemResourceLinkUpdate>) this.CategorizeExternalLinks(context, this.Id, externalLinks));
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestPlanWorkItem.CreateWitLinks");
      }
    }

    internal List<WorkItemResourceLinkUpdate> CategorizeExternalLinks(
      TestManagementRequestContext context,
      int workItemId,
      IList<TestExternalLink> externalLinks)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "TestPlanWorkItem.CategorizeExternalLinks");
        List<WorkItemResourceLinkUpdate> resourceLinkUpdateList1 = new List<WorkItemResourceLinkUpdate>();
        if (externalLinks != null && externalLinks.Count > 0)
        {
          TfsTestManagementRequestContext request = context as TfsTestManagementRequestContext;
          List<TestExternalLink> testExternalLinkList = context.RequestContext.GetService<IWitHelper>().QueryHyperLinks(context, workItemId);
          foreach (TestExternalLink externalLink in (IEnumerable<TestExternalLink>) externalLinks)
          {
            TestExternalLink changedLink = externalLink;
            TestExternalLink testExternalLink1 = testExternalLinkList.Find((Predicate<TestExternalLink>) (l => l.LinkId == changedLink.LinkId));
            if (testExternalLink1 == null)
            {
              if (!string.IsNullOrEmpty(changedLink.Uri))
              {
                TestExternalLink testExternalLink2 = testExternalLinkList.Find((Predicate<TestExternalLink>) (l => string.Equals(l.Uri, changedLink.Uri, StringComparison.OrdinalIgnoreCase)));
                if (testExternalLink2 != null)
                {
                  context.TraceError("BusinessLayer", ServerResources.LinkAlreadyExistsErrorMessage, (object) testExternalLink2.Uri);
                  throw new TestManagementValidationException(string.Format(ServerResources.LinkAlreadyExistsErrorMessage, (object) testExternalLink2.Uri));
                }
                context.TraceVerbose("BusinessLayer", "TestPlanWorkItem.CategorizeExternalLinks Adding link:{0}", (object) changedLink.ToString());
                FileLinkInfo hyperLink = TestExternalLink.ToHyperLink(request, changedLink);
                List<WorkItemResourceLinkUpdate> resourceLinkUpdateList2 = resourceLinkUpdateList1;
                WorkItemResourceLinkUpdate resourceLinkUpdate = new WorkItemResourceLinkUpdate();
                resourceLinkUpdate.Comment = hyperLink.Comment;
                resourceLinkUpdate.Location = hyperLink.Path;
                resourceLinkUpdate.Type = new ResourceLinkType?(ResourceLinkType.Hyperlink);
                resourceLinkUpdate.UpdateType = LinkUpdateType.Add;
                resourceLinkUpdateList2.Add(resourceLinkUpdate);
              }
              else
                context.TraceError("BusinessLayer", "TestPlanWorkItem.CategorizeExternalLinks link uri is null");
            }
            else
            {
              FileLinkInfo hyperLink = TestExternalLink.ToHyperLink(request, changedLink);
              if (string.IsNullOrEmpty(changedLink.Uri))
              {
                context.TraceVerbose("BusinessLayer", "TestPlanWorkItem.CategorizeExternalLinks Deleting link:{0}", (object) changedLink.ToString());
                List<WorkItemResourceLinkUpdate> resourceLinkUpdateList3 = resourceLinkUpdateList1;
                WorkItemResourceLinkUpdate resourceLinkUpdate = new WorkItemResourceLinkUpdate();
                resourceLinkUpdate.Comment = hyperLink.Comment;
                resourceLinkUpdate.Location = hyperLink.Path;
                resourceLinkUpdate.ResourceId = new int?(hyperLink.FieldId);
                resourceLinkUpdate.Type = new ResourceLinkType?(ResourceLinkType.Hyperlink);
                resourceLinkUpdate.UpdateType = LinkUpdateType.Delete;
                resourceLinkUpdateList3.Add(resourceLinkUpdate);
              }
              else
              {
                if (!string.Equals(changedLink.Uri, testExternalLink1.Uri, StringComparison.OrdinalIgnoreCase))
                {
                  context.TraceError("BusinessLayer", ServerResources.DuplicateLinkErrorMessage);
                  throw new TestManagementValidationException(ServerResources.DuplicateLinkErrorMessage);
                }
                context.TraceVerbose("BusinessLayer", "TestPlanWorkItem.CategorizeExternalLinks Updating link:{0}", (object) changedLink.ToString());
                List<WorkItemResourceLinkUpdate> resourceLinkUpdateList4 = resourceLinkUpdateList1;
                WorkItemResourceLinkUpdate resourceLinkUpdate = new WorkItemResourceLinkUpdate();
                resourceLinkUpdate.Comment = hyperLink.Comment;
                resourceLinkUpdate.Location = hyperLink.Path;
                resourceLinkUpdate.ResourceId = new int?(hyperLink.FieldId);
                resourceLinkUpdate.Type = new ResourceLinkType?(ResourceLinkType.Hyperlink);
                resourceLinkUpdate.UpdateType = LinkUpdateType.Update;
                resourceLinkUpdateList4.Add(resourceLinkUpdate);
              }
            }
          }
        }
        return resourceLinkUpdateList1;
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestPlanWorkItem.CategorizeExternalLinks");
      }
    }

    internal List<WorkItemResourceLinkUpdate> CreateExternalLinks(
      TestManagementRequestContext context,
      List<TestExternalLink> hyperLinks)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "WITHelper.CreateExternalLinks");
        List<WorkItemResourceLinkUpdate> externalLinks = new List<WorkItemResourceLinkUpdate>();
        // ISSUE: explicit non-virtual call
        if (hyperLinks != null && __nonvirtual (hyperLinks.Count) > 0)
        {
          TfsTestManagementRequestContext request = context as TfsTestManagementRequestContext;
          foreach (TestExternalLink hyperLink1 in hyperLinks)
          {
            TestExternalLink hyperLink = hyperLink1;
            if (!string.IsNullOrEmpty(hyperLink.Uri) && externalLinks.Find((Predicate<WorkItemResourceLinkUpdate>) (l =>
            {
              int? resourceId = l.ResourceId;
              int linkId = hyperLink.LinkId;
              return resourceId.GetValueOrDefault() == linkId & resourceId.HasValue;
            })) == null)
            {
              FileLinkInfo hyperLink2 = TestExternalLink.ToHyperLink(request, hyperLink);
              List<WorkItemResourceLinkUpdate> resourceLinkUpdateList = externalLinks;
              WorkItemResourceLinkUpdate resourceLinkUpdate = new WorkItemResourceLinkUpdate();
              resourceLinkUpdate.Comment = hyperLink2.Comment;
              resourceLinkUpdate.Location = hyperLink2.Path;
              resourceLinkUpdate.Type = new ResourceLinkType?(ResourceLinkType.Hyperlink);
              resourceLinkUpdate.UpdateType = LinkUpdateType.Add;
              resourceLinkUpdateList.Add(resourceLinkUpdate);
            }
          }
        }
        return externalLinks;
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "WITHelper.CreateExternalLinks");
      }
    }

    internal static List<TestPlan> FetchPlans(
      TestManagementRequestContext context,
      string projectName,
      List<int> ids,
      bool includeDetails = true)
    {
      try
      {
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestPlanWorkItem.FetchPlans"))
        {
          context.TraceEnter("BusinessLayer", "TestPlanWorkItem.FetchPlans");
          List<TestPlan> testPlans = new List<TestPlan>();
          List<TCMWorkItemBase> workItems = TCMWorkItemBase.GetWorkItems(context, projectName, ids, WitCategoryRefName.TestPlan);
          if (workItems != null && workItems.Count > 0)
          {
            Dictionary<string, Guid> identitiesMap = new Dictionary<string, Guid>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            TestPlanWorkItem.PopulatePlanProperties(context, projectName, testPlans, identitiesMap, workItems);
            if (includeDetails)
              TestPlanWorkItem.ResolveAndPopulateIdentityFields(context, testPlans, identitiesMap);
          }
          return testPlans;
        }
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestPlanWorkItem.FetchPlans");
      }
    }

    private static void PopulatePlanProperties(
      TestManagementRequestContext context,
      string projectName,
      List<TestPlan> testPlans,
      Dictionary<string, Guid> identitiesMap,
      List<TCMWorkItemBase> tcmWits)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      foreach (TCMWorkItemBase tcmWit in tcmWits)
      {
        TestPlan testPlan = TestPlan.FromWorkItem(context, projectFromName, (TestPlanWorkItem) tcmWit);
        if (testPlan != null)
        {
          if (!string.IsNullOrEmpty(testPlan.OwnerName))
            identitiesMap[testPlan.OwnerName] = Guid.Empty;
          if (!string.IsNullOrEmpty(testPlan.LastUpdatedByName))
            identitiesMap[testPlan.LastUpdatedByName] = Guid.Empty;
          testPlans.Add(testPlan);
        }
      }
    }

    private static void ResolveAndPopulateIdentityFields(
      TestManagementRequestContext context,
      List<TestPlan> testPlans,
      Dictionary<string, Guid> identitiesMap)
    {
      TCMWorkItemBase.ResolveDisplayNames(context, identitiesMap);
      TestPlanWorkItem.PopulateIdentityFields(context, testPlans, identitiesMap);
    }

    private static void PopulateIdentityFields(
      TestManagementRequestContext context,
      List<TestPlan> testPlans,
      Dictionary<string, Guid> identitiesMap)
    {
      foreach (TestPlan testPlan in testPlans)
      {
        testPlan.Owner = !string.IsNullOrEmpty(testPlan.OwnerName) ? identitiesMap[testPlan.OwnerName] : Guid.Empty;
        testPlan.LastUpdatedBy = !string.IsNullOrEmpty(testPlan.LastUpdatedByName) ? identitiesMap[testPlan.LastUpdatedByName] : Guid.Empty;
        context.TraceVerbose("BusinessLayer", "WITHelper.FetchPlans: PlanId:{0}, Owner:{1} LastUpdatedBy:{2}", (object) testPlan.PlanId, (object) testPlan.Owner, (object) testPlan.LastUpdatedBy);
      }
    }

    protected override void CopyProperties(
      TestManagementRequestContext context,
      TCMWorkItemBase tcmWit)
    {
      base.CopyProperties(context, tcmWit);
      TestPlanWorkItem testPlanWorkItem = tcmWit as TestPlanWorkItem;
      this.StartDate = testPlanWorkItem.StartDate;
      this.EndDate = testPlanWorkItem.EndDate;
    }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    protected override string[] WitFields => this.planWitFields;

    public override string ProcessConfigCategoryName => "TestPlanWorkItems";

    internal TestPlan TCMPlan { get; set; }

    protected override byte DefaultTCMState => 1;

    protected override void PopulateFieldsAfterCreate()
    {
    }
  }
}
