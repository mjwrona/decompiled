// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.SyncSubstitution
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.AzureAd.Icm.Types
{
  public class SyncSubstitution
  {
    [Required]
    public long SubstitutionId { get; set; }

    public DateTimeOffset? SubstitutionDate { get; set; }

    public long? TeamId { get; set; }

    public long? OriginalContactId { get; set; }

    public long? SubstituteContactId { get; set; }

    public long? ShiftId { get; set; }

    public string TimeZoneId { get; set; }

    public TimeSpan? RotationTransitionTime { get; set; }

    public string RotationTransitionDay { get; set; }

    public DateTimeOffset RotationSeedDate { get; set; }

    public short? RotationLength { get; set; }

    public string SysChangeOperation { get; set; }

    public long? SysChangeVersion { get; set; }

    public IList<SyncSubstitution.SyncTeamShift> TeamShifts { get; set; }

    public class SyncTeamShift
    {
      public long ShiftId { get; set; }

      public short Order { get; set; }

      public short Duration { get; set; }
    }
  }
}
