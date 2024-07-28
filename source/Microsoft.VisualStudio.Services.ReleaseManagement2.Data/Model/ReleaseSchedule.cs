// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class ReleaseSchedule
  {
    public Guid JobId { get; set; }

    public string TimeZoneId { get; set; }

    public int StartHours { get; set; }

    public int StartMinutes { get; set; }

    public bool ScheduleOnlyWithChanges { get; set; }

    public ScheduleDays DaysToRelease { get; set; }

    public override int GetHashCode() => this.ToString().GetHashCode();

    public ReleaseSchedule DeepClone() => (ReleaseSchedule) this.MemberwiseClone();

    public override bool Equals(object obj) => obj is ReleaseSchedule releaseSchedule && this.ToString().Equals(releaseSchedule.ToString(), StringComparison.OrdinalIgnoreCase);

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.StringifiedReleaseScheduleFormat, (object) this.JobId, (object) this.TimeZoneId, (object) this.StartHours, (object) this.StartMinutes, (object) this.DaysToRelease, (object) this.ScheduleOnlyWithChanges);
  }
}
