// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.AsyncAgentAssignmentNotifcationSender
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class AsyncAgentAssignmentNotifcationSender : IAgentAssignmentNotificationSender
  {
    public void Notify(
      IVssRequestContext requestContext,
      string taskLabel,
      Func<IVssRequestContext, Task> notification)
    {
      requestContext.Fork<DistributedTaskRequestDispatcherService>((Func<IVssRequestContext, Task>) (async context => await notification(context)), taskLabel);
    }
  }
}
