// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SpinlockInformation
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SpinlockInformation
  {
    public string Name { get; set; }

    public long Collisions { get; set; }

    public long Spins { get; set; }

    public float SpinsPerCollision { get; set; }

    public long SleepTime { get; set; }

    public long Backoffs { get; set; }
  }
}
