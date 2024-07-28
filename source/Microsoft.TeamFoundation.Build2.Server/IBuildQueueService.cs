// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IBuildQueueService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DefaultServiceImplementation(typeof (BuildQueueService))]
  public interface IBuildQueueService : IVssFrameworkService
  {
    AgentPoolQueue AddQueue(IVssRequestContext requestContext, AgentPoolQueue queue);

    AgentPoolQueue GetQueue(IVssRequestContext requestContext, int queueId);

    IEnumerable<AgentPoolQueue> GetQueues(IVssRequestContext requestContext, string name = "*");

    IEnumerable<AgentPoolQueue> GetQueuesByPoolId(IVssRequestContext requestContext, int poolId);

    void DeleteQueues(IVssRequestContext requestContext, IEnumerable<int> queueIds);
  }
}
