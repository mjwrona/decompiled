// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Session
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using Microsoft.TeamFoundation.TestManagement.Server.Legacy;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Internal;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  [SoapInclude(typeof (TestRunBase))]
  public class Session : TestRunBase
  {
    private static readonly string[] s_userStoryTitleField = new string[2]
    {
      "System.Id",
      "System.Title"
    };
    private List<SessionNote> m_sessionNotes;
    private List<SessionBookmark> m_sessionBookmarks;
    private static readonly string c_tcmAttachmentUrlReplacementString = new string(new char[1]
    {
      '\b'
    });

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, UseClientDefinedProperty = true, PropertyName = "Id")]
    [QueryMapping]
    public int SessionId { get; set; }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public long Duration { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping(EnumType = typeof (SessionState))]
    public byte State { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string Sprint { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string ComputerName { get; set; }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int UserStoryId { get; set; }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int CharterId { get; set; }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int FeedbackId { get; set; }

    [ClientProperty(ClientVisibility.Private)]
    [XmlArray]
    [XmlArrayItem(Type = typeof (SessionNote))]
    public List<SessionNote> Notes
    {
      get
      {
        if (this.m_sessionNotes == null)
          this.m_sessionNotes = new List<SessionNote>();
        return this.m_sessionNotes;
      }
    }

    [ClientProperty(ClientVisibility.Private)]
    [XmlArray]
    [XmlArrayItem(Type = typeof (SessionBookmark))]
    public List<SessionBookmark> Bookmarks
    {
      get
      {
        if (this.m_sessionBookmarks == null)
          this.m_sessionBookmarks = new List<SessionBookmark>();
        return this.m_sessionBookmarks;
      }
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int ConfigurationId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string ConfigurationName { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public string UserStoryTitle { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Session Id={0}", (object) this.SessionId);

    internal Session Create(
      TfsTestManagementRequestContext context,
      TestSettings settings,
      string teamProjectName,
      bool isFeedbackSession)
    {
      byte state = this.State;
      if (this.Owner == Guid.Empty)
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "Owner"));
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName((TestManagementRequestContext) context, teamProjectName);
      if (!isFeedbackSession)
        context.SecurityManager.CheckPublishTestResultsPermission((TestManagementRequestContext) context, projectFromName.String);
      this.State = (byte) 1;
      if (settings != null)
      {
        settings.IsPublic = false;
        settings.AreaId = context.AreaPathsCache.GetIdAndThrow((TestManagementRequestContext) context, settings.AreaPath).Id;
      }
      Guid teamFoundationId = context.UserTeamFoundationId;
      string teamFoundationName = context.UserTeamFoundationName;
      this.TestSettingsId = this.CreateTestSettings(context, settings, projectFromName.GuidId, teamFoundationId);
      Session session;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) context))
        session = planningDatabase.CreateSession(projectFromName.GuidId, this, teamFoundationId, this.GetSessionSource(isFeedbackSession));
      session.TestSettingsId = this.TestSettingsId;
      session.LastUpdatedBy = teamFoundationId;
      session.LastUpdatedByName = teamFoundationName;
      if (this.Owner != Guid.Empty)
      {
        session.Owner = this.Owner;
        session.OwnerName = IdentityHelper.ResolveIdentityToName((TestManagementRequestContext) context, session.Owner);
      }
      else
      {
        session.Owner = session.LastUpdatedBy;
        session.OwnerName = session.LastUpdatedByName;
      }
      if ((int) state != (int) this.State)
      {
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) context))
        {
          this.State = state;
          this.SessionId = session.SessionId;
          this.Revision = session.Revision;
          UpdatedProperties updatedProperties = (UpdatedProperties) planningDatabase.UpdateSession(this, teamFoundationId, projectFromName.GuidId);
          session.Revision = updatedProperties.Revision;
          session.LastUpdated = updatedProperties.LastUpdated;
        }
      }
      Session.TrySetUserStoryTitleAndFixAttachmentDownloadHandler((TestManagementRequestContext) context, session, false);
      return session;
    }

    internal UpdatedProperties Update(
      TestManagementRequestContext context,
      string projectName,
      bool isFeedBackSession)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (!isFeedBackSession)
        context.SecurityManager.CheckPublishTestResultsPermission(context, projectFromName.String);
      this.ReplaceAttachmentDownloadHandler(context as TfsTestManagementRequestContext);
      UpdatedSessionProperties sessionProperties = (UpdatedSessionProperties) null;
      Guid teamFoundationId = context.UserTeamFoundationId;
      try
      {
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          sessionProperties = planningDatabase.UpdateSession(this, teamFoundationId, projectFromName.GuidId);
      }
      catch (TestObjectUpdatedException ex)
      {
        return new UpdatedProperties() { Revision = -1 };
      }
      if (sessionProperties.IsSessionStarted)
      {
        this.LastUpdated = sessionProperties.LastUpdated;
        this.Revision = sessionProperties.Revision;
        this.LastUpdatedBy = sessionProperties.LastUpdatedBy;
        this.TeamProject = projectName;
      }
      return new UpdatedProperties()
      {
        Id = sessionProperties.Id,
        LastUpdated = sessionProperties.LastUpdated,
        LastUpdatedBy = sessionProperties.LastUpdatedBy,
        LastUpdatedByName = context.UserTeamFoundationName,
        Revision = sessionProperties.Revision
      };
    }

    internal static void Delete(
      TestManagementRequestContext context,
      int[] sessionIds,
      string projectName)
    {
      if (sessionIds == null)
        return;
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      context.SecurityManager.CheckDeleteTestResultsPermission(context, projectFromName.String);
      Guid teamFoundationId = context.UserTeamFoundationId;
      foreach (int sessionId in sessionIds)
      {
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          planningDatabase.QueueDeleteSession(projectFromName.GuidId, sessionId, teamFoundationId);
      }
      context.TestManagementHost.SignalTfsJobService(context, IdConstants.ProjectDeletionCleanupJobId);
    }

    internal static List<Session> Query(
      TestManagementRequestContext context,
      int sessionId,
      Guid owner,
      string buildUri,
      string teamProjectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return new List<Session>();
      context.TestManagementHost.Replicator.UpdateCss(context);
      List<Session> sessions = (List<Session>) null;
      Dictionary<Guid, List<Session>> projetcsMap = new Dictionary<Guid, List<Session>>();
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        sessions = planningDatabase.QuerySessions(sessionId, owner, buildUri, projectFromName.GuidId, out projetcsMap);
      Session.UpdateProjectDataForSessions(context, projetcsMap);
      Session.UpdateBuildDetailsForSessions(context, sessions);
      return Session.TrySetUserStoryTitlesAndFixAttachmentDownloadHandler(context, sessions);
    }

    internal static List<Session> Query(
      TestManagementRequestContext context,
      ResultsStoreQuery query)
    {
      ArgumentUtility.CheckForNull<ResultsStoreQuery>(query, nameof (query), context.RequestContext.ServiceName);
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, query.TeamProjectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return new List<Session>();
      List<Session> sessions = (List<Session>) null;
      Dictionary<Guid, List<Session>> projetcsMap = new Dictionary<Guid, List<Session>>();
      ProjectQueryTranslator projectQueryTranslator = new ProjectQueryTranslator(context, query, (Func<string, string, string>) null, TestPlanningWiqlConstants.TestPlanningTablesForWiql);
      query.QueryText = projectQueryTranslator.TranslateQuery();
      int dataspaceId;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        dataspaceId = planningDatabase.GetDataspaceId(projectFromName.GuidId);
      string whereClause = projectQueryTranslator.GenerateWhereClause(dataspaceId);
      string orderClause = projectQueryTranslator.GenerateOrderClause();
      List<KeyValuePair<int, string>> valueLists = projectQueryTranslator.GenerateValueLists();
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        sessions = planningDatabase.QuerySessions(whereClause, orderClause, valueLists, out projetcsMap);
      Session.UpdateProjectDataForSessions(context, projetcsMap);
      Session.UpdateBuildDetailsForSessions(context, sessions);
      return Session.TrySetUserStoryTitlesAndFixAttachmentDownloadHandler(context, sessions);
    }

    internal static List<Session> QueryInMultipleProjects(
      TestManagementRequestContext context,
      ResultsStoreQuery query)
    {
      List<Session> sessions = (List<Session>) null;
      Dictionary<Guid, List<Session>> projetcsMap = new Dictionary<Guid, List<Session>>();
      ProjectQueryTranslator projectQueryTranslator = new ProjectQueryTranslator(context, query, (Func<string, string, string>) null, TestPlanningWiqlConstants.TestPlanningTablesForWiql);
      query.QueryText = projectQueryTranslator.TranslateQuery();
      string multipleProjects = projectQueryTranslator.GenerateWhereClauseInMultipleProjects();
      List<KeyValuePair<int, string>> valueLists = projectQueryTranslator.GenerateValueLists();
      string orderClause = projectQueryTranslator.GenerateOrderClause();
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        sessions = planningDatabase.QuerySessions(multipleProjects, orderClause, valueLists, out projetcsMap);
      Session.UpdateProjectDataForSessions(context, projetcsMap);
      Session.UpdateBuildDetailsForSessions(context, sessions);
      Session.ProcessQueryResults(context, sessions);
      return Session.TrySetUserStoryTitlesAndFixAttachmentDownloadHandler(context, sessions);
    }

    internal static int QueryCount(TestManagementRequestContext context, ResultsStoreQuery query)
    {
      ArgumentUtility.CheckForNull<ResultsStoreQuery>(query, nameof (query), context.RequestContext.ServiceName);
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, query.TeamProjectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return 0;
      ProjectQueryTranslator projectQueryTranslator = new ProjectQueryTranslator(context, query, (Func<string, string, string>) null, TestPlanningWiqlConstants.TestPlanningTablesForWiql);
      query.QueryText = projectQueryTranslator.TranslateQuery();
      int dataspaceId;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        dataspaceId = planningDatabase.GetDataspaceId(projectFromName.GuidId);
      string whereClause = projectQueryTranslator.GenerateWhereClause(dataspaceId);
      List<KeyValuePair<int, string>> valueLists = projectQueryTranslator.GenerateValueLists();
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        return managementDatabase.QueryObjectsCount(whereClause, valueLists, "vw_Session");
    }

    internal static void CreateAssociatedWorkItems(
      TestManagementRequestContext context,
      int[] identifiers,
      string[] workItemUris)
    {
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        planningDatabase.CreateAssociatedWorkItems(identifiers, workItemUris);
    }

    internal static void DeleteAssociatedWorkItems(
      TestManagementRequestContext context,
      int[] identifiers,
      string[] workItemUris)
    {
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        planningDatabase.DeleteAssociatedWorkItems(identifiers, workItemUris);
    }

    internal static int[][] QueryAssociatedWorkItemsFromSessions(
      TestManagementRequestContext context,
      int[] sessionIds,
      string projectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      int[][] numArray = new int[sessionIds.Length][];
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
      {
        Dictionary<int, List<string>> dictionary = planningDatabase.QueryAssociatedWorkItemsForSessions(projectFromName.GuidId, sessionIds);
        for (int index = 0; index < sessionIds.Length; ++index)
        {
          List<string> uris = (List<string>) null;
          if (dictionary.TryGetValue(sessionIds[index], out uris))
            numArray[index] = ArtifactHelper.ConvertWorkItemUrisToIds(uris).ToArray();
        }
        return numArray;
      }
    }

    private int CreateTestSettings(
      TfsTestManagementRequestContext context,
      TestSettings settings,
      Guid projectId,
      Guid userId)
    {
      if (settings != null)
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties newUpdateProperties;
        if (context.LegacyTcmServiceHelper.TryCreateTestSettingsCompat(context.RequestContext, projectId, TestSettingsContractConverter.Convert(settings), out newUpdateProperties))
          return newUpdateProperties.Id;
        if (context.IsFeatureEnabled("TestManagement.Server.CreateIfNotExistsTestSettings"))
        {
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create((TestManagementRequestContext) context))
          {
            if (settings != null)
              return managementDatabase.CreateIfNotExistsTestSettings((TestManagementRequestContext) context, projectId, settings, userId).Id;
          }
        }
        else
        {
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create((TestManagementRequestContext) context))
          {
            if (settings != null)
              return managementDatabase.CreateTestSettings((TestManagementRequestContext) context, projectId, settings, userId).Id;
          }
        }
      }
      return 0;
    }

    private int GetSessionSource(bool isFeedbackSession)
    {
      if (isFeedbackSession)
        return 2;
      if (this.TestPlanId == -1)
        return 3;
      return this.TestPlanId > 0 ? 1 : 0;
    }

    private static void ProcessQueryResults(
      TestManagementRequestContext context,
      List<Session> sessions)
    {
      int count = sessions.Count;
      sessions.RemoveAll((Predicate<Session>) (session => !context.SecurityManager.HasViewTestResultsPermission(context, session.TeamProjectUri)));
      context.TraceInfo("BusinessLayer", "Filtered out {0} sessions. Returning {1} sessions.", (object) (count - sessions.Count), (object) sessions.Count);
    }

    private void ReplaceAttachmentDownloadHandler(TfsTestManagementRequestContext context)
    {
      string attachmentDownloadHandler = Session.GetTcmAttachmentDownloadHandler(context.RequestContext);
      if (string.IsNullOrEmpty(this.Comment))
        return;
      this.Comment = ExtensionMethods.Replace(this.Comment, attachmentDownloadHandler, Session.c_tcmAttachmentUrlReplacementString, StringComparison.OrdinalIgnoreCase);
    }

    private static Session TrySetUserStoryTitleAndFixAttachmentDownloadHandler(
      TestManagementRequestContext context,
      Session session,
      bool resolveNames = true)
    {
      TestManagementRequestContext context1 = context;
      List<Session> sessions = new List<Session>();
      sessions.Add(session);
      int num = resolveNames ? 1 : 0;
      return Session.TrySetUserStoryTitlesAndFixAttachmentDownloadHandler(context1, sessions, num != 0)[0];
    }

    private static List<Session> TrySetUserStoryTitlesAndFixAttachmentDownloadHandler(
      TestManagementRequestContext context,
      List<Session> sessions,
      bool resolveNames = true)
    {
      if (sessions == null || sessions.Count == 0)
        return sessions;
      Dictionary<int, string> dictionary = new Dictionary<int, string>();
      List<int> list = sessions.Select<Session, int>((Func<Session, int>) (s => s.UserStoryId)).Where<int>((Func<int, bool>) (id => id > 0)).Distinct<int>().ToList<int>();
      foreach (WorkItemFieldData workItemFieldValue in context.RequestContext.GetService<TeamFoundationWorkItemService>().GetWorkItemFieldValues(context.RequestContext, (IEnumerable<int>) list, (IEnumerable<string>) Session.s_userStoryTitleField, 16, new DateTime?(), 200, WorkItemRetrievalMode.NonDeleted, false, false))
        dictionary.Add(workItemFieldValue.Id, workItemFieldValue.Title);
      foreach (Session session in sessions)
      {
        string str;
        if (session.UserStoryId > 0 && dictionary.TryGetValue(session.UserStoryId, out str))
          session.UserStoryTitle = str;
        if (!string.IsNullOrEmpty(session.Comment) && session.Comment.Contains(Session.c_tcmAttachmentUrlReplacementString))
          session.Comment = session.Comment.Replace(Session.c_tcmAttachmentUrlReplacementString, Session.GetTcmAttachmentDownloadHandler(context.RequestContext));
      }
      if (resolveNames)
        TestRunBase.ResolveUserNames<Session>(context, sessions);
      TestRunBase.PopulateVersion<Session>((IList<Session>) sessions);
      return sessions;
    }

    private static string GetTcmAttachmentDownloadHandler(
      IVssRequestContext teamFoundationRequestContext)
    {
      ILocationService service = teamFoundationRequestContext.GetService<ILocationService>();
      return service.LocationForAccessMapping(teamFoundationRequestContext, "AttachmentDownload", "TestManagement", service.DetermineAccessMapping(teamFoundationRequestContext));
    }

    internal static Session FindArtifact(
      TestManagementRequestContext context,
      int sessionId,
      ref TestRunArtifactInfo info)
    {
      info.UpdateSession(sessionId, context as TfsTestManagementRequestContext);
      if (info.HasViewRunPermission)
      {
        List<Session> sessionList = new List<Session>();
        Dictionary<Guid, List<Session>> projetcsMap = new Dictionary<Guid, List<Session>>();
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          sessionList = planningDatabase.QuerySessions(sessionId, Guid.Empty, (string) null, info.Project.GuidId, out projetcsMap);
        Session.UpdateProjectDataForSessions(context, projetcsMap);
        Session.UpdateBuildDetailsForSessions(context, sessionList);
        if (sessionList != null && sessionList.Count == 1)
        {
          TestRunBase.ResolveUserNames<Session>(context, sessionList);
          return sessionList[0];
        }
      }
      return (Session) null;
    }

    internal static void UpdateProjectDataForSessions(
      TestManagementRequestContext context,
      Dictionary<Guid, List<Session>> projectsSessionsMap)
    {
      if (projectsSessionsMap == null || !projectsSessionsMap.Any<KeyValuePair<Guid, List<Session>>>())
        return;
      foreach (Guid key in projectsSessionsMap.Keys)
      {
        ProjectInfo projectFromGuid = context.ProjectServiceHelper.GetProjectFromGuid(key);
        foreach (TestRunBase testRunBase in projectsSessionsMap[key])
          testRunBase.TeamProject = projectFromGuid.Name;
      }
    }

    internal static void UpdateBuildDetailsForSessions(
      TestManagementRequestContext context,
      List<Session> sessions)
    {
      if (sessions == null || !sessions.Any<Session>())
        return;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
      {
        foreach (Session session in sessions)
        {
          if (session.BuildConfigurationId > 0)
          {
            BuildConfiguration buildConfiguration = managementDatabase.QueryBuildConfigurationById(session.BuildConfigurationId, out Guid _);
            session.BuildFlavor = buildConfiguration.BuildFlavor;
            session.BuildPlatform = buildConfiguration.BuildPlatform;
          }
        }
      }
    }
  }
}
