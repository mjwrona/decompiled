// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.OneMRXSession
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  [CLSCompliant(false)]
  public class OneMRXSession
  {
    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public List<int> TestRuns { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string Type { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string Name { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public long Id { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public Guid Uid { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public DateTime StartTimeUTC { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public DateTime EndTimeUTC { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.Timeline<SessionTimelineType>> Timeline { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public TestResultsSessionState State { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public SessionResult Result { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public SessionSourcePipeline SessionSourcePipeline { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public Source Source { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.Layout> Layout { get; set; }

    public virtual long CreateSession(TestManagementRequestContext context, string teamProjectName)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestSession.Create"))
      {
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
        long session = 0;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          session = managementDatabase.CreateTestSession(projectFromName.GuidId, this);
        context.TraceInfo("BusinessLayer", "Test session successfully created. TestrunId = {0}, ProjectName = {1}", (object) this.Uid, (object) teamProjectName);
        return session;
      }
    }

    internal static List<OneMRXSession> GetSession(
      TestManagementRequestContext context,
      string teamProjectName,
      int buildId)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestSession.Get"))
      {
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
        List<OneMRXSession> sessionByBuildId;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          sessionByBuildId = managementDatabase.GetTestSessionByBuildId(projectFromName.GuidId, buildId);
        Dictionary<long, List<int>> dictionary = OneMRXSession.FetchTestRunSessionMapping(context, projectFromName, sessionByBuildId);
        foreach (OneMRXSession oneMrxSession in sessionByBuildId)
        {
          if (dictionary.ContainsKey(oneMrxSession.Id))
            oneMrxSession.TestRuns = dictionary[oneMrxSession.Id];
        }
        return sessionByBuildId;
      }
    }

    internal static List<Microsoft.TeamFoundation.TestManagement.WebApi.Layout> GetSessionLayout(
      TestManagementRequestContext context,
      string teamProjectName,
      Guid sessionId)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestSessionLayout.Get"))
      {
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Layout> layoutBySessionId;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          layoutBySessionId = managementDatabase.GetLayoutBySessionId(projectFromName.GuidId, sessionId);
        return layoutBySessionId;
      }
    }

    internal static Dictionary<long, List<int>> FetchTestRunSessionMapping(
      TestManagementRequestContext context,
      GuidAndString projectId,
      List<OneMRXSession> sessions)
    {
      List<long> sessionIds = new List<long>();
      foreach (OneMRXSession session in sessions)
        sessionIds.Add(session.Id);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        return managementDatabase.GetTestRunByTestSession(projectId.GuidId, sessionIds);
    }
  }
}
