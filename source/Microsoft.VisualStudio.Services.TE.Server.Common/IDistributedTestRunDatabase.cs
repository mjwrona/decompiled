// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.IDistributedTestRunDatabase
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.TestExecution.Server.Database.Model;
using System;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  public interface IDistributedTestRunDatabase
  {
    void CreateDistributedTestRun(Guid projectId, string environmentUri, string runProperties);

    void UpdateDistributedTestRun(Guid projectId, string environmentUri, int testRunId);

    void DeleteDistributedTestRun(Guid projectId, string environmentUri);

    void DeleteDistributedTestRuns(int numberOfDaysOlder);

    int QueryDistributedTestRun(Guid projectId, string environmentUri, out string runProperties);

    DistributedTestRunDbModel QueryDistributedTestRun(Guid projectId, string environmentUri);

    DistributedTestRunDbModel QueryDistributedTestRun(Guid projectId, int testRunId);
  }
}
