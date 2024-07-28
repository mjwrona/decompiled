// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.IImpactDataBaseComponent
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.TestImpact.WebApi.Contracts;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  public interface IImpactDataBaseComponent
  {
    void PublishTestSignatures(
      Guid projectId,
      int testRunId,
      int testResultId,
      int configurationId,
      int definitionType,
      int definitionId,
      IEnumerable<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Signature> signatures,
      string automatedTestName);

    ImpactedTests QueryImpactedTests(
      Guid projectId,
      int definitionType,
      int definitionId,
      int runId);

    void DeleteTestMethod(int deletionBatchSize, int waitDaysForCleanup);
  }
}
