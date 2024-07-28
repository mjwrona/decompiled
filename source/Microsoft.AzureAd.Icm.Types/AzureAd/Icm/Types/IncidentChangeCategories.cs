// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.IncidentChangeCategories
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;

namespace Microsoft.AzureAd.Icm.Types
{
  [Flags]
  public enum IncidentChangeCategories : long
  {
    None = 0,
    AssignContact = 1,
    AssignTenant = 2,
    AssignTeam = 4,
    Edit = 8,
    Activate = 16, // 0x0000000000000010
    Resolve = 32, // 0x0000000000000020
    RelationshipAdd = 64, // 0x0000000000000040
    RelationshipRemove = 128, // 0x0000000000000080
    BridgeSet = 256, // 0x0000000000000100
    BridgeRemove = 512, // 0x0000000000000200
    IncidentAdd = 1024, // 0x0000000000000400
    ConnectorUpdate = 2048, // 0x0000000000000800
    PirReportEdit = 4096, // 0x0000000000001000
    Mitigate = 8192, // 0x0000000000002000
    Unresolve = 16384, // 0x0000000000004000
    SeverityDowngrade = 32768, // 0x0000000000008000
    SeverityUpgrade = 65536, // 0x0000000000010000
    NotificationsUpdate = 131072, // 0x0000000000020000
    DeclareOutage = 262144, // 0x0000000000040000
    UndeclareOutage = 524288, // 0x0000000000080000
    HitCount = 1048576, // 0x0000000000100000
  }
}
