// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IBuildEventService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DefaultServiceImplementation(typeof (BuildEventService))]
  public interface IBuildEventService : IVssFrameworkService
  {
    BuildChangeEvent AddBuildEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      BuildEventType buildEventType);

    BuildEventResults GetBuildEvents(
      IVssRequestContext requestContext,
      BuildEventStatus buildEventStatus,
      int? maxCount);

    List<BuildChangeEvent> UpdateBuildEventsStatus(
      IVssRequestContext requestContext,
      List<BuildChangeEvent> buildEvents);

    void DeleteBuildEvents(
      IVssRequestContext requestContext,
      BuildEventStatus buildEventStatus,
      DateTime minCreatedTime,
      int? batchSize);

    bool IsQueueHealthy(IVssRequestContext requestContext);
  }
}
