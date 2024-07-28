// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseSchedule
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  public class ReleaseSchedule : ReleaseManagementSecuredObject
  {
    [DataMember]
    public Guid JobId { get; set; }

    [DataMember]
    public string TimeZoneId { get; set; }

    [DataMember]
    public int StartHours { get; set; }

    [DataMember]
    public int StartMinutes { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool ScheduleOnlyWithChanges { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [XmlIgnore]
    public ScheduleDays DaysToRelease { get; set; }

    public override int GetHashCode() => this.JobId.GetHashCode();

    public override bool Equals(object obj) => obj is ReleaseSchedule releaseSchedule && !(this.JobId != releaseSchedule.JobId) && string.Equals(this.TimeZoneId, releaseSchedule.TimeZoneId, StringComparison.OrdinalIgnoreCase) && this.StartHours == releaseSchedule.StartHours && this.StartMinutes == releaseSchedule.StartMinutes && this.DaysToRelease == releaseSchedule.DaysToRelease;
  }
}
