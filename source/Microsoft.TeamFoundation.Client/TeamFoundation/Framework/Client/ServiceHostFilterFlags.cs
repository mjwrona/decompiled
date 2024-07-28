// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ServiceHostFilterFlags
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Client
{
  [Flags]
  public enum ServiceHostFilterFlags
  {
    None = 0,
    IncludeChildren = 1,
    IncludeQueuedServicingDetails = 2,
    IncludeRunningServicingDetails = 4,
    IncludeCompletedServicingDetails = 8,
    IncludeActiveServicingDetails = IncludeRunningServicingDetails | IncludeQueuedServicingDetails, // 0x00000006
    IncludeAllServicingDetails = IncludeActiveServicingDetails | IncludeCompletedServicingDetails, // 0x0000000E
  }
}
