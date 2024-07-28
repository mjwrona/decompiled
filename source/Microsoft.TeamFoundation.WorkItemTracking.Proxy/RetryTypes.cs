// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.RetryTypes
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  [Flags]
  internal enum RetryTypes
  {
    None = 0,
    Deadlock = 1,
    StaleView = 2,
    All = StaleView | Deadlock, // 0x00000003
  }
}
