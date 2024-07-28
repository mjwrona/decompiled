// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationJobQueueFlags
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Flags]
  internal enum TeamFoundationJobQueueFlags
  {
    None = 0,
    QueuedAsTemplateJob = 1,
    ExecutedAsTemplateJob = 2,
    QueuedWhileDormant = 8192, // 0x00002000
    AwokenByHostAccess = 16384, // 0x00004000
    DormantDueToInactiveHost = 32768, // 0x00008000
    IgnoresDormancy = 65536, // 0x00010000
    DormantDueToStoppedHost = 131072, // 0x00020000
  }
}
