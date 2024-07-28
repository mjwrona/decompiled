// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.ValueSpecifiedFields
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;

namespace Microsoft.AzureAd.Icm.Types
{
  [Flags]
  public enum ValueSpecifiedFields : long
  {
    None = 0,
    CommitDate = 1,
    CustomerName = 2,
    MitigatedDate = 4,
    TsgId = 8,
    ImpactStartDate = 16, // 0x0000000000000010
    HowFixed = 32, // 0x0000000000000020
    IncidentSubType = 64, // 0x0000000000000040
    TsgOutput = 128, // 0x0000000000000080
    MonitorId = 256, // 0x0000000000000100
    SupportTicketId = 512, // 0x0000000000000200
    SubscriptionId = 1024, // 0x0000000000000400
    ServiceResponsible = 2048, // 0x0000000000000800
    ResolvedDate = 4096, // 0x0000000000001000
  }
}
