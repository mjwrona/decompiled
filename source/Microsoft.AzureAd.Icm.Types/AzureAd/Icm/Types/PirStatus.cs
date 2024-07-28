// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.PirStatus
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.AzureAd.Icm.Types
{
  [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags", Justification = "All report states are mutually exclusive")]
  [Flags]
  public enum PirStatus
  {
    Unknown = 0,
    NotStarted = 1,
    Abandoned = 2,
    InProgress = 4,
    ReadyForReview = 8,
    Completed = 16, // 0x00000010
    Approved = 32, // 0x00000020
    Published = 64, // 0x00000040
  }
}
