// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationNotificationTask
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TeamFoundationNotificationTask : TeamFoundationTask<int>
  {
    public TeamFoundationNotificationTask(TeamFoundationTaskCallback callback)
      : this(callback, (object) null, 0)
    {
    }

    public TeamFoundationNotificationTask(
      TeamFoundationTaskCallback callback,
      object taskArgs,
      int interval)
      : this(callback, taskArgs, DateTime.UtcNow.AddMilliseconds((double) interval), interval)
    {
    }

    public TeamFoundationNotificationTask(
      TeamFoundationTaskCallback callback,
      object taskArgs,
      DateTime startTime,
      int interval)
      : base(callback, taskArgs, startTime, interval)
    {
    }
  }
}
