// Decompiled with JetBrains decompiler
// Type: Microsoft.Icm.Directory.Contracts.OnCallSetting
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;

namespace Microsoft.Icm.Directory.Contracts
{
  public class OnCallSetting : ModifiableDocument
  {
    public long TeamId { get; set; }

    public string TimeZoneId { get; set; }

    public bool DisableOnCallReminderEmailNotifications { get; set; }

    public bool DisableOnCallCalendarInvites { get; set; }

    public short DaysBeforeRotationOnCallReminderToBeSent { get; set; }

    public short DaysBeforeRotationOnCallCalendarInvitesToBeSent { get; set; }

    public short NumberOfRotationsToIncludeInOnCallReminderNotifications { get; set; }

    public DateTimeOffset? LastUpdatedOnCallRotationStartTime { get; set; }

    public DateTimeOffset? LastUserTriggerToUpdateOnCallCacheTime { get; set; }

    public DateTimeOffset? NextOnCallReminderNotificationTime { get; set; }

    public string PreviousOnCallReminderJsonPayload { get; set; }
  }
}
