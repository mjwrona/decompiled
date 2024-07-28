// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.IBuildQueueExtension
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.Build.Server
{
  [InheritedExport]
  internal interface IBuildQueueExtension
  {
    void BuildsQueued(IVssRequestContext requestContext, IList<QueuedBuild> builds, bool startNow);

    void QueueStatusChanged(IVssRequestContext requestContext, BuildDefinition definition);

    void ServiceHostChanged(IVssRequestContext requestContext, BuildServiceHost serviceHost);

    bool BuildStarting(
      IVssRequestContext requestContext,
      StartBuildData startBuildData,
      BuildServiceHost serviceHost,
      BuildDefinition definition);

    void BuildFinished(
      IVssRequestContext requestContext,
      BuildDetail buildDetail,
      bool ServerInitiated);

    void PostProcessBuildAgentQueryResult(
      IVssRequestContext requestContext,
      IEnumerable<BuildServiceHost> serviceHosts,
      IEnumerable<BuildAgent> agents);
  }
}
