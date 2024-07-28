// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ILatestBuildResultCacheService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DefaultServiceImplementation(typeof (LatestBuildResultCacheService))]
  public interface ILatestBuildResultCacheService : IVssFrameworkService
  {
    (BuildDefinition Definition, BuildResult? BuildResult) GetBuildResult(
      IVssRequestContext requestContext,
      Guid projectId,
      string definition,
      string branchName,
      string stageName,
      string jobName,
      string configuration);

    void TryUpdateBuildResult(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      int buildId,
      BuildResult buildResult);
  }
}
