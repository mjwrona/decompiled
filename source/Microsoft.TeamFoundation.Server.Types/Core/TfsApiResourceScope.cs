// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TfsApiResourceScope
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using System;

namespace Microsoft.TeamFoundation.Server.Core
{
  [Flags]
  public enum TfsApiResourceScope
  {
    Deployment = 1,
    Application = 2,
    Collection = 4,
    Project = 8,
    Team = 16, // 0x00000010
  }
}
