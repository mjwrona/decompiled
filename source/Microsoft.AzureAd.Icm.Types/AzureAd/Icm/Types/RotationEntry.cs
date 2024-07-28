// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.RotationEntry
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class RotationEntry
  {
    public RotationEntry(DateTime startDate, DateTime endDate)
    {
      this.ShiftStartTime = new TimeSpan(0, 0, 0);
      this.ShiftDuration = new TimeSpan(24, 0, 0);
      this.StartDate = startDate;
      this.EndDate = endDate;
      this.OnCallContacts = new List<Contact>();
    }

    public RotationEntry(
      DateTime startDate,
      DateTime endDate,
      TimeSpan shiftStartTime,
      TimeSpan shiftDuration)
    {
      this.ShiftStartTime = shiftStartTime;
      this.ShiftDuration = shiftDuration;
      this.StartDate = startDate;
      this.EndDate = endDate;
      this.OnCallContacts = new List<Contact>();
    }

    [DataMember]
    public DateTime StartDate { get; set; }

    [DataMember]
    public DateTime EndDate { get; set; }

    [DataMember]
    public List<Contact> OnCallContacts { get; set; }

    [DataMember]
    public TimeSpan ShiftStartTime { get; set; }

    [DataMember]
    public TimeSpan ShiftDuration { get; set; }
  }
}
