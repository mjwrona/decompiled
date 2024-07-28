// Decompiled with JetBrains decompiler
// Type: Microsoft.Icm.Directory.Contracts.TimeSlotOnCall
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Icm.Directory.Contracts
{
  public class TimeSlotOnCall
  {
    public TimeSlotOnCall() => this.Contacts = (IList<ContactOnCall>) new List<ContactOnCall>();

    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset EndTime { get; set; }

    public DateTimeOffset? RotationStartTime { get; set; }

    public DateTimeOffset? RotationEndTime { get; set; }

    public DateTimeOffset? CycleStartTime { get; set; }

    public DateTimeOffset? CycleEndTime { get; set; }

    public short? CycleOrder { get; set; }

    public string ShiftName { get; set; }

    public DateTimeOffset? ShiftStartTime { get; set; }

    public DateTimeOffset? ShiftEndTime { get; set; }

    public short? ShiftOrder { get; set; }

    public IList<ContactOnCall> Contacts { get; set; }
  }
}
