// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationTestManagementSessionService
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TeamFoundationTestManagementSessionService : 
    TfsTestManagementService,
    ITeamFoundationTestManagementSessionService,
    IVssFrameworkService
  {
    private const string EXPLORED_WORKITEM_FOR_SESSION = "ExploredWorkItem";
    private const string ASSOCIATED_WORKITEM_FOR_SESSION = "AssociatedWorkItem";

    public TeamFoundationTestManagementSessionService()
    {
    }

    public TeamFoundationTestManagementSessionService(TfsTestManagementRequestContext requestContext)
      : base((TestManagementRequestContext) requestContext)
    {
    }

    TestSession ITeamFoundationTestManagementSessionService.UpdateTestSession(
      IVssRequestContext requestContext,
      TestSession testSession,
      TeamProjectReference teamProject)
    {
      return this.ExecuteAction<TestSession>(requestContext, "TeamFoundationTestManagementSessionService.UpdateTestSession", (Func<TestSession>) (() =>
      {
        TestSession session = TeamFoundationTestManagementSessionService.FillSessionUpdateProperties(testSession, this.GetTfsTestManagementRequestContext(requestContext));
        TestSessionWorkItemReference[] workItemsFilled = (TestSessionWorkItemReference[]) null;
        List<TestSessionExploredWorkItemReference> workItemsExplored = (List<TestSessionExploredWorkItemReference>) null;
        if (session.PropertyBag != null && !string.IsNullOrWhiteSpace(session.PropertyBag.GetProperty("AssociatedWorkItem")))
        {
          workItemsFilled = JsonUtilities.Deserialize<TestSessionWorkItemReference[]>(session.PropertyBag.GetProperty("AssociatedWorkItem"), true);
          this.ValidateAssociateWorkItemIds(workItemsFilled, requestContext.ServiceName);
        }
        if (session.PropertyBag != null && !string.IsNullOrWhiteSpace(session.PropertyBag.GetProperty("ExploredWorkItem")))
        {
          workItemsExplored = JsonUtilities.Deserialize<List<TestSessionExploredWorkItemReference>>(session.PropertyBag.GetProperty("ExploredWorkItem"), true);
          this.ValidateExploredWorkItemIds(workItemsExplored, requestContext.ServiceName);
        }
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(requestContext))
        {
          if (workItemsFilled != null && workItemsFilled.Length != 0)
            planningDatabase.CreateAssociatedWorkItemsForTestSession(session.Id, workItemsFilled);
          if (workItemsExplored != null && workItemsExplored.Count > 0)
            planningDatabase.CreateExploredWorkItemsForTestSession(session, workItemsExplored);
          if (session.State != TestSessionState.Completed && session.State != TestSessionState.Paused && session.State != TestSessionState.Declined)
          {
            if (session.State != TestSessionState.InProgress)
              goto label_15;
          }
          session = planningDatabase.UpdateTestSession(session);
        }
label_15:
        requestContext.TraceInfo("BusinessLayer", "TeamFoundationTestManagementSessionService.UpdateTestSession projectId = {0}, sessionId = {1}", (object) teamProject.Id, (object) testSession.Id);
        return session;
      }), 1015069, "TestManagement", "BusinessLayer");
    }

    TestSession ITeamFoundationTestManagementSessionService.CreateTestSession(
      IVssRequestContext requestContext,
      TestSession testSession,
      TeamProjectReference teamProject,
      string teamField)
    {
      return this.ExecuteAction<TestSession>(requestContext, "TeamFoundationTestManagementSessionService.CreateTestSession", (Func<TestSession>) (() =>
      {
        ArgumentUtility.CheckForNull<ShallowReference>(testSession.Area, "Area", requestContext.ServiceName);
        ArgumentUtility.CheckStringForNullOrWhiteSpace(testSession.Area.Name, "AreaPath", requestContext.ServiceName);
        ArgumentUtility.CheckStringForNullOrWhiteSpace(testSession.Title, "Title", requestContext.ServiceName);
        TestSession session = TeamFoundationTestManagementSessionService.FillTestSessionProperties(testSession, this.GetTfsTestManagementRequestContext(requestContext));
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(requestContext))
        {
          testSession = planningDatabase.CreateTestSession(session, teamField);
          session.Id = testSession.Id;
          session.Revision = testSession.Revision;
        }
        requestContext.TraceInfo("BusinessLayer", "TeamFoundationTestManagementSessionService.CreateTestSession projectId = {0}, sessionId = {1}", (object) teamProject.Id, (object) testSession.Id);
        return session;
      }), 1015069, "TestManagement", "BusinessLayer");
    }

    List<TestSession> ITeamFoundationTestManagementSessionService.GetTestSessions(
      IVssRequestContext requestContext,
      TeamProjectReference projectReference,
      WebApiTeam team,
      int period,
      bool allSessions,
      bool includeAllProperties,
      List<int> sources,
      List<int> states)
    {
      return this.ExecuteAction<List<TestSession>>(requestContext, "TeamFoundationTestManagementSessionService.GetTestSessions", (Func<List<TestSession>>) (() =>
      {
        bool IsTeamFieldAreaPath = true;
        Validator.CheckAndGetProjectGuidFromName((TestManagementRequestContext) this.GetTfsTestManagementRequestContext(requestContext), projectReference.Name);
        List<string> teamFieldDetails = this.GetTeamFieldDetails(requestContext, team, ref IsTeamFieldAreaPath);
        if (teamFieldDetails == null || teamFieldDetails.Count <= 0)
          return new List<TestSession>();
        List<TestSession> testSessions = new List<TestSession>();
        TfsTestManagementRequestContext managementRequestContext = this.GetTfsTestManagementRequestContext(requestContext);
        List<int> sessionIds = this.FetchAllSessionDataForGivenPeriodAndTeam(managementRequestContext, requestContext, projectReference, period, allSessions, sources, states, teamFieldDetails, IsTeamFieldAreaPath);
        if (sessionIds != null && sessionIds.Count > 0)
        {
          Dictionary<int, TestSession> sessionIdToSession = new Dictionary<int, TestSession>();
          List<int> workItemRefListForSession = new List<int>();
          Dictionary<int, List<TestSessionWorkItemReference>> sessionIdToListOfWorkItemRef = new Dictionary<int, List<TestSessionWorkItemReference>>();
          List<int> exploredItemRefListForSession = new List<int>();
          Dictionary<int, List<TestSessionExploredWorkItemReference>> sessionIdToListOfExploredItemRef = new Dictionary<int, List<TestSessionExploredWorkItemReference>>();
          using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(requestContext))
            sessionIdToSession = planningDatabase.QueryTestSession(managementRequestContext, projectReference.Id.ToString(), sessionIds, ref workItemRefListForSession, ref sessionIdToListOfWorkItemRef, ref exploredItemRefListForSession, ref sessionIdToListOfExploredItemRef);
          testSessions = this.ProcessSessionDetails(requestContext, workItemRefListForSession, exploredItemRefListForSession, sessionIdToSession, sessionIdToListOfWorkItemRef, sessionIdToListOfExploredItemRef);
        }
        requestContext.TraceInfo("BusinessLayer", "TeamFoundationTestManagementSessionService.GetTestSessions projectId = {0}, period = {1}, includeAllSessions = {2}", (object) projectReference.Id, (object) period, (object) allSessions);
        return testSessions;
      }), 1015069, "TestManagement", "BusinessLayer");
    }

    private List<string> GetTeamFieldDetails(
      IVssRequestContext requestContext,
      WebApiTeam team,
      ref bool IsTeamFieldAreaPath)
    {
      List<string> teamFieldDetails = new List<string>();
      if (team == null)
        return teamFieldDetails;
      ProjectProcessConfiguration processConfiguration = this.GetProjectProcessConfiguration(requestContext, team.ProjectId);
      if (processConfiguration != null)
        IsTeamFieldAreaPath = processConfiguration.IsTeamFieldAreaPath();
      ITeamFieldValue[] teamFieldValues = requestContext.GetService<ITeamConfigurationService>().GetTeamSettings(requestContext, team, true, false).TeamFieldConfig.TeamFieldValues;
      for (int index = 0; index < teamFieldValues.Length; ++index)
      {
        string str = teamFieldValues[index].Value;
        if (IsTeamFieldAreaPath)
        {
          int startIndex = str.IndexOf('\\');
          Guid projectId;
          if (startIndex != -1)
          {
            projectId = team.ProjectId;
            str = projectId.ToString() + str.Substring(startIndex);
          }
          else
          {
            projectId = team.ProjectId;
            str = projectId.ToString();
          }
          if (teamFieldValues[index].IncludeChildren)
            str += "%";
        }
        teamFieldDetails.Add(str);
      }
      return teamFieldDetails;
    }

    internal ProjectProcessConfiguration GetProjectProcessConfiguration(
      IVssRequestContext requestContext,
      Guid projectGuid)
    {
      try
      {
        ProjectConfigurationService service = requestContext.GetService<ProjectConfigurationService>();
        string projectUri1 = ProjectInfo.GetProjectUri(projectGuid);
        IVssRequestContext requestContext1 = requestContext;
        string projectUri2 = projectUri1;
        return service.GetProcessSettings(requestContext1, projectUri2, true);
      }
      catch (Exception ex)
      {
        return (ProjectProcessConfiguration) null;
      }
    }

    private List<TestSession> ProcessSessionDetails(
      IVssRequestContext requestContext,
      List<int> workItemRefListForSession,
      List<int> exploredItemRefListForSession,
      Dictionary<int, TestSession> sessionIdToSession,
      Dictionary<int, List<TestSessionWorkItemReference>> sessionIdToListOfWorkItemRef,
      Dictionary<int, List<TestSessionExploredWorkItemReference>> sessionIdToListOfExploredItemRef)
    {
      List<TestSession> testSessionList = new List<TestSession>();
      Dictionary<int, WorkItemFieldData> dictionary = new Dictionary<int, WorkItemFieldData>();
      Dictionary<int, WorkItemFieldData> idToFieldData = this.FetchWorkItemFieldData(requestContext, workItemRefListForSession, exploredItemRefListForSession);
      foreach (int key in sessionIdToSession.Keys)
      {
        this.UpdateAssociatedWorkItemInfoForSession(sessionIdToSession, key, sessionIdToListOfWorkItemRef, idToFieldData);
        this.UpdateExploredWorkItemInfoForSession(sessionIdToSession, key, sessionIdToListOfExploredItemRef, idToFieldData);
        this.UpdateDisplayNameOfSessionOwner(requestContext, sessionIdToSession[key]);
        testSessionList.Add(sessionIdToSession[key]);
      }
      return testSessionList;
    }

    private void UpdateExploredWorkItemInfoForSession(
      Dictionary<int, TestSession> sessionIdToSession,
      int idToTestSessionKey,
      Dictionary<int, List<TestSessionExploredWorkItemReference>> sessionIdToListOfExploredItemRef,
      Dictionary<int, WorkItemFieldData> idToFieldData)
    {
      List<TestSessionExploredWorkItemReference> workItemReferenceList1 = new List<TestSessionExploredWorkItemReference>();
      List<TestSessionExploredWorkItemReference> workItemReferenceList2 = new List<TestSessionExploredWorkItemReference>();
      if (!sessionIdToListOfExploredItemRef.TryGetValue(idToTestSessionKey, out workItemReferenceList1))
        return;
      foreach (TestSessionExploredWorkItemReference workItemReference in workItemReferenceList1)
      {
        WorkItemFieldData workItemFieldData = new WorkItemFieldData();
        if (idToFieldData.TryGetValue(workItemReference.Id, out workItemFieldData))
        {
          workItemReference.Type = workItemFieldData.WorkItemType;
          workItemReferenceList2.Add(workItemReference);
        }
      }
      if (workItemReferenceList2 == null || workItemReferenceList2.Count <= 0)
        return;
      sessionIdToSession[idToTestSessionKey].PropertyBag.AddOrUpdateProperties("ExploredWorkItem", workItemReferenceList2.ToArray().Serialize<TestSessionExploredWorkItemReference[]>(true));
    }

    private void UpdateDisplayNameOfSessionOwner(
      IVssRequestContext requestContext,
      TestSession session)
    {
      TfsTestManagementRequestContext managementRequestContext = this.GetTfsTestManagementRequestContext(requestContext);
      Guid result = Guid.Empty;
      if (!Guid.TryParse(session.Owner.Id, out result))
        return;
      string name = IdentityHelper.ResolveIdentityToName((TestManagementRequestContext) managementRequestContext, result);
      session.Owner.DisplayName = name;
    }

    private void UpdateAssociatedWorkItemInfoForSession(
      Dictionary<int, TestSession> sessionIdToSession,
      int idToTestSessionKey,
      Dictionary<int, List<TestSessionWorkItemReference>> sessionIdToListOfWorkItemRef,
      Dictionary<int, WorkItemFieldData> idToFieldData)
    {
      List<TestSessionWorkItemReference> workItemReferenceList1 = new List<TestSessionWorkItemReference>();
      List<TestSessionWorkItemReference> workItemReferenceList2 = new List<TestSessionWorkItemReference>();
      if (!sessionIdToListOfWorkItemRef.TryGetValue(idToTestSessionKey, out workItemReferenceList1))
        return;
      foreach (TestSessionWorkItemReference workItemReference in workItemReferenceList1)
      {
        WorkItemFieldData workItemFieldData = new WorkItemFieldData();
        if (idToFieldData.TryGetValue(workItemReference.Id, out workItemFieldData))
        {
          workItemReference.Type = workItemFieldData.WorkItemType;
          workItemReferenceList2.Add(workItemReference);
        }
      }
      if (workItemReferenceList2 == null || workItemReferenceList2.Count <= 0)
        return;
      sessionIdToSession[idToTestSessionKey].PropertyBag.AddOrUpdateProperties("AssociatedWorkItem", workItemReferenceList2.ToArray().Serialize<TestSessionWorkItemReference[]>(true));
    }

    private Dictionary<int, WorkItemFieldData> FetchWorkItemFieldData(
      IVssRequestContext requestContext,
      List<int> workItemRefListForSession,
      List<int> exploredItemRefListForSession)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (FetchWorkItemFieldData), "WorkItem")))
      {
        TeamFoundationWorkItemService service = requestContext.GetService<TeamFoundationWorkItemService>();
        Dictionary<int, WorkItemFieldData> dictionary = new Dictionary<int, WorkItemFieldData>();
        string[] fields = new string[2]
        {
          "System.Id",
          "System.WorkItemType"
        };
        List<int> workItemIds = new List<int>();
        if (workItemRefListForSession != null)
          workItemIds.AddRange((IEnumerable<int>) workItemRefListForSession);
        if (workItemRefListForSession != null)
          workItemIds.AddRange((IEnumerable<int>) exploredItemRefListForSession);
        if (workItemIds.Count > 0)
        {
          foreach (WorkItemFieldData workItemFieldValue in service.GetWorkItemFieldValues(requestContext, (IEnumerable<int>) workItemIds, (IEnumerable<string>) fields, 16, new DateTime?(), 200, WorkItemRetrievalMode.NonDeleted, false, false))
            dictionary[workItemFieldValue.Id] = workItemFieldValue;
        }
        return dictionary;
      }
    }

    private static TestSession FillTestSessionProperties(
      TestSession testSession,
      TfsTestManagementRequestContext tcmRequestContext)
    {
      Guid teamFoundationId = tcmRequestContext.UserTeamFoundationId;
      string name = IdentityHelper.ResolveIdentityToName((TestManagementRequestContext) tcmRequestContext, teamFoundationId);
      TestSession testSession1 = new TestSession()
      {
        Title = testSession.Title,
        Project = testSession.Project,
        Owner = IdentityHelper.ToIdentityRef(tcmRequestContext.RequestContext, teamFoundationId.ToString(), name),
        LastUpdatedBy = IdentityHelper.ToIdentityRef(tcmRequestContext.RequestContext, teamFoundationId.ToString(), name),
        State = testSession.State == TestSessionState.Unspecified ? TestSessionState.InProgress : testSession.State,
        Area = new ShallowReference()
        {
          Id = tcmRequestContext.AreaPathsCache.GetIdAndThrow((TestManagementRequestContext) tcmRequestContext, testSession.Area.Name).Id.ToString(),
          Name = testSession.Area.Name
        },
        Source = testSession.Source
      };
      if (testSession.PropertyBag != null)
        testSession1.PropertyBag = testSession.PropertyBag;
      return testSession1;
    }

    private static TestSession FillSessionUpdateProperties(
      TestSession testSession,
      TfsTestManagementRequestContext tcmRequestContext)
    {
      Guid teamFoundationId = tcmRequestContext.UserTeamFoundationId;
      string name = IdentityHelper.ResolveIdentityToName((TestManagementRequestContext) tcmRequestContext, teamFoundationId);
      TestSession testSession1 = new TestSession()
      {
        Project = testSession.Project,
        LastUpdatedBy = IdentityHelper.ToIdentityRef(tcmRequestContext.RequestContext, teamFoundationId.ToString(), name),
        Revision = testSession.Revision,
        Id = testSession.Id,
        Title = testSession.Title
      };
      testSession1.State = testSession.State == TestSessionState.Completed || testSession.State == TestSessionState.Paused || testSession.State == TestSessionState.Declined || testSession.State == TestSessionState.InProgress ? testSession.State : TestSessionState.Unspecified;
      if (!string.IsNullOrWhiteSpace(testSession.Comment))
        testSession1.Comment = testSession.Comment;
      if (testSession.PropertyBag != null)
        testSession1.PropertyBag = testSession.PropertyBag;
      return testSession1;
    }

    private void ValidateAssociateWorkItemIds(
      TestSessionWorkItemReference[] workItemsFilled,
      string expectedServiceName)
    {
      foreach (TestSessionWorkItemReference workItemReference in workItemsFilled)
        ArgumentUtility.CheckGreaterThanZero((float) workItemReference.Id, string.Format("workItem {0} should be positive integer", (object) workItemReference.Id), expectedServiceName);
    }

    private void ValidateExploredWorkItemIds(
      List<TestSessionExploredWorkItemReference> workItemsExplored,
      string expectedServiceName)
    {
      foreach (TestSessionExploredWorkItemReference workItemReference in workItemsExplored)
        ArgumentUtility.CheckGreaterThanZero((float) workItemReference.Id, string.Format("workItem {0} should be positive integer", (object) workItemReference.Id), expectedServiceName);
    }

    private List<int> FetchAllSessionDataForGivenPeriodAndTeam(
      TfsTestManagementRequestContext testManagementRequestContext,
      IVssRequestContext requestContext,
      TeamProjectReference projectReference,
      int period,
      bool allSessions,
      List<int> sources,
      List<int> states,
      List<string> teamFieldsOfTeam,
      bool isTeamFieldAreaPath)
    {
      List<int> intList = new List<int>();
      if (isTeamFieldAreaPath)
        testManagementRequestContext.TestManagementHost.Replicator.UpdateCss((TestManagementRequestContext) testManagementRequestContext);
      Guid sessionOwner = Guid.Empty;
      if (!allSessions)
        sessionOwner = this.GetTfsTestManagementRequestContext(requestContext).UserTeamFoundationId;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(requestContext))
        return planningDatabase.GetSessionIdsOfTeam(projectReference.Id.ToString(), period, sessionOwner, sources, states, isTeamFieldAreaPath, teamFieldsOfTeam);
    }
  }
}
