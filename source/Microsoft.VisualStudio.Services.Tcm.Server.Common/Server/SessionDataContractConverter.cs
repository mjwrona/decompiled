// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.SessionDataContractConverter
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class SessionDataContractConverter
  {
    protected TestManagementRequestContext m_requestContext;

    public SessionDataContractConverter(TestManagementRequestContext requestContext) => this.m_requestContext = requestContext;

    public OneMRXSession UpdateSession(TestResultsSession session)
    {
      ArgumentUtility.CheckForEmptyGuid(session.Uid, "Uid", "Test Results");
      ArgumentUtility.CheckForEmptyGuid(session.Source?.SessionId, "SessionId", "Test Results");
      return new OneMRXSession()
      {
        TestRuns = session.TestRuns,
        Type = session.Type,
        Name = session.Name,
        Uid = session.Uid,
        StartTimeUTC = session.StartTimeUTC,
        EndTimeUTC = session.EndTimeUTC,
        Timeline = session.Timeline,
        State = session.State,
        Result = session.Result,
        SessionSourcePipeline = session.SessionSourcePipeline,
        Source = session.Source,
        Layout = session.Layout
      };
    }

    public List<TestResultsSession> GetSession(List<OneMRXSession> oneMrxSessions)
    {
      List<TestResultsSession> session = new List<TestResultsSession>();
      foreach (OneMRXSession oneMrxSession in oneMrxSessions)
        session.Add(new TestResultsSession()
        {
          Type = oneMrxSession.Type,
          Name = oneMrxSession.Name,
          Id = oneMrxSession.Id,
          Uid = oneMrxSession.Uid,
          StartTimeUTC = oneMrxSession.StartTimeUTC,
          EndTimeUTC = oneMrxSession.EndTimeUTC,
          Timeline = oneMrxSession.Timeline,
          State = oneMrxSession.State,
          Result = oneMrxSession.Result,
          SessionSourcePipeline = oneMrxSession.SessionSourcePipeline,
          Source = oneMrxSession.Source,
          Layout = oneMrxSession.Layout,
          TestRuns = oneMrxSession.TestRuns
        });
      return session;
    }
  }
}
