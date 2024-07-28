// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.TeamFoundationJobState
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Common
{
  public enum TeamFoundationJobState
  {
    Dormant = -1, // 0xFFFFFFFF
    QueuedScheduled = 0,
    Running = 1,
    [Obsolete("Pausing jobs is no longer supported.")] Paused = 2,
    [Obsolete("Pausing jobs is no longer supported.")] Pausing = 3,
    [Obsolete("Pausing jobs is no longer supported.")] Resuming = 4,
    Stopping = 5,
  }
}
