// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.Api.Team
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.AzureAd.Icm.Types.Api
{
  public class Team
  {
    [Key]
    public long Id { get; set; }

    public TimeSpan RotationTransitionTime { get; set; }

    public DateTime FirstRotationPeriodDate { get; set; }

    public TimeSpan RotationPeriodLength { get; set; }

    public ICollection<Shift> Shifts { get; set; }

    public string TimeZoneId { get; set; }

    public string PublicId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Email { get; set; }

    public bool IsTombstoned { get; set; }

    public bool IsAssignable { get; set; }

    public bool IsVirtual { get; set; }

    public int RotationMemberCount { get; set; }

    public int HighSeverityThreshold { get; set; }

    public virtual Tenant Tenant { get; set; }

    public virtual ICollection<Contact> Members { get; set; }

    public override string ToString() => this.Name + " (" + (object) this.Id + ")";
  }
}
