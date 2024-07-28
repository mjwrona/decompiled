// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.IDtaAgentDatabaseComponent
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Test.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  public interface IDtaAgentDatabaseComponent : IDisposable
  {
    TestAgent RegisterAgent(TestAgent testAgent);

    void UnRegisterAgent(int testAgentId);

    TestAgent QueryTestAgent(int testAgentId);

    IEnumerable<TestAgent> QueryTestAgents(TestAgentsQuery query);

    void AssociateAgentsWithRun(int testRunId, string dtlEnvironmentUrl, string autEnvironmentUrl);

    void UpdateAgentHeartBeat(int testAgentId);
  }
}
