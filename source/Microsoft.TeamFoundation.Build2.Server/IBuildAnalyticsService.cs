// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IBuildAnalyticsService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DefaultServiceImplementation(typeof (BuildAnalyticsService))]
  public interface IBuildAnalyticsService : IVssFrameworkService
  {
    Task<List<BuildAnalyticsData>> GetBuildsByDateAsync(
      IVssRequestContext requestContext,
      int dataspaceId,
      int batchSize,
      DateTime? fromDate);

    Task<List<ShallowBuildAnalyticsData>> GetShallowBuildAnaltyticsDataByDateAsync(
      IVssRequestContext requestContext,
      int dataspaceId,
      int batchSize,
      DateTime? fromDate);

    Task<List<BuildDefinitionAnalyticsData>> GetBuildDefinitionsByDateAsync(
      IVssRequestContext requestContext,
      int dataspaceId,
      int batchSize,
      DateTime? fromDate);
  }
}
