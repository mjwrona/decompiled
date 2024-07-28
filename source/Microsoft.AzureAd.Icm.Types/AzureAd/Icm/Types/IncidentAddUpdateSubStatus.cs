// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.IncidentAddUpdateSubStatus
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;

namespace Microsoft.AzureAd.Icm.Types
{
  [Flags]
  public enum IncidentAddUpdateSubStatus
  {
    None = 0,
    Resolved = 1,
    Activated = 2,
    ConnectorChanged = 4,
    Transferred = 8,
    Suppressed = 16, // 0x00000010
    Mitigated = 32, // 0x00000020
    Unresolved = 64, // 0x00000040
  }
}
