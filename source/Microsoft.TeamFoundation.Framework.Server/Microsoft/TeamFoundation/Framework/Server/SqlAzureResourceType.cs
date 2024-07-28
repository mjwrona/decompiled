// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlAzureResourceType
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public enum SqlAzureResourceType
  {
    None = 0,
    TemporaryDiskSpace = 1,
    TemporaryLogSpace = 2,
    HighVolumeTransactions = 4,
    HighVolumeNetworkIO = 8,
    HighVolumeCPU = 16, // 0x00000010
    DatabaseQuotaExceeded = 32, // 0x00000020
    Disabled = 64, // 0x00000040
    TooManyConcurrentRequests = 128, // 0x00000080
  }
}
