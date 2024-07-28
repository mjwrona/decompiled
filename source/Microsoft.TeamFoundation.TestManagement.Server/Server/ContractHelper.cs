// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ContractHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public static class ContractHelper
  {
    public static TestSession ToTestSession(Session session)
    {
      if (session == null)
        return (TestSession) null;
      TestSession testSession = new TestSession()
      {
        Id = session.SessionId,
        Title = session.Title,
        EndDate = session.CompleteDate,
        State = (TestSessionState) session.State,
        StartDate = session.CreationDate,
        LastUpdatedDate = session.LastUpdated,
        Revision = session.Revision,
        LastUpdatedBy = new IdentityRef(),
        Owner = new IdentityRef()
      };
      testSession.Owner.Id = session.Owner.ToString();
      testSession.LastUpdatedBy.Id = session.LastUpdatedBy.ToString();
      return testSession;
    }
  }
}
