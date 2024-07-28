// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.SyncOnCallSetting
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.AzureAd.Icm.Types
{
  public class SyncOnCallSetting
  {
    [Required]
    [Range(1.0, 9.2233720368547758E+18)]
    public long TeamId { get; set; }

    [Required]
    public string TimeZoneId { get; set; }

    public bool DisableOnCallReminders { get; set; }

    public bool DisableCalendarInvites { get; set; }

    public int NotifyDaysBeforeRotation { get; set; }

    public int CalendarInvitesNotifyDaysBeforeRotation { get; set; }

    public int? NotifyRotationCycles { get; set; }

    public DateTimeOffset ModifiedDate { get; set; }

    public long? ReferencedTeamId { get; set; }
  }
}
