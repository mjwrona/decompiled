// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.ITfsTestImpactCatalogService
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestImpact.WebApi.Contracts;
using System;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [DefaultServiceImplementation(typeof (TfsTestImpactCatalogService))]
  [CLSCompliant(false)]
  public interface ITfsTestImpactCatalogService : IVssFrameworkService
  {
    ImpactedTests QueryImpactedTests(
      TestImpactRequestContext context,
      Guid projectId,
      DefinitionRunInfo definitionRunInfo,
      int currentTestRunId,
      TestInclusionOptions typesToInclude);

    BuildType QueryTIAEnabledRun(
      TestImpactRequestContext requestContext,
      Guid projectId,
      int buildId);

    void PublishCodeSignatures(
      TestImpactRequestContext requestContext,
      Guid projectId,
      TestResultSignaturesInfo results);
  }
}
