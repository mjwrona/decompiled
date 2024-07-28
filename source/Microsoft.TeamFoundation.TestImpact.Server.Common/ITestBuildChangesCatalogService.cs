// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.ITestBuildChangesCatalogService
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestImpact.WebApi.Contracts;
using System;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [DefaultServiceImplementation(typeof (TestBuildChangesCatalogService))]
  [CLSCompliant(false)]
  public interface ITestBuildChangesCatalogService : IVssFrameworkService
  {
    void PublishBuildChanges(
      TestImpactRequestContext context,
      Guid projectId,
      DefinitionRunInfo definitionRunInfo,
      TestImpactBuildData testImpactBuildData);

    TestImpactBuildData QueryCodeChanges(
      TestImpactRequestContext requestContext,
      Guid projectId,
      DefinitionRunInfo definitionRunInfo);
  }
}
